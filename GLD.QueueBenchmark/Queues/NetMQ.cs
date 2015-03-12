/// PM> Install-Package NetMQ

using System;
using System.Configuration;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;

namespace GLD.QueueBenchmark
{
    internal class NetMQ : IQueue
    {
       private string _address = ConfigurationManager.AppSettings["GLD.QueueBenchmark.NetMq.Address"];
        private NetMQContext _ctx;
        private PublisherSocket _sock;

        public NetMQ()
        {
            _ctx = NetMQContext.Create();
            _sock = _ctx.CreatePublisherSocket();

            _sock.Bind(_address);
            Thread.Sleep(1000);
        }

        public void Dispose()
        {
            if (_sock != null) _sock.Dispose();
            if (_ctx != null) _ctx.Dispose();
        }

        #region IQueue Members

        public byte[] Receive()
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] buffer)
        {
            _sock.Send(buffer);
            //_sock.Send(buffer, buffer.Length, dontWait:true);
        }

        #endregion
    }
}