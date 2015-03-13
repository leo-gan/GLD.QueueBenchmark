using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        public static byte[][] SendTests(int repetitions, int bufferSize,
                                     Dictionary<string, IQueueSender> queues)
        {
            var testBuffers = new byte[repetitions][];
            var measurements = new Dictionary<string, long[]>();
            foreach (var queue in queues)
                measurements[queue.Key] = new long[repetitions];

            for (int i = 0; i < repetitions; i++)
            {
                testBuffers[i] = TestBuffer.GetBuffer(bufferSize); 
                foreach (var queue in queues)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    queue.Value.Send(testBuffers[i]);
                    sw.Stop();
                    measurements[queue.Key][i] = sw.ElapsedTicks;
                    Trace.WriteLine(queue.Key + ": " + sw.ElapsedTicks);
                }
            }
            ReportMeasurments(measurements);
            return testBuffers;
        }
        public static void PurgeQueue(Dictionary<string, IQueueReceiver> receivers)
        {
            foreach (var receiver in receivers)
                receiver.Value.Purge();
        }

        public static byte[][] ReceiveTests(int repetitions, int bufferSize,
                                        Dictionary<string, IQueueReceiver> queues)
        {
            var testBuffers = new byte[repetitions][];
            var measurements = new Dictionary<string, long[]>();
            foreach (var queue in queues)
                measurements[queue.Key] = new long[repetitions];

            for (int i = 0; i < repetitions; i++)
            {
                foreach (var queue in queues)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    testBuffers[i] = queue.Value.Receive();
                    sw.Stop();
                    measurements[queue.Key][i] = sw.ElapsedTicks;
                    Trace.WriteLine(i + ":\t" + queue.Key + ": " + sw.ElapsedTicks);
                }
            }
            ReportMeasurments(measurements);
            return testBuffers;
        }
        private static void ReportMeasurments(Dictionary<string, long[]> measurements)
        {
            ReportHeader();
            foreach (var oneTestMeasurments in measurements)
                ReportMeasurment(oneTestMeasurments);
        }

        private static void ReportMeasurment(KeyValuePair<string, long[]> oneTestMeasurements)
        {
            string report = String.Format("{0, -20}    {1,11:N0}  {2,11:N0}  {3,11:N0}",
                oneTestMeasurements.Key, AverageTime(oneTestMeasurements.Value),
                MinTime(oneTestMeasurements.Value), MaxTime(oneTestMeasurements.Value));

            Console.WriteLine(report);
            Trace.WriteLine(report);
        }

        private static void ReportHeader()
        {
            const string header =   "Queue:             Time, ticks: Avg,        Min,          Max\n"
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

 
    }
}