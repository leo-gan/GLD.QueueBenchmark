/// PM> Install-Package NetMQ

using System.Configuration;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;

namespace GLD.QueueBenchmark.Senders
{
    internal class NetMQ : IQueueSender
    {
        private readonly string _address = ConfigurationManager.AppSettings["NetMq.Address"];
        private readonly NetMQContext _ctx;
        private readonly PublisherSocket _sock;

        public NetMQ()
        {
            _ctx = NetMQContext.Create();
            _sock = _ctx.CreatePublisherSocket();

            _sock.Bind(_address);
            Thread.Sleep(1000);
        }

        #region IQueueSender Members

        public void Send(byte[] buffer)
        {
            _sock.Send(buffer);
            //_sock.Send(buffer, buffer.Length, dontWait:true);
        }

        #endregion

        public void Dispose()
        {
            if (_sock != null) _sock.Dispose();
            if (_ctx != null) _ctx.Dispose();
        }
    }
}