using System;
using System.Collections.Generic;

namespace GLD.QueueBenchmark
{
    internal class TestBuffer
    {
        public TestBuffer(int bufferSize)
        {
            Buffer = new byte[bufferSize];

            byte[] guid = Guid.NewGuid().ToByteArray();
            for (int i = 0; i < guid.Length; i++)
            {
                Buffer[i] = guid[i];
                if (bufferSize <= guid.Length) return;
            }
            for (int i = guid.Length; i < bufferSize; i++)
                Buffer[i] = (byte) 'a';
        }

        public byte[] Buffer { set; get; }

        public List<string> Compare(byte[] comparable)
        {
            var errors = new List<string> {"  ************** Comparison failed! "};
            if (comparable == null)
            {
                errors.Add("\tcomparable: is null!");
                return errors;
            }

            if (Buffer.Length != comparable.Length)
            {
                errors.Add(String.Format("\tBuffer.Length({0}) != comparable.Length({1})",
                    Buffer.Length, comparable.Length));
                return errors;
            }

            for (int i = 0; i < Buffer.Length; i++)
                if (Buffer[i] != comparable[i])
                {
                    errors.Add(String.Format("\tBuffer[{0}]:{1} != comparable[{0}]:{2}",
                        i, Buffer[i], comparable[i]));
                    break;
                }
            return errors;
        }
    }
}