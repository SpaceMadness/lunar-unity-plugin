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
            Assert.AreEqual(".txt", CFileUtils.GetExtension(path));
            Assert.AreEqual("file", CFileUtils.GetFileNameWithoutExtension(path));
        }

        [Test]
        public void TestFileEmptyExt()
        {
            string path = "/usr/tmp/file";
            Assert.AreEqual("", CFileUtils.GetExtension(path));
            Assert.AreEqual("file", CFileUtils.GetFileNameWithoutExtension(path));
        }

        [Test]
        public void TestFileReplaceExt()
        {
            string path = "autoexec.cfg";
            Assert.AreEqual("autoexec.cfg", CFileUtils.ChangeExtension(path, ".cfg"));

            path = "usr/bin/autoexec.cfg";
            Assert.AreEqual("usr/bin/autoexec.cfg", CFileUtils.ChangeExtension(path, ".cfg"));

            path = "usr/bin/autoexec.CFG";
            Assert.AreEqual("usr/bin/autoexec.cfg", CFileUtils.ChangeExtension(path, ".cfg"));

            path = "usr/bin/autoexec";
            Assert.AreEqual("usr/bin/autoexec.cfg", CFileUtils.ChangeExtension(path, ".cfg"));
        }

        [Test]
        public void TestDeleteFiles()
        {
            string rootPath = Path.Combine(Path.GetTempPath(), "folder");
            if (Directory.Exists(rootPath)) Directory.Delete(rootPath, true);

            Directory.CreateDirectory(rootPath);

            string path1 = Path.Combine(rootPath, "file1.txt");
            CFileUtils.Write(path1, "text");

            string path2 = Path.Combine(rootPath, "file2.txt");
            CFileUtils.Write(path2, "text");

            Assert.IsTrue(CFileUtils.FileExists(path1));
            Assert.IsTrue(CFileUtils.FileExists(path2));

            Assert.IsTrue(CFileUtils.Delete(path1));
            Assert.IsFalse(CFileUtils.FileExists(path1));
            Assert.IsFalse(CFileUtils.Delete(path1));

            Assert.IsTrue(CFileUtils.Delete(rootPath));
            Assert.IsFalse(CFileUtils.FileExists(rootPath));
            Assert.IsFalse(CFileUtils.Delete(rootPath));

            Assert.IsFalse(CFileUtils.FileExists(path2));
            Assert.IsFalse(CFileUtils.Delete(path2));
        }
    }
}

