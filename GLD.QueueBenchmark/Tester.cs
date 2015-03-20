using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GLD.QueueBenchmark
{
    internal struct Measurements
    {
        public string QueueName;
        public long[] Times;

        public Measurements(string queueName, int repetitions)
        {
            QueueName = queueName;
            Times = new long[repetitions];
        }
    }

    internal class Tester
    {
        public static byte[][] SendTests(string operation, int repetitions, int bufferSize,
                                         Dictionary<string, IQueueSender> queues)
        {
            var testBuffers = new byte[repetitions][];
            var measurements = new Dictionary<string, long[]>();
            foreach (var queue in queues)
                measurements[queue.Key] = new long[repetitions];

            for (int i = 0; i < repetitions; i++)
            {
                testBuffers[i] = TestBuffer.GetBuffer(bufferSize, i);
                foreach (var queue in queues)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    queue.Value.Send(testBuffers[i]);
                    sw.Stop();
                    measurements[queue.Key][i] = sw.ElapsedTicks;
                }
            }
            ReportMeasurments(operation, measurements);
            return testBuffers;
        }

        public static void PurgeQueue(Dictionary<string, IQueueReceiver> receivers)
        {
            foreach (var receiver in receivers)
                receiver.Value.Purge();
        }

        public static Dictionary<string, byte[][]> ReceiveTests(string operation, int repetitions,
                                                                int bufferSize,
                                                                Dictionary<string, IQueueReceiver>
                                                                    queues)
        {
            var testBuffers = new Dictionary<string, byte[][]>();
            var measurements = new Dictionary<string, long[]>();
            foreach (var queue in queues)
            {
                testBuffers.Add(queue.Key, new byte[repetitions][]);
                measurements[queue.Key] = new long[repetitions];
            }

            for (int i = 0; i < repetitions; i++)
                foreach (var queue in queues)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    testBuffers[queue.Key].SetValue(queue.Value.Receive(), i);
                    sw.Stop();
                    measurements[queue.Key][i] = sw.ElapsedTicks;
                }
            ReportMeasurments(operation, measurements);
            return testBuffers;
        }

        private static void ReportMeasurments(string operation,
                                              Dictionary<string, long[]> measurements)
        {
            ReportRawData(operation, measurements);
            ReportHeader(operation);
            foreach (var oneTestMeasurments in measurements)
                ReportMeasurment(oneTestMeasurments);
        }

        private static void ReportRawData(string operation, Dictionary<string, long[]> measurements)
        {
            var sb = new StringBuilder();
            sb.Append(operation + ":");
            foreach (var m in measurements)
            {
                sb.Append("\n\t" + m.Key + ":\n\t\t");
                for (int i = 0; i < m.Value.Length;)
                {
                    sb.Append(String.Format("{0,11:N0} ", m.Value[i++]));
                    if (i%10 == 0) sb.Append("\n\t\t");
                }
            }
            Trace.WriteLine(sb.ToString());
        }

        private static void ReportMeasurment(KeyValuePair<string, long[]> oneTestMeasurements)
        {
            string report = String.Format("{0, -20}    {1,11:N0}  {2,11:N0}  {3,11:N0}",
                oneTestMeasurements.Key, AverageTime(oneTestMeasurements.Value),
                MinTime(oneTestMeasurements.Value), MaxTime(oneTestMeasurements.Value));

            Console.WriteLine(report);
            Trace.WriteLine(report);
        }

        private static void ReportHeader(string operation)
        {
            string header = "\n" + operation
                            + "\nQueue:             Time, ticks: Avg,        Min,          Max\n"
                            + "=============================================================";

            Console.WriteLine(header);
            Trace.WriteLine(header);
        }

        private static void ReportErrors(List<string> errors)
        {
            if (errors.Count <= 1) return;
            foreach (string error in errors)
            {
                Trace.WriteLine(error);
                Console.WriteLine(error);
            }
        }

        private static double MaxTime(long[] times)
        {
            if (times == null || times.Length == 0) return 0;
            return times.Max();
        }

        private static double MinTime(long[] times)
        {
            if (times == null || times.Length == 0) return 0;
            return times.Min();
        }

        private static double AverageTime(long[] times)
        {
            // Calculate the average times discarding
            // the 5% min and 5% max test times
            if (times == null || times.Length == 0) return 0;

            Array.Sort(times);
            int repetitions = times.Length;
            long totalTime = 0;
            var discardCount = (int) Math.Round(repetitions*0.05);
            if (discardCount == 0 && repetitions > 2) discardCount = 1;
            int count = repetitions - discardCount;
            for (int i = discardCount; i < count; i++)
                totalTime += times[i];

            return ((double) totalTime)/(count - discardCount);
        }

        public static void ResultIs(byte[][] sentOriginal,
                                    Dictionary<string, byte[][]> receivedResults)
        {
            foreach (var result in receivedResults)
            {
                Array.Sort(result.Value, new ByteArrayComparer()); // it could be unordered!
                for (int i = 0; i < result.Value.Length; i++)
                    ReportErrors(TestBuffer.Compare(result.Key + "[" + i + "]", sentOriginal[i],
                        result.Value[i]));
            }
        }
    }

    internal class ByteArrayComparer : IComparer<byte[]>
    {
        #region IComparer<byte[]> Members

        public int Compare(byte[] x, byte[] y)
        {
            if (x == null && y == null) return 0;
            if (y == null) return 1;
            if (x == null) return -1;
            if (x.Length > y.Length) return 1;
            if (x.Length < y.Length) return -1;
            // first  bytes 4 used to store the array id. We try to sort by this id first.
            if (x.Length < 4) // array without id:
            {
                for (int i = 0; i < x.Length; i++)
                    if (x[i] > y[i]) return 1;
                    else if (x[i] < y[i]) return -1;
                return 0;
            }
            int xId = BitConverter.ToInt32(x, 0);
            int yId = BitConverter.ToInt32(y, 0);
            if (xId > yId) return 1;
            if (xId < yId) return -1;

            // if id-s are equal (it is not good), compary all other bytes:
            for (int i = 4; i < x.Length; i++)
                if (x[i] > y[i]) return 1;
                else if (x[i] < y[i]) return -1;
            return 0;
        }

        #endregion
    }
}