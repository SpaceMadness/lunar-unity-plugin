using NUnit.Framework;

using System;
using System.IO;
using System.Collections.Generic;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

namespace LunarPlugin.Test
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class FileUtilsTest
    {
        [Test]
        public void TestFileExt()
        {
            string path = "/usr/tmp/file.txt";
            Assert.AreEqual(".txt", FileUtils.GetExtension(path));
            Assert.AreEqual("file", FileUtils.GetFileNameWithoutExtension(path));
        }

        [Test]
        public void TestFileEmptyExt()
        {
            string path = "/usr/tmp/file";
            Assert.AreEqual("", FileUtils.GetExtension(path));
            Assert.AreEqual("file", FileUtils.GetFileNameWithoutExtension(path));
        }

        [Test]
        public void TestFileReplaceExt()
        {
            string path = "autoexec.cfg";
            Assert.AreEqual("autoexec.cfg", FileUtils.ChangeExtension(path, ".cfg"));

            path = "usr/bin/autoexec.cfg";
            Assert.AreEqual("usr/bin/autoexec.cfg", FileUtils.ChangeExtension(path, ".cfg"));

            path = "usr/bin/autoexec.CFG";
            Assert.AreEqual("usr/bin/autoexec.cfg", FileUtils.ChangeExtension(path, ".cfg"));

            path = "usr/bin/autoexec";
            Assert.AreEqual("usr/bin/autoexec.cfg", FileUtils.ChangeExtension(path, ".cfg"));
        }

        [Test]
        public void TestDeleteFiles()
        {
            string rootPath = Path.Combine(Path.GetTempPath(), "folder");
            if (Directory.Exists(rootPath)) Directory.Delete(rootPath, true);

            Directory.CreateDirectory(rootPath);

            string path1 = Path.Combine(rootPath, "file1.txt");
            FileUtils.Write(path1, "text");

            string path2 = Path.Combine(rootPath, "file2.txt");
            FileUtils.Write(path2, "text");

            Assert.IsTrue(FileUtils.FileExists(path1));
            Assert.IsTrue(FileUtils.FileExists(path2));

            Assert.IsTrue(FileUtils.Delete(path1));
            Assert.IsFalse(FileUtils.FileExists(path1));
            Assert.IsFalse(FileUtils.Delete(path1));

            Assert.IsTrue(FileUtils.Delete(rootPath));
            Assert.IsFalse(FileUtils.FileExists(rootPath));
            Assert.IsFalse(FileUtils.Delete(rootPath));

            Assert.IsFalse(FileUtils.FileExists(path2));
            Assert.IsFalse(FileUtils.Delete(path2));
        }
    }
}

