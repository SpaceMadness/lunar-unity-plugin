using System;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using LunarPlugin;
using LunarPluginInternal;
using LunarEditor;
using LunarPlugin.Test;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class CommandTokenizerTest : TestFixtureBase
    {
        [Test]
        public static void TestAutoCompleteTokenEmpty()
        {
            Assert.AreEqual("", CCommandTokenizer.GetAutoCompleteToken(""));
        }

        [Test]
        public static void TestAutoCompleteTokenSingleWord()
        {
            Assert.AreEqual("test", CCommandTokenizer.GetAutoCompleteToken("test"));
        }

        [Test]
        public static void TestAutoCompleteTokenSingleWordWithSpaceAtTheEnd()
        {
            Assert.AreEqual("", CCommandTokenizer.GetAutoCompleteToken("test "));
        }
    }
}

