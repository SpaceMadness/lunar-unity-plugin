using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using LunarPlugin;
using LunarEditor;
using LunarPluginInternal;

using LunarPlugin.Test;

namespace PreferencesTests
{
    using Assert = NUnit.Framework.Assert;

    [TestFixture]
    public class PreferencesTest : TestFixtureBase
    {
        private static readonly string kPreferencesPath = "prefs.plist";

        [SetUp]
        public void SetUp()
        {
            File.Delete(kPreferencesPath);
            CTimerManager.CancelTimers();
        }

        [Test]
        public void TestPrefs()
        {
            bool boolValue = true;
            bool boolValue2 = true;

            int intValue = 10;
            int intValue2 = Int32.MaxValue;
            int intValue3 = Int32.MinValue;

            float floatValue = 3.14f;
            float floatValue2 = -3.14f;
            float floatValue3 = 0.0f;
            float floatValue4 = float.NegativeInfinity;
            float floatValue5 = float.PositiveInfinity;
            float floatValue6 = float.NaN;

            double doubleValue = 6.28;
            double doubleValue2 = -6.28;
            double doubleValue3 = 0.0;
            double doubleValue4 = double.NegativeInfinity;
            double doubleValue5 = double.PositiveInfinity;
            double doubleValue6 = double.NaN;

            DateTime timeValue = new DateTime(2015, 2, 25, 16, 04, 31);

            string stringValue = "String value";
            string stringValue2 = "";
            string stringValue3 = null;

            byte[] byteArrayValue = { 1, 2, 3 };
            byte[] byteArrayValue2 = {};
            byte[] byteArrayValue3 = null;

            Dictionary<string, object> dictionaryValue = new Dictionary<string, object>();
            dictionaryValue["key1"] = "value1";
            dictionaryValue["key2"] = "value2";
            dictionaryValue["key3"] = "value3";
            Dictionary<string, object> dictionaryValue2 = new Dictionary<string, object>();
            Dictionary<string, object> dictionaryValue3 = null;

            List<object> listValue = new List<object>();
            listValue.Add("value1");
            listValue.Add("value2");
            listValue.Add("value3");
            List<object> listValue2 = new List<object>();
            List<object> listValue3 = null;

            CPreferences prefs = new CPreferences(kPreferencesPath);
            prefs.Set("boolValue", boolValue);
            prefs.Set("boolValue2", boolValue2);

            prefs.Set("intValue", intValue);
            prefs.Set("intValue2", intValue2);
            prefs.Set("intValue3", intValue3);

            prefs.Set("floatValue", floatValue);
            prefs.Set("floatValue2", floatValue2);
            prefs.Set("floatValue3", floatValue3);
            prefs.Set("floatValue4", floatValue4);
            prefs.Set("floatValue5", floatValue5);
            prefs.Set("floatValue6", floatValue6);

            prefs.Set("doubleValue", doubleValue);
            prefs.Set("doubleValue2", doubleValue2);
            prefs.Set("doubleValue3", doubleValue3);
            prefs.Set("doubleValue4", doubleValue4);
            prefs.Set("doubleValue5", doubleValue5);
            prefs.Set("doubleValue6", doubleValue6);

            prefs.Set("timeValue", timeValue);

            prefs.Set("stringValue", stringValue);
            prefs.Set("stringValue2", stringValue2);
            prefs.Set("stringValue3", stringValue3);

            prefs.Set("byteArrayValue", byteArrayValue);
            prefs.Set("byteArrayValue2", byteArrayValue2);
            prefs.Set("byteArrayValue3", byteArrayValue3);

            prefs.Set("dictionaryValue", dictionaryValue);
            prefs.Set("dictionaryValue2", dictionaryValue2);
            prefs.Set("dictionaryValue3", dictionaryValue3);

            prefs.Set("listValue", listValue);
            prefs.Set("listValue2", listValue2);
            prefs.Set("listValue3", listValue3);

            prefs.Save();

            prefs = new CPreferences(kPreferencesPath);

            Assert.AreEqual(boolValue, prefs.GetBool("boolValue"));
            Assert.AreEqual(boolValue2, prefs.GetBool("boolValue2"));

            Assert.AreEqual(intValue, prefs.GetInt("intValue"));
            Assert.AreEqual(intValue2, prefs.GetInt("intValue2"));
            Assert.AreEqual(intValue3, prefs.GetInt("intValue3"));

            Assert.AreEqual(floatValue, prefs.GetFloat("floatValue"));
            Assert.AreEqual(floatValue2, prefs.GetFloat("floatValue2"));
            Assert.AreEqual(floatValue3, prefs.GetFloat("floatValue3"));
            Assert.AreEqual(floatValue4, prefs.GetFloat("floatValue4"));
            Assert.AreEqual(floatValue5, prefs.GetFloat("floatValue5"));
            Assert.AreEqual(floatValue6, prefs.GetFloat("floatValue6"));

            Assert.AreEqual(doubleValue, prefs.GetDouble("doubleValue"));
            Assert.AreEqual(doubleValue2, prefs.GetDouble("doubleValue2"));
            Assert.AreEqual(doubleValue3, prefs.GetDouble("doubleValue3"));
            Assert.AreEqual(doubleValue4, prefs.GetDouble("doubleValue4"));
            Assert.AreEqual(doubleValue5, prefs.GetDouble("doubleValue5"));
            Assert.AreEqual(doubleValue6, prefs.GetDouble("doubleValue6"));

            Assert.AreEqual(timeValue, prefs.GetDate("timeValue"));

            Assert.AreEqual(stringValue, prefs.GetString("stringValue"));
            Assert.AreEqual(stringValue2, prefs.GetString("stringValue2"));
            Assert.AreEqual(stringValue3, prefs.GetString("stringValue3"));

            Assert.AreEqual(byteArrayValue, prefs.GetByteArray("byteArrayValue"));
            Assert.AreEqual(byteArrayValue2, prefs.GetByteArray("byteArrayValue2"));
            Assert.AreEqual(byteArrayValue3, prefs.GetByteArray("byteArrayValue3"));

            Assert.AreEqual(dictionaryValue, prefs.GetDictionary("dictionaryValue"));
            Assert.AreEqual(dictionaryValue2, prefs.GetDictionary("dictionaryValue2"));
            Assert.AreEqual(dictionaryValue3, prefs.GetDictionary("dictionaryValue3"));

            Assert.AreEqual(listValue, prefs.GetArray("listValue"));
            Assert.AreEqual(listValue2, prefs.GetArray("listValue2"));
            Assert.AreEqual(listValue3, prefs.GetArray("listValue3"));
        }

