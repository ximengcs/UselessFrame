using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace XFrame.Modules.Archives
{
    public partial class DataArchive
    {
        private class BytesBuilder
        {
            private int m_Code;
            private MemoryStream m_Stream;
            private MemoryStream m_DataStream;
            private List<FileInfo> m_Infos;
            private int m_StreamPos;
            private int m_DataPos;

            private int m_InfoSize;
            private IntPtr m_P;

            public BytesBuilder(int code)
            {
                m_Code = code;
                m_Infos = new List<FileInfo>();
            }

            public byte[] To(Node node)
            {
                m_Infos.Clear();
                m_Stream = new MemoryStream(1024);
                m_DataStream = new MemoryStream(1024);

                m_DataPos = 0;
                InnerWriteNode(node);

                m_StreamPos = 0;
                byte[] code = BitConverter.GetBytes(m_Code);
                m_Stream.Write(code, 0, code.Length);
                m_StreamPos = code.Length;

                byte[] count = BitConverter.GetBytes(m_Infos.Count);
                m_Stream.Write(count, 0, count.Length);
                m_StreamPos += count.Length;

                int infoSize = Marshal.SizeOf<FileInfo>();
                int blockSize = infoSize * m_Infos.Count + m_StreamPos;
                IntPtr p = Marshal.AllocHGlobal(infoSize);
                byte[] buffer = new byte[infoSize];
                for (int i = 0; i < m_Infos.Count; i++)
                {
                    FileInfo info = m_Infos[i];
                    info.Offset += blockSize;

                    Marshal.StructureToPtr(info, p, false);
                    Marshal.Copy(p, buffer, 0, infoSize);

                    m_Stream.Write(buffer, 0, infoSize);
                    m_StreamPos += infoSize;
                }
                Marshal.FreeHGlobal(p);

                byte[] data = m_DataStream.ToArray();
                m_Stream.Write(data, 0, data.Length);
                data = m_Stream.ToArray();

                m_DataStream.Dispose();
                m_Stream.Dispose();
                m_DataStream = null;
                m_Stream = null;

                return data;
            }

            public Node From(byte[] data)
            {
                int intSize = Marshal.SizeOf<int>();
                m_InfoSize = Marshal.SizeOf<FileInfo>();

                m_StreamPos = intSize;
                int count = BitConverter.ToInt32(data, m_StreamPos);
                m_StreamPos += intSize;

                m_P = Marshal.AllocHGlobal(m_InfoSize);
                m_Index = 0;
                Node root = InnerReadNode(null, data, count);
                Marshal.FreeHGlobal(m_P);
                return root;
            }

            public int ReadCode(byte[] data)
            {
                return BitConverter.ToInt32(data, 0);
            }
            private int m_Index;

            private Node InnerReadNode(Node parent, byte[] data, int count)
            {
                if (m_Index >= count)
                    return parent;
                Marshal.Copy(data, m_StreamPos, m_P, m_InfoSize);
                m_StreamPos += m_InfoSize;
                FileInfo info = Marshal.PtrToStructure<FileInfo>(m_P);
                m_Index++;
                Node node = new Node(info.Type, info.Name);
                node.Parent = parent;
                if (parent != null)
                    parent.Children.Add(node.Name, node);

                if (info.Type == FileType.Directory)
                {
                    for (int i = 0; i < info.FileCount; i++)
                        InnerReadNode(node, data, count);
                    return node;
                }
                else
                {
                    byte[] buffer = new byte[info.Size];
                    Array.Copy(data, info.Offset, buffer, 0, info.Size);
                    node.Data = buffer;
                    return parent;
                }
            }

            private void InnerWriteNode(Node node)
            {
                FileInfo info = new FileInfo();
                info.Name = node.Name;
                info.Type = node.Type;
                info.FileCount = node.Children.Count;

                if (info.Type == FileType.File)
                {
                    info.Offset = m_DataPos;
                    info.Size = node.Data.Length;

                    m_DataStream.Write(node.Data, 0, node.Data.Length);
                    m_DataPos += info.Size;
                }

                m_Infos.Add(info);
                foreach (Node child in node.Children.Values)
                    InnerWriteNode(child);
            }
        }
    }
}
