//
//  Commands.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using System;
using System.Collections.Generic;
using System.Text;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
    //////////////////////////////////////////////////////////////////////////////
    
    [CCommand("cmdlist", Description="Lists all available terminal commands.")]
    class Cmd_cmdlist : CCommand
    {
        [CCommandOption(Name="all", ShortName="a", Description="List all commands (including system)")]
        private bool includeSystem;
        
        bool Execute(string prefix = null)
        {
            CommandListOptions options = CCommand.DefaultListOptions;
            if (includeSystem)
            {
                options |= CommandListOptions.System;
            }
            
            Print(ListCommandNames(prefix, options));
            
            return true;
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return ListCommandNames(token, CCommand.DefaultListOptions);
        }

        private string[] ListCommandNames(string prefix, CommandListOptions options)
        {
            IList<CCommand> commands = CRegistery.ListCommands(delegate(CCommand cmd)
            {
                return !(cmd is CVarCommand) && CRegistery.ShouldListCommand(cmd, prefix, options);
            });

            return Collection.Map(commands, delegate(CCommand cmd)
            {
                return C(cmd.Name, cmd.ColorCode);
            });
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
            CommandListOptions options = CCommand.DefaultListOptions;
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
                    string[] names = Collection.Map(vars, delegate(CVar cvar) {
                        return StringUtils.C(cvar.Name, ColorCode.TableVar);
                    });
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

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return CRegistery.ListVarNames(token, CCommand.DefaultListOptions);
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

            cmd.ParentCommand = this;
            cmd.SetValue(cmd.BoolValue ? 0 : 1);
            cmd.ParentCommand = null;

            return true;
        }
        
        protected override IList<string> AutoCompleteArgs(string commandLine, string prefix)
        {
            IList<CVar> vars = CRegistery.ListVars(delegate(CVarCommand cmd)
            {
                return cmd.IsBool && CRegistery.ShouldListCommand(cmd, prefix, CCommand.DefaultListOptions);
            });
            
            if (vars.Count == 0)
            {
                return null;
            }

            return Collection.Map(vars, delegate(CVar cvar) {
                return StringUtils.C(cvar.Name, ColorCode.TableVar);
            });
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

            cmd.ParentCommand = this;
            cmd.SetDefault();
            cmd.ParentCommand = null;

            return true;
        }
        
        protected override IList<string> AutoCompleteArgs(string commandLine, string prefix)
        {
            return AutoCompleteArgs(prefix);
        }

        internal static IList<string> AutoCompleteArgs(string prefix)
        {
            IList<CVar> vars = CRegistery.ListVars(prefix, CCommand.DefaultListOptions);
            if (vars.Count == 0)
            {
                return null;
            }

            return Collection.Map(vars, delegate(CVar cvar)
            {
                return StringUtils.C(cvar.Name, ColorCode.TableVar);
            });
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("resetAll", Description="Resets all cvars to their default values.")]
    class Cmd_resetAll : CCommand
    {
        void Execute(string prefix = null)
        {
            IList<CCommand> cmds = CRegistery.ListCommands(prefix);
            foreach (CCommand cmd in cmds)
            {
                CVarCommand cvarCmd = cmd as CVarCommand;
                if (cvarCmd != null)
                {
                    cvarCmd.ParentCommand = this;
                    cvarCmd.SetDefault();
                    cvarCmd.ParentCommand = null;
                }
            }
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string prefix)
        {
            return Cmd_reset.AutoCompleteArgs(prefix);
        }
    }
    
    //////////////////////////////////////////////////////////////////////////////
    
    [CCommand("bind", Description="Assigns a key to command(s).")]
    class Cmd_bind : CCommand
    {
        public Cmd_bind()
        {
            this.IsIgnoreOptions = true; // we don't want to miss negative operation commands
        }

        bool Execute(string stroke, string command = null) // TODO: refactor me!
        {
            string name = stroke.ToLower();
            
            if (!string.IsNullOrEmpty(command))
            {
                CShortCut shortCut;
                if (!CShortCut.TryParse(stroke, out shortCut))
                {
                    PrintError("Invalid shortcut: {0}", name);
                    return false;
                }

                string keyUpCommand = null;

                char keyDownOp = command[0];
                if (keyDownOp == '+' || keyDownOp == '-')
                {
                    if (command.Length == 1)
                    {
                        PrintError("Identifier expected!");
                        return false;
                    }

                    string identifier = command.Substring(1);

                    // register operation command
                    CCommand keyDownCmd = CRegistery.FindCommand(command);
                    if (keyDownCmd == null)
                    {
                        keyDownCmd = new COperationCommand(keyDownOp, identifier);
                        CRegistery.Register(keyDownCmd);
                        keyDownCmd.SetFlag(CCommandFlags.System, true);
                    }

                    // register opposite operation command
                    char keyUpOp = OppositeOperation(keyDownOp);
                    keyUpCommand = keyUpOp + identifier;
                    CCommand keyUpCmd = CRegistery.FindCommand(keyUpCommand);
                    if (keyUpCmd == null)
                    {
                        keyUpCmd = new COperationCommand(keyUpOp, identifier);
                        CRegistery.Register(keyUpCmd);
                        keyUpCmd.SetFlag(CCommandFlags.System, true);
                    }
                }
                
                CBindings.Bind(shortCut, StringUtils.UnArg(command), keyUpCommand != null ? StringUtils.UnArg(keyUpCommand) : null);
                
                PostNotification(
                    CCommandNotifications.CBindingsChanged,
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

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return StringUtils.Filter(CBindings.BindingsNames, token);
        }

        private static char OppositeOperation(char op)
        {
            if (op == '+') return '-';
            if (op == '-') return '+';

            throw new ArgumentException("Unknown operation '" + op + "'");
        }
        
        public static string ToString(CBinding b)
        {
            return string.Format("bind {0} {1}", b.shortCut.ToString(), StringUtils.Arg(b.cmdKeyDown));
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
                PrintIndent("bind {0} {1}", b.shortCut.ToString(), StringUtils.Arg(b.cmdKeyDown));
            }
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return AutoCompleteArgs(token);
        }

        internal static IList<string> AutoCompleteArgs(string token)
        {
            return CBindings.ListShortCuts(token);
        }
    }
    
    //////////////////////////////////////////////////////////////////////////////
    
    [CCommand("unbind", Description="Unbinds a key.")]
    class Cmd_unbind : CCommand
    {
        bool Execute(string key)
        {
            string token = key.ToLower();

            CShortCut shortCut;
            if (!CShortCut.TryParse(token, out shortCut))
            {
                PrintError("Invalid shortcut: {0}", token);
                return false;
            }
            
            CBindings.Unbind(shortCut); // TODO: unbind 'operation' commands
            
            PostNotification(
                CCommandNotifications.CBindingsChanged,
                CCommandNotifications.KeyManualMode, this.IsManualMode
            );
            
            return true;
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return Cmd_bindlist.AutoCompleteArgs(token);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("unbindAll", Description="Unbinds all keys.")]
    class Cmd_unbindAll : CCommand
    {
        void Execute()
        {
            if (CBindings.Count > 0)
            {
                CBindings.Clear();
                PostNotification();
            }
        }

        void Execute(string prefix)
        {
            if (CBindings.Count > 0)
            {
                IList<CBinding> bindings = CBindings.List(prefix);
                if (bindings.Count > 0)
                {
                    foreach (CBinding binding in bindings)
                    {
                        CBindings.Unbind(binding.shortCut);
                    }

                    PostNotification();
                }
            }
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return Cmd_bindlist.AutoCompleteArgs(token);
        }

        void PostNotification()
        {
            PostNotification(CCommandNotifications.CBindingsChanged, 
                CCommandNotifications.KeyManualMode, this.IsManualMode);
        }
    }
    
    //////////////////////////////////////////////////////////////////////////////
    
    [CCommand("alias", Description="Creates an alias name for command(s).")]
    class Cmd_alias : CCommand
    {
        void Execute(string name, string commands)
        {
            CRegistery.AddAlias(name, StringUtils.UnArg(commands));
            
            PostNotification(
                CCommandNotifications.CAliasesChanged, 
                CCommandNotifications.KeyName, name,
                CCommandNotifications.KeyManualMode, this.IsManualMode
            );
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return AutoCompleteArgs(token);
        }

        internal static IList<string> AutoCompleteArgs(string token)
        {
            IList<CAliasCommand> aliases = CRegistery.ListAliases();
            if (aliases != null && aliases.Count > 0)
            {
                return Collection.Map(aliases, delegate(CAliasCommand alias)
                {
                    return alias.Name;
                });
            }

            return null;
        }
        
        public static IList<string> ListAliasesConfig()
        {
            return Collection.Map(CRegistery.ListAliases(), delegate(CAliasCommand alias)
            {
                return ToString(alias);
            });
        }
        
        private static string ToString(CAliasCommand cmd)
        {
            return string.Format("alias {0} {1}", cmd.Name, StringUtils.Arg(cmd.Alias));
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("aliaslist", Description="List current aliases.")]
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
                    string[] names = Collection.Map(cmds, delegate(CAliasCommand cmd)
                    {
                        return cmd.Name;
                    });
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

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return Cmd_alias.AutoCompleteArgs(token);
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

    //////////////////////////////////////////////////////////////////////////////
    
    [CCommand("unalias", Description="Remove an alias name for command(s)")]
    class Cmd_unalias : CCommand
    {
        void Execute(string name)
        {
            if (CRegistery.RemoveAlias(name))
            {
                
                PostNotification(
                    CCommandNotifications.CAliasesChanged, 
                    CCommandNotifications.KeyName, name,
                    CCommandNotifications.KeyManualMode, this.IsManualMode
                );
            }
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return Cmd_alias.AutoCompleteArgs(token);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("unaliasAll", Description="Remove all command aliases")]
    class Cmd_unaliasAll : CCommand
    {
        void Execute(string prefix = null)
        {
            IList<CAliasCommand> aliases = CRegistery.ListAliases(prefix);

            if (aliases.Count > 0)
            {
                foreach (CAliasCommand alias in aliases)
                {
                    CRegistery.RemoveAlias(alias.Name);
                }

                PostNotification(
                    CCommandNotifications.CAliasesChanged, 
                    CCommandNotifications.KeyManualMode, this.IsManualMode
                );
            }
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return Cmd_alias.AutoCompleteArgs(token);
        }
    }

    //////////////////////////////////////////////////////////////////////////////
    
    [CCommand("exec", Description="Executes a config file.")]
    class Cmd_exec : CCommand
    {
        bool Execute(string filename)
        {
            try
            {
                IList<string> lines = ConfigHelper.ReadConfig(filename);
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
            catch (Exception e)
            {
                if (this.IsManualMode)
                {
                    PrintError("Can't load config: {0}", e);
                }

                return false;
            }
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return ConfigHelper.ListConfigs(token);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("writeconfig", Description="Writes a config file.")]
    class Cmd_writeconfig : CCommand
    {
        bool Execute(string filename)
        {
            IList<string> lines = ReusableLists.NextAutoRecycleList<string>();
            
            // cvars
            AddEntries(lines, ListCvars(), "cvars");
            
            // bindings
            AddEntries(lines, ListBindings(), "bindings");
            
            // aliases
            AddEntries(lines, ListAliases(), "aliases");

            try
            {
                ConfigHelper.WriteConfig(filename, lines);
                return true;
            }
            catch (Exception e)
            {
                if (this.IsManualMode)
                {
                    PrintError("Can't write config: " + e.Message);
                }
            }

            return false;
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string token) // TODO: better autocompletion
        {
            List<string> configs = new List<string>(ConfigHelper.ListConfigs(token));

            // TODO: refactor this
            if (!configs.Contains(Constants.ConfigDefault))
            {
                configs.Add(Constants.ConfigDefault);
            }
            if (!configs.Contains(Constants.ConfigAutoExec))
            {
                configs.Add(Constants.ConfigAutoExec);
            }
            if (!configs.Contains(Constants.ConfigPlayMode))
            {
                configs.Add(Constants.ConfigPlayMode);
            }

            return StringUtils.Filter(configs, token);
        }

        private static void AddEntries(IList<string> outList, IList<string> entries, string groupName)
        {
            if (entries.Count > 0)
            {
                if (outList.Count > 0)
                {
                    outList.Add("");
                }

                outList.Add("// " + groupName);
                foreach (string entry in entries)
                {
                    outList.Add(entry);
                }
            }
        }

        private static IList<string> ListCvars()
        {
            IList<CVar> cvars = CRegistery.ListVars(delegate(CVarCommand cmd)
            {
                return !cmd.IsDefault && !cmd.HasFlag(CFlags.NoArchive);
            });

            IList<string> entries = new List<string>(cvars.Count);
            foreach (CVar cvar in cvars)
            {
                if (cvar.Value != null)
                {
                    entries.Add(string.Format("{0} {1}", cvar.Name, StringUtils.Arg(cvar.Value)));
                }
                else
                {
                    entries.Add("null " + cvar.Name);
                }
            }

            return entries;
        }
        
        private static IList<string> ListBindings()
        {
            IList<CBinding> bindings = CBindings.List();

            IList<string> entries = new List<string>(bindings.Count);
            foreach (CBinding binding in bindings)
            {
                entries.Add(string.Format("bind {0} {1}", binding.shortCut.ToString(), StringUtils.Arg(binding.cmdKeyDown)));
            }

            return entries;
        }
        
        private static IList<string> ListAliases()
        {
            return Cmd_alias.ListAliasesConfig();
        }
    }
    
    //////////////////////////////////////////////////////////////////////////////
    
    [CCommand("cat", Description="Prints the content of a config file")]
    class Cmd_cat : CCommand
    {
        [CCommandOption(ShortName="e", Description="Opens config editor")]
        bool edit;

        bool Execute(string filename)
        {
            try
            {
                if (edit)
                {
                    string configPath = ConfigHelper.GetConfigPath(filename);
                    if (!FileUtils.FileExists(configPath))
                    {
                        PrintError("File does not exist: {0}", configPath);
                        return false;
                    }

                    EditorUtility.OpenWithDefaultApp(configPath);
                    return true;
                }
                
                IList<string> lines = ConfigHelper.ReadConfig(filename);
                foreach (string line in lines)
                {
                    PrintIndent(line);
                }
                
                return true;
            }
            catch (Exception e)
            {
                PrintIndent("Can't read config: {0}", e.Message);
                return false;
            }
        }

        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            return ConfigHelper.ListConfigs(token);
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
        
        protected override IList<string> AutoCompleteArgs(string commandLine, string token)
        {
            IList<CCommand> commands = CRegistery.ListCommands(delegate(CCommand command)
            {
                return !(command is CVarCommand) && 
                       !(command is CDelegateCommand) && 
                       !(command is CAliasCommand) &&
                       CRegistery.ShouldListCommand(command, token, CCommand.DefaultListOptions);
            });
            
            if (commands.Count == 0)
            {
                return null;
            }

            return Collection.Map(commands, delegate(CCommand cmd)
            {
                return StringUtils.C(cmd.Name, cmd.ColorCode);
            });
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("clear", Description="Clears current terminal window.")]
    class Cmd_clear : CCommand
    {
        void Execute()
        {
            ClearTerminal();
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    [CCommand("menu", Description="Invokes the menu item in the specified path")]
    class Cmd_menu : CCommand
    {
        bool Execute(string menuItemPath)
        {
            return EditorApplication.ExecuteMenuItem(menuItemPath);
        }
    }

    //////////////////////////////////////////////////////////////////////////////
    
    [CCommand("timeScale", Description="The scale at which the time is passing")]
    class Cmd_timeScale : CPlayModeCommand
    {
        [CCommandOption(ShortName="v")]
        bool verbose;

        void Execute()
        {
            PrintCurrent();
        }

        bool Execute(float value)
        {
            if (value >= 0)
            {
                Time.timeScale = value;

                if (verbose)
                {
                    PrintCurrent();
                }

                return true;
            }

            PrintError("Wrong value: {0}", value.ToString());
            return false;
        }

        void PrintCurrent()
        {
            PrintIndent("Time scale: {0}", Time.timeScale.ToString());
        }
    }

    #if LUNAR_DEVELOPMENT

    //////////////////////////////////////////////////////////////////////////////
    // These are unrelease commands (might be available in future versions)
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

    [CCommand("colors")]
    class Cmd_colors : CCommand
    {
        void Execute()
        {
            string[] names = Enum.GetNames(typeof(ColorCode));
            for (int i = 0; i < names.Length; ++i)
            {
                Print("{0}: {1}", i, StringUtils.C(names[i], (ColorCode)i));
            }
        }
        
        bool Execute(int index, string rgb)
        {
            uint value;
            try
            {
                value = Convert.ToUInt32(rgb, 16);
            }
            catch (Exception)
            {
                PrintError("Wrong color value");
                return false; 
            }
            
            ColorCode[] values = (ColorCode[]) Enum.GetValues(typeof(ColorCode));
            if (index >= 0 && index < values.Length)
            {
                Color color = ColorUtils.FromRGB(value);
                EditorSkin.SetColor(values[index], color);
                
                Print("{0}: {1}", index, StringUtils.C(values[index].ToString(), values[index]));
            }
            else
            {
                PrintError("Wrong index");
                Execute();
            }
            
            return true;
        }
    }
    
    [CCommand("break")]
    class Cmd_break : CPlayModeCommand
    {
        void Execute()
        {
            Editor.Break();
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

    [CCommand("throw_exception")]
    class Cmd_throw_exception : CCommand
    {
        bool Execute()
        {
            throw new Exception("Test exception");
        }
    }

    [CCommand("echo", Description="Logs debug message to Unity console")]
    class Cmd_echo : CCommand
    {
        void Execute(string message)
        {
            Debug.Log(message);
        }
    }
    
    #endif

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
    }
}
