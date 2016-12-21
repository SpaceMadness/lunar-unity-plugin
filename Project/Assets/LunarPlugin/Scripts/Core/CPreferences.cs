//
//  CPreferences.cs
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

﻿using System;
using System.Collections.Generic;
using System.IO;

using LunarPlugin;
using LunarPluginInternal;
using LunarPluginExternal.PlistCS;

namespace LunarPluginInternal
{
    class CPreferences
    {
        private string m_path;
        private IDictionary<string, object> m_data;

        //////////////////////////////////////////////////////////////////////////////

        internal CPreferences()
        {
            m_data = new Dictionary<string, object>();
        }

        internal CPreferences(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            m_path = path;
            Load();
        }

        //////////////////////////////////////////////////////////////////////////////

        public bool Load()
        {
            lock (this)
            {
                try
                {
                    if (m_path == null)
                    {
                        CLog.e("Can't load settings: path is null");
                        return false;
                    }

                    if (CFileUtils.FileExists(m_path))
                    {
                        m_data = CPlist.readPlist(m_path) as Dictionary<string, object>;
                        return true;
                    }
                }
                catch (Exception e)
                {
                    CLog.error(e, "Can't read settings: {0}", m_path);
                }

                m_data = new Dictionary<string, object>();
                return false;
            }
        }

        public bool Save()
        {
            lock (this)
            {
                try
                {
                    if (m_path == null)
                    {
                        CLog.e("Can't save settings: path is null");
                        return false;
                    }

                    using (Stream stream = CFileUtils.OpenWrite(m_path))
                    {
                        CPlist.writeBinary(m_data, stream);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    CLog.error(e, "Can't save settings: {0}", m_path);
                    return false;
                }
            }
        }

        private void SaveDelayed()
        {
            Save();
        }

        private void Save(bool immediately)
        {
            if (m_path == null)
            {
                return;
            }

            if (immediately)
            {
                CTimerManager.CancelTimer(SaveDelayed);
                Save();
            }
            else
            {
                CTimerManager.ScheduleTimerOnce(SaveDelayed);
            }
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Operations

        public void Set(string key, bool value, bool saveImmediately = false)
        {
            SetObject(key, value ? 1 : 0, saveImmediately);
        }

        public void Set(string key, int value, bool saveImmediately = false)
        {
            SetObject(key, value, saveImmediately);
        }

        public void Set(string key, float value, bool saveImmediately = false)
        {
            Set(key, (double)value, saveImmediately);
        }

        public void Set(string key, double value, bool saveImmediately = false)
        {
            SetObject(key, value, saveImmediately);
        }

        public void Set(string key, DateTime value, bool saveImmediately = false)
        {
            SetObject(key, value, saveImmediately);
        }

        public void Set(string key, string value, bool saveImmediately = false)
        {
            SetObject(key, value, saveImmediately);
        }

        public void Set(string key, byte[] value, bool saveImmediately = false)
        {
            SetObject(key, value, saveImmediately);
        }

        public void Set(string key, Dictionary<string, object> value, bool saveImmediately = false)
        {
            SetObject(key, value, saveImmediately);
        }

        public void Set(string key, List<object> value, bool saveImmediately = false)
        {
            SetObject(key, value, saveImmediately);
        }

        private void SetObject(string key, object value, bool saveImmediately)
        {
            lock (this)
            {
                if (value != null)
                {
                    m_data[key] = value;
                }
                else
                {
                    m_data.Remove(key);
                }

                Save(saveImmediately);
            }
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            return GetInt(key, 0) != 0;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            object value;
            if (TryGetValue(key, out value))
            {
                if (value is int)
                {
                    return (int)value;
                }

                if (value is float)
                {
                    return (int)((float)value);
                }

                if (value is double)
                {
                    return (int)((double)value);
                }
            }

            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue = 0.0f)
        {
            object value;
            if (TryGetValue(key, out value))
            {
                if (value is float)
                {
                    return (float)value;
                }

                if (value is double)
                {
                    return (float)((double)value);
                }

                if (value is int)
                {
                    return ((float)(int)value);
                }
            }

            return defaultValue;
        }

        public double GetDouble(string key, double defaultValue = 0.0)
        {
            object value;
            if (TryGetValue(key, out value))
            {
                if (value is double)
                {
                    return (double)value;
                }

                if (value is float)
                {
                    return (double)((float)value);
                }

                if (value is int)
                {
                    return ((double)(int)value);
                }
            }

            return defaultValue;
        }

        public string GetString(string key, string defaultValue = null)
        {
            object value;
            if (TryGetValue(key, out value) && value is string)
            {
                return (string)value;
            }

            return defaultValue;
        }

        public DateTime GetDate(string key, DateTime defaultValue = default(DateTime))
        {
            object value;
            if (TryGetValue(key, out value) && value is DateTime)
            {
                return (DateTime)value;
            }

            return defaultValue;
        }

        public byte[] GetByteArray(string key, byte[] defaultValue = null)
        {
            object value;
            if (TryGetValue(key, out value) && value is byte[])
            {
                return (byte[])value;
            }

            return defaultValue;
        }

        public Dictionary<string, object> GetDictionary(string key, Dictionary<string, object> defaultValue = null)
        {
            object value;
            if (TryGetValue(key, out value) && value is Dictionary<string, object>)
            {
                return (Dictionary<string, object>)value;
            }

            return defaultValue;
        }

        public List<object> GetArray(string key, List<object> defaultValue = null)
        {
            object value;
            if (TryGetValue(key, out value) && value is List<object>)
            {
                return (List<object>)value;
            }

            return defaultValue;
        }

        public bool TryGetValue(string key, out object value)
        {
            lock (this)
            {
                return m_data.TryGetValue(key, out value);
            }
        }

        public void DeleteAll(bool saveImmediately = false)
        {
            lock (this)
            {
                m_data.Clear();
                Save(saveImmediately);
            }
        }

        public IDictionary<string, object> ListPreferences(string token = null)
        {
            if (string.IsNullOrEmpty(token))
            {
                return m_data;
            }
            
            IDictionary<string, object> data = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> e in m_data)
            {
                if (CStringUtils.StartsWithIgnoreCase(e.Key, token))
                {
                    data[e.Key] = e.Value;
                }
            }
            
            return data;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public string Path
        {
            get { return m_path; }
        }

        #endregion
    }
}