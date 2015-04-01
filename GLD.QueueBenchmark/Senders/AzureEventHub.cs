// PM> Install-Package WindowsAzure.ServiceBus
// Create an Azure ServiceBus namespace. It is optional, if you also have it. 
// Create an  event hub 
// Create the [Manage,] Send and Listen policies
// Copy the policies from the ServiceBus configuration page into the config files of the sender and receiver applications

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace GLD.QueueBenchmark.Senders
{
    internal class AzureEventHub : IQueueSender
    {
     private readonly EventHubClient _client;
        private readonly EventHubSender _partinionedSender;
        private const string PartitionId = "0";

        public AzureEventHub()
        {
            string eventHubName = ConfigurationManager.AppSettings["AzureEventHub.EnentHubName"];
                _client =
                    EventHubClient.CreateFromConnectionString(
                        ConfigurationManager.AppSettings["AzureEventHub.SendConnectionString"], eventHubName);
                _partinionedSender = _client.CreatePartitionedSender(PartitionId);
       }

        #region IQueueSender Members

        public void Send(byte[] buffer)
        {
            var eventData = new EventData(buffer);
            _partinionedSender.SendAsync(eventData);
        }

        public void SendBatch(IEnumerable<byte[]> buffers)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public void Dispose()
        {
            if (_partinionedSender != null) _partinionedSender.Close();
            if (_client != null) _client.Close();
        }
    }
}