
using System;
using System.Collections.Generic;

namespace UselessFrame.Net
{
    public class ByteBufferPool : IDisposable
    {
        private Queue<byte[]> _queue0;
        private Queue<byte[]> _queue1;
        private Queue<byte[]> _queue2;
        private Queue<byte[]> _queue3;
        private Queue<byte[]> _queue4;

        public ByteBufferPool()
        {
            _queue0 = new Queue<byte[]>(128);
            _queue1 = new Queue<byte[]>(1024);
            _queue2 = new Queue<byte[]>(512);
            _queue3 = new Queue<byte[]>(256);
            _queue4 = new Queue<byte[]>(128);
        }

        private void GetQueue(int count, out Queue<byte[]> queue, out int targetCount)
        {
            queue = null;
            targetCount = count;
            if (count == 4)
            {
                queue = _queue0;
            }
            else if (count <= 128)
            {
                queue = _queue1;
                targetCount = 128;
            }
            else if (count <= 256)
            {
                queue = _queue2;
                targetCount = 256;
            }
            else if (count <= 512)
            {
                queue = _queue3;
                targetCount = 512;
            }
            else if (count <= 1024)
            {
                queue = _queue4;
                targetCount = 1024;
            }
        }

        public byte[] Require(int count)
        {
            GetQueue(count, out Queue<byte[]> queue, out int targetCount);

            if (queue != null && queue.Count > 0)
            {
                byte[] bytes = queue.Dequeue();
                Console.WriteLine($"ByteBufferPool Reuse {bytes.GetHashCode()} {count} {bytes.Length} {_queue0.Count} {_queue1.Count} {_queue2.Count} {_queue3.Count} {_queue4.Count}");
                return bytes;
            }

            return new byte[targetCount];
        }

        public void Release(byte[] data)
        {
            GetQueue(data.Length, out Queue<byte[]> queue, out int targetCount);
            if (queue != null)
            {
                queue.Enqueue(data);
            }
        }

        public void Dispose()
        {
            _queue0 = null;
            _queue1 = null;
            _queue2 = null;
            _queue3 = null;
            _queue4 = null;
        }
    }
}
