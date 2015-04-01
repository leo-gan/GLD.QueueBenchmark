// PM> Install-Package WindowsAzure.ServiceBus
// Create an Azure ServiceBus namespace. It is optional, if you also have it. 
// Create a queue 
// Create the Manage, Send and Listen policies
// Copy the policies from the ServiceBus configuration page into the config files of the sender and receiver applications

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.ServiceBus.Messaging;

namespace GLD.QueueBenchmark.Senders
{
    internal class AzureQueue : IQueueSender
    {
        private readonly QueueClient _client;

        public AzureQueue()
        {
            //NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(
            //    ConfigurationManager.AppSettings[
            //        "AzureQueue.ManageConnectionString"]);
            string queueName =
                ConfigurationManager.AppSettings["AzureQueue.QueueName"];
            //if (!namespaceManager.QueueExists(queueName))
            //    namespaceManager.CreateQueue(queueName);
            _client = QueueClient.CreateFromConnectionString(
                ConfigurationManager.AppSettings[
                    "AzureQueue.SendConnectionString"], queueName);
        }

        #region IQueueSender Members

        public void Send(byte[] buffer)
        {
                 var brokeredMsg = new BrokeredMessage(buffer);
                _client.SendAsync(brokeredMsg);
          }

        public void SendBatch(IEnumerable<byte[]> buffers)
        {
            var brokeredMsgs = buffers.Select(buffer => new BrokeredMessage(buffer)).ToList();
            _client.SendBatchAsync(brokeredMsgs);
        }
        #endregion

        public void Dispose()
        {
            if (_client != null) _client.Close();
        }
    }
}