using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using LunarPlugin;

namespace LunarPluginInternal
{
    //////////////////////////////////////////////////////////////////////////////

    [CCommand("@tag")]
    class Cmd_systag : CCommand
    {
        [CCommandOption(Required=true)]
        private string name;

        [CCommandOption]
        private bool enabled;

        [CCommandOption]
        private int color;

        void Execute()
        {
            Tag tag = Tag.Find(name);
            if (tag == null)
            {
                tag = new Tag(name); // the tag would be registered
            }

            tag.Enabled = enabled;
            tag.Color = ColorUtils.FromRGBA((uint)color);
            tag.BackColor = ColorUtils.FromRGBA((uint)color);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("log")] // TODO: add description
    class Cmd_log : CCommand
    {
        [CCommandOption(ShortName="l", Values="exception,error,warn,info,debug,verbose")]
        private string level;

        private LogLevel m_lastLogLevel;

        bool Execute(string[] args)
        {
            if (level != null)
            {
                LogLevel logLevel = FromString(level);
                if (logLevel == null)
                {
                    PrintError("Unexpected log level: '{0}'", level);
                    PrintUsage();
                    return false;
                }

                Log.Level = logLevel;

                PrintLogLevel();
                return true;
            }

            // TODO: add actions
            if (args.Length == 1)
            {
                string arg = args[0];
                if (arg == "enable" || arg == "on" || arg == "1")
                {
                    if (m_lastLogLevel != null) Log.Level = m_lastLogLevel;

                    PrintLogLevel();
                    return true;
                }
                if (arg == "disable" || arg == "off" || arg == "0")
                {
                    m_lastLogLevel = Log.Level;
                    Log.Level = null;

                    PrintLogLevel();
                    return true;
                }
            }

            PrintLogLevel();
            return true;
        }

        private void PrintLogLevel()
        {
            Print("Log level: {0}", Log.Level != null ? Log.Level.Name : "<none>");
        }

        private LogLevel FromString(String name)
        {
            if (name == "debug")     return LogLevel.Debug;
            if (name == "exception") return LogLevel.Exception;
            if (name == "error")     return LogLevel.Error;
            if (name == "info")      return LogLevel.Info;
            if (name == "warn")      return LogLevel.Warn;
            if (name == "verbose")   return LogLevel.Verbose;

            return null;
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("tag")] // TODO: description
    class Cmd_tag : CCommand
    {
        bool Execute(string[] args)
        {
            if (args.Length == 0)
            {
                ICollection<Tag> tags = Tag.ListTags();
                if (tags != null && tags.Count > 0)
                {
                    string[] names = new string[tags.Count];
                    int index = 0;
                    foreach (Tag t in tags)
                    {
                        names[index++] = t.Name;
                    }

                    Array.Sort(names);
                    Print(names);
                }
            }

            return true;
        }

        private int CompareTags(Tag t1, Tag t2)
        {
            return t1.Name.CompareTo(t2.Name);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("cmdlist", Description="Lists all available terminal commands.")]
    class Cmd_cmdlist : CCommand
    {
        [CCommandOption(Name="all", ShortName="a", Description="List all commands (including system)")]
        private bool includeSystem;

        bool Execute(string prefix = null)
        {
            CommandListOptions options = CommandListOptions.None;
            if (Config.isDebugBuild)
            {
                options |= CommandListOptions.Debug;
            }
            if (includeSystem)
            {
                options |= CommandListOptions.System;
            }

            IList<CCommand> commands = CRegistery.ListCommands(delegate(CCommand cmd)
            {
                return !(cmd is CVarCommand) && CRegistery.ShouldListCommand(cmd, prefix, options);
            });

            if (commands.Count > 0)
            {
                string[] names = new string[commands.Count];
                for (int i = 0; i < commands.Count; ++i)
                {
                    CCommand cmd = commands[i];
                    names[i] = C(cmd.Name, cmd.ColorCode);
                }
                Print(names);
            }

            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("cvarlist", Description="Lists all available cvars and their values.")]
    class Cmd_cvarlist : CCommand
    {
        [CCommandOption(Name="short", ShortName="s", Description="Outputs only names")]
        private bool shortList;

