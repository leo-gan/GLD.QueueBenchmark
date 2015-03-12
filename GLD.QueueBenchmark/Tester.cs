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
        public static void SendTests(int repetitions, int bufferSize,
                                     Dictionary<string, IQueue> queues)
        {
            var measurements = new Dictionary<string, long[]>();
            foreach (var queue in queues)
                measurements[queue.Key] = new long[repetitions];

            var original = new TestBuffer(bufferSize); // the same data for all queues
            for (int i = 0; i < repetitions; i++)
                foreach (var queue in queues)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    queue.Value.Send(original.Buffer);
                    sw.Stop();
                    measurements[queue.Key][i] = sw.ElapsedTicks;
                    Trace.WriteLine(queue.Key + ": " + sw.ElapsedTicks);
                    //List<string> errors = original.Compare(processed);
                    //errors[0] = queue.Key + errors[0];
                    //ReportErrors(errors);
                }
            ReportAllResults(measurements);
        }

        private static void ReportAllResults(Dictionary<string, long[]> measurements)
        {
            ReportTestResultHeader();
            foreach (var oneTestMeasurments in measurements)
                ReportTestResult(oneTestMeasurments);
        }

        private static void ReportTestResult(KeyValuePair<string, long[]> oneTestMeasurements)
        {
            string report = String.Format("{0, -20}  {1,9:N0} {2,11:N0}",
                oneTestMeasurements.Key, AverageTime(oneTestMeasurements.Value),
                MaxTime(oneTestMeasurements.Value));

            Console.WriteLine(report);
            Trace.WriteLine(report);
        }

        private static void ReportTestResultHeader()
        {
            const string header = "Serializer:          Time: Avg,    Max ticks   \n"
                                  + "===============================================";

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

        public static double MaxTime(long[] times)
        {
            if (times == null || times.Length == 0) return 0;
            long max = times.Max();
            return max;
        }

        public static double AverageTime(long[] times)
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