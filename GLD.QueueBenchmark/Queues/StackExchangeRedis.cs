/// Download and Intall Redis: https://github.com/rgl/redis/downloads as a Windows Service.
/// Start Redis windows service.
/// PM> Install-Package StackExchange.Redis

using System;
using System.Configuration;
using System.Threading;
using NetMQ;
using StackExchange.Redis;

namespace GLD.QueueBenchmark
{
    internal class StackExchangeRedis : IQueue
    {
        private readonly string _address = ConfigurationManager.AppSettings["GLD.QueueBenchmark.StackExchangeRedis.ConnectionString"];
        private readonly string _queueName = ConfigurationManager.AppSettings["GLD.QueueBenchmark.StackExchangeRedis.QueueName"];

        private IDatabase _db;
        private ConnectionMultiplexer _redis;

        public StackExchangeRedis()
        {
                _redis = ConnectionMultiplexer.Connect(_address);
                _db = _redis.GetDatabase();
        }

        public void Dispose()
        {
            if (_redis != null) _redis.Dispose();
        }

        #region IQueue Members

        public byte[] Receive()
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] buffer)
        {
            _db.ListLeftPush(_queueName, (RedisValue)buffer, When.Always, CommandFlags.None);
        }

        #endregion
    }
}