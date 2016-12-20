using System;
using System.Collections.Generic;

using NUnit.Framework;

using LunarPlugin;
using LunarPlugin.Test;
using LunarEditor;
using LunarPluginInternal;

namespace CCommandTests
{
    [TestFixture]
    public class CCommandTestCommandSplitter : TestFixtureBase
    {
        #region Single command

        [Test]
        public void TestSingleCommandSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test");
            AssertList(commands, "test");
        }

        [Test]
        public void TestSingleCommandWithArgSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test --arg");
            AssertList(commands, "test --arg");
        }

        [Test]
        public void TestSingleCommandWithArgsSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test --arg1 --arg2");
            AssertList(commands, "test --arg1 --arg2");
        }

        [Test]
        public void TestSingleCommandWithArgsAndQuotesSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test --arg1 \"argument && quotes\"");
            AssertList(commands, "test --arg1 \"argument && quotes\"");
        }

        [Test]
        public void TestSingleCommandWithArgsAndSingleQuotesSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test --arg1 'argument && quotes'");
            AssertList(commands, "test --arg1 'argument && quotes'");
        }

        [Test]
        public void TestSingleCommandWithArgsQuotesAndInnerQuotesSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test --arg1 \"argument \\\"&&\\\" quotes\"");
            AssertList(commands, "test --arg1 \"argument \\\"&&\\\" quotes\"");
        }

        [Test]
        public void TestSingleCommandWithArgsSingleQuotesAndInnerQuotesSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test --arg1 'argument \"&&\" quotes'");
            AssertList(commands, "test --arg1 'argument \"&&\" quotes'");
        }

        [Test]
        public void TestSingleCommandWithArgsQuotesAndInnerSingleQuotesSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test --arg1 \"argument '&&' quotes\"");
            AssertList(commands, "test --arg1 \"argument '&&' quotes\"");
        }

        #endregion

        #region Multiple commands

        [Test]
        public void TestMultipleCommandsSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test1 && test2");
            AssertList(commands, "test1", "test2");
        }

        [Test]
        public void TestMultipleCommandsWithArgSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test1 --arg1 && test2 --arg2");
            AssertList(commands, "test1 --arg1", "test2 --arg2");
        }

        [Test]
        public void TestMultipleCommandsWithArgsSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test1 --arg1 --arg2 && test2 --arg3 --arg4");
            AssertList(commands, "test1 --arg1 --arg2", "test2 --arg3 --arg4");
        }

        [Test]
        public void TestMultipleCommandsWithArgsAndQuotesSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test1 --arg1 \"a1 && a2\" && test2 --arg2 \"b1 && b2\"");
            AssertList(commands, "test1 --arg1 \"a1 && a2\"", "test2 --arg2 \"b1 && b2\"");
        }

        [Test]
        public void TestMultipleCommandsWithArgsAndSingleQuotesSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test1 --arg1 'a1 && a2' && test2 --arg2 'b1 && b2'");
            AssertList(commands, "test1 --arg1 'a1 && a2'", "test2 --arg2 'b1 && b2'");
        }

        [Test]
        public void TestMultipleCommandsWithArgsQuotesAndInnerQuotesSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test1 --arg1 \"a1 \\\"&&\\\" a2\" && test2 --arg2 \"b1 \\\"&&\\\" b2\"");
            AssertList(commands, "test1 --arg1 \"a1 \\\"&&\\\" a2\"", "test2 --arg2 \"b1 \\\"&&\\\" b2\"");
        }

        [Test]
        public void TestMultipleCommandsWithArgsQuotesAndInnerSingleQuotesSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test --arg1 \"argument '&&' quotes\"");
            AssertList(commands, "test --arg1 \"argument '&&' quotes\"");
        }

        [Test]
        public void TestMultipleCommandsWithArgsSingleQuotesAndInnerQuotesSplit()
        {
            IList<string> commands = CCommandSplitter.Split("test1 --arg1 'a1 \"&&\" a2' && test2 --arg2 'b1 \"&&\" b2'");
            AssertList(commands, "test1 --arg1 'a1 \"&&\" a2'", "test2 --arg2 'b1 \"&&\" b2'");
        }

        [Test]
        public void TestMultipleCommandsWithNoSpaces()
        {
            IList<string> commands = CCommandSplitter.Split("test1&&test2");
            AssertList(commands, "test1", "test2");
        }

        [Test]
        public void TestMultipleCommandsWithNoSpacesAndArgs()
        {
            IList<string> commands = CCommandSplitter.Split("test1 --arg1&&test2 --arg2");
            AssertList(commands, "test1 --arg1", "test2 --arg2");
        }

        [Test]
        public void TestMoar()
        {
            IList<string> commands = CCommandSplitter.Split("bind t 'echo \"test-1\" && echo \"test-2\"'");
            AssertList(commands, "bind t 'echo \"test-1\" && echo \"test-2\"'");
        }

        #endregion
    }
}