        [CCommandOption(Name="all", ShortName="a", Description="List all vars (including system)")]
        private bool includeSystem;

        bool Execute(string prefix = null)
        {
            CommandListOptions options = CommandListOptions.None;
            if (Config.isDebugBuild)
            {
                options |= CommandListOptions.Debug;
            }
            if (includeSystem)
            {
                options |= CommandListOptions.System;
            }

            // TODO: refactoring
            IList<CVar> vars = CRegistery.ListVars(prefix, options);
            if (vars.Count > 0)
            {
                if (shortList)
                {
                    string[] names = new string[vars.Count];
                    for (int i = 0; i < vars.Count; ++i)
                    {
                        names[i] = StringUtils.C(vars[i].Name, ColorCode.TableVar);
                    }
                    Print(names);
                }
                else
                {
                    StringBuilder result = new StringBuilder();
                    for (int i = 0; i < vars.Count; ++i)
                    {
                        CVar cvar = vars[i];
                        result.AppendFormat("  {0} {1}", StringUtils.C(cvar.Name, ColorCode.TableVar), StringUtils.Arg(cvar.Value));

                        // TODO: better color highlight
                        if (!cvar.IsDefault)
                        {
                            result.AppendFormat(" {0} {1}", StringUtils.C("default", ColorCode.TableVar), cvar.DefaultValue);
                        }

                        if (i < vars.Count - 1)
                        {
                            result.Append('\n');
                        }
                    }

                    Print(result.ToString());
                }
            }

            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("bind", Description="Assigns a key to command(s).")]
    class Cmd_bind : CCommand
    {
        bool Execute(string key, string command = null)
        {
            string name = key.ToLower();

            if (command != null)
            {
                KeyCode Code = CBindings.Parse(name);
                if (Code == KeyCode.None)
                {
                    PrintError("Unknown key: {0}", name);
                    return false;
                }

                CBindings.Bind(Code, StringUtils.UnArg(command));

                PostNotification(CCommandNotifications.CBindingsChanged,
                    CCommandNotifications.KeyManualMode, this.IsManualMode
                );

                return true;
            }

            IList<CBinding> bindings = CBindings.List(name);
            if (bindings.Count > 0)
            {
                foreach (CBinding b in bindings)
                {
                    PrintIndent(ToString(b));
                }
            }
            else
            {
                PrintIndent("No bindings");
            }

            return true;
        }

