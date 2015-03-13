namespace GLD.QueueBenchmark
{
    internal interface IQueueSender
    {
        void Send(byte[] buffer);
    }
}