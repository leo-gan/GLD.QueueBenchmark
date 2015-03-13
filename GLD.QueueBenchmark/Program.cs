using System;
using System.Collections.Generic;
using GLD.QueueBenchmark.Senders;

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
            var queues = new Dictionary<string, IQueueSender>
            {
                {"MS Azure Queue", new AzureQueue()},
                {"MS Azure Topic", new AzureTopic()},
                {"MS MSMQ", new Msmq()},
                {"NetMQ", new Senders.NetMQ()},
                {"StackExchange Redis", new StackExchangeRedis()},
            };
            Tester.SendTests(repetitions, bufferSize, queues);
        }
    }
}