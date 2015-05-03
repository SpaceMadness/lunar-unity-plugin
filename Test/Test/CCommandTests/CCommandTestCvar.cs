using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

using LunarPlugin;
using LunarPluginInternal;

using UnityEngine;

namespace CCommandTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class CCommandTestCvar : CCommandTest
    {
        [Test]
        public void TestCvarBool()
        {
            CVar cvar = new CVar("var", false);

            Assert.IsTrue(cvar.IsBool);
            Assert.IsTrue(cvar.IsInt);
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual("0", cvar.DefaultValue);

            Assert.AreEqual(false, cvar.BoolValue);
            Assert.AreEqual(0, cvar.IntValue);
            Assert.AreEqual(0.0f, cvar.FloatValue);
            Assert.AreEqual("0", cvar.Value);


            Execute("var 1");
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(true, cvar.BoolValue);
            Assert.AreEqual(1, cvar.IntValue);
            Assert.AreEqual(1.0f, cvar.FloatValue);
            Assert.AreEqual("1", cvar.Value);

            Execute("var 0");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(false, cvar.BoolValue);
            Assert.AreEqual(0, cvar.IntValue);
            Assert.AreEqual(0.0f, cvar.FloatValue);
            Assert.AreEqual("0", cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(false, cvar.BoolValue);
            Assert.AreEqual(0, cvar.IntValue);
            Assert.AreEqual(0.0f, cvar.FloatValue);
            Assert.AreEqual("0", cvar.Value);
        }

        [Test]
        public void TestCvarBool2()
        {
            CVar cvar = new CVar("var", true);

            Assert.IsTrue(cvar.IsBool);
            Assert.IsTrue(cvar.IsInt);
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual("1", cvar.DefaultValue);

            Assert.AreEqual(true, cvar.BoolValue);
            Assert.AreEqual(1, cvar.IntValue);
            Assert.AreEqual(1.0f, cvar.FloatValue);
            Assert.AreEqual("1", cvar.Value);

            Execute("var 0");
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(false, cvar.BoolValue);
            Assert.AreEqual(0, cvar.IntValue);
            Assert.AreEqual(0.0f, cvar.FloatValue);
            Assert.AreEqual("0", cvar.Value);

            Execute("var 1");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(true, cvar.BoolValue);
            Assert.AreEqual(1, cvar.IntValue);
            Assert.AreEqual(1.0f, cvar.FloatValue);
            Assert.AreEqual("1", cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(true, cvar.BoolValue);
            Assert.AreEqual(1, cvar.IntValue);
            Assert.AreEqual(1.0f, cvar.FloatValue);
            Assert.AreEqual("1", cvar.Value);
        }

        [Test]
        public void TestCvarInt()
        {
            CVar cvar = new CVar("var", 128);
            Assert.IsTrue(cvar.IsInt);
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual("128", cvar.DefaultValue);

            Assert.AreEqual(128, cvar.IntValue);
            Assert.AreEqual(128.0f, cvar.FloatValue);
            Assert.AreEqual("128", cvar.Value);

            Execute("var -128");
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(-128, cvar.IntValue);
            Assert.AreEqual(-128.0f, cvar.FloatValue);
            Assert.AreEqual("-128", cvar.Value);

            Execute("var 128");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(128, cvar.IntValue);
            Assert.AreEqual(128.0f, cvar.FloatValue);
            Assert.AreEqual("128", cvar.Value);

            Execute("var 0");
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(0, cvar.IntValue);
            Assert.AreEqual(0.0f, cvar.FloatValue);
            Assert.AreEqual("0", cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(128, cvar.IntValue);
            Assert.AreEqual(128.0f, cvar.FloatValue);
            Assert.AreEqual("128", cvar.Value);
        }

        [Test]
        public void TestCvarFloat()
        {
            CVar cvar = new CVar("var", 3.14f);
            Assert.IsTrue(cvar.IsFloat);
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual("3.14", cvar.DefaultValue);

            Assert.AreEqual(3, cvar.IntValue);
            Assert.AreEqual(3.14f, cvar.FloatValue);
            Assert.AreEqual("3.14", cvar.Value);

            Execute("var -3.14");
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(-3, cvar.IntValue);
            Assert.AreEqual(-3.14f, cvar.FloatValue);
            Assert.AreEqual("-3.14", cvar.Value);

            Execute("var 3.14");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(3, cvar.IntValue);
            Assert.AreEqual(3.14f, cvar.FloatValue);
            Assert.AreEqual("3.14", cvar.Value);

            Execute("var 0");
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(0, cvar.IntValue);
            Assert.AreEqual(0.0f, cvar.FloatValue);
            Assert.AreEqual("0", cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(3, cvar.IntValue);
            Assert.AreEqual(3.14f, cvar.FloatValue);
            Assert.AreEqual("3.14", cvar.Value);
        }

        [Test]
        public void TestCvarString()
        {
            CVar cvar = new CVar("var", "Default string");
            Assert.IsTrue(cvar.IsString);
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual("Default string", cvar.DefaultValue);
            Assert.AreEqual("Default string", cvar.Value);

            Execute("var 'Some string'");
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual("Some string", cvar.Value);

            Execute("var 'Default string'");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual("Default string", cvar.Value);

            Execute("var 'Some other string'");
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual("Some other string", cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual("Default string", cvar.Value);
        }

        [Test]
        public void TestCvarString2()
        {
            CVar cvar = new CVar("var", null);
            Assert.IsTrue(cvar.IsString);
            Assert.IsTrue(cvar.IsDefault);
            Assert.IsNull(cvar.DefaultValue);
            Assert.IsNull(cvar.Value);

            Execute("var 'Some string'");
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual("Some string", cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.IsNull(cvar.Value);
        }

        [Test]
        public void TestCvarColor()
        {
            Color color = ColorUtils.FromRGBA(0x12345678);
            string colorDefault = string.Format("{0} {1} {2} {3}", color.r, color.g, color.b, color.a);

            CVar cvar = new CVar("var", color);
            Assert.IsTrue(cvar.IsColor);
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(colorDefault, cvar.DefaultValue);

            Assert.AreEqual(color, cvar.ColorValue);
            Assert.AreEqual(0x12345678, cvar.IntValue);
            Assert.AreEqual(colorDefault, cvar.Value);

            Color otherColor = new Color(0.1f, 0.2f, 0.3f, 0.4f);
            uint otherColorValue = ColorUtils.ToRGBA(ref otherColor);

            Execute("var 0.1 0.2 0.3 0.4");
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(otherColor, cvar.ColorValue);
            Assert.AreEqual(otherColorValue, (uint)cvar.IntValue);
            Assert.AreEqual("0.1 0.2 0.3 0.4", cvar.Value);

            otherColorValue = 0x87654321;
            otherColor = ColorUtils.FromRGBA(0x87654321);
            Execute("var 0x{0}", otherColorValue.ToString("X"));
            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(otherColor, cvar.ColorValue);
            Assert.AreEqual(otherColorValue, (uint)cvar.IntValue);
            Assert.AreEqual("0.5294118 0.3960785 0.2627451 0.1294118", cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(colorDefault, cvar.Value);
            Assert.AreEqual(color, cvar.ColorValue);
            Assert.AreEqual(0x12345678, cvar.IntValue);
        }

        [Test]
        public void TestCvarRect()
        {
            float x = 1.2f;
            float y = 3.4f;
            float w = 5.6f;
            float h = 7.8f;

            Rect rect = new Rect(x, y, w, h);
            string rectValue = string.Format("{0} {1} {2} {3}",
                rect.x,
                rect.y,
                rect.width,
                rect.height);

            CVar cvar = new CVar("var", rect);
            Assert.IsTrue(cvar.IsRect);
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(rectValue, cvar.DefaultValue);

            Assert.AreEqual(rect, cvar.RectValue);
            Assert.AreEqual(new Color(x, y, w, h), cvar.ColorValue);
            Assert.AreEqual(new Vector2(x, y), cvar.Vector2Value);
            Assert.AreEqual(new Vector3(x, y, w), cvar.Vector3Value);
            Assert.AreEqual(new Vector4(x, y, w, h), cvar.Vector4Value);
            Assert.AreEqual(rectValue, cvar.Value);

            Rect anotherRect = new Rect(-1.2f, -3.4f, -5.6f, -7.8f);
            string anotherRectValue = string.Format("{0} {1} {2} {3}",
                anotherRect.x,
                anotherRect.y,
                anotherRect.width,
                anotherRect.height);

            Execute("var {0}", anotherRectValue);

            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(anotherRect, cvar.RectValue);
            Assert.AreEqual(new Vector2(anotherRect.x, anotherRect.y), cvar.Vector2Value);
            Assert.AreEqual(new Vector3(anotherRect.x, anotherRect.y, anotherRect.width), cvar.Vector3Value);
            Assert.AreEqual(new Vector4(anotherRect.x, anotherRect.y, anotherRect.width, anotherRect.height), cvar.Vector4Value);
            Assert.AreEqual(anotherRectValue, cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(new Color(x, y, w, h), cvar.ColorValue);
            Assert.AreEqual(new Vector2(x, y), cvar.Vector2Value);
            Assert.AreEqual(new Vector3(x, y, w), cvar.Vector3Value);
            Assert.AreEqual(new Vector4(x, y, w, h), cvar.Vector4Value);
            Assert.AreEqual(rectValue, cvar.Value);
        }

        [Test]
        public void TestCvarVector2()
        {
            float x = 1.2f;
            float y = 3.4f;
            Vector2 value = new Vector2(x, y);
            string valueString = string.Format("{0} {1}", x, y);

            CVar cvar = new CVar("var", value);
            Assert.IsTrue(cvar.IsVector2);
            Assert.AreEqual(valueString, cvar.DefaultValue);

            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(value, cvar.Vector2Value);
            Assert.AreEqual(new Vector3(x, y), cvar.Vector3Value);
            Assert.AreEqual(new Vector4(x, y), cvar.Vector4Value);
            Assert.AreEqual(valueString, cvar.Value);

            Vector2 anotherValue = new Vector2(-1.2f, -3.4f);
            string anotherValueString = string.Format("{0} {1}", anotherValue.x, anotherValue.y);

            Execute("var {0} {1}", 
                anotherValue.x,
                anotherValue.y
            );

            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(anotherValue, cvar.Vector2Value);
            Assert.AreEqual(new Vector3(anotherValue.x, anotherValue.y), cvar.Vector3Value);
            Assert.AreEqual(new Vector4(anotherValue.x, anotherValue.y), cvar.Vector4Value);
            Assert.AreEqual(anotherValueString, cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(value, cvar.Vector2Value);
            Assert.AreEqual(new Vector3(x, y), cvar.Vector3Value);
            Assert.AreEqual(new Vector4(x, y), cvar.Vector4Value);
            Assert.AreEqual(valueString, cvar.Value);
        }

        [Test]
        public void TestCvarVector3()
        {
            float x = 1.2f;
            float y = 3.4f;
            float z = 5.6f;
            Vector3 value = new Vector3(x, y, z);
            string valueString = string.Format("{0} {1} {2}", x, y, z);

            CVar cvar = new CVar("var", value);
            Assert.IsTrue(cvar.IsVector3);
            Assert.AreEqual(valueString, cvar.DefaultValue);

            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(value, cvar.Vector3Value);
            Assert.AreEqual(new Vector2(x, y), cvar.Vector2Value);
            Assert.AreEqual(new Vector4(x, y, z), cvar.Vector4Value);
            Assert.AreEqual(valueString, cvar.Value);

            Vector3 anotherValue = new Vector3(-1.2f, -3.4f, -5.6f);
            string anotherValueString = string.Format("{0} {1} {2}", anotherValue.x, anotherValue.y, anotherValue.z);

            Execute("var {0} {1} {2}", 
                anotherValue.x,
                anotherValue.y,
                anotherValue.z
            );

            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(anotherValue, cvar.Vector3Value);
            Assert.AreEqual(new Vector2(anotherValue.x, anotherValue.y), cvar.Vector2Value);
            Assert.AreEqual(new Vector4(anotherValue.x, anotherValue.y, anotherValue.z), cvar.Vector4Value);
            Assert.AreEqual(anotherValueString, cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(value, cvar.Vector3Value);
            Assert.AreEqual(new Vector2(x, y), cvar.Vector2Value);
            Assert.AreEqual(new Vector4(x, y, z), cvar.Vector4Value);
            Assert.AreEqual(valueString, cvar.Value);
        }

        [Test]
        public void TestCvarVector4()
        {
            float x = 1.2f;
            float y = 3.4f;
            float z = 5.6f;
            float w = 7.8f;
            Vector4 value = new Vector4(x, y, z, w);
            string valueString = string.Format("{0} {1} {2} {3}", x, y, z, w);

            CVar cvar = new CVar("var", value);
            Assert.IsTrue(cvar.IsVector4);
            Assert.AreEqual(valueString, cvar.DefaultValue);

            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(value, cvar.Vector4Value);
            Assert.AreEqual(new Vector2(x, y), cvar.Vector2Value);
            Assert.AreEqual(new Vector3(x, y, z), cvar.Vector3Value);
            Assert.AreEqual(valueString, cvar.Value);

            Vector4 anotherValue = new Vector4(-1.2f, -3.4f, -5.6f, -7.8f);
            string anotherValueString = string.Format("{0} {1} {2} {3}", anotherValue.x, anotherValue.y, anotherValue.z, anotherValue.w);

            Execute("var {0} {1} {2} {3}", 
                anotherValue.x,
                anotherValue.y,
                anotherValue.z,
                anotherValue.w
            );

            Assert.IsFalse(cvar.IsDefault);
            Assert.AreEqual(anotherValue, cvar.Vector4Value);
            Assert.AreEqual(new Vector2(anotherValue.x, anotherValue.y), cvar.Vector2Value);
            Assert.AreEqual(new Vector3(anotherValue.x, anotherValue.y, anotherValue.z), cvar.Vector3Value);
            Assert.AreEqual(anotherValueString, cvar.Value);

            Execute("reset var");
            Assert.IsTrue(cvar.IsDefault);
            Assert.AreEqual(value, cvar.Vector4Value);
            Assert.AreEqual(new Vector2(x, y), cvar.Vector2Value);
            Assert.AreEqual(new Vector3(x, y, z), cvar.Vector3Value);
            Assert.AreEqual(valueString, cvar.Value);
        }

        [SetUp]
        public void SetUp()
        {
            RunSetUp();

            RegisterCommands(new reset());
        }

        [TearDown]
        public void TearDown()
        {
            RunTearDown();
        }
    }
}

