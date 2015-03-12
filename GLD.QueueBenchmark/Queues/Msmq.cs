/// add reference to System.Messaging
/// using System.Messaging;
/// Make sure the Message Queuing (MSMQ) Windows Serivce installed and running.
/// Create queue

using System;
using System.Configuration;
using System.Messaging;


namespace GLD.QueueBenchmark.Queues
{
    internal class Msmq : IQueue
    {
        private readonly string _address = ConfigurationManager.AppSettings["GLD.QueueBenchmark.Msmq.Address"];
        private MessageQueue _q;

        public Msmq()
        {
            // if queue is not existeed, create it
                if (!MessageQueue.Exists(_address))
                    MessageQueue.Create(_address);
                _q = new MessageQueue(_address) { Formatter = new BinaryMessageFormatter() };
        }

        public void Dispose()
        {
            if (_q != null) _q.Dispose();
        }

        #region IQueue Members

        public byte[] Receive()
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] buffer)
        {
            _q.Send(buffer);
        }

        #endregion
    }
}