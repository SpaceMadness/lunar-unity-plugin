package com.lunar.network;

import com.lunar.core.ConcurrentQueue;
import com.lunar.debug.Log;
import com.lunar.util.ThreadPool;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.net.SocketAddress;

enum NetPeerState
{
    Created,
    Connecting,
    Connected,
    Disconnecting,
    Disconnected
}

public class NetClientPeer extends NetPeer
{
    private NetPeerState state;

    private Socket socket;
    private NetMulticastSocket multicastSocket;

    private ConcurrentQueue<NetMessage> incomingMessages;
    private ConcurrentQueue<NetMessage> outgoingMessages;

    public NetClientPeer(NetPeerConfiguration config)
    {
        super(config);

        incomingMessages = new ConcurrentQueue<NetMessage>();
        outgoingMessages = new ConcurrentQueue<NetMessage>();

        state = NetPeerState.Created;
    }

    @Override
    public void start()
    {
        multicastSocket = new NetMulticastSocket(getConfiguration())
        {
            @Override
            protected void receiveIncomingMessage(NetMessage msg)
            {
                enqueueMessage(msg);
            }
        };
        multicastSocket.start();
    }

    @Override
    public void connect(final SocketAddress address)
    {
        if (address == null)
        {
            throw new NullPointerException("Address is null");
        }

        // FIXME: check state
        state = NetPeerState.Connecting;

        ThreadPool.defaultPool().execute(new Runnable()
        {
            @Override
            public void run()
            {
                connectSync(address);
            }
        });
    }

    @Override
    public void close()
    {
        closeSocket();
    }

    private void closeSocket()
    {
        try
        {
            if (socket != null)
            {
                socket.close();
            }
        }
        catch (IOException e)
        {
        }
    }

    private void connectSync(SocketAddress address)
    {
        try
        {
            Socket socket = new Socket();
            socket.setReceiveBufferSize(getConfiguration().receiveBufferSize);
            socket.setSendBufferSize(getConfiguration().sendBufferSize);
            socket.connect(address);

            this.socket = socket;
            setRemoteAddress(socket.getRemoteSocketAddress());

            enqueueStatusChangedMessage(NetConnectionStatus.Connecting);
            state = NetPeerState.Connecting;

            final InputStream input = socket.getInputStream();
            final OutputStream output = socket.getOutputStream();

            // message reader
            final AsyncMessageHandler reader = new AsyncMessageStreamReader(input)
            {
                @Override
                protected void receiveIncomingMessage(NetMessage msg)
                {
                    enqueueMessage(msg);
                }

                @Override
                protected void handlerMessageError(String message)
                {
                    handlerError(message);
                }
            };
            reader.start();

            // message writer
            AsyncMessageHandler writer = new AsyncMessageStreamWriter(output)
            {
                @Override
                protected NetMessage dequeueOutgoingMessage() throws Exception
                {
                    return dequeueMessage();
                }

                @Override
                protected void handlerMessageError(String message)
                {
                    handlerError(message);
                }
            };
            writer.start();


        }
        catch (Exception e)
        {
            Log.e("Can't connect to the sever: " + e.getMessage());
            state = NetPeerState.Disconnected;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Messages

    @Override
    public NetMessage readMessage()
    {
        return incomingMessages.dequeue();
    }

    @Override
    public void sendMessage(NetMessage msg)
    {
        outgoingMessages.enqueue(msg);
    }

    @Override
    public void sendDiscoveryRequest(NetMessage payload)
    {
        multicastSocket.sendDiscoveryRequest(payload);
    }

    @Override
    public NetMessage createMessage()
    {
        return createMessage(NetMessageType.Data);
    }

    protected NetMessage peekMessage()
    {
        return incomingMessages.peek();
    }

    protected NetMessage createMessage(NetMessageType type)
    {
        return NetMessagePool.getMessage(type);
    }

    private void enqueueMessage(NetMessage msg)
    {
        incomingMessages.enqueue(msg);
    }

    private NetMessage dequeueMessage() throws InterruptedException
    {
        return outgoingMessages.take();
    }

    private void enqueueStatusChangedMessage(NetConnectionStatus status)
    {
        NetMessage msg = createMessage(NetMessageType.StatusChanged);
        msg.Write(status.byteValue());
        enqueueMessage(msg);
    }

    private synchronized void handlerError(String message)
    {
        Log.e("Error: " + message);

        if (state != NetPeerState.Disconnected)
        {
            state = NetPeerState.Disconnected;
            closeSocket();

            enqueueStatusChangedMessage(NetConnectionStatus.Disconnected);
        }
    }
}