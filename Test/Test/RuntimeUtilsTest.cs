using System;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using NUnit.Framework;

namespace LunarPlugin.Test
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class RuntimeUtilsTest
    {
        [Test]
        public void TestExtractFileName()
        {
            string stackTrace = "LunarPlugin.Cmd_echo:Execute(String[]) (at Assets/Plugins/LunarPlugin/Console/CConsole_common.cs:329)\n" +
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

            string expected = "CConsole_common.cs";

            string actual = CEditorStackTrace.ExtractFileName(stackTrace);
            Assert.AreEqual(expected, actual);
        }
    }
}

