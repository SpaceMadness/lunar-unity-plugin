using System;
using System.Collections.Generic;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using NUnit.Framework;

namespace LunarPlugin.Test
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture()]
    public class CStackTraceTest : TestFixtureBase
    {
        #region Trim

        [Test]
        public void TestTrimFrames()
        {
            string stackTrace = "LunarPlugin.Log:StackTrace(Int32) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:365)\n" +
                "LunarPlugin.Log:LogMessage(Tag, LogLevel, String) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:296)\n" +
                "LunarPlugin.Log:d(String, Object[]) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:198)\n" +
                "LunarPlugin.Cmd_echo:Execute(String[]) (at Assets/Plugins/LunarPlugin/Console/CConsole_common.cs:329)\n" +
                "LunarPlugin.CCommand:ExecuteGuarded(List`1, String) (at Assets/Plugins/LunarPlugin/Console/CCommand.cs:147)\n" +
                "LunarPlugin.CCommand:ExecuteTokens(List`1, String) (at Assets/Plugins/LunarPlugin/Console/CCommand.cs:75)\n" +
                "LunarPlugin.CommandProcessor:TryExecute(String) (at Assets/Plugins/LunarPlugin/Console/CommandProcessor.cs:35)\n" +
                "LunarPlugin.App:ExecCommand(String) (at Assets/Plugins/LunarPlugin/App.cs:273)\n" +
                "TerminalWindow:<CreateUI>m__2(TextField, KeyCode, Boolean) (at Assets/Editor/TerminalWindow.cs:48)\n" +
                "LunarPlugin.TextField:OnGUI() (at Assets/Plugins/LunarPlugin/UI/TextField.cs:69)\n" +
                "LunarPlugin.View:DrawChildren(Boolean) (at Assets/Plugins/LunarPlugin/UI/View.cs:392)\n" +
                "LunarPlugin.View:DrawGUI() (at Assets/Plugins/LunarPlugin/UI/View.cs:70)\n" +
                "LunarPlugin.View:OnGUI() (at Assets/Plugins/LunarPlugin/UI/View.cs:62)\n" +
                "Window:OnGUI() (at Assets/Editor/Window.cs:37)\n" +
                "System.Reflection.MonoMethod:InternalInvoke(Object, Object[], Exception&)\n" +
                "System.Reflection.MonoMethod:Invoke(Object, BindingFlags, Binder, Object[], CultureInfo) (at /Users/builduser/buildslave/mono-runtime-and-classlibs/build/mcs/class/corlib/System.Reflection/MonoMethod.cs:222)\n" +
                "System.Reflection.MethodBase:Invoke(Object, Object[]) (at /Users/builduser/buildslave/mono-runtime-and-classlibs/build/mcs/class/corlib/System.Reflection/MethodBase.cs:115)\n" +
                "UnityEditor.HostView:Invoke(String, Object)\n" +
                "UnityEditor.HostView:Invoke(String)\n" +
                "UnityEditor.DockArea:OnGUI()";

            string expected = "LunarPlugin.Cmd_echo:Execute(String[]) (at Assets/Plugins/LunarPlugin/Console/CConsole_common.cs:329)\n" +
                "LunarPlugin.CCommand:ExecuteGuarded(List`1, String) (at Assets/Plugins/LunarPlugin/Console/CCommand.cs:147)\n" +
                "LunarPlugin.CCommand:ExecuteTokens(List`1, String) (at Assets/Plugins/LunarPlugin/Console/CCommand.cs:75)\n" +
                "LunarPlugin.CommandProcessor:TryExecute(String) (at Assets/Plugins/LunarPlugin/Console/CommandProcessor.cs:35)\n" +
                "LunarPlugin.App:ExecCommand(String) (at Assets/Plugins/LunarPlugin/App.cs:273)\n" +
                "TerminalWindow:<CreateUI>m__2(TextField, KeyCode, Boolean) (at Assets/Editor/TerminalWindow.cs:48)\n" +
                "LunarPlugin.TextField:OnGUI() (at Assets/Plugins/LunarPlugin/UI/TextField.cs:69)\n" +
                "LunarPlugin.View:DrawChildren(Boolean) (at Assets/Plugins/LunarPlugin/UI/View.cs:392)\n" +
                "LunarPlugin.View:DrawGUI() (at Assets/Plugins/LunarPlugin/UI/View.cs:70)\n" +
                "LunarPlugin.View:OnGUI() (at Assets/Plugins/LunarPlugin/UI/View.cs:62)\n" +
                "Window:OnGUI() (at Assets/Editor/Window.cs:37)\n" +
                "System.Reflection.MonoMethod:InternalInvoke(Object, Object[], Exception&)\n" +
                "System.Reflection.MonoMethod:Invoke(Object, BindingFlags, Binder, Object[], CultureInfo) (at /Users/builduser/buildslave/mono-runtime-and-classlibs/build/mcs/class/corlib/System.Reflection/MonoMethod.cs:222)\n" +
                "System.Reflection.MethodBase:Invoke(Object, Object[]) (at /Users/builduser/buildslave/mono-runtime-and-classlibs/build/mcs/class/corlib/System.Reflection/MethodBase.cs:115)\n" +
                "UnityEditor.HostView:Invoke(String, Object)\n" +
                "UnityEditor.HostView:Invoke(String)\n" +
                "UnityEditor.DockArea:OnGUI()";

            string actual = CStackTrace.ExtractStackTrace(stackTrace, 3);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestTrimAllFrames()
        {
            string stackTrace = "LunarPlugin.Log:StackTrace(Int32) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:365)\n" +
                "LunarPlugin.Log:LogMessage(Tag, LogLevel, String) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:296)\n" +
                "LunarPlugin.Log:d(String, Object[]) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:198)";

            string expected = "";

            string actual = CStackTrace.ExtractStackTrace(stackTrace, 3);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestTrimTooManyFrames()
        {
            string stackTrace = "LunarPlugin.Log:StackTrace(Int32) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:365)\n" +
                "LunarPlugin.Log:LogMessage(Tag, LogLevel, String) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:296)\n" +
                "LunarPlugin.Log:d(String, Object[]) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:198)";

            string expected = "";

            string actual = CStackTrace.ExtractStackTrace(stackTrace, 4);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestTrimWrongStackTrace()
        {
            string stackTrace = "LunarPlugin.Log:StackTrace(Int32) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:365)";
            string expected = "";

            string actual = CStackTrace.ExtractStackTrace(stackTrace, 3);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestTrimStackTraceWithLineBreakAtTheEnd()
        {
            string stackTrace = "LunarPlugin.Log:StackTrace(Int32) (at Assets/Plugins/LunarPlugin/Debugging/Log.cs:365)\n";
            string expected = "";

            string actual = CStackTrace.ExtractStackTrace(stackTrace, 1);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Optimize

        [Test]
        public void TestOptimize()
        {
            string stackTrace = "  at System.Reflection.MonoMethod.Invoke (System.Object obj, BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) [0x000eb] in /Users/builduser/buildslave/mono-runtime-and-classlibs/build/mcs/class/corlib/System.Reflection/MonoMethod.cs:232 \n" +
                "  at System.Reflection.MethodBase.Invoke (System.Object obj, System.Object[] parameters) [0x00000]\n" +
                "  at System.Reflection.MethodBase.Invoke (System.Object obj)\n" +
                "  at Test.<OnGUIHelper>m__4 () [0x00013] in /Users/alementuev/dev/my/unity-debug-kit/Project/Assets/Test.cs:100 \n" +
                "  at LunarPlugin.Timer.DefaultTimerCallback (LunarPlugin.Timer timer) [0x00000] in /Users/alementuev/dev/my/unity-debug-kit/Project/Assets/Plugins/LunarPlugin/Core/Timer.cs:76 \n" +
                "  at LunarPlugin.Timer.Fire () [0x00008] in /Users/alementuev/dev/my/unity-debug-kit/Project/Assets/Plugins/LunarPlugin/Core/Timer.cs:51 ";

            CStackTraceLine[] expected =
            {
                new CStackTraceLine("System.Reflection.MonoMethod.Invoke(Object, BindingFlags, Binder, Object[], CultureInfo) (at /Users/builduser/buildslave/mono-runtime-and-classlibs/build/mcs/class/corlib/System.Reflection/MonoMethod.cs:232)", "/Users/builduser/buildslave/mono-runtime-and-classlibs/build/mcs/class/corlib/System.Reflection/MonoMethod.cs", 232),
                new CStackTraceLine("System.Reflection.MethodBase.Invoke(Object, Object[])"),
                new CStackTraceLine("System.Reflection.MethodBase.Invoke(Object)"),
                new CStackTraceLine("Test.<OnGUIHelper>m__4() (at Assets/Test.cs:100)", "Assets/Test.cs", 100),
                new CStackTraceLine("LunarPlugin.Timer.DefaultTimerCallback(Timer) (at Assets/Plugins/LunarPlugin/Core/Timer.cs:76)", "Assets/Plugins/LunarPlugin/Core/Timer.cs", 76),
                new CStackTraceLine("LunarPlugin.Timer.Fire() (at Assets/Plugins/LunarPlugin/Core/Timer.cs:51)", "Assets/Plugins/LunarPlugin/Core/Timer.cs", 51)
            };

            CStackTraceLine[] actual = CEditorStackTrace.ParseStackTrace(stackTrace);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestOptimizeAlreadyOptimized()
        {
            string stackTrace = "Test.foo(String, Timer, Boolean[]) (at Assets/Test.cs:120)\n" +
                "Test.<OnGUIHelper>m__4() (at Assets/Test.cs:98)\n" +
                "LunarPlugin.Timer.DefaultTimerCallback(Timer) (at Assets/Plugins/LunarPlugin/Core/Timer.cs:76)\n" +
                "LunarPlugin.Timer.Fire() (at Assets/Plugins/LunarPlugin/Core/Timer.cs:51)\n" +
                "UnityEditor.HostView:Invoke(String, Object)\n" +
                "UnityEditor.HostView:Invoke(String)\n" +
                "UnityEditor.DockArea:OnGUI()";

            CStackTraceLine[] expected =
            {
                new CStackTraceLine("Test.foo(String, Timer, Boolean[]) (at Assets/Test.cs:120)", "Assets/Test.cs", 120),
                new CStackTraceLine("Test.<OnGUIHelper>m__4() (at Assets/Test.cs:98)", "Assets/Test.cs", 98),
                new CStackTraceLine("LunarPlugin.Timer.DefaultTimerCallback(Timer) (at Assets/Plugins/LunarPlugin/Core/Timer.cs:76)", "Assets/Plugins/LunarPlugin/Core/Timer.cs", 76),
                new CStackTraceLine("LunarPlugin.Timer.Fire() (at Assets/Plugins/LunarPlugin/Core/Timer.cs:51)", "Assets/Plugins/LunarPlugin/Core/Timer.cs", 51),
                new CStackTraceLine("UnityEditor.HostView:Invoke(String, Object)"),
                new CStackTraceLine("UnityEditor.HostView:Invoke(String)"),
                new CStackTraceLine("UnityEditor.DockArea:OnGUI()")
            };

            CStackTraceLine[] actual = CEditorStackTrace.ParseStackTrace(stackTrace);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        [Test]
        public void TestParsingUnityStackTrace()
        {
            string data = "UnityEngine.Debug:Log(Object)\n" +
                "Test:<CreateRootView>m__0(Button) (at Assets/Test.cs:106)\n" +
                "LunarPlugin.Button:OnGUI() (at Assets/Plugins/LunarPlugin/UI/Button.cs:21)\n" +
                "LunarPlugin.View:DrawChildren(Boolean) (at Assets/Plugins/LunarPlugin/UI/View.cs:382)\n" +
                "LunarPlugin.View:DrawGUI() (at Assets/Plugins/LunarPlugin/UI/View.cs:70)\n" +
                "LunarPlugin.View:OnGUI() (at Assets/Plugins/LunarPlugin/UI/View.cs:62)\n" +
                "Test:OnGUI() (at Assets/Test.cs:52)";

            string[] lines = data.Split('\n');
            CSourcePathEntry[] expected = 
            {
                CSourcePathEntry.Invalid,
                new CSourcePathEntry("Assets/Test.cs", 106),
                new CSourcePathEntry("Assets/Plugins/LunarPlugin/UI/Button.cs", 21),
                new CSourcePathEntry("Assets/Plugins/LunarPlugin/UI/View.cs", 382),
                new CSourcePathEntry("Assets/Plugins/LunarPlugin/UI/View.cs", 70),
                new CSourcePathEntry("Assets/Plugins/LunarPlugin/UI/View.cs", 62),
                new CSourcePathEntry("Assets/Test.cs", 52)
            };

            for (int i = 0; i < lines.Length; ++i)
            {
                CSourcePathEntry actual;
                CUnityStackTraceParser.TryParse(lines[i], out actual);

                Assert.AreEqual(expected[i].IsValid, actual.IsValid);
                Assert.AreEqual(expected[i].sourcePath, actual.sourcePath);
                Assert.AreEqual(expected[i].lineNumber, actual.lineNumber);
            }
        }

        [Test]
        public void TestParsingCompilerWarningSourceEntries()
        {
            string line = "[E]: Assets/Editor/Console/ConsoleView.cs(637,18): error CS1525: Unexpected symbol `if', expecting `)', `,', `;', `[', or `='";

            CSourcePathEntry expected = new CSourcePathEntry("Assets/Editor/Console/ConsoleView.cs", 637);
            CSourcePathEntry actual;
            bool succeed = CEditorStackTrace.TryParseCompilerMessage(line, out actual);
            Assert.IsTrue(succeed);

            Assert.AreEqual(expected.IsValid, actual.IsValid);
            Assert.AreEqual(expected.sourcePath, actual.sourcePath);
            Assert.AreEqual(expected.lineNumber, actual.lineNumber);
        }
    }
}

