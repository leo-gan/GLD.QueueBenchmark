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
            var senders = new Dictionary<string, IQueueSender>
            {
                {"MS Azure Queue", new Senders.AzureQueue()},
                {"MS Azure Topic", new Senders.AzureTopic()},
                {"MS MSMQ", new Senders.Msmq()},
                {"NetMQ", new Senders.NetMQ()},
                {"StackExchange Redis", new Senders.StackExchangeRedis()},
            };
            var receivers = new Dictionary<string, IQueueReceiver>
            {
                {"MS Azure Queue", new Receivers.AzureQueue()},
                {"MS Azure Topic", new Receivers.AzureTopic()},
                {"MS MSMQ", new Receivers.Msmq()},
                {"NetMQ", new Receivers.NetMQ()},
                {"StackExchange Redis", new Receivers.StackExchangeRedis()},
            };
            Tester.PurgeQueue(receivers);
            var sentOriginal = Tester.SendTests("Sending", repetitions, bufferSize, senders);
            var receivedResult = Tester.ReceiveTests("Receiving", repetitions, bufferSize, receivers);
            Tester.ResultIs(sentOriginal, receivedResult);
        }
    }
}