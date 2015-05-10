using UnityEngine;

using System;
using System.Collections.Generic;

namespace LunarPluginInternal
{
    struct CBinding
    {
        private KeyCode m_key;
        private string m_name;
        private string m_cmdKeyDown;
        private string m_cmdKeyUp;

        public CBinding(KeyCode key, string cmd, string cmdOpposite)
        {
            m_key = key;
            m_cmdKeyDown = cmd;
            m_cmdKeyUp = cmdOpposite;
            m_name = null;

            UpdateName();
        }

        private void UpdateName()
        {
            m_name = m_key.ToString().ToLower();
        }

        #region Properties

        public KeyCode key
        {
            get { return m_key; }
            set
            {
                m_key = value;
                UpdateName();
            }
        }

        public string cmdKeyDown
        {
            get { return m_cmdKeyDown; }
            set { m_cmdKeyDown = value; }
        }

        public string cmdKeyUp
        {
            get { return m_cmdKeyUp; }
            set { m_cmdKeyUp = value; }
        }

        public string name 
        {
            get { return m_name; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} {1}", key, cmdKeyDown);
        }
    }

    delegate bool CCommandBindingsFilter(CBinding binding);

    class CBindings
    {
        private static List<CBinding> m_bindings = new List<CBinding>();
        private static IDictionary<string, KeyCode> m_keycodeLookup;

        public static void Bind(KeyCode key, string cmd, string cmdOpposite = null)
        {
            if (cmd == null)
            {
                throw new NullReferenceException("Command is null");
            }
             
            int index = IndexOf(key);
            if (index != -1)
            {
                CBinding existing = m_bindings[index];
                existing.cmdKeyDown = cmd;
                existing.cmdKeyUp = cmdOpposite;
                m_bindings[index] = existing;
            }
            else
            {
                m_bindings.Add(new CBinding(key, cmd, cmdOpposite));
            }
        }

        public static bool Unbind(KeyCode key)
        {
            int index = IndexOf(key);
            if (index != -1)
            {
                m_bindings.RemoveAt(index);
                return true;
            }

            return false;
        }

        public static IList<CBinding> List(string prefix = null)
        {
            if (prefix != null)
            {
                return List(delegate(CBinding binding) {
                    return StringUtils.StartsWithIgnoreCase(binding.name, prefix);
                });
            }

            return m_bindings;
        }

        public static IList<CBinding> List(CCommandBindingsFilter filter)
        {
            if (filter == null)
            {
                throw new NullReferenceException("Filter is null");
            }

            IList<CBinding> list = ReusableLists.NextAutoRecycleList<CBinding>();
            for (int i = 0; i < m_bindings.Count; ++i)
            {
                if (filter(m_bindings[i]))
                {
                    list.Add(m_bindings[i]);
                }
            }

            return list;
        }

        public static bool FindBinding(KeyCode key, out CBinding result)
        {
            for (int i = 0; i < m_bindings.Count; ++i)
            {
                if (m_bindings[i].key == key)
                {
                    result = m_bindings[i];
                    return true;
                }
            }

            result = default(CBinding);
            return false;
        }

        public static void Clear()
        {
            m_bindings.Clear();
        }

        #region List operations

