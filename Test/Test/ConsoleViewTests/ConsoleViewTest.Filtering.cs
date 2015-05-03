using UnityEngine;

using System;
using System.Collections.Generic;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using LunarPlugin.Test;
using NUnit.Framework;

namespace ConsoleViewTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public partial class ConsoleViewFilteringTest : ConsoleViewTest
    {
        [Test]
        public void TestFilterPriority()
        {
            List<int> priorities = new List<int>();

            ConsoleViewFilterBase filter1 = new TestFilter(1, delegate(ref ConsoleViewCellEntry e)
            {
                priorities.Add(1);
                return true;
            });

            ConsoleViewFilterBase filter2 = new TestFilter(2, delegate(ref ConsoleViewCellEntry e)
            {
                priorities.Add(2);
                return true;
            });

            ConsoleViewFilterBase filter3 = new TestFilter(3, delegate(ref ConsoleViewCellEntry e)
            {
                priorities.Add(3);
                return true;
            });

            ConsoleViewCompositeFilter compositeFilter = new ConsoleViewCompositeFilter();
            compositeFilter.AddFilter(filter2);
            compositeFilter.AddFilter(filter1);
            compositeFilter.AddFilter(filter3);
            compositeFilter.AddFilter(filter1);
            compositeFilter.AddFilter(filter2);
            compositeFilter.AddFilter(filter3);

            ConsoleViewCellEntry entry = default(ConsoleViewCellEntry);
            compositeFilter.Apply(ref entry);

            AssertList(priorities, 3, 2, 1);
        }

        [Test]
        public void TestCompositeFilterApply()
        {
            List<int> priorities = new List<int>();

            ConsoleViewFilterBase filter1 = new TestFilter(1, delegate(ref ConsoleViewCellEntry e)
            {
                priorities.Add(1);
                return true;
            });

            ConsoleViewFilterBase filter2 = new TestFilter(2, delegate(ref ConsoleViewCellEntry e)
            {
                priorities.Add(2);
                return false;
            });

            ConsoleViewFilterBase filter3 = new TestFilter(3, delegate(ref ConsoleViewCellEntry e)
            {
                priorities.Add(3);
                return true;
            });

            ConsoleViewCompositeFilter compositeFilter = new ConsoleViewCompositeFilter();
            compositeFilter.AddFilter(filter2);
            compositeFilter.AddFilter(filter1);
            compositeFilter.AddFilter(filter3);
            compositeFilter.AddFilter(filter1);
            compositeFilter.AddFilter(filter2);
            compositeFilter.AddFilter(filter3);

            ConsoleViewCellEntry entry = default(ConsoleViewCellEntry);
            compositeFilter.Apply(ref entry);

            AssertList(priorities, 3, 2);
        }

        [Test]
        public void TestCompositeFilterRemove()
        {
            List<int> priorities = new List<int>();

            ConsoleViewFilterBase filter1 = new TestFilter(1, delegate(ref ConsoleViewCellEntry e)
            {
                priorities.Add(1);
                return true;
            });

            ConsoleViewFilterBase filter2 = new TestFilter(2, delegate(ref ConsoleViewCellEntry e)
            {
                priorities.Add(2);
                return true;
            });

            ConsoleViewFilterBase filter3 = new TestFilter(3, delegate(ref ConsoleViewCellEntry e)
            {
                priorities.Add(3);
                return true;
            });

            ConsoleViewCompositeFilter compositeFilter = new ConsoleViewCompositeFilter();
            compositeFilter.AddFilter(filter1);
            compositeFilter.AddFilter(filter2);
            compositeFilter.AddFilter(filter3);

            compositeFilter.RemoveFilter(filter1);
            compositeFilter.RemoveFilter(filter3);

            ConsoleViewCellEntry entry = default(ConsoleViewCellEntry);
            compositeFilter.Apply(ref entry);

            AssertList(priorities, 2);
        }

        [Test]
        public void TestFiltering()
        {
            MockConsole console = new MockConsole();
            console.Add(LogLevel.Debug, tag, "line1");
            console.Add(LogLevel.Debug, tag, "line11");
            console.Add(LogLevel.Debug, tag, "line111");
            console.Add(LogLevel.Debug, tag, "line1111");
            console.Add(LogLevel.Debug, tag, "foo");

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterText("l");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111");

            consoleView.SetFilterText("li");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111");

            consoleView.SetFilterText("lin");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111");

            consoleView.SetFilterText("line");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111");

            consoleView.SetFilterText("line1");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111");

            consoleView.SetFilterText("line11");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111", "line1111");

            consoleView.SetFilterText("line111");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line111", "line1111");

            consoleView.SetFilterText("line1111");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1111");

            consoleView.SetFilterText("line11111");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView);

            consoleView.SetFilterText("line1111");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1111");

            consoleView.SetFilterText("line111");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line111", "line1111");

            consoleView.SetFilterText("line11");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111", "line1111");

            consoleView.SetFilterText("line1");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111");

            consoleView.SetFilterText("line");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111");

            consoleView.SetFilterText("lin");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111");

            consoleView.SetFilterText("li");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111");

            consoleView.SetFilterText("l");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111");

            consoleView.SetFilterText("");
            Assert.IsFalse(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line1111", "foo");
        }

        [Test]
        public void TestFilterAndAdd()
        {
            MockConsole console = new MockConsole();

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterText("line11");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView);

            console.Add(LogLevel.Debug, tag, "line1");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView);

            console.Add(LogLevel.Debug, tag, "line11");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11");

            console.Add(LogLevel.Debug, tag, "line111");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111");

            console.Add(LogLevel.Debug, tag, "line12");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111");

            console.Add(LogLevel.Debug, tag, "foo1");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111");

            console.Add(LogLevel.Debug, tag, "foo12");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111");

            console.Add(LogLevel.Debug, tag, "foo123");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111");

            consoleView.SetFilterText("");
            Assert.IsFalse(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line12", "foo1", "foo12", "foo123");

            consoleView.SetFilterText("foo1");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "foo1", "foo12", "foo123");

            consoleView.SetFilterText("foo12");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "foo12", "foo123");

            consoleView.SetFilterText("foo123");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "foo123");

            consoleView.SetFilterText("foo1234");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView);

            consoleView.SetFilterText("");
            Assert.IsFalse(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1", "line11", "line111", "line12", "foo1", "foo12", "foo123");
        }

        [Test]
        public void TestFilteringAndClear()
        {
            MockConsole console = new MockConsole();
            console.Add(LogLevel.Debug, tag, "line1");
            console.Add(LogLevel.Debug, tag, "line11");
            console.Add(LogLevel.Debug, tag, "line111");
            console.Add(LogLevel.Debug, tag, "line1111");
            console.Add(LogLevel.Debug, tag, "foo");

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterText("line111");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line111", "line1111");

            console.Clear();
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView);

            console.Add(LogLevel.Debug, tag, "line12");
            console.Add(LogLevel.Debug, tag, "line112");
            console.Add(LogLevel.Debug, tag, "line1112");
            console.Add(LogLevel.Debug, tag, "line11112");
            console.Add(LogLevel.Debug, tag, "foo2");

            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line1112", "line11112");

            consoleView.SetFilterText("");
            Assert.IsFalse(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line12", "line112", "line1112", "line11112", "foo2");
        }

        [Test]
        public void TestFilterAndOverflow()
        {
            MockConsole console = new MockConsole(10);
            console.Add(LogLevel.Debug, tag, "a1");
            console.Add(LogLevel.Debug, tag, "a2");
            console.Add(LogLevel.Debug, tag, "a3");
            console.Add(LogLevel.Debug, tag, "a4");
            console.Add(LogLevel.Debug, tag, "b1");
            console.Add(LogLevel.Debug, tag, "b2");

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterText("a");
            AssertVisibleRows(consoleView, "a1", "a2", "a3", "a4");

            console.Add(LogLevel.Debug, tag, "b3");
            AssertVisibleRows(consoleView, "a1", "a2", "a3", "a4");

            console.Add(LogLevel.Debug, tag, "a5");
            AssertVisibleRows(consoleView, "a1", "a2", "a3", "a4", "a5");

            console.Add(LogLevel.Debug, tag, "b4");
            AssertVisibleRows(consoleView, "a1", "a2", "a3", "a4", "a5");

            console.Add(LogLevel.Debug, tag, "a6");
            AssertVisibleRows(consoleView, "a1", "a2", "a3", "a4", "a5", "a6");

            console.Add(LogLevel.Debug, tag, "b5");
            AssertVisibleRows(consoleView, "a2", "a3", "a4", "a5", "a6");

            console.Add(LogLevel.Debug, tag, "b6");
            AssertVisibleRows(consoleView, "a3", "a4", "a5", "a6");

            console.Add(LogLevel.Debug, tag, "a7");
            AssertVisibleRows(consoleView, "a4", "a5", "a6", "a7");

            console.Add(LogLevel.Debug, tag, "a8");
            AssertVisibleRows(consoleView, "a5", "a6", "a7", "a8");

            console.Add(LogLevel.Debug, tag, "a9");
            AssertVisibleRows(consoleView, "a5", "a6", "a7", "a8", "a9");

            console.Add(LogLevel.Debug, tag, "b7");
            AssertVisibleRows(consoleView, "a5", "a6", "a7", "a8", "a9");

            console.Add(LogLevel.Debug, tag, "b8");
            AssertVisibleRows(consoleView, "a5", "a6", "a7", "a8", "a9");

            console.Add(LogLevel.Debug, tag, "a10");
            AssertVisibleRows(consoleView, "a6", "a7", "a8", "a9", "a10");

            console.Add(LogLevel.Debug, tag, "b9");
            AssertVisibleRows(consoleView, "a6", "a7", "a8", "a9", "a10");

            console.Add(LogLevel.Debug, tag, "b10");
            AssertVisibleRows(consoleView, "a7", "a8", "a9", "a10");

            console.Add(LogLevel.Debug, tag, "b11");
            AssertVisibleRows(consoleView, "a7", "a8", "a9", "a10");

            console.Add(LogLevel.Debug, tag, "b12");
            AssertVisibleRows(consoleView, "a7", "a8", "a9", "a10");

            console.Add(LogLevel.Debug, tag, "b13");
            AssertVisibleRows(consoleView, "a8", "a9", "a10");

            console.Add(LogLevel.Debug, tag, "b14");
            AssertVisibleRows(consoleView, "a9", "a10");

            console.Add(LogLevel.Debug, tag, "b15");
            AssertVisibleRows(consoleView, "a10");

            console.Add(LogLevel.Debug, tag, "b16");
            AssertVisibleRows(consoleView, "a10");

            console.Add(LogLevel.Debug, tag, "b17");
            AssertVisibleRows(consoleView, "a10");

            console.Add(LogLevel.Debug, tag, "b18");
            AssertVisibleRows(consoleView);

            console.Add(LogLevel.Debug, tag, "a11");
            AssertVisibleRows(consoleView, "a11");

            console.Add(LogLevel.Debug, tag, "b19");
            AssertVisibleRows(consoleView, "a11");

            console.Add(LogLevel.Debug, tag, "a12");
            AssertVisibleRows(consoleView, "a11", "a12");

            consoleView.SetFilterText("");
            AssertVisibleRows(consoleView, "b12", "b13", "b14", "b15", "b16", "b17", "b18", "a11", "b19", "a12");
        }

        [Test]
        public void TestFilterByLogLevel()
        {
            MockConsole console = new MockConsole(10);
            LogLevel[] levels = {
                LogLevel.Verbose,
                LogLevel.Debug,
                LogLevel.Info,
                LogLevel.Warn,
                LogLevel.Error,
                LogLevel.Exception
            };

            foreach (LogLevel l in levels)
            {
                console.Add(l, tag, l.Name);
            }

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterLogLevel(LogLevel.Verbose);
            Assert.IsFalse(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                LogLevel.Verbose.Name,
                LogLevel.Debug.Name,
                LogLevel.Info.Name,
                LogLevel.Warn.Name,
                LogLevel.Error.Name,
                LogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(LogLevel.Debug);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                LogLevel.Debug.Name,
                LogLevel.Info.Name,
                LogLevel.Warn.Name,
                LogLevel.Error.Name,
                LogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(LogLevel.Info);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                LogLevel.Info.Name,
                LogLevel.Warn.Name,
                LogLevel.Error.Name,
                LogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(LogLevel.Warn);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                LogLevel.Warn.Name,
                LogLevel.Error.Name,
                LogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(LogLevel.Error);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                LogLevel.Error.Name,
                LogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(LogLevel.Exception);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                LogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(LogLevel.Error);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                LogLevel.Error.Name,
                LogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(LogLevel.Warn);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                LogLevel.Warn.Name,
                LogLevel.Error.Name,
                LogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(LogLevel.Info);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                LogLevel.Info.Name,
                LogLevel.Warn.Name,
                LogLevel.Error.Name,
                LogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(LogLevel.Debug);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                LogLevel.Debug.Name,
                LogLevel.Info.Name,
                LogLevel.Warn.Name,
                LogLevel.Error.Name,
                LogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(LogLevel.Verbose);
            Assert.IsFalse(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                LogLevel.Verbose.Name,
                LogLevel.Debug.Name,
                LogLevel.Info.Name,
                LogLevel.Warn.Name,
                LogLevel.Error.Name,
                LogLevel.Exception.Name
            );
        }

        [Test]
        public void TestFilterByTag()
        {
            Tag tag1 = new Tag("tag1");
            Tag tag2 = new Tag("tag2");
            Tag tag3 = new Tag("tag3");

            MockConsole console = new MockConsole(10);

            console.Add(LogLevel.Debug, tag1, "tag1-1");
            console.Add(LogLevel.Debug, tag2, "tag2-1");
            console.Add(LogLevel.Debug, tag1, "tag1-2");
            console.Add(LogLevel.Debug, tag3, "tag3-1");
            console.Add(LogLevel.Debug, tag1, "tag1-3");

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            AssertVisibleRows(consoleView,
                "tag1-1",
                "tag2-1",
                "tag1-2",
                "tag3-1",
                "tag1-3"
            );

            consoleView.AddFilterTags(tag1);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                "tag1-1",
                "tag1-2",
                "tag1-3"
            );

            consoleView.AddFilterTags(tag2);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                "tag1-1",
                "tag2-1",
                "tag1-2",
                "tag1-3"
            );

            consoleView.AddFilterTags(tag3);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                "tag1-1",
                "tag2-1",
                "tag1-2",
                "tag3-1",
                "tag1-3"
            );

            consoleView.RemoveFilterTags(tag1);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                "tag2-1",
                "tag3-1"
            );

            consoleView.RemoveFilterTags(tag2);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                "tag3-1"
            );

            consoleView.RemoveFilterTags(tag3);
            Assert.IsFalse(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                "tag1-1",
                "tag2-1",
                "tag1-2",
                "tag3-1",
                "tag1-3"
            );
        }

        [Test]
        public void TestMixedFiltersLevelTextTag()
        {
            Tag tag1 = new Tag("tag1");
            Tag tag2 = new Tag("tag2");
            Tag tag3 = new Tag("tag3");

            MockConsole console = new MockConsole(64);

            console.Add(LogLevel.Exception, tag1, "tog1-exception");
            console.Add(LogLevel.Exception, tag1, "tag1-exception");

            console.Add(LogLevel.Error, tag2, "tog2-error");
            console.Add(LogLevel.Error, tag2, "tag2-error");

            console.Add(LogLevel.Warn, tag3, "tog3-warning");
            console.Add(LogLevel.Warn, tag3, "tag3-warning");

            console.Add(LogLevel.Info, tag1, "tog1-info");
            console.Add(LogLevel.Info, tag1, "tag1-info");

            console.Add(LogLevel.Debug, tag2, "tog2-debug");
            console.Add(LogLevel.Debug, tag2, "tag2-debug");

            console.Add(LogLevel.Verbose, tag3, "tog3-verbose");
            console.Add(LogLevel.Verbose, tag3, "tag3-verbose");

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterLogLevel(LogLevel.Warn);
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tog1-exception",
                "tag1-exception",
                "tog2-error",
                "tag2-error",
                "tog3-warning",
                "tag3-warning"
            );

            consoleView.SetFilterText("tag");
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tag1-exception",
                "tag2-error",
                "tag3-warning"
            );

            consoleView.SetFilterTags(tag1, tag3);
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tag1-exception",
                "tag3-warning"
            );

            consoleView.SetFilterLogLevel(LogLevel.Verbose);
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tag1-exception",
                "tag3-warning",
                "tag1-info",
                "tag3-verbose"
            );

            consoleView.SetFilterText("");
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tog1-exception",
                "tag1-exception",
                "tog3-warning",
                "tag3-warning",
                "tog1-info",
                "tag1-info",
                "tog3-verbose",
                "tag3-verbose"
            );

            consoleView.SetFilterTags();
            Assert.IsFalse(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tog1-exception",
                "tag1-exception",
                "tog2-error",
                "tag2-error",
                "tog3-warning",
                "tag3-warning",
                "tog1-info",
                "tag1-info",
                "tog2-debug",
                "tag2-debug",
                "tog3-verbose",
                "tag3-verbose"
            );
        }

        [Test]
        public void TestMixedFiltersTextLevelTag()
        {
            Tag tag1 = new Tag("tag1");
            Tag tag2 = new Tag("tag2");
            Tag tag3 = new Tag("tag3");

            MockConsole console = new MockConsole(64);

            console.Add(LogLevel.Exception, tag1, "tog1-exception");
            console.Add(LogLevel.Exception, tag1, "tag1-exception");

            console.Add(LogLevel.Error, tag2, "tog2-error");
            console.Add(LogLevel.Error, tag2, "tag2-error");

            console.Add(LogLevel.Warn, tag3, "tog3-warning");
            console.Add(LogLevel.Warn, tag3, "tag3-warning");

            console.Add(LogLevel.Info, tag1, "tog1-info");
            console.Add(LogLevel.Info, tag1, "tag1-info");

            console.Add(LogLevel.Debug, tag2, "tog2-debug");
            console.Add(LogLevel.Debug, tag2, "tag2-debug");

            console.Add(LogLevel.Verbose, tag3, "tog3-verbose");
            console.Add(LogLevel.Verbose, tag3, "tag3-verbose");

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterText("tag");
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tag1-exception",
                "tag2-error",
                "tag3-warning",
                "tag1-info",
                "tag2-debug",
                "tag3-verbose"
            );

            consoleView.SetFilterLogLevel(LogLevel.Warn);
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tag1-exception",
                "tag2-error",
                "tag3-warning"
            );

            consoleView.SetFilterTags(tag1, tag3);
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tag1-exception",
                "tag3-warning"
            );

            consoleView.SetFilterText("");
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tog1-exception",
                "tag1-exception",
                "tog3-warning",
                "tag3-warning"
            );

            consoleView.SetFilterTags();
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tog1-exception",
                "tag1-exception",
                "tog2-error",
                "tag2-error",
                "tog3-warning",
                "tag3-warning"
            );

            consoleView.SetFilterLogLevel(LogLevel.Verbose);
            Assert.IsFalse(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tog1-exception",
                "tag1-exception",
                "tog2-error",
                "tag2-error",
                "tog3-warning",
                "tag3-warning",
                "tog1-info",
                "tag1-info",
                "tog2-debug",
                "tag2-debug",
                "tog3-verbose",
                "tag3-verbose"
            );
        }

        [Test]
        public void TestMixedFiltersTagLevelText()
        {
            Tag tag1 = new Tag("tag1");
            Tag tag2 = new Tag("tag2");
            Tag tag3 = new Tag("tag3");

            MockConsole console = new MockConsole(64);

            console.Add(LogLevel.Exception, tag1, "tog1-exception");
            console.Add(LogLevel.Exception, tag1, "tag1-exception");

            console.Add(LogLevel.Error, tag2, "tog2-error");
            console.Add(LogLevel.Error, tag2, "tag2-error");

            console.Add(LogLevel.Warn, tag3, "tog3-warning");
            console.Add(LogLevel.Warn, tag3, "tag3-warning");

            console.Add(LogLevel.Info, tag1, "tog1-info");
            console.Add(LogLevel.Info, tag1, "tag1-info");

            console.Add(LogLevel.Debug, tag2, "tog2-debug");
            console.Add(LogLevel.Debug, tag2, "tag2-debug");

            console.Add(LogLevel.Verbose, tag3, "tog3-verbose");
            console.Add(LogLevel.Verbose, tag3, "tag3-verbose");

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterTags(tag1, tag3);
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tog1-exception",
                "tag1-exception",
                "tog3-warning",
                "tag3-warning",
                "tog1-info",
                "tag1-info",
                "tog3-verbose",
                "tag3-verbose"
            );

            consoleView.SetFilterLogLevel(LogLevel.Warn);
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tog1-exception",
                "tag1-exception",
                "tog3-warning",
                "tag3-warning"
            );

            consoleView.SetFilterText("tag");
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tag1-exception",
                "tag3-warning"
            );

            consoleView.SetFilterLogLevel(LogLevel.Verbose);
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tag1-exception",
                "tag3-warning",
                "tag1-info",
                "tag3-verbose"
            );

            consoleView.SetFilterTags();
            Assert.IsTrue(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tag1-exception",
                "tag2-error",
                "tag3-warning",
                "tag1-info",
                "tag2-debug",
                "tag3-verbose"
            );

            consoleView.SetFilterText("");
            Assert.IsFalse(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                "tog1-exception",
                "tag1-exception",
                "tog2-error",
                "tag2-error",
                "tog3-warning",
                "tag3-warning",
                "tog1-info",
                "tag1-info",
                "tog2-debug",
                "tag2-debug",
                "tog3-verbose",
                "tag3-verbose"
            );
        }

        delegate bool TestFilterDelegate(ref ConsoleViewCellEntry entry);

        class TestFilter : ConsoleViewFilterBase
        {
            private TestFilterDelegate m_delegate;

            public TestFilter(int priority, TestFilterDelegate del)
                : base(priority)
            {
                m_delegate = del;
            }

            public override bool Apply(ref ConsoleViewCellEntry entry)
            {
                return m_delegate(ref entry);
            }
        }
    }
}