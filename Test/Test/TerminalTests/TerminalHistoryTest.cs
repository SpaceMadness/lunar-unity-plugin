using System;
using NUnit.Framework;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using LunarPlugin.Test;

namespace TerminalTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture()]
    public class TerminalHistoryTest
    {
        [Test()]
        public void TestAddingItems()
        {
            CTerminalHistory history = new CTerminalHistory(5);

            history.Push("1");
            AssertHistory(history, "1");

            history.Push("2");
            AssertHistory(history, "1", "2");

            history.Push("3");
            AssertHistory(history, "1", "2", "3");

            history.Push("4");
            AssertHistory(history, "1", "2", "3", "4");

            history.Push("5");
            AssertHistory(history, "1", "2", "3", "4", "5");

            history.Push("6");
            AssertHistory(history, "2", "3", "4", "5", "6");

            history.Push("7");
            AssertHistory(history, "3", "4", "5", "6", "7");

            history.Push("8");
            AssertHistory(history, "4", "5", "6", "7", "8");

            history.Push("9");
            AssertHistory(history, "5", "6", "7", "8", "9");

            history.Push("10");
            AssertHistory(history, "6", "7", "8", "9", "10");

            history.Push("11");
            AssertHistory(history, "7", "8", "9", "10", "11");
        }

        [Test()]
        public void TestIteratingItems()
        {
            CTerminalHistory history = new CTerminalHistory(5);
            
            history.Push("1");
            AssertIterateBack(history, "1");
            AssertIterateForward(history);
            
            history.Push("2");
            AssertIterateBack(history, "2", "1");
            AssertIterateForward(history, "2");
            
            history.Push("3");
            AssertIterateBack(history, "3", "2", "1");
            AssertIterateForward(history, "2", "3");
            
            history.Push("4");
            AssertIterateBack(history, "4", "3", "2", "1");
            AssertIterateForward(history, "2", "3", "4");
            
            history.Push("5");
            AssertIterateBack(history, "5", "4", "3", "2", "1");
            AssertIterateForward(history, "2", "3", "4", "5");
            
            history.Push("6");
            AssertIterateBack(history, "6", "5", "4", "3", "2");
            AssertIterateForward(history, "3", "4", "5", "6");
            
            history.Push("7");
            AssertIterateBack(history, "7", "6", "5", "4", "3");
            AssertIterateForward(history, "4", "5", "6", "7");
            
            history.Push("8");
            AssertIterateBack(history, "8", "7", "6", "5", "4");
            AssertIterateForward(history, "5", "6", "7", "8");
            
            history.Push("9");
            AssertIterateBack(history, "9", "8", "7", "6", "5");
            AssertIterateForward(history, "6", "7", "8", "9");
            
            history.Push("10");
            AssertIterateBack(history, "10", "9", "8", "7", "6");
            AssertIterateForward(history, "7", "8", "9", "10");
            
            history.Push("11");
            AssertIterateBack(history, "11", "10", "9", "8", "7");
            AssertIterateForward(history, "8", "9", "10", "11");
        }

        private void AssertHistory(CTerminalHistory history, params string[] values)
        {
            Assert.AreEqual(values.Length, history.Count);
            for (int i = 0; i < values.Length; ++i)
            {
                Assert.AreEqual(values[i], history[i]);
            }
        }

        private void AssertIterateBack(CTerminalHistory history, params string[] values)
        {
            int index = 0;

            string value;
            while ((value = history.Prev()) != null)
            {
                Assert.AreEqual(values[index], value);
                ++index;
            }

            Assert.AreEqual(values.Length, index);
        }

        private void AssertIterateForward(CTerminalHistory history, params string[] values)
        {
            int index = 0;
            
            string value;
            while ((value = history.Next()) != null)
            {
                Assert.AreEqual(values[index], value);
                ++index;
            }
            
            Assert.AreEqual(values.Length, index);
        }
    }
}

