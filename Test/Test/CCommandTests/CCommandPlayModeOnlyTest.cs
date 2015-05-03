using System;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using LunarPlugin;
using LunarPluginInternal;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class CCommandPlayModeOnlyTest : CCommandTest
    {
        [Test]
        public void TestRunInPlayMode()
        {
            Runtime.IsPlaying = true;
            Execute("playmode");

            AssertResult("executed");
        }

        [Test]
        public void TestRunInEditorMode()
        {
            Runtime.IsPlaying = false;
            Execute("playmode");

            AssertResult();
        }

        [SetUp]
        public void SetUp()
        {
            base.RunSetUp();
            CRegistery.Register(new Cmd_playmode(delegate {
                AddResult("executed");
            }));
        }

        [TearDown]
        public void TearDown()
        {
            base.RunTearDown();
            Runtime.IsPlaying = true;
        }
    }

    class Cmd_playmode : CPlayModeCommand
    {
        private Action m_actionDelegate;

        public Cmd_playmode(Action actionDelegate)
        {
            this.Name = "playmode";
            m_actionDelegate = actionDelegate;
        }

        bool Execute()
        {
            m_actionDelegate();
            return true;
        }
    }
}

