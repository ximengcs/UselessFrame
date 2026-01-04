
using System;
using System.Collections.Generic;
using System.Text;

namespace UselessFrame.Net
{
    public partial class ByteStringTree
    {
        private Dictionary<byte, Node> _nodes;
        private Dictionary<string, byte[]> _contentBytes;

        public ByteStringTree(int size)
        {
            _nodes = new Dictionary<byte, Node>(size);
            _contentBytes = new Dictionary<string, byte[]>(size);
        }

        public int WriteTo(string content, Span<byte> span)
        {
            if (_contentBytes.TryGetValue(content, out byte[] bytes))
            {
                for (int i = 0; i < bytes.Length; i++)
                    span[i] = bytes[i];
                return bytes.Length;
            }
            else
            {
                return 0;
            }
        }

        public ReadOnlySpan<byte> GetBytes(string content)
        {
            if (_contentBytes.TryGetValue(content, out byte[] bytes))
                return bytes;
            return null;
        }

        public string GetString(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length <= 0)
                return null;

            byte b = bytes[0];
            if (_nodes.TryGetValue(b, out Node node))
            {
                return node.MatchString(bytes, 1);
            }
            else
            {
                return null;
            }
        }

        public bool Contains(string content)
        {
            return _contentBytes.ContainsKey(content);
        }

        public void Add(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            if (_contentBytes.ContainsKey(content))
                return;

            byte[] bytes = Encoding.UTF8.GetBytes(content);
            _contentBytes.Add(content, bytes);

            byte b = bytes[0];
            if (!_nodes.TryGetValue(b, out Node node))
            {
                node = new Node(b);
                _nodes.Add(b, node);
            }

            node.Add(content, bytes, 1);
        }
    }
}
