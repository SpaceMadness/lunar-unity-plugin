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
    public partial class ConsoleViewClearTest : ConsoleViewTest
    {
        [Test]
        public void TestClearAndAddItems()
        {
            MockConsole console = new MockConsole();
            console.Add(LogLevel.Debug, tag, "line1");
            console.Add(LogLevel.Debug, tag, "line11");
            console.Add(LogLevel.Debug, tag, "line111");
            console.Add(LogLevel.Debug, tag, "line1111");
            console.Add(LogLevel.Debug, tag, "foo");

            ConsoleView consoleView = new MockConsoleView(console, 320, 230);

            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111", "foo");
            Assert.AreEqual(5, consoleView.RowsCount);

            console.Clear();
            AssertVisibleRows(consoleView);
            Assert.AreEqual(0, consoleView.RowsCount);

            console.Add(LogLevel.Debug, tag, "line12");
            console.Add(LogLevel.Debug, tag, "line112");
            console.Add(LogLevel.Debug, tag, "line1112");
            console.Add(LogLevel.Debug, tag, "line11112");
            console.Add(LogLevel.Debug, tag, "foo1");
            console.Add(LogLevel.Debug, tag, "foo2");

            AssertVisibleRows(consoleView, "line12", "line112", "line1112", "line11112", "foo1", "foo2");
            Assert.AreEqual(6, consoleView.RowsCount);
        }
    }
}

