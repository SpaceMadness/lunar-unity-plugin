using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using LunarPluginInternal;
using Lidgren.Network;

namespace LunarEditor
{
    public class MobileConnections : EditorWindow
    {
        [MenuItem("Window/Lunar/Mobile Connections")]
        static void OpenWindow()
        {
            MobileConnections window = EditorWindow.GetWindow<MobileConnections>();
            window.titleContent = new GUIContent("Mobile.Connectiions");
        }

        int port;
        MobileUdpClient udpClient { get { return EditorApp.Imp.udpClient; } }

        void OnGUI()
        {
            Discover();
            ServerList();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void Discover()
        {
            EditorGUILayout.LabelField("Discover:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            port = EditorGUILayout.IntField("port", port, GUILayout.Width(220f));
            if (GUILayout.Button("Discover", EditorStyles.miniButton, GUILayout.Width(100f)))
                udpClient.Discover(port);
            EditorGUILayout.EndHorizontal();
        }

        void ServerList()
        {
            EditorGUILayout.LabelField("Server List:", EditorStyles.boldLabel);

            float width = 0f;
            List<GUIContent> contents = new List<GUIContent>();
            for (int i = 0; i < udpClient.serverList.Count; ++i)
            {
                MobileUdpClient.Server server = udpClient.serverList[i];
                GUIContent content = new GUIContent(server.ToString());
                Vector2 size = EditorStyles.label.CalcSize(content);
                contents.Add(content);
                if (width < size.x)
                    width = size.x;
            }

            NetClient client = udpClient.client;
            NetConnection conn = client.ServerConnection;
            for (int i = 0; i < udpClient.serverList.Count; ++i)
            {
                MobileUdpClient.Server server = udpClient.serverList[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(contents[i], GUILayout.Width(width + 30f));
                if (null != conn && conn.RemoteEndPoint == server.endpoint)
                {
                    if (conn.Status == NetConnectionStatus.Connected)
                    {
                        if (GUILayout.Button("Disonnect", EditorStyles.miniButton, GUILayout.Width(100f)))
                            client.Disconnect("GoodBye");
                    }
                    else
                    {
                        EditorGUILayout.LabelField(conn.Status.ToString());
                    }
                }
                else
                {
                    if (GUILayout.Button("Connect", EditorStyles.miniButton, GUILayout.Width(100f)))
                        client.Connect(server.endpoint);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
