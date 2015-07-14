using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using LunarPlugin;

namespace LunarPluginInternal
{
    static class FileUtils
    {
        public static bool FileExists(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("Path is null");
            }

            return File.Exists(path) || Directory.Exists(path);
        }

        public static string GetAbsolutePath(string path)
        {
            return Path.GetFullPath(path);
        }

        public static bool Delete(string path)
        {
            string absolutePath = GetAbsolutePath(path);
            if (FileExists(absolutePath))
            {
                System.IO.File.Delete(absolutePath);
                return !FileExists(absolutePath);
            }

            return false;
        }

        public static IList<string> Read(string path)
        {
            string absolutePath = GetAbsolutePath(path);
            
            if (!FileExists(absolutePath))
            {
                throw new IOException("File does not exist: " + path);
            }
            
            using (StreamReader reader = new StreamReader(absolutePath, Encoding.UTF8))
            {
                IList<string> lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                
                return lines;
            }
        }
        
        public static void Write(string filename, IList<string> lines)
        {
            string path = GetAbsolutePath(filename);

            DirectoryInfo parent = Directory.GetParent(path);
            if (parent == null)
            {
                throw new IOException("Can't resolve parent directory: " + path);
            }

            if (!parent.Exists)
            {
                parent.Create();

                if (!parent.Exists)
                {
                    throw new IOException("Can't create parent directory: " + parent);
                }
            }

            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public static void Write(string filename, string text)
        {
            string path = GetAbsolutePath(filename);

            DirectoryInfo parent = Directory.GetParent(path);
            if (parent == null)
            {
                throw new IOException("Can't resolve parent directory: " + path);
            }

            if (!parent.Exists)
            {
                parent.Create();

                if (!parent.Exists)
                {
                    throw new IOException("Can't create parent directory: " + parent);
                }
            }

            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.Write(text);
            }
        }
        
        public static System.IO.Stream OpenRead(string path)
        {
            string absolutePath = GetAbsolutePath(path);
            return System.IO.File.OpenRead(absolutePath);
        }
        
        public static System.IO.Stream OpenWrite(string path)
        {
            string absolutePath = GetAbsolutePath(path);
            return System.IO.File.OpenWrite(absolutePath);
        }

        //////////////////////////////////////////////////////////////////////////////

        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public static string ChangeExtension(string path, string ext)
        {
            return Path.ChangeExtension(path, ext);
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static bool IsPathRooted(string path)
        {
            return Path.IsPathRooted(path);
        }

        //////////////////////////////////////////////////////////////////////////////

        public static string FixPath(string path)
        {
            return FixSlashes(path);
        }
        
        public static string FixPath(params string[] moreComponents)
        {
            if (moreComponents.Length > 1)
            {
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < moreComponents.Length; ++i)
                {
                    result.Append(moreComponents [i]);
                    if (i < moreComponents.Length - 1 && result.Length > 0 && result[result.Length - 1] != '/')
                    {
                        result.Append('/');
                    }
                }
                
                return FixPath(result.ToString());
            }
            
            return FixPath(moreComponents[0]);
        }

        private static string FixSlashes(string path)
        {
            return path.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString());
        }

        //////////////////////////////////////////////////////////////////////////////

        public static string GetDataSubpath(string subpath)
        {
            return Path.Combine(DataPath, subpath);
        }

        public static string DataPath
        {
            get { return Path.Combine(UnityEngine.Application.persistentDataPath, "Lunar"); }
        }
    }
}