        public static string ToString(CBinding b)
        {
            return string.Format("bind {0} {1}", b.name, StringUtils.Arg(b.cmd));
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("unbind", Description="Unbinds a key.")]
    class Cmd_unbind : CCommand
    {
        bool Execute(string key)
        {
            KeyCode keyCode = CBindings.Parse(key.ToLower());
            if (keyCode == KeyCode.None)
            {
                PrintError("Unknown key: {0}", key);
                return false;
            }

            CBindings.Unbind(keyCode);

            PostNotification(CCommandNotifications.CBindingsChanged,
                CCommandNotifications.KeyManualMode, this.IsManualMode
            );

            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("bindlist", Description="List all currently bound keys and what command they are bound to.")]
    class Cmd_bindlist : CCommand
    {
        void Execute(string prefix = null)
        {
            IList<CBinding> bindings = CBindings.List(prefix);
            foreach (CBinding b in bindings)
            {
                PrintIndent("bind {0} \"{1}\"", b.name, b.cmd);
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("unbindall", Description="Unbinds all keys.\t")]
    class Cmd_unbindall : CCommand
    {
        void Execute()
        {
            CBindings.Clear();

            PostNotification(CCommandNotifications.CBindingsChanged,
                CCommandNotifications.KeyManualMode, this.IsManualMode
            );
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("alias", Description="Creates an alias name for command(s)")]
    class Cmd_alias : CCommand
    {
        void Execute(string name, string commands)
        {
            CRegistery.AddAlias(name, StringUtils.UnArg(commands));

            PostNotification(CCommandNotifications.CAliasesChanged, 
                CCommandNotifications.KeyName, name,
                CCommandNotifications.KeyManualMode, this.IsManualMode
            );
        }

        public static void ListAliasesConfig(IList<string> lines)
        {
            IList<CAliasCommand> aliases = CRegistery.ListAliases();

            for (int i = 0; i < aliases.Count; ++i)
            {
                lines.Add(ToString(aliases[i]));
            }
        }

        private static string ToString(CAliasCommand cmd)
        {
            return string.Format("alias {0} {1}", cmd.Name, StringUtils.Arg(cmd.Alias));
        }
    }

    [CCommand("aliaslist", Description="List current aliases")]
    class Cmd_aliaslist : CCommand
    {
        [CCommandOption(Name="short", ShortName="s", Description="Outputs only names")]
        private bool shortList;

        bool Execute(string prefix = null)
        {
            IList<CAliasCommand> cmds = CRegistery.ListAliases(prefix);
            if (cmds.Count > 0)
            {
                if (shortList)
                {
                    string[] names = new string[cmds.Count];
                    for (int i = 0; i < cmds.Count; ++i)
                    {
                        names[i] = cmds[i].Name;
                    }
                    Print(names);
                }
                else
                {
                    foreach (CAliasCommand cmd in cmds)
                    {
                        PrintIndent("{0} {1}", cmd.Name, cmd.Alias);
                    }
                }
            }

            return true;
        }

        public static void ListAliasesConfig(IList<string> lines)
        {
            IList<CAliasCommand> aliases = CRegistery.ListAliases();

            for (int i = 0; i < aliases.Count; ++i)
            {
                lines.Add(ToString(aliases[i]));
            }
        }

        private static string ToString(CAliasCommand cmd)
        {
            return string.Format("alias {0} {1}", cmd.Name, StringUtils.Arg(cmd.Alias));
        }
    }

    [CCommand("unalias", Description="Remove an alias name for command(s)")]
    class Cmd_unalias : CCommand
    {
        void Execute(string name)
        {
            if (CRegistery.RemoveAlias(name))
            {

                PostNotification(CCommandNotifications.CAliasesChanged, 
                    CCommandNotifications.KeyName, name,
                    CCommandNotifications.KeyManualMode, this.IsManualMode
                );
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("exit", Description="Shuts down a connected client.")]
    class Cmd_exit : CCommand
    {
        void Execute()
        {
            UnityEngine.Application.Quit();
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("toggle", Description="Toggles boolean cvar value.")]
    class Cmd_toggle : CCommand
    {
        bool Execute(string cvarName)
        {
            CVarCommand cmd = CRegistery.FindCvarCommand(cvarName);
            if (cmd == null)
            {
                PrintError("Can't find cvar '" + cvarName + "'");
                return false;
            }

            if (!cmd.IsInt)
            {
                PrintError("Can't toggle non-int value");
                return false;
            }

            cmd.SetValue(cmd.BoolValue ? 0 : 1);
            return true;
        }

        protected override string[] AutoCompleteArgs(string commandLine, string prefix)
        {
            IList<CVar> vars = CRegistery.ListVars(delegate(CVarCommand cmd)
                {
                    return cmd.IsBool && CRegistery.ShouldListCommand(cmd, prefix);
                });

            if (vars.Count == 0)
            {
                return null;
            }

            string[] values = new string[vars.Count];
            for (int i = 0; i < vars.Count; ++i)
            {
                values[i] = StringUtils.C(vars[i].Name, ColorCode.TableVar);
            }
            return values;
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("reset", Description="Resets cvar to its default value.")]
    class Cmd_reset : CCommand
    {
        bool Execute(string name)
        {
            CVarCommand cmd = CRegistery.FindCvarCommand(name);
            if (cmd == null)
            {
                PrintError("Can't find cvar: '{0}'", name);
                return false;
            }

            cmd.SetDefault();
            return true;
        }

        protected override string[] AutoCompleteArgs(string commandLine, string prefix)
        {
            IList<CVar> vars = CRegistery.ListVars(prefix);
            if (vars.Count == 0)
            {
                return null;
            }

            string[] values = new string[vars.Count];
            for (int i = 0; i < vars.Count; ++i)
            {
                values[i] = StringUtils.C(vars[i].Name, ColorCode.TableVar);
            }
            return values;
        }
    }

    [CCommand("cvar_restart", Description="Resets all cvars to their default values.")]
    class Cmd_cvar_restart : CCommand
    {
        void Execute(string prefix = null)
        {
            IList<CCommand> cmds = CRegistery.ListCommands(prefix);
            foreach (CCommand cmd in cmds)
            {
                CVarCommand cvarCmd = cmd as CVarCommand;
                if (cvarCmd != null)
                {
                    cvarCmd.SetDefault();
                }
            }
        }

        protected override string[] AutoCompleteArgs(string commandLine, string prefix)
        {
            IList<CVar> vars = CRegistery.ListVars(prefix);
            if (vars.Count == 0)
            {
                return null;
            }

            string[] values = new string[vars.Count];
            for (int i = 0; i < vars.Count; ++i)
            {
                values[i] = StringUtils.C(vars[i].Name, ColorCode.TableVar);
            }
            return values;
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    #if LUNAR_DEVELOPMENT

    [CCommand("test")]
    class Cmd_test : CCommand
    {
        bool Execute()
        {
            Log.d("Debug Debug Debug Debug Debug Debug Debug Debug Debug Debug Debug Debug Debug Debug Debug Debug Debug Debug Debug");
            Log.w("Warning Warning Warning Warning Warning Warning Warning Warning Warning Warning Warning Warning Warning Warning Warning");
            Log.i("Info Info Info Info Info Info Info Info Info Info Info Info Info Info Info Info Info Info Info Info Info Info Info ");
            Log.v("Verbose Verbose Verbose Verbose Verbose Verbose Verbose Verbose Verbose Verbose Verbose Verbose Verbose Verbose Verbose ");
            Log.e("Error Error Error Error Error Error Error Error Error Error Error Error Error Error Error Error Error Error Error ");

            return true;
        }
    }

    [CCommand("throw_exception")]
    class Cmd_throw_exception : CCommand
    {
        bool Execute()
        {
            TimerManager.ScheduleTimer(delegate()
            {
                throw new Exception("Test exception");
            });

            return true;
        }
    }

    #endif

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("echo", Description="Logs debug message to Unity console")]
    class Cmd_echo : CCommand
    {
        void Execute(string message)
        {
            Debug.Log(message);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("exec", Description="Executes a config file.")]
    class Cmd_exec : CCommand
    {
        bool Execute(string filename)
        {
            string path = CCommandHelper.GetConfigPath(filename);
            if (!FileUtils.FileExists(path))
            {
                if (this.IsManualMode)
                {
                    PrintError("Can't exec config: file not found");
                }
                return false;
            }

            IList<string> lines = FileUtils.Read(path);
            if (lines == null)
            {
                if (this.IsManualMode)
                {
                    PrintError("Can't exec config: error reading file");
                }
                return false;
            }

            foreach (string line in lines)
            {
                string trim = line.Trim();
                if (trim.Length == 0 || trim.StartsWith("//"))
                {
                    continue;
                }

                ExecCommand(line);
            }

            PostNotification(CCommandNotifications.ConfigLoaded);
            return true;
        }
    }

    [CCommand("writeconfig", Description="Writes a config file.")]
    class Cmd_writeconfig : CCommand
    {
        void Execute(string filename)
        {
            IList<string> lines = ReusableLists.NextAutoRecycleList<string>();

            // cvars
            ListCvars(lines);

            // bindings
            ListBindings(lines);

            // aliases
            ListAliases(lines);

            string path = CCommandHelper.GetConfigPath(filename);
            FileUtils.Write(path, lines);
        }

        private static void ListCvars(IList<string> lines)
        {
            IList<CVar> cvars = CRegistery.ListVars(delegate(CVarCommand cmd)
            {
                return !cmd.IsDefault && !cmd.HasFlag(CFlags.NoArchive);
            });

            if (cvars.Count > 0)
            {
                lines.Add("// cvars");
            }

            for (int i = 0; i < cvars.Count; ++i)
            {
                CVar c = cvars[i];
                if (c.Value != null)
                {
                    lines.Add(string.Format("{0} {1}", c.Name, StringUtils.Arg(c.Value)));
                }
                else
                {
                    lines.Add("null " + c.Name);
                }
            }
        }

        private static void ListBindings(IList<string> lines)
        {
            IList<CBinding> bindings = CBindings.List();
            if (bindings.Count > 0)
            {
                lines.Add("// key bindings");
            }

            for (int i = 0; i < bindings.Count; ++i)
            {
                lines.Add(string.Format("bind {0} {1}", bindings[i].name, StringUtils.Arg(bindings[i].cmd)));
            }
        }

        private static void ListAliases(IList<string> lines)
        {
            lines.Add("");
            lines.Add("// aliases");
            Cmd_alias.ListAliasesConfig(lines);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("cat", Description="Prints the content of a config file")]
    class Cmd_cat : CCommand
    {
        bool Execute(string filename = null)
        {
            string name = filename != null ? filename : "default.cfg";

            string path = CCommandHelper.GetConfigPath(name);
            if (!FileUtils.FileExists(path))
            {
                PrintError("Can't find config file: '{0}'", path);
                return false;
            }

            IList<string> lines = FileUtils.Read(path);
            foreach (string line in lines)
            {
                PrintIndent(line);
            }

            return true;
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("man", Description="Prints command usage")]
    class Cmd_man : CCommand
    {
        bool Execute(string command)
        {
            CCommand cmd = CRegistery.FindCommand(command);
            if (cmd == null)
            {
                PrintError("{0}: command not found \"{1}\"", this.Name, command);
                return false;
            }

            cmd.Delegate = this.Delegate;
            cmd.PrintUsage(true);
            cmd.Delegate = null;

            return true;
        }

        protected override string[] AutoCompleteArgs(string commandLine, string token)
        {
            // TODO: add unit tests

            IList<CCommand> commands = CRegistery.ListCommands(delegate(CCommand command)
                {
                    return !(command is CVarCommand) && 
                        !(command is CDelegateCommand) && 
                        !(command is CAliasCommand) &&
                        CRegistery.ShouldListCommand(command, token);
                });

            if (commands.Count == 0)
            {
                return null;
            }

            string[] values = new string[commands.Count];
            for (int i = 0; i < commands.Count; ++i)
            {
                values[i] = StringUtils.C(commands[i].Name, commands[i].ColorCode);
            }
            return values;
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    class CVars
    {
        public static readonly CVar c_historySize = new CVar("c_historySize", 32768, CFlags.System);
        public static readonly CVar d_assertsEnabled = new CVar("d_assertsEnabled", Config.isDebugBuild, CFlags.System);

        #if LUNAR_DEVELOPMENT
        public static readonly CVar g_drawVisibleCells = new CVar("g_drawVisibleCells", false);
        #endif
    }

    static class CCommandHelper
    {
        public static string CreateCommandLine(string[] args, int startIndex = 0)
        {
            StringBuilder buffer = StringBuilderPool.NextAutoRecycleBuilder();
            for (int i = startIndex; i < args.Length; ++i)
            {
                buffer.Append(StringUtils.Arg(args[i]));
                if (i < args.Length - 1)
                {
                    buffer.Append(' ');
                }
            }

            return buffer.ToString();
        }

        public static string GetConfigPath(string filename)
        {
            string path = FileUtils.ChangeExtension(filename, ".cfg");
            if (FileUtils.IsPathRooted(path))
            {
                return path;
            }

            return System.IO.Path.Combine(ConfigPath, path);
        }

        public static string ConfigPath
        {
            get { return System.IO.Path.Combine(FileUtils.DataPath, "configs"); }
        }
    }
}
