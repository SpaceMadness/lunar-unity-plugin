using System;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using LunarPlugin;
using LunarPluginInternal;
using LunarEditor;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;
    
    [TestFixture]
    public class CommandAutocompletionTest : CCommandTestFixture
    {
        //////////////////////////////////////////////////////////////////////////////
        // Commands

        [Test]
        public void testCommandsSuggestion()
        {
            assertSuggestions("¶", //
                    "Alias1",//
                    "Alias2",//
                    "Alias3",//
                    "test1", //
                    "test2", //
                    "test3", //
                    "Var1",  //
                    "Var12", //
                    "Var2"
            );
        }

        [Test]
        public void testCommandsSuggestionFiltered1()
        {
            assertSuggestions("a¶", //
                    "Alias1",//
                    "Alias2",//
                    "Alias3"
            );
        }

        [Test]
        public void testCommandsSuggestionFiltered2()
        {
            assertSuggestions("t¶", //
                    "test1", //
                    "test2", //
                    "test3"
            );
        }

        [Test]
        public void testCommandsSuggestionFiltered3()
        {
            assertSuggestions("v¶", //
                    "Var1",  //
                    "Var12", //
                    "Var2"
            );
        }

        [Test]
        public void testCommandsSuggestionFiltered4()
        {
            assertSuggestions("var1¶", //
                    "Var1",  //
                    "Var12"
            );
        }

        [Test]
        public void testCommandsSuggestionFiltered5()
        {
            assertSuggestions("var12¶", //
                    "Var12"
            );
        }

        [Test]
        public void testCommandsSuggestionFiltered6()
        {
            assertSuggestions("Var123¶");
        }

        //////////////////////////////////////////////////////////////////////////////
        // Options

        [Test]
        public void testOptionsSuggestion()
        {
            assertSuggestions("test1 --¶", //
                    "--boolOpt1", //
                    "--boolOpt12", //
                    "--boolOpt2", //
                    "--op1", //
                    "--op11", //
                    "--op111", //
                    "--op112", //
                    "--op113", //
                    "--op12", //
                    "--op13"
            );
        }

        [Test]
        public void testOptionsSuggestionMultiple()
        {
            assertSuggestions("test1 --boolOpt1 --¶", //
                    "--boolOpt1", //
                    "--boolOpt12", //
                    "--boolOpt2", //
                    "--op1", //
                    "--op11", //
                    "--op111", //
                    "--op112", //
                    "--op113", //
                    "--op12", //
                    "--op13"
            );
        }

        [Test]
        public void testOptionsSuggestionMultipleWithValue()
        {
            assertSuggestions("test1 --boolOpt1 --op1 value1 --¶", //
                    "--boolOpt1", //
                    "--boolOpt12", //
                    "--boolOpt2", //
                    "--op1", //
                    "--op11", //
                    "--op111", //
                    "--op112", //
                    "--op113", //
                    "--op12", //
                    "--op13"
            );
        }

        [Test]
        public void testOptionsSuggestionMultipleWithIncorrectValue()
        {
            assertSuggestions("test1 --boolOpt1 --op1 foo --¶", //
                    "--boolOpt1", //
                    "--boolOpt12", //
                    "--boolOpt2", //
                    "--op1", //
                    "--op11", //
                    "--op111", //
                    "--op112", //
                    "--op113", //
                    "--op12", //
                    "--op13"
            );
        }

        [Test]
        public void testOptionsSuggestionMultipleWithArg()
        {
            assertSuggestions("test1 --boolOpt1 --op12 arg --¶", //
                    "--boolOpt1", //
                    "--boolOpt12", //
                    "--boolOpt2", //
                    "--op1", //
                    "--op11", //
                    "--op111", //
                    "--op112", //
                    "--op113", //
                    "--op12", //
                    "--op13"
            );
        }

        [Test]
        public void testOptionsSuggestionFiltered1()
        {
            assertSuggestions("test1 --b¶", //
                    "--boolOpt1", //
                    "--boolOpt12", //
                    "--boolOpt2"
            );
        }

        [Test]
        public void testOptionsSuggestionFiltered2()
        {
            assertSuggestions("test1 --boolOpt1¶", //
                    "--boolOpt1", //
                    "--boolOpt12"
            );
        }

        [Test]
        public void testOptionsSuggestionFiltered3()
        {
            assertSuggestions("test1 --boolOpt12¶", //
                    "--boolOpt12"
            );
        }

        [Test]
        public void testOptionsSuggestionFiltered4()
        {
            assertSuggestions("test2 --boolOpt ¶");
        }

        [Test]
        public void testOptionsSuggestionFiltered5()
        {
            assertSuggestions("test1 --boolOpt123¶");
        }

        [Test]
        public void testOptionsSuggestionFilteredWithArgs()
        {
            assertSuggestions("test1 --boolOpt12 ¶", //
                    "foo", //
                    "val1", //
                    "val12", //
                    "val123", //
                    "val2" //
            );
        }

        [Test]
        public void testOptionValuesSuggestion()
        {
            assertSuggestions("test3 --opt ¶", //
                    "a2", "aa1", "aa11", "aa111", "aa112", "aa113", "aa12", "aa13", "b"
            );
        }

        [Test]
        public void testOptionValuesSuggestionFiltered1()
        {
            assertSuggestions("test3 --opt a¶", //
                    "a2", "aa1", "aa11", "aa111", "aa112", "aa113", "aa12", "aa13"
            );
        }

        [Test]
        public void testOptionValuesSuggestionFiltered2()
        {
            assertSuggestions("test3 --opt aa111¶", //
                    "aa111"
            );
        }

        [Test]
        public void testOptionValuesSuggestionFilteredWithArgs1()
        {
            assertSuggestions("test3 --opt aa111 ¶",
                    "arg1", //
                    "arg12", //
                    "arg2"
            );
        }

        [Test]
        public void testOptionValuesSuggestionFilteredWithArgs2()
        {
            assertSuggestions("test3 --opt aa111 arg1¶",
                    "arg1", //
                    "arg12"
            );
        }

        //////////////////////////////////////////////////////////////////////////////
        // Lifecycle

        [SetUp]
        protected override void RunSetUp()
        {
            base.RunSetUp();
        
            new CVar("Var1", false);
            new CVar("Var12", false);
            new CVar("Var2", false);

            RegisterCommand(typeof(Cmd_alias));

            RegisterCommand(typeof(Cmd_test1), false);
            RegisterCommand(typeof(Cmd_test2), false);
            RegisterCommand(typeof(Cmd_test3), false);

            Execute("alias Alias1 test1");
            Execute("alias Alias2 test2");
            Execute("alias Alias3 test3");
        }

        //////////////////////////////////////////////////////////////////////////////
        // Custom commands

        class Cmd_test1 : CCommand
        {
            [CCommandOption(ShortName="o1")]
            public string op1;
            
            [CCommandOption(ShortName="o11")]
            public string op11;
            
            [CCommandOption(ShortName="o12")]
            public string op12;
            
            [CCommandOption(ShortName="o13")]
            public string op13;
            
            [CCommandOption(ShortName="o111")]
            public string op111;
            
            [CCommandOption(ShortName="o112")]
            public string op112;
            
            [CCommandOption(ShortName="o113")]
            public string op113;
            
            [CCommandOption(ShortName="b1")]
            public bool boolOpt1;
            
            [CCommandOption(ShortName="b12")]
            public bool boolOpt12;
            
            [CCommandOption(ShortName="b2")]
            public bool boolOpt2;
            
            public Cmd_test1()
            {
                this.Values = new string[] {
                    "foo",
                    "val1",
                    "val123",
                    "val12",
                    "val2"
                };
            }
            
            void Execute()
            {
            }
        }

        class Cmd_test2 : CCommand
        {
            [CCommandOption(ShortName="b")]
            public bool boolOpt;
            
            void Execute()
            {
            }
        }

        class Cmd_test3 : CCommand
        {
            [CCommandOption(ShortName="o", Values="aa1,aa11,aa12,aa13,aa111,aa112,aa113,a2,b")]
            public string opt;
            
            [CCommandOption(ShortName="a", Values="aa1,aa11,aa12,aa13,aa111,aa112,aa113,a2,b")]
            public string act;
         
            public Cmd_test3()
            {
                this.Values = new string[] { "arg1", "arg12", "arg2" };
            }

            void Execute()
            {
            }
        }
    }
}

