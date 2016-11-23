using UnityEngine;
using System;
using LunarPluginInternal;

namespace LunarPlugin
{
    public class MobileAppObject : MonoBehaviour, MobileUdpServer.IResponse
    {
        #region static
        public const string appIdentifier = "Lunar.MobileApp";
        static MobileAppObject sInstance; 

        internal static void LogTerminal(string line)
        {
            sInstance.mServer.Send(line);
        }

        internal static void LogTerminal(string[] table)
        {
            sInstance.mServer.Send(table);
        }

        internal static void LogTerminal(Exception e, string message)
        {
            sInstance.mServer.Send(e, message);
        }
        #endregion static

        #region inspector
        public int port;
        public bool startNetworkAnyway;
        #endregion inspector

        MobileUdpServer mServer;
        string mIdentifier = "Lunar.MobileAppListener";

        public string identifier { get { return mIdentifier; } }

        public bool ExecCommand(string commandline, bool manual)
        {
            return MobileApp.ExecCommand(commandline, manual);
        }

        void Awake()
        {
            if (!Runtime.IsEditor)
            {
                sInstance = this;
                DontDestroyOnLoad(gameObject);
                MobileApp.Initialize();

                TimerManager.ScheduleTimer(() =>
                {
                    ThreadUtils.InitOnMainThread(); // we need to make sure this call is done on the main thread
                });
            }

            if (!Runtime.IsEditor || startNetworkAnyway)
            {
                mServer = new MobileUdpServer();
                mServer.Start(appIdentifier, port, this);
            }
        }

        void Update()
        {
            if (!Runtime.IsEditor)
                MobileApp.Update();
            if (null != mServer)
                mServer.Update();
        }
    }
}