        [Test]
        public void TestSaveDelayed()
        {
            CPreferences prefs = new CPreferences(kPreferencesPath);
            prefs.Set("key", "value");

            CPreferences otherPrefs = new CPreferences(kPreferencesPath);
            Assert.IsNull(otherPrefs.GetString("key"));

            CTimerManager.RunUpdate(0.5f);

            otherPrefs = new CPreferences(kPreferencesPath);
            Assert.AreEqual("value", otherPrefs.GetString("key"));
        }

        [Test]
        public void TestReadingUnityPrefs()
        {
            /*
             * PlayerPrefs.SetInt("int1", Int32.MinValue);
             * PlayerPrefs.SetInt("int2", 0);
             * PlayerPrefs.SetInt("int3", Int32.MaxValue);
             * 
             * PlayerPrefs.SetFloat("float1", -3.14f);
             * PlayerPrefs.SetFloat("float2", 0);
             * PlayerPrefs.SetFloat("float3", 3.14f);
             * 
             * PlayerPrefs.SetString("str1", "Some string");
             * PlayerPrefs.SetString("str2", "");
             * PlayerPrefs.SetString("str3", null);
             */

            string prefsFile = "unity_test_player_prefs.plist";
            Assert.IsTrue(File.Exists(prefsFile), "File doesn't exist: " + prefsFile);

            CPreferences prefs = new CPreferences(prefsFile);

            Assert.AreEqual(Int32.MinValue, prefs.GetInt("int1"));
            Assert.AreEqual((float)Int32.MinValue, prefs.GetFloat("int1"));
            Assert.AreEqual((double)Int32.MinValue, prefs.GetDouble("int1"));

            Assert.AreEqual(0, prefs.GetInt("int2", -1));
            Assert.AreEqual(0, prefs.GetFloat("int2", -1));
            Assert.AreEqual(0, prefs.GetDouble("int2", -1));

            Assert.AreEqual(Int32.MaxValue, prefs.GetInt("int3"));
            Assert.AreEqual((float)Int32.MaxValue, prefs.GetFloat("int3"));
            Assert.AreEqual((double)Int32.MaxValue, prefs.GetDouble("int3"));

            Assert.AreEqual(-3.14f, prefs.GetFloat("float1"));
            Assert.AreEqual(-3, prefs.GetInt("float1"));
            Assert.AreEqual((double)-3.14f, prefs.GetDouble("float1"));

            Assert.AreEqual(0.0f, prefs.GetFloat("float2", -1.0f));
            Assert.AreEqual(0, prefs.GetInt("float2", -1));
            Assert.AreEqual((double)0.0f, prefs.GetDouble("float2", -1.0));

            Assert.AreEqual(3.14f, prefs.GetFloat("float3"));
            Assert.AreEqual(3, prefs.GetInt("float3"));
            Assert.AreEqual((double)3.14f, prefs.GetDouble("float3"));

            Assert.AreEqual("Some string", prefs.GetString("str1"));
            Assert.AreEqual("", prefs.GetString("str2"));
            Assert.AreEqual("", prefs.GetString("str3")); // null -> "" by plist format
        }

