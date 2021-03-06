﻿// PM> Install-Package WindowsAzure.ServiceBus
// Create an Azure ServiceBus namespace. It is optional, if you also have it. 
// Create a queue 
// Create the Manage, Send and Listen policies
// Copy the policies from the ServiceBus configuration page into the config files of the sender and receiver applications

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Microsoft.ServiceBus.Messaging;

namespace GLD.QueueBenchmark.Receivers
{
    internal class AzureTopic : IQueueReceiver
    {
        private readonly SubscriptionClient _client;
        private string _topicName;

        public AzureTopic()
        {
            _topicName = ConfigurationManager.AppSettings[
                "AzureTopic.TopicName"];
            string subscriptionName = ConfigurationManager.AppSettings[
                "AzureTopic.SubscriptionName"];
            _client = SubscriptionClient.CreateFromConnectionString(
                ConfigurationManager.AppSettings[
                    "AzureTopic.ListenConnectionString"], _topicName, subscriptionName);
        }

        #region IQueueReceiver Members

        public byte[] Receive()
        {
            BrokeredMessage brokeredMsg = null;
            byte[] buffer = null;
            try
            {
                brokeredMsg = _client.Receive(new TimeSpan(0, 0, 1));
                if (brokeredMsg == null) return null;
                //using (var bodyStream = brokeredMsg.GetBody<MemoryStream>())
                //{
                //    buffer = bodyStream.GetBuffer();
                //}
                buffer = brokeredMsg.GetBody<byte[]>();
                brokeredMsg.Complete();
            }
            catch (Exception ex)
            {
                Trace.Write("Exception ****** " + ex.Message);
                throw;
            }
            return buffer;
        }

        public IEnumerable<byte[]> ReceiveBatch(int batchSize)
        {
            throw new NotImplementedException();
        }

        public void Purge()
        {
            Trace.Write("GLD.QueueBenchmark.Receivers.AzureTopic.Purge() ");
            while (_client.Peek() != null)
            {
                BrokeredMessage brokeredMsg = _client.Receive();
                brokeredMsg.Complete();
            }
            Trace.WriteLine("finished sucessfully");

        }

        #endregion

        public void Dispose()
        {
            if (_client != null) _client.Close();
        }
    }
}