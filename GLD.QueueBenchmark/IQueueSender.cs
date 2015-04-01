using System.Collections.Generic;

namespace GLD.QueueBenchmark
{
    internal interface IQueueSender
    {
        void Send(byte[] buffer);
        void SendBatch(IEnumerable<byte[]> buffers);
    }
}