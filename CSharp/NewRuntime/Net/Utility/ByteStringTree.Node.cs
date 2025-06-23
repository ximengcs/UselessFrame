
using System;
using System.Collections.Generic;

namespace UselessFrame.Net
{
    public partial class ByteStringTree
    {
        private class Node
        {
            private byte _data;
            private string _content;
            private Dictionary<byte, Node> _children;

            public Node(byte bit)
            {
                _data = bit;
                _children = new Dictionary<byte, Node>();
            }

            public string MatchString(ReadOnlySpan<byte> bytes, int index)
            {
                if (bytes.Length == index)
                {
                    if (_content != null)
                    {
                        return _content;
                    }
                    else
                    {
                        return null;
                    }
                }

                byte b = bytes[index];
                if (_children.TryGetValue(b, out Node node))
                {
                    return node.MatchString(bytes, index + 1);
                }
                else
                {
                    return null;
                }
            }

            public void Add(string content, byte[] bytes, int index)
            {
                if (index == bytes.Length)
                {
                    _content = content;
                    return;
                }

                byte b = bytes[index];
                if (!_children.TryGetValue(b, out Node node))
                {
                    node = new Node(b);
                    _children.Add(b, node);
                }

                node.Add(content, bytes, index + 1);
            }
        }
    }
}
