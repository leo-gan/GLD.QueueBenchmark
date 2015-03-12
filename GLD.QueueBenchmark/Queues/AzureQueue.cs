// PM> Install-Package WindowsAzure.ServiceBus
// Create an Azure ServiceBus namespace. It is optional, if you also have it. 
// Create a queue 
// Create the Manage, Send and Listen policies
// Copy the policies from the ServiceBus configuration page into the config files of the sender and receiver applications

using System;
using System.Configuration;
using System.IO;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace GLD.QueueBenchmark
{
    internal class AzureQueue : IQueue
    {
        private readonly QueueClient _client;

        public AzureQueue()
        {
            //NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(
            //    ConfigurationManager.AppSettings[
            //        "GLD.QueueBenchmark.AzureQueue.ManageConnectionString"]);
            string queueName =
                ConfigurationManager.AppSettings["GLD.QueueBenchmark.AzureQueue.QueueName"];
            //if (!namespaceManager.QueueExists(queueName))
            //    namespaceManager.CreateQueue(queueName);
            _client = QueueClient.CreateFromConnectionString(
                ConfigurationManager.AppSettings[
                    "GLD.QueueBenchmark.AzureQueue.SendConnectionString"], queueName);
        }

        #region IQueue Members

        public byte[] Receive()
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] buffer)
        {
            using(var stream = new MemoryStream(buffer))
            {
                var brokeredMsg = new BrokeredMessage(stream);
                _client.SendAsync(brokeredMsg);
            }
        }

        #endregion

        public void Dispose()
        {
            if (_client != null) _client.Close();
        }
    }
}