using NUnit.Framework;
using System;
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
    }
}

