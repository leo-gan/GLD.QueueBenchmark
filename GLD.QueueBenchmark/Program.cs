using System;
using System.Collections.Generic;
using GLD.QueueBenchmark.Queues;

namespace GLD.QueueBenchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int repetitions = int.Parse(args[0]);
            int bufferSize = int.Parse(args[1]);
            Console.WriteLine("Repetitions: " + repetitions);
            Console.WriteLine("Buffer Size: " + bufferSize);
            var queues = new Dictionary<string, IQueue>
            {
                {"MSMQ", new Msmq()},
            };
            Tester.SendTests(repetitions, bufferSize, queues);
        }
    }
}