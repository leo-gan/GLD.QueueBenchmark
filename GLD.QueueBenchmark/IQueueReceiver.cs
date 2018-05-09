using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLD.QueueBenchmark
{
    internal interface IQueueReceiver
    {
        byte[] Receive();
        IEnumerable<byte[]> ReceiveBatch(int batchSize);
        void Purge();
    }
}
