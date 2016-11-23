using UnityEngine;
using System;
using System.Net;
using System.Collections.Generic;
using Lidgren.Network;
using LunarPlugin;

namespace LunarPluginInternal
{
    class MobileUdpClient
    {
        public class Server : IEquatable<Server>
        {
            public string identifier;
            public IPEndPoint endpoint;

            public override string ToString()
            {
                return string.Format("{0}({1})", identifier, endpoint);
            }

            public override int GetHashCode()
            {
                return endpoint.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return Equals((Server)obj);
            }

            public bool Equals(Server other)
            {
                if (null == other)
                    return false;

                return 0 == string.Compare(identifier, other.identifier) &&
                       endpoint.Equals(other.endpoint);
            }
        }

        NetClient mClient;
        List<Server> mServerList = new List<Server>();
        ICCommandDelegate mCommandDelegate;

        public NetClient client { get { return mClient; } }
        public List<Server> serverList { get { return mServerList; } }
        public NetConnectionStatus connectionStatus { get { return mClient.ConnectionStatus; } }

        public MobileUdpClient(ICCommandDelegate commandDelegate)
        {
            mCommandDelegate = commandDelegate;
            NetPeerConfiguration config = new NetPeerConfiguration(MobileAppObject.appIdentifier);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            mClient = new NetClient(config);
            mClient.Start();
        }

        public void Discover(int port)
        {
            mServerList.Clear();
            mClient.DiscoverLocalPeers(port);
        }

        public void Send(string commandline, bool manual)
        {
            if (!string.IsNullOrEmpty(commandline))
            {
                NetOutgoingMessage msg = mClient.CreateMessage();
                msg.Write(commandline);
                msg.Write(manual);
                mClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void Update()
        {
            NetIncomingMessage im;
            while ((im = mClient.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.ErrorMessage:
                        {
                            Debug.LogError(im.ReadString());
                            break;
                        }
                    case NetIncomingMessageType.WarningMessage:
                        {
                            Debug.LogWarning(im.ReadString());
                            break;
                        }
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        {
                            Debug.Log(im.ReadString());
                            break;
                        }
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string reason = im.ReadString();
                            Debug.LogFormat(
                                "{0} -> {1}: {2}",
                                NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier),
                                status, reason);
                            break;
                        }
                    case NetIncomingMessageType.DiscoveryResponse:
                        {
                            Server server = new Server()
                            {
                                identifier = im.ReadString(),
                                endpoint = im.SenderEndPoint,
                            };

                            if (!mServerList.Contains(server))
                                mServerList.Add(server);
                            break;
                        }
                    case NetIncomingMessageType.Data:
                        {
                            int key = im.ReadInt32();
                            if (0 == key)
                            {
                                mCommandDelegate.LogTerminal(im.ReadString());
                            }
                            else if (1 == key)
                            {
                                int len = im.ReadInt32();
                                string[] table = new string[len];
                                for (int i = 0; i < len; ++i)
                                    table[i] = im.ReadString();
                                mCommandDelegate.LogTerminal(table);
                            }
                            else if (2 == key)
                            {
                                mCommandDelegate.LogTerminal(im.ReadString());
                                mCommandDelegate.LogTerminal(im.ReadString());
                                mCommandDelegate.LogTerminal(im.ReadString());
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}