        // TODO: binary search
        private static int IndexOf(KeyCode key)
        {
            for (int i = 0; i < m_bindings.Count; ++i)
            {
                if (m_bindings[i].key == key)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region Lookup

        private static IDictionary<string, KeyCode> CreateLookup()
        {
            IDictionary<string, KeyCode> lookup = new Dictionary<string, KeyCode>();
            lookup["backspace"] = KeyCode.Backspace;
            lookup["delete"] = KeyCode.Delete;
            lookup["tab"] = KeyCode.Tab;
            lookup["clear"] = KeyCode.Clear;
            lookup["return"] = KeyCode.Return;
            lookup["pause"] = KeyCode.Pause;
            lookup["escape"] = KeyCode.Escape;
            lookup["space"] = KeyCode.Space;
            lookup["num0"] = KeyCode.Keypad0;
            lookup["num1"] = KeyCode.Keypad1;
            lookup["num2"] = KeyCode.Keypad2;
            lookup["num3"] = KeyCode.Keypad3;
            lookup["num4"] = KeyCode.Keypad4;
            lookup["num5"] = KeyCode.Keypad5;
            lookup["num6"] = KeyCode.Keypad6;
            lookup["num7"] = KeyCode.Keypad7;
            lookup["num8"] = KeyCode.Keypad8;
            lookup["num9"] = KeyCode.Keypad9;
            lookup["0"] = KeyCode.Alpha0;
            lookup["1"] = KeyCode.Alpha1;
            lookup["2"] = KeyCode.Alpha2;
            lookup["3"] = KeyCode.Alpha3;
            lookup["4"] = KeyCode.Alpha4;
            lookup["5"] = KeyCode.Alpha5;
            lookup["6"] = KeyCode.Alpha6;
            lookup["7"] = KeyCode.Alpha7;
            lookup["8"] = KeyCode.Alpha8;
            lookup["9"] = KeyCode.Alpha9;
            lookup["keypadperiod"] = KeyCode.KeypadPeriod;
            lookup["keypaddivide"] = KeyCode.KeypadDivide;
            lookup["keypadmultiply"] = KeyCode.KeypadMultiply;
            lookup["keypadminus"] = KeyCode.KeypadMinus;
            lookup["keypadplus"] = KeyCode.KeypadPlus;
            lookup["keypadenter"] = KeyCode.KeypadEnter;
            lookup["keypadequals"] = KeyCode.KeypadEquals;
            lookup["uparrow"] = KeyCode.UpArrow;
            lookup["downarrow"] = KeyCode.DownArrow;
            lookup["rightarrow"] = KeyCode.RightArrow;
            lookup["leftarrow"] = KeyCode.LeftArrow;
            lookup["insert"] = KeyCode.Insert;
            lookup["home"] = KeyCode.Home;
            lookup["end"] = KeyCode.End;
            lookup["pageup"] = KeyCode.PageUp;
            lookup["pagedown"] = KeyCode.PageDown;
            lookup["f1"] = KeyCode.F1;
            lookup["f2"] = KeyCode.F2;
            lookup["f3"] = KeyCode.F3;
            lookup["f4"] = KeyCode.F4;
            lookup["f5"] = KeyCode.F5;
            lookup["f6"] = KeyCode.F6;
            lookup["f7"] = KeyCode.F7;
            lookup["f8"] = KeyCode.F8;
            lookup["f9"] = KeyCode.F9;
            lookup["f10"] = KeyCode.F10;
            lookup["f11"] = KeyCode.F11;
            lookup["f12"] = KeyCode.F12;
            lookup["f13"] = KeyCode.F13;
            lookup["f14"] = KeyCode.F14;
            lookup["f15"] = KeyCode.F15;
            lookup["alpha0"] = KeyCode.Alpha0;
            lookup["alpha1"] = KeyCode.Alpha1;
            lookup["alpha2"] = KeyCode.Alpha2;
            lookup["alpha3"] = KeyCode.Alpha3;
            lookup["alpha4"] = KeyCode.Alpha4;
            lookup["alpha5"] = KeyCode.Alpha5;
            lookup["alpha6"] = KeyCode.Alpha6;
            lookup["alpha7"] = KeyCode.Alpha7;
            lookup["alpha8"] = KeyCode.Alpha8;
            lookup["alpha9"] = KeyCode.Alpha9;
            lookup["exclaim"] = KeyCode.Exclaim;
            lookup["doublequote"] = KeyCode.DoubleQuote;
            lookup["hash"] = KeyCode.Hash;
            lookup["dollar"] = KeyCode.Dollar;
            lookup["ampersand"] = KeyCode.Ampersand;
            lookup["quote"] = KeyCode.Quote;
            lookup["leftparen"] = KeyCode.LeftParen;
            lookup["rightparen"] = KeyCode.RightParen;
            lookup["asterisk"] = KeyCode.Asterisk;
            lookup["plus"] = KeyCode.Plus;
            lookup["comma"] = KeyCode.Comma;
            lookup["minus"] = KeyCode.Minus;
            lookup["period"] = KeyCode.Period;
            lookup["slash"] = KeyCode.Slash;
            lookup["colon"] = KeyCode.Colon;
            lookup["semicolon"] = KeyCode.Semicolon;
            lookup["less"] = KeyCode.Less;
            lookup["equals"] = KeyCode.Equals;
            lookup["greater"] = KeyCode.Greater;
            lookup["question"] = KeyCode.Question;
            lookup["at"] = KeyCode.At;
            lookup["leftbracket"] = KeyCode.LeftBracket;
            lookup["backslash"] = KeyCode.Backslash;
            lookup["rightbracket"] = KeyCode.RightBracket;
            lookup["caret"] = KeyCode.Caret;
            lookup["underscore"] = KeyCode.Underscore;
            lookup["backquote"] = KeyCode.BackQuote;
            lookup["a"] = KeyCode.A;
            lookup["b"] = KeyCode.B;
            lookup["c"] = KeyCode.C;
            lookup["d"] = KeyCode.D;
            lookup["e"] = KeyCode.E;
            lookup["f"] = KeyCode.F;
            lookup["g"] = KeyCode.G;
            lookup["h"] = KeyCode.H;
            lookup["i"] = KeyCode.I;
            lookup["j"] = KeyCode.J;
            lookup["k"] = KeyCode.K;
            lookup["l"] = KeyCode.L;
            lookup["m"] = KeyCode.M;
            lookup["n"] = KeyCode.N;
            lookup["o"] = KeyCode.O;
            lookup["p"] = KeyCode.P;
            lookup["q"] = KeyCode.Q;
            lookup["r"] = KeyCode.R;
            lookup["s"] = KeyCode.S;
            lookup["t"] = KeyCode.T;
            lookup["u"] = KeyCode.U;
            lookup["v"] = KeyCode.V;
            lookup["w"] = KeyCode.W;
            lookup["x"] = KeyCode.X;
            lookup["y"] = KeyCode.Y;
            lookup["z"] = KeyCode.Z;
            lookup["numlock"] = KeyCode.Numlock;
            lookup["capslock"] = KeyCode.CapsLock;
            lookup["scrolllock"] = KeyCode.ScrollLock;
            lookup["rightshift"] = KeyCode.RightShift;
            lookup["leftshift"] = KeyCode.LeftShift;
            lookup["rightcontrol"] = KeyCode.RightControl;
            lookup["leftcontrol"] = KeyCode.LeftControl;
            lookup["rightalt"] = KeyCode.RightAlt;
            lookup["leftalt"] = KeyCode.LeftAlt;
            lookup["leftcommand"] = KeyCode.LeftCommand;
            lookup["leftapple"] = KeyCode.LeftApple;
            lookup["leftwindows"] = KeyCode.LeftWindows;
            lookup["rightcommand"] = KeyCode.RightCommand;
            lookup["rightapple"] = KeyCode.RightApple;
            lookup["rightwindows"] = KeyCode.RightWindows;
            lookup["altgr"] = KeyCode.AltGr;
            lookup["help"] = KeyCode.Help;
            lookup["print"] = KeyCode.Print;
            lookup["sysreq"] = KeyCode.SysReq;
            lookup["break"] = KeyCode.Break;
            lookup["menu"] = KeyCode.Menu;
            lookup["mouse0"] = KeyCode.Mouse0;
            lookup["mouse1"] = KeyCode.Mouse1;
            lookup["mouse2"] = KeyCode.Mouse2;
            lookup["mouse3"] = KeyCode.Mouse3;
            lookup["mouse4"] = KeyCode.Mouse4;
            lookup["mouse5"] = KeyCode.Mouse5;
            lookup["mouse6"] = KeyCode.Mouse6;
            lookup["joystickbutton0"] = KeyCode.JoystickButton0;
            lookup["joystickbutton1"] = KeyCode.JoystickButton1;
            lookup["joystickbutton2"] = KeyCode.JoystickButton2;
            lookup["joystickbutton3"] = KeyCode.JoystickButton3;
            lookup["joystickbutton4"] = KeyCode.JoystickButton4;
            lookup["joystickbutton5"] = KeyCode.JoystickButton5;
            lookup["joystickbutton6"] = KeyCode.JoystickButton6;
            lookup["joystickbutton7"] = KeyCode.JoystickButton7;
            lookup["joystickbutton8"] = KeyCode.JoystickButton8;
            lookup["joystickbutton9"] = KeyCode.JoystickButton9;
            lookup["joystickbutton10"] = KeyCode.JoystickButton10;
            lookup["joystickbutton11"] = KeyCode.JoystickButton11;
            lookup["joystickbutton12"] = KeyCode.JoystickButton12;
            lookup["joystickbutton13"] = KeyCode.JoystickButton13;
            lookup["joystickbutton14"] = KeyCode.JoystickButton14;
            lookup["joystickbutton15"] = KeyCode.JoystickButton15;
            lookup["joystickbutton16"] = KeyCode.JoystickButton16;
            lookup["joystickbutton17"] = KeyCode.JoystickButton17;
            lookup["joystickbutton18"] = KeyCode.JoystickButton18;
            lookup["joystickbutton19"] = KeyCode.JoystickButton19;
            lookup["joystick1button0"] = KeyCode.Joystick1Button0;
            lookup["joystick1button1"] = KeyCode.Joystick1Button1;
            lookup["joystick1button2"] = KeyCode.Joystick1Button2;
            lookup["joystick1button3"] = KeyCode.Joystick1Button3;
            lookup["joystick1button4"] = KeyCode.Joystick1Button4;
            lookup["joystick1button5"] = KeyCode.Joystick1Button5;
            lookup["joystick1button6"] = KeyCode.Joystick1Button6;
            lookup["joystick1button7"] = KeyCode.Joystick1Button7;
            lookup["joystick1button8"] = KeyCode.Joystick1Button8;
            lookup["joystick1button9"] = KeyCode.Joystick1Button9;
            lookup["joystick1button10"] = KeyCode.Joystick1Button10;
            lookup["joystick1button11"] = KeyCode.Joystick1Button11;
            lookup["joystick1button12"] = KeyCode.Joystick1Button12;
            lookup["joystick1button13"] = KeyCode.Joystick1Button13;
            lookup["joystick1button14"] = KeyCode.Joystick1Button14;
            lookup["joystick1button15"] = KeyCode.Joystick1Button15;
            lookup["joystick1button16"] = KeyCode.Joystick1Button16;
            lookup["joystick1button17"] = KeyCode.Joystick1Button17;
            lookup["joystick1button18"] = KeyCode.Joystick1Button18;
            lookup["joystick1button19"] = KeyCode.Joystick1Button19;
            lookup["joystick2button0"] = KeyCode.Joystick2Button0;
            lookup["joystick2button1"] = KeyCode.Joystick2Button1;
            lookup["joystick2button2"] = KeyCode.Joystick2Button2;
            lookup["joystick2button3"] = KeyCode.Joystick2Button3;
            lookup["joystick2button4"] = KeyCode.Joystick2Button4;
            lookup["joystick2button5"] = KeyCode.Joystick2Button5;
            lookup["joystick2button6"] = KeyCode.Joystick2Button6;
            lookup["joystick2button7"] = KeyCode.Joystick2Button7;
            lookup["joystick2button8"] = KeyCode.Joystick2Button8;
            lookup["joystick2button9"] = KeyCode.Joystick2Button9;
            lookup["joystick2button10"] = KeyCode.Joystick2Button10;
            lookup["joystick2button11"] = KeyCode.Joystick2Button11;
            lookup["joystick2button12"] = KeyCode.Joystick2Button12;
            lookup["joystick2button13"] = KeyCode.Joystick2Button13;
            lookup["joystick2button14"] = KeyCode.Joystick2Button14;
            lookup["joystick2button15"] = KeyCode.Joystick2Button15;
            lookup["joystick2button16"] = KeyCode.Joystick2Button16;
            lookup["joystick2button17"] = KeyCode.Joystick2Button17;
            lookup["joystick2button18"] = KeyCode.Joystick2Button18;
            lookup["joystick2button19"] = KeyCode.Joystick2Button19;
            lookup["joystick3button0"] = KeyCode.Joystick3Button0;
            lookup["joystick3button1"] = KeyCode.Joystick3Button1;
            lookup["joystick3button2"] = KeyCode.Joystick3Button2;
            lookup["joystick3button3"] = KeyCode.Joystick3Button3;
            lookup["joystick3button4"] = KeyCode.Joystick3Button4;
            lookup["joystick3button5"] = KeyCode.Joystick3Button5;
            lookup["joystick3button6"] = KeyCode.Joystick3Button6;
            lookup["joystick3button7"] = KeyCode.Joystick3Button7;
            lookup["joystick3button8"] = KeyCode.Joystick3Button8;
            lookup["joystick3button9"] = KeyCode.Joystick3Button9;
            lookup["joystick3button10"] = KeyCode.Joystick3Button10;
            lookup["joystick3button11"] = KeyCode.Joystick3Button11;
            lookup["joystick3button12"] = KeyCode.Joystick3Button12;
            lookup["joystick3button13"] = KeyCode.Joystick3Button13;
            lookup["joystick3button14"] = KeyCode.Joystick3Button14;
            lookup["joystick3button15"] = KeyCode.Joystick3Button15;
            lookup["joystick3button16"] = KeyCode.Joystick3Button16;
            lookup["joystick3button17"] = KeyCode.Joystick3Button17;
            lookup["joystick3button18"] = KeyCode.Joystick3Button18;
            lookup["joystick3button19"] = KeyCode.Joystick3Button19;
            lookup["joystick4button0"] = KeyCode.Joystick4Button0;
            lookup["joystick4button1"] = KeyCode.Joystick4Button1;
            lookup["joystick4button2"] = KeyCode.Joystick4Button2;
            lookup["joystick4button3"] = KeyCode.Joystick4Button3;
            lookup["joystick4button4"] = KeyCode.Joystick4Button4;
            lookup["joystick4button5"] = KeyCode.Joystick4Button5;
            lookup["joystick4button6"] = KeyCode.Joystick4Button6;
            lookup["joystick4button7"] = KeyCode.Joystick4Button7;
            lookup["joystick4button8"] = KeyCode.Joystick4Button8;
            lookup["joystick4button9"] = KeyCode.Joystick4Button9;
            lookup["joystick4button10"] = KeyCode.Joystick4Button10;
            lookup["joystick4button11"] = KeyCode.Joystick4Button11;
            lookup["joystick4button12"] = KeyCode.Joystick4Button12;
            lookup["joystick4button13"] = KeyCode.Joystick4Button13;
            lookup["joystick4button14"] = KeyCode.Joystick4Button14;
            lookup["joystick4button15"] = KeyCode.Joystick4Button15;
            lookup["joystick4button16"] = KeyCode.Joystick4Button16;
            lookup["joystick4button17"] = KeyCode.Joystick4Button17;
            lookup["joystick4button18"] = KeyCode.Joystick4Button18;
            lookup["joystick4button19"] = KeyCode.Joystick4Button19;

            return lookup;
        }

        public static KeyCode Parse(string name)
        {
            if (m_keycodeLookup == null)
            {
                m_keycodeLookup = CreateLookup();
            }

            KeyCode code;
            if (m_keycodeLookup.TryGetValue(name, out code))
            {
                return code;
            }

            return KeyCode.None;
        }

        #endregion

        #region Properties

        internal static IList<CBinding> BindingsList
        {
            get { return m_bindings; }
        }

        #endregion
    }
}

