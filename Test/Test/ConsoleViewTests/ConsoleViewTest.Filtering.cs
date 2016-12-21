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

            CConsoleViewFilterBase filter1 = new TestFilter(1, delegate(ref CConsoleViewCellEntry e)
            {
                priorities.Add(1);
                return true;
            });

            CConsoleViewFilterBase filter2 = new TestFilter(2, delegate(ref CConsoleViewCellEntry e)
            {
                priorities.Add(2);
                return true;
            });

            CConsoleViewFilterBase filter3 = new TestFilter(3, delegate(ref CConsoleViewCellEntry e)
            {
                priorities.Add(3);
                return true;
            });

            CConsoleViewCompositeFilter compositeFilter = new CConsoleViewCompositeFilter();
            compositeFilter.AddFilter(filter2);
            compositeFilter.AddFilter(filter1);
            compositeFilter.AddFilter(filter3);
            compositeFilter.AddFilter(filter1);
            compositeFilter.AddFilter(filter2);
            compositeFilter.AddFilter(filter3);

            CConsoleViewCellEntry entry = default(CConsoleViewCellEntry);
            compositeFilter.Apply(ref entry);

            AssertList(priorities, 3, 2, 1);
        }

        [Test]
        public void TestCompositeFilterApply()
        {
            List<int> priorities = new List<int>();

            CConsoleViewFilterBase filter1 = new TestFilter(1, delegate(ref CConsoleViewCellEntry e)
            {
                priorities.Add(1);
                return true;
            });

            CConsoleViewFilterBase filter2 = new TestFilter(2, delegate(ref CConsoleViewCellEntry e)
            {
                priorities.Add(2);
                return false;
            });

            CConsoleViewFilterBase filter3 = new TestFilter(3, delegate(ref CConsoleViewCellEntry e)
            {
                priorities.Add(3);
                return true;
            });

            CConsoleViewCompositeFilter compositeFilter = new CConsoleViewCompositeFilter();
            compositeFilter.AddFilter(filter2);
            compositeFilter.AddFilter(filter1);
            compositeFilter.AddFilter(filter3);
            compositeFilter.AddFilter(filter1);
            compositeFilter.AddFilter(filter2);
            compositeFilter.AddFilter(filter3);

            CConsoleViewCellEntry entry = default(CConsoleViewCellEntry);
            compositeFilter.Apply(ref entry);

            AssertList(priorities, 3, 2);
        }

        [Test]
        public void TestCompositeFilterRemove()
        {
            List<int> priorities = new List<int>();

            CConsoleViewFilterBase filter1 = new TestFilter(1, delegate(ref CConsoleViewCellEntry e)
            {
                priorities.Add(1);
                return true;
            });

            CConsoleViewFilterBase filter2 = new TestFilter(2, delegate(ref CConsoleViewCellEntry e)
            {
                priorities.Add(2);
                return true;
            });

            CConsoleViewFilterBase filter3 = new TestFilter(3, delegate(ref CConsoleViewCellEntry e)
            {
                priorities.Add(3);
                return true;
            });

            CConsoleViewCompositeFilter compositeFilter = new CConsoleViewCompositeFilter();
            compositeFilter.AddFilter(filter1);
            compositeFilter.AddFilter(filter2);
            compositeFilter.AddFilter(filter3);

            compositeFilter.RemoveFilter(filter1);
            compositeFilter.RemoveFilter(filter3);

            CConsoleViewCellEntry entry = default(CConsoleViewCellEntry);
            compositeFilter.Apply(ref entry);

            AssertList(priorities, 2);
        }

        [Test]
        public void TestFiltering()
        {
            MockConsole console = new MockConsole();
            console.Add(CLogLevel.Debug, tag, "line1");
            console.Add(CLogLevel.Debug, tag, "line11");
            console.Add(CLogLevel.Debug, tag, "line111");
            console.Add(CLogLevel.Debug, tag, "line1111");
            console.Add(CLogLevel.Debug, tag, "foo");

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

            console.Add(CLogLevel.Debug, tag, "line1");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView);

            console.Add(CLogLevel.Debug, tag, "line11");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11");

            console.Add(CLogLevel.Debug, tag, "line111");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111");

            console.Add(CLogLevel.Debug, tag, "line12");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111");

            console.Add(CLogLevel.Debug, tag, "foo1");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111");

            console.Add(CLogLevel.Debug, tag, "foo12");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line11", "line111");

            console.Add(CLogLevel.Debug, tag, "foo123");
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
            console.Add(CLogLevel.Debug, tag, "line1");
            console.Add(CLogLevel.Debug, tag, "line11");
            console.Add(CLogLevel.Debug, tag, "line111");
            console.Add(CLogLevel.Debug, tag, "line1111");
            console.Add(CLogLevel.Debug, tag, "foo");

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterText("line111");
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView, "line111", "line1111");

            console.Clear();
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView);

            console.Add(CLogLevel.Debug, tag, "line12");
            console.Add(CLogLevel.Debug, tag, "line112");
            console.Add(CLogLevel.Debug, tag, "line1112");
            console.Add(CLogLevel.Debug, tag, "line11112");
            console.Add(CLogLevel.Debug, tag, "foo2");

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
            console.Add(CLogLevel.Debug, tag, "a1");
            console.Add(CLogLevel.Debug, tag, "a2");
            console.Add(CLogLevel.Debug, tag, "a3");
            console.Add(CLogLevel.Debug, tag, "a4");
            console.Add(CLogLevel.Debug, tag, "b1");
            console.Add(CLogLevel.Debug, tag, "b2");

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterText("a");
            AssertVisibleRows(consoleView, "a1", "a2", "a3", "a4");

            console.Add(CLogLevel.Debug, tag, "b3");
            AssertVisibleRows(consoleView, "a1", "a2", "a3", "a4");

            console.Add(CLogLevel.Debug, tag, "a5");
            AssertVisibleRows(consoleView, "a1", "a2", "a3", "a4", "a5");

            console.Add(CLogLevel.Debug, tag, "b4");
            AssertVisibleRows(consoleView, "a1", "a2", "a3", "a4", "a5");

            console.Add(CLogLevel.Debug, tag, "a6");
            AssertVisibleRows(consoleView, "a1", "a2", "a3", "a4", "a5", "a6");

            console.Add(CLogLevel.Debug, tag, "b5");
            AssertVisibleRows(consoleView, "a2", "a3", "a4", "a5", "a6");

            console.Add(CLogLevel.Debug, tag, "b6");
            AssertVisibleRows(consoleView, "a3", "a4", "a5", "a6");

            console.Add(CLogLevel.Debug, tag, "a7");
            AssertVisibleRows(consoleView, "a4", "a5", "a6", "a7");

            console.Add(CLogLevel.Debug, tag, "a8");
            AssertVisibleRows(consoleView, "a5", "a6", "a7", "a8");

            console.Add(CLogLevel.Debug, tag, "a9");
            AssertVisibleRows(consoleView, "a5", "a6", "a7", "a8", "a9");

            console.Add(CLogLevel.Debug, tag, "b7");
            AssertVisibleRows(consoleView, "a5", "a6", "a7", "a8", "a9");

            console.Add(CLogLevel.Debug, tag, "b8");
            AssertVisibleRows(consoleView, "a5", "a6", "a7", "a8", "a9");

            console.Add(CLogLevel.Debug, tag, "a10");
            AssertVisibleRows(consoleView, "a6", "a7", "a8", "a9", "a10");

            console.Add(CLogLevel.Debug, tag, "b9");
            AssertVisibleRows(consoleView, "a6", "a7", "a8", "a9", "a10");

            console.Add(CLogLevel.Debug, tag, "b10");
            AssertVisibleRows(consoleView, "a7", "a8", "a9", "a10");

            console.Add(CLogLevel.Debug, tag, "b11");
            AssertVisibleRows(consoleView, "a7", "a8", "a9", "a10");

            console.Add(CLogLevel.Debug, tag, "b12");
            AssertVisibleRows(consoleView, "a7", "a8", "a9", "a10");

            console.Add(CLogLevel.Debug, tag, "b13");
            AssertVisibleRows(consoleView, "a8", "a9", "a10");

            console.Add(CLogLevel.Debug, tag, "b14");
            AssertVisibleRows(consoleView, "a9", "a10");

            console.Add(CLogLevel.Debug, tag, "b15");
            AssertVisibleRows(consoleView, "a10");

            console.Add(CLogLevel.Debug, tag, "b16");
            AssertVisibleRows(consoleView, "a10");

            console.Add(CLogLevel.Debug, tag, "b17");
            AssertVisibleRows(consoleView, "a10");

            console.Add(CLogLevel.Debug, tag, "b18");
            AssertVisibleRows(consoleView);

            console.Add(CLogLevel.Debug, tag, "a11");
            AssertVisibleRows(consoleView, "a11");

            console.Add(CLogLevel.Debug, tag, "b19");
            AssertVisibleRows(consoleView, "a11");

            console.Add(CLogLevel.Debug, tag, "a12");
            AssertVisibleRows(consoleView, "a11", "a12");

            consoleView.SetFilterText("");
            AssertVisibleRows(consoleView, "b12", "b13", "b14", "b15", "b16", "b17", "b18", "a11", "b19", "a12");
        }

        [Test]
        public void TestFilterByLogLevel()
        {
            MockConsole console = new MockConsole(10);
            CLogLevel[] levels = {
                CLogLevel.Verbose,
                CLogLevel.Debug,
                CLogLevel.Info,
                CLogLevel.Warn,
                CLogLevel.Error,
                CLogLevel.Exception
            };

            foreach (CLogLevel l in levels)
            {
                console.Add(l, tag, l.Name);
            }

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterLogLevel(CLogLevel.Verbose);
            Assert.IsFalse(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                CLogLevel.Verbose.Name,
                CLogLevel.Debug.Name,
                CLogLevel.Info.Name,
                CLogLevel.Warn.Name,
                CLogLevel.Error.Name,
                CLogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(CLogLevel.Debug);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                CLogLevel.Debug.Name,
                CLogLevel.Info.Name,
                CLogLevel.Warn.Name,
                CLogLevel.Error.Name,
                CLogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(CLogLevel.Info);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                CLogLevel.Info.Name,
                CLogLevel.Warn.Name,
                CLogLevel.Error.Name,
                CLogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(CLogLevel.Warn);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                CLogLevel.Warn.Name,
                CLogLevel.Error.Name,
                CLogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(CLogLevel.Error);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                CLogLevel.Error.Name,
                CLogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(CLogLevel.Exception);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                CLogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(CLogLevel.Error);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                CLogLevel.Error.Name,
                CLogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(CLogLevel.Warn);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                CLogLevel.Warn.Name,
                CLogLevel.Error.Name,
                CLogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(CLogLevel.Info);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                CLogLevel.Info.Name,
                CLogLevel.Warn.Name,
                CLogLevel.Error.Name,
                CLogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(CLogLevel.Debug);
            Assert.IsTrue(consoleView.IsFiltering);
            AssertVisibleRows(consoleView,
                CLogLevel.Debug.Name,
                CLogLevel.Info.Name,
                CLogLevel.Warn.Name,
                CLogLevel.Error.Name,
                CLogLevel.Exception.Name
            );

            consoleView.SetFilterLogLevel(CLogLevel.Verbose);
            Assert.IsFalse(consoleView.IsFiltering);

            AssertVisibleRows(consoleView,
                CLogLevel.Verbose.Name,
                CLogLevel.Debug.Name,
                CLogLevel.Info.Name,
                CLogLevel.Warn.Name,
                CLogLevel.Error.Name,
                CLogLevel.Exception.Name
            );
        }

        [Test]
        public void TestFilterByTag()
        {
            CTag tag1 = new CTag("tag1");
            CTag tag2 = new CTag("tag2");
            CTag tag3 = new CTag("tag3");

            MockConsole console = new MockConsole(10);

            console.Add(CLogLevel.Debug, tag1, "tag1-1");
            console.Add(CLogLevel.Debug, tag2, "tag2-1");
            console.Add(CLogLevel.Debug, tag1, "tag1-2");
            console.Add(CLogLevel.Debug, tag3, "tag3-1");
            console.Add(CLogLevel.Debug, tag1, "tag1-3");

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
            CTag tag1 = new CTag("tag1");
            CTag tag2 = new CTag("tag2");
            CTag tag3 = new CTag("tag3");

            MockConsole console = new MockConsole(64);

            console.Add(CLogLevel.Exception, tag1, "tog1-exception");
            console.Add(CLogLevel.Exception, tag1, "tag1-exception");

            console.Add(CLogLevel.Error, tag2, "tog2-error");
            console.Add(CLogLevel.Error, tag2, "tag2-error");

            console.Add(CLogLevel.Warn, tag3, "tog3-warning");
            console.Add(CLogLevel.Warn, tag3, "tag3-warning");

            console.Add(CLogLevel.Info, tag1, "tog1-info");
            console.Add(CLogLevel.Info, tag1, "tag1-info");

            console.Add(CLogLevel.Debug, tag2, "tog2-debug");
            console.Add(CLogLevel.Debug, tag2, "tag2-debug");

            console.Add(CLogLevel.Verbose, tag3, "tog3-verbose");
            console.Add(CLogLevel.Verbose, tag3, "tag3-verbose");

            MockConsoleView consoleView = new MockConsoleView(console, 320, 230);

            consoleView.SetFilterLogLevel(CLogLevel.Warn);
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

            consoleView.SetFilterLogLevel(CLogLevel.Verbose);
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
            CTag tag1 = new CTag("tag1");
            CTag tag2 = new CTag("tag2");
            CTag tag3 = new CTag("tag3");

            MockConsole console = new MockConsole(64);

            console.Add(CLogLevel.Exception, tag1, "tog1-exception");
            console.Add(CLogLevel.Exception, tag1, "tag1-exception");

            console.Add(CLogLevel.Error, tag2, "tog2-error");
            console.Add(CLogLevel.Error, tag2, "tag2-error");

            console.Add(CLogLevel.Warn, tag3, "tog3-warning");
            console.Add(CLogLevel.Warn, tag3, "tag3-warning");

            console.Add(CLogLevel.Info, tag1, "tog1-info");
            console.Add(CLogLevel.Info, tag1, "tag1-info");

            console.Add(CLogLevel.Debug, tag2, "tog2-debug");
            console.Add(CLogLevel.Debug, tag2, "tag2-debug");

            console.Add(CLogLevel.Verbose, tag3, "tog3-verbose");
            console.Add(CLogLevel.Verbose, tag3, "tag3-verbose");

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

            consoleView.SetFilterLogLevel(CLogLevel.Warn);
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

            consoleView.SetFilterLogLevel(CLogLevel.Verbose);
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
            CTag tag1 = new CTag("tag1");
            CTag tag2 = new CTag("tag2");
            CTag tag3 = new CTag("tag3");

            MockConsole console = new MockConsole(64);

            console.Add(CLogLevel.Exception, tag1, "tog1-exception");
            console.Add(CLogLevel.Exception, tag1, "tag1-exception");

            console.Add(CLogLevel.Error, tag2, "tog2-error");
            console.Add(CLogLevel.Error, tag2, "tag2-error");

            console.Add(CLogLevel.Warn, tag3, "tog3-warning");
            console.Add(CLogLevel.Warn, tag3, "tag3-warning");

            console.Add(CLogLevel.Info, tag1, "tog1-info");
            console.Add(CLogLevel.Info, tag1, "tag1-info");

            console.Add(CLogLevel.Debug, tag2, "tog2-debug");
            console.Add(CLogLevel.Debug, tag2, "tag2-debug");

            console.Add(CLogLevel.Verbose, tag3, "tog3-verbose");
            console.Add(CLogLevel.Verbose, tag3, "tag3-verbose");

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

            consoleView.SetFilterLogLevel(CLogLevel.Warn);
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

            consoleView.SetFilterLogLevel(CLogLevel.Verbose);
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

        delegate bool TestFilterDelegate(ref CConsoleViewCellEntry entry);

        class TestFilter : CConsoleViewFilterBase
        {
            private TestFilterDelegate m_delegate;

            public TestFilter(int priority, TestFilterDelegate del)
                : base(priority)
            {
                m_delegate = del;
            }

            public override bool Apply(ref CConsoleViewCellEntry entry)
            {
                return m_delegate(ref entry);
            }
        }
    }
}