using System;

using LunarPlugin;

namespace LunarPluginInternal
{
    delegate void TestingPlatformAssertDelegate(string message, string stackTrace);

    class TestingPlatform : PlatformImpl
    {
        private static TestingPlatformAssertDelegate s_assertDelegate = DefaultAssertDelegate;

        public override void AssertMessage(string message, string stackTrace)
        {
            s_assertDelegate(message, stackTrace);
        }

        private static void DefaultAssertDelegate(string message, string stackTrace)
        {
            throw new Exception("Assertion failed: " + message);
        }

        public static TestingPlatformAssertDelegate AssertDelegate
        {
            get { return s_assertDelegate; }
            set { s_assertDelegate = value != null ? value : DefaultAssertDelegate; }
        }
    }
}

