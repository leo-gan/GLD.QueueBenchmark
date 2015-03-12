namespace GLD.QueueBenchmark
{
    internal interface IQueue
    {
        byte[] Receive();
        void Send(byte[] buffer);
    }
}