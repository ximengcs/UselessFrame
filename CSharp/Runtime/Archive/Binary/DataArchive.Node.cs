using System;
using System.IO;
using XFrame.Utility;
using XFrame.Modules.Diagnotics;
using System.Collections.Generic;

namespace XFrame.Modules.Archives
{
    public partial class DataArchive
    {
        private class Node : IDisposable
        {
            public FileType Type;
            public string Name;
            public byte[] Data;

            public Node Parent;
            public Dictionary<string, Node> Children;

            public Node(FileType type, string name)
            {
                Name = name;
                Type = type;
                Children = new Dictionary<string, Node>();
            }

            public void Write(string path, byte[] data)
            {
                Node child = InnerGetOrNew(FileType.File, path);
                child.Data = data;
            }

            public byte[] Read(string path)
            {
                Node child = InnerGet(path);
                if (child != null && child.Type == FileType.File)
                    return child.Data;
                else
                {
                    Log.Warning(Log.XFrame, "Read Error");
                    return default;
                }
            }

            public void AddFolder(string path)
            {
                InnerGetOrNew(FileType.Directory, path);
            }

            public void Delete(string path)
            {
                Node child = InnerGet(path);
                if (child != null)
                {
                    child.Parent.Children.Remove(child.Name);
                    child.Dispose();
                }
            }

            public void Export(string path)
            {
                InnerExport(this, path);
            }

            public void Import(string path)
            {
                InnerImport(this, path);
            }

            public void Dispose()
            {
                foreach (Node node in Children.Values)
                    node.Dispose();

                Name = null;
                Data = null;
                Parent = null;
                Children = null;
            }

            private void InnerImport(Node node, string curPath)
            {
                if (node != null)
                {
                    foreach (string dir in Directory.EnumerateDirectories(curPath))
                    {
                        string curName = Path.GetFileName(dir);
                        if (!node.Children.TryGetValue(curName, out Node child))
                        {
                            child = new Node(FileType.Directory, curName);
                            node.Children.Add(child.Name, child);
                            child.Parent = node;
                        }

                        InnerImport(child, dir);
                    }

                    foreach (string file in Directory.EnumerateFiles(curPath))
                    {
                        string curName = Path.GetFileName(file);
                        if (!node.Children.ContainsKey(curName))
                        {
                            Node child = new Node(FileType.File, curName);
                            node.Children.Add(child.Name, child);
                            child.Parent = node;
                            child.Data = File.ReadAllBytes(file);
                        }
                    }
                }
            }

            private void InnerExport(Node node, string curPath)
            {
                if (node != null)
                {
                    if (node.Type == FileType.Directory)
                    {
                        if (!string.IsNullOrEmpty(node.Name))
                            curPath = Path.Combine(curPath, node.Name);
                        if (!Directory.Exists(curPath))
                            Directory.CreateDirectory(curPath);
                        foreach (Node child in node.Children.Values)
                            InnerExport(child, curPath);
                    }
                    else
                    {
                        if (!Directory.Exists(curPath))
                            Directory.CreateDirectory(curPath);
                        curPath = Path.Combine(curPath, node.Name);
                        File.WriteAllBytes(curPath, node.Data);
                    }
                }
            }

            private Node InnerGet(string path)
            {
                int count = PathUtility.CheckFileName(path, out string thisName, out string suplusName);
                if (count > 0)
                {
                    if (count == 1)
                    {
                        if (Children.TryGetValue(thisName, out Node node))
                            return node;
                        else
                            return default;
                    }
                    else
                    {
                        if (Children.TryGetValue(thisName, out Node node))
                            return node.InnerGet(suplusName);
                        else
                            return default;
                    }
                }
                else
                {
                    return default;
                }
            }

            private Node InnerGetOrNew(FileType type, string path)
            {
                int count = PathUtility.CheckFileName(path, out string thisName, out string suplusName);
                if (count > 0)
                {
                    if (count == 1)
                    {
                        if (!Children.TryGetValue(thisName, out Node node))
                        {
                            node = new Node(type, thisName);
                            Children.Add(thisName, node);
                            node.Parent = this;
                        }
                        return node;
                    }
                    else
                    {
                        if (!Children.TryGetValue(thisName, out Node node))
                        {
                            node = new Node(FileType.Directory, thisName);
                            Children.Add(thisName, node);
                            node.Parent = this;
                        }

                        return node.InnerGetOrNew(type, suplusName);
                    }
                }
                else
                {
                    return default;
                }
            }
        }
    }
}
