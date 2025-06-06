
using System.Collections.Generic;

namespace Core.Network
{
    public class SpanPool
    {
        private Queue<byte[]> _queue1;
        private Queue<byte[]> _queue2;
        private Queue<byte[]> _queue3;
        private Queue<byte[]> _queue4;

        public SpanPool()
        {
            _queue1 = new Queue<byte[]>(1024);
            _queue2 = new Queue<byte[]>(512);
            _queue3 = new Queue<byte[]>(256);
            _queue4 = new Queue<byte[]>(128);
        }

        private void GetQueue(int count, out Queue<byte[]> queue, out int targetCount)
        {
            queue = null;
            targetCount = count;
            if (count <= 128)
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
                return queue.Dequeue();
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
    }
}
