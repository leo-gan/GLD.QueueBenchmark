// PM> Install-Package WindowsAzure.ServiceBus
// Create an Azure ServiceBus namespace. It is optional, if you also have it. 
// Create a queue 
// Create the Manage, Send and Listen policies
// Copy the policies from the ServiceBus configuration page into the config files of the sender and receiver applications

using System;
using System.Configuration;
using System.Diagnostics;
using Microsoft.ServiceBus.Messaging;

namespace GLD.QueueBenchmark.Receivers
{
    internal class AzureEventHub : IQueueReceiver
    {
        private const string PartitionId = "0";
        private readonly EventHubClient _client;
        private readonly EventHubReceiver _receiver;

        public AzureEventHub()
        {
            string eventHubName = ConfigurationManager.AppSettings["AzureEventHub.EnentHubName"];
            _client = EventHubClient.CreateFromConnectionString(
                ConfigurationManager.AppSettings["AzureEventHub.ListenConnectionString"],
                eventHubName);
            EventHubConsumerGroup group = _client.GetDefaultConsumerGroup();

            _receiver = group.CreateReceiver(PartitionId, DateTime.UtcNow);
        }

        #region IQueueReceiver Members

        public byte[] Receive()
        {
            EventData message;
            try
            {
                message = _receiver.Receive(new TimeSpan(0, 0, 1));
            }
            catch (Exception ex)
            {
                Trace.Write("Exception ****** " + ex.Message);
                throw;
            }
            return message.GetBytes();
        }

        public void Purge()
        {
            Trace.Write("GLD.QueueBenchmark.Receivers.AzureTopic.Purge() ");
            // do nothing. There is a message retention. And constuctor sets offset to the tail, kind of.
            Trace.WriteLine("finished sucessfully");
        }

        #endregion

        public void Dispose()
        {
            if (_client != null) _client.Close();
        }
    }
}