        [Test]
        public void TestListPreferences()
        {
            CPreferences prefs = new CPreferences();
            prefs.Set("p1", "p1");
            prefs.Set("p12", "p12");
            prefs.Set("P12", "P12");
            prefs.Set("P123", "P123");
            prefs.Set("o123", "o123");

            List<string> actual = new List<string>(prefs.ListPreferences().Keys);
            AssertList(actual, "p1", "p12", "P12", "P123", "o123");
        }

        [Test]
        public void TestListPreferencesToken()
        {
            CPreferences prefs = new CPreferences();
            prefs.Set("p1", "p1");
            prefs.Set("p12", "p12");
            prefs.Set("P12", "P12");
            prefs.Set("P123", "P123");
            prefs.Set("o123", "o123");

            List<string> actual = new List<string>(prefs.ListPreferences("").Keys);
            AssertList(actual, "p1", "p12", "P12", "P123", "o123");
        }

        [Test]
        public void TestListPreferencesEmpty()
        {
            CPreferences prefs = new CPreferences();
            prefs.Set("p1", "p1");
            prefs.Set("p12", "p12");
            prefs.Set("P12", "P12");
            prefs.Set("P123", "P123");
            prefs.Set("o123", "o123");

            List<string> actual = new List<string>(prefs.ListPreferences("p").Keys);
            AssertList(actual, "p1", "p12", "P12", "P123");
        }

        [Test]
        public void TestListPreferencesTokenOne()
        {
            CPreferences prefs = new CPreferences();
            prefs.Set("p1", "p1");
            prefs.Set("p12", "p12");
            prefs.Set("P12", "P12");
            prefs.Set("P123", "P123");
            prefs.Set("o123", "o123");

            List<string> actual = new List<string>(prefs.ListPreferences("p123").Keys);
            AssertList(actual, "P123");
        }

        [Test]
        public void TestListPreferencesTokenNone()
        {
            CPreferences prefs = new CPreferences();
            prefs.Set("p1", "p1");
            prefs.Set("p12", "p12");
            prefs.Set("P12", "P12");
            prefs.Set("P123", "P123");
            prefs.Set("o123", "o123");

            List<string> actual = new List<string>(prefs.ListPreferences("p1234").Keys);
            AssertList(actual);
        }

        [Test]
        public void TestFindPreferences()
        {
            CPreferences prefs = new CPreferences();
            prefs.Set("p1", "p1");
            prefs.Set("p12", "p12");
            prefs.Set("P12", "P12");
            prefs.Set("P123", "P123");
            prefs.Set("o123", "o123");

            object value;
            bool succeed = prefs.TryGetValue("p1", out value);

            Assert.IsTrue(succeed);
            Assert.AreEqual("p1", value);
        }

        [Test]
        public void TestFindMissingPreferences()
        {
            CPreferences prefs = new CPreferences();
            prefs.Set("p1", "p1");
            prefs.Set("p12", "p12");
            prefs.Set("P12", "P12");
            prefs.Set("P123", "P123");
            prefs.Set("o123", "o123");

            object value;
            bool succeed = prefs.TryGetValue("p1!", out value);

            Assert.IsFalse(succeed);
        }
    }
}

