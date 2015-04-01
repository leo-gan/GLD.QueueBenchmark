/// Download and Intall Redis: https://github.com/rgl/redis/downloads as a Windows Service.
/// Start Redis windows service.
/// PM> Install-Package StackExchange.Redis

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using StackExchange.Redis;

namespace GLD.QueueBenchmark.Receivers
{
    internal class StackExchangeRedis : IQueueReceiver
    {
        private readonly string _address =
            ConfigurationManager.AppSettings["StackExchangeRedis.ConnectionString"];

        private readonly string _queueName =
            ConfigurationManager.AppSettings["StackExchangeRedis.QueueName"];

        private readonly ConnectionMultiplexer _redis;
        private IDatabase _db;

        public StackExchangeRedis()
        {
            _redis = ConnectionMultiplexer.Connect(_address);
            _db = _redis.GetDatabase();
        }

        #region IQueueReceiver Members

        public byte[] Receive()
        {
            return (byte[])_db.ListRightPop(_queueName);
        }

        public IEnumerable<byte[]> ReceiveBatch(int batchSize)
        {
            throw new NotImplementedException();
        }

        public void Purge()
        {
            var queueLen = _db.ListLength(_queueName);
            Trace.Write("GLD.QueueBenchmark.Receivers.StackExchangeRedis.Purge: queue lenght: " + queueLen);
            for (var i = 0; i < queueLen; i++)
                _db.ListRightPop(_queueName);
            Trace.WriteLine(" Now it is " + _db.ListLength(_queueName));
        }

        #endregion

        public void Dispose()
        {
            if (_redis != null) _redis.Dispose();
        }
    }
}