using UnityEngine;
using System;
using Lidgren.Network;

namespace LunarPluginInternal
{
    class MobileUdpServer
    {
        public interface IResponse
        {
            string identifier { get; }
            bool ExecCommand(string commandline, bool manual);
        }

        public void Start(string appIdentifier, int port, IResponse response)
        {
            mResponse = response;
            NetPeerConfiguration config = new NetPeerConfiguration(appIdentifier);
            config.Port = port;
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            mServer = new NetServer(config);
            mServer.Start();
        }

        public void Send(string line)
        {
            if (null != mSession)
            {
                NetOutgoingMessage msg = mServer.CreateMessage();
                msg.Write(0);
                msg.Write(line);
                mServer.SendMessage(msg, mSession, NetDeliveryMethod.Unreliable);
            }
        }

        public void Send(string[] table)
        {
            if (null != mSession && table.Length > 0)
            {
                NetOutgoingMessage msg = mServer.CreateMessage();
                msg.Write(1);
                msg.Write(table.Length);
                for (int i = 0; i < table.Length; ++i)
                    msg.Write(table[i]);
                mServer.SendMessage(msg, mSession, NetDeliveryMethod.Unreliable);
            }
        }

        public void Send(Exception e, string message)
        {
            if (null != mSession)
            {
                NetOutgoingMessage msg = mServer.CreateMessage();
                msg.Write(2);
                msg.Write(message);
                msg.Write(e.Message);
                msg.Write(e.StackTrace);
                mServer.SendMessage(msg, mSession, NetDeliveryMethod.Unreliable);
            }
        }

        public void Update()
        {
            NetIncomingMessage im;
            while ((im = mServer.ReadMessage()) != null)
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
                    case NetIncomingMessageType.DiscoveryRequest:
                        {
                            NetOutgoingMessage response = mServer.CreateMessage(mResponse.identifier);
                            mServer.SendDiscoveryResponse(response, im.SenderEndPoint);
                            break;
                        }
                    case NetIncomingMessageType.Data:
                        {
                            string commandline = im.ReadString();
                            bool manual = im.ReadBoolean();
                            mSession = im.SenderConnection;
                            mResponse.ExecCommand(commandline, manual);
                            mSession = null;
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        NetServer mServer;
        NetConnection mSession;
        IResponse mResponse;
    }
}
