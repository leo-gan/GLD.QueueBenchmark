// PM> Install-Package WindowsAzure.ServiceBus
// Create an Azure ServiceBus namespace. It is optional, if you also have it. 
// Create a topic 
// Create a subscription 
// Create the [Manage,] Send and Listen policies
// Copy the policies from the ServiceBus configuration page into the config files of the sender and receiver applications

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace GLD.QueueBenchmark.Senders
{
    internal class AzureTopic : IQueueSender
    {
        private readonly TopicClient _client;

        public AzureTopic()
        {
            string topicName = ConfigurationManager.AppSettings[
                "AzureTopic.TopicName"];
            _client = TopicClient.CreateFromConnectionString(
                ConfigurationManager.AppSettings[
                    "AzureTopic.SendConnectionString"], topicName);
        }

        #region IQueueSender Members

        public void Send(byte[] buffer)
        {
            //using (var stream = new MemoryStream(buffer))
            //{
            //    var brokeredMsg = new BrokeredMessage(stream);
            //    _client.SendAsync(brokeredMsg);
            //    //_client.Send(brokeredMsg);
            //}
            _client.SendAsync(new BrokeredMessage(buffer));
        }

        public void SendBatch(IEnumerable<byte[]> buffers)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public void Dispose()
        {
            if (_client != null) _client.Close();
        }
    }
}