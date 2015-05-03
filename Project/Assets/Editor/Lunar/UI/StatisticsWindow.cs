using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using LunarPlugin;
using LunarPluginInternal;

namespace LunarEditor
{
#if LUNAR_DEVELOPMENT

    class StatisticsWindow : Window
    {
        private ValueLabel m_timersCountLabel;

        private ValueCompositeView m_notificationsView;

        public StatisticsWindow()
            : base("Statistics")
        {
        }

        protected override void CreateUI()
        {
            TimerManager.ScheduleTimer(UpdateStatistics, 0.0f, true);

            m_timersCountLabel = new ValueLabel("Timers count");
            AddSubview(m_timersCountLabel);

            m_notificationsView = new ValueCompositeView("Notifications:");
            AddSubview(m_notificationsView);

            this.RootView.ArrangeVert();
        }

        private void UpdateStatistics()
        {
            UpdateTimers();
            UpdateNotifications();
        }

        private void UpdateTimers()
        {
            // FIXME
        }

        private void UpdateNotifications()
        {
            #if LUNAR_DEVELOPMENT
            NotificationCenter cnt = NotificationCenter.SharedInstance;
            if (cnt != null)
            {
                float oldHeight = m_notificationsView.Height;

                IDictionary<string, NotificationDelegateList> registry = cnt.RegistryMap;
                foreach (KeyValuePair<string, NotificationDelegateList> e in registry)
                {
                    m_notificationsView[e.Key] = e.Value.Count;
                }

                if (oldHeight != m_notificationsView.Height)
                {
                    this.RootView.ArrangeVert();
                }
            }
            #endif
        }

        [MenuItem("Window/Lunar/Statistics")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<StatisticsWindow>();
        }
    }

    class ValueLabel : Label
    {
        private string m_title;
        private int m_value;

        public ValueLabel(string title)
            : base(title + ": 999")
        {
            m_title = title;
        }

        public int Value
        {
            get { return m_value; }
            set 
            {
                if (m_value != value)
                {
                    m_value = value;
                    this.Text = string.Format("{0}: {1}", StringUtils.C(m_title, ColorCode.Plain), m_value.ToString());

                    Repaint();
                }
            }
        }
    }

    class ValueCompositeView : View
    {
        private IDictionary<string, ValueLabel> m_lookup;

        public ValueCompositeView(string title)
        {
            m_lookup = new Dictionary<string, ValueLabel>();
            AddSubview(new Label(title));
            ResizeToFitSubviews();
        }

        public int this[string name]
        {
            set
            {
                ValueLabel label;
                if (!m_lookup.TryGetValue(name, out label))
                {
                    label = new ValueLabel("  " + name);
                    m_lookup[name] = label;

                    AddSubview(label);
                    ArrangeVert();
                    ResizeToFitSubviews();
                }

                label.Value = value;
                Repaint();
            }
        }
    }

#endif // LUNAR_DEVELOPMENT
}

