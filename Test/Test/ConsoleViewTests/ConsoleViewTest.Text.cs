using UnityEngine;

using System;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using NUnit.Framework;

namespace ConsoleViewTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public partial class ConsoleViewTextTest : ConsoleViewTest
    {
        [Test]
        public void TestCopyText()
        {
            string expected = "line 1\n" +
                "line 2\n" +
                "line 3\n" +
                "line 4";

            string[] lines = expected.Split('\n');

            MockConsole console = new MockConsole();
            for (int i = 0; i < lines.Length; ++i)
            {
                console.Add(CLogLevel.Debug, tag, lines[i]);
            }

            ConsoleView consoleView = new MockConsoleView(console, 320, 230);
            string actual = consoleView.GetText();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestCopyPartialText()
        {
            string text = "line 1\n" +
                "line 2\n" +
                "line 3\n" +
                "line 4";

            string[] lines = text.Split('\n');

            MockConsole console = new MockConsole();
            for (int i = 0; i < lines.Length; ++i)
            {
                console.Add(CLogLevel.Debug, tag, lines[i]);
            }

            ConsoleView consoleView = new MockConsoleView(console, 320, 230);
            string actual = consoleView.GetText(1, 2);

            string expected = "line 2\n" +
                "line 3";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestCopyOverflowText()
        {
            string text = "line 1\n" +
                "line 2\n" +
                "line 3\n" +
                "line 4\n" +
                "line 5\n" +
                "line 6";

            string[] lines = text.Split('\n');

            MockConsole console = new MockConsole(3);
            for (int i = 0; i < lines.Length; ++i)
            {
                console.Add(CLogLevel.Debug, tag, lines[i]);
            }

            ConsoleView consoleView = new MockConsoleView(console, 320, 230);
            string actual = consoleView.GetText();

            string expected = "line 4\n" +
                "line 5\n" +
                "line 6";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestCopyOverflowPartialText()
        {
            string text = "line 1\n" +
                "line 2\n" +
                "line 3\n" +
                "line 4\n" +
                "line 5\n" +
                "line 6";

            string[] lines = text.Split('\n');

            MockConsole console = new MockConsole(4);
            for (int i = 0; i < lines.Length; ++i)
            {
                console.Add(CLogLevel.Debug, tag, lines[i]);
            }

            ConsoleView consoleView = new MockConsoleView(console, 320, 230);
            string actual = consoleView.GetText(3, 2);

            string expected = "line 4\n" +
                "line 5";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestCopyRichText()
        {
            string expected = "line 1\n" +
                "line 2\n" +
                "line 3\n" +
                "line 4";

            string[] lines = expected.Split('\n');

            MockConsole console = new MockConsole();
            for (int i = 0; i < lines.Length; ++i)
            {
                string line = StringUtils.C(lines[i], ColorCode.LevelDebug);
                console.Add(CLogLevel.Debug, tag, line);
            }

            ConsoleView consoleView = new MockConsoleView(console, 320, 230);
            string actual = consoleView.GetText();

            Assert.AreEqual(expected, actual);
        }
    }
}

