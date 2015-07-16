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
            Assert.AreEqual("", CommandTokenizer.GetAutoCompleteToken(""));
        }

        [Test]
        public static void TestAutoCompleteTokenSingleWord()
        {
            Assert.AreEqual("test", CommandTokenizer.GetAutoCompleteToken("test"));
        }

        [Test]
        public static void TestAutoCompleteTokenSingleWordWithSpaceAtTheEnd()
        {
            Assert.AreEqual("", CommandTokenizer.GetAutoCompleteToken("test "));
        }
    }
}

