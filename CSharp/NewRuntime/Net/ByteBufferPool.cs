
using System;
using System.Collections.Concurrent;

namespace UselessFrame.Net
{
    public class ByteBufferPool : IDisposable
    {
        private ConcurrentQueue<byte[]> _queue0;
        private ConcurrentQueue<byte[]> _queue1;
        private ConcurrentQueue<byte[]> _queue2;
        private ConcurrentQueue<byte[]> _queue3;
        private ConcurrentQueue<byte[]> _queue4;

        public ByteBufferPool()
        {
            _queue0 = new ConcurrentQueue<byte[]>();
            _queue1 = new ConcurrentQueue<byte[]>();
            _queue2 = new ConcurrentQueue<byte[]>();
            _queue3 = new ConcurrentQueue<byte[]>();
            _queue4 = new ConcurrentQueue<byte[]>();
        }

        private void GetQueue(int count, out ConcurrentQueue<byte[]> queue, out int targetCount)
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
            GetQueue(count, out ConcurrentQueue<byte[]> queue, out int targetCount);

            if (queue != null && queue.Count > 0)
            {
                if (queue.TryDequeue(out byte[] bytes))
                    return bytes;
            }

            return new byte[targetCount];
        }

        public void Release(byte[] data)
        {
            GetQueue(data.Length, out ConcurrentQueue<byte[]> queue, out int targetCount);
            if (queue != null)
            {
                queue.Enqueue(data);
            }
        }

        public void Dispose()
        {
            _queue0.Clear();
            _queue1.Clear();
            _queue2.Clear();
            _queue3.Clear();
            _queue4.Clear();
            _queue0 = null;
            _queue1 = null;
            _queue2 = null;
            _queue3 = null;
            _queue4 = null;
        }
    }
}
