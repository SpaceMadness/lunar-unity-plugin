package com.lunar.network;

import com.lunar.core.ConcurrentQueue;
import com.lunar.debug.Log;
import com.lunar.util.HackByteArrayInputStream;
import com.lunar.util.HackByteArrayOutputStream;
import com.lunar.util.ThreadPool;
import com.lunar.util.UTF8;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.DatagramPacket;
import java.net.InetSocketAddress;
import java.net.MulticastSocket;
import java.net.SocketAddress;

public abstract class NetMulticastSocket
{
    private static final String TAG = "NetMulticastSocket";

    private final NetPeerConfiguration configuration;
    private final ConcurrentQueue<NetTuple> outgoingMessages;

    private MulticastSocket socket;

    public NetMulticastSocket(NetPeerConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new NullPointerException("Configuration is null");
        }

        this.configuration = configuration;
        this.outgoingMessages = new ConcurrentQueue<NetTuple>();
    }

    public void start()
    {
        ThreadPool.defaultPool().execute(new Runnable()
        {
            @Override
            public void run()
            {
                try
                {
                    runGuarded();
                }
                catch (Exception e)
                {
                    e.printStackTrace();
                }
            }

            public void runGuarded() throws IOException
            {
                socket = new MulticastSocket();

                AsyncMessageHandler reader = new AsyncMessageReader()
                {
                    private static final int MAX_PACKET_SIZE = 1280;

                    private final HackByteArrayInputStream input =
                            new HackByteArrayInputStream(MAX_PACKET_SIZE);

                    @Override
                    protected NetMessage readMessage() throws IOException
                    {
                        input.reset();

                        try
                        {
                            byte[] data = input.innerBuffer();
                            DatagramPacket packet = new DatagramPacket(data, data.length);
                            socket.receive(packet);

                            InetSocketAddress remoteAddress = (InetSocketAddress) packet.getSocketAddress();
                            return readMessage(input, remoteAddress);
                        }
                        catch (IOException e)
                        {
                            Log.v(TAG, "Can't read message: %s", e.getMessage());
                            return null;
                        }
                    }

                    private NetMessage readMessage(InputStream stream, InetSocketAddress remoteAddress) throws IOException
                    {
                        if (!readMulticastHeader(stream))
                        {
                            return null;
                        }

                        // header
                        if (stream.available() < headerBuffer.length)
                        {
                            Log.v(TAG, "Can't read message: not enough data: %d", stream.available());
                            return null;
                        }
                        readBuffer(stream, headerBuffer);

                        NetMessageType type = NetMessageType.valueOf(headerBuffer[0]);
                        if (type != NetMessageType.DiscoveryResponse && type != NetMessageType.DiscoveryRequest)
                        {
                            Log.v(TAG, "Can't read message: unexpected message type: %d", headerBuffer[0]);
                            return null;
                        }

                        int payloadLength = ((headerBuffer[1] & 0xff) << 24) |
                                            ((headerBuffer[2] & 0xff) << 16) |
                                            ((headerBuffer[3] & 0xff) << 8) |
                                             (headerBuffer[4] & 0xff);

                        if (payloadLength > stream.available())
                        {
                            Log.v(TAG, "Can't read message: message is too big: %d", payloadLength);
                            return null;
                        }

                        if (type == NetMessageType.DiscoveryResponse)
                        {
                            int b1 = stream.read() & 0xff;
                            int b2 = stream.read() & 0xff;

                            int listeningPort = (b1 << 8) | b2;
                            remoteAddress = new InetSocketAddress(remoteAddress.getAddress(), listeningPort);
                        }

                        NetMessage msg = createMessage(type);
                        msg.setLength(payloadLength);
                        msg.setRemoteAddress(remoteAddress);

                        // payload
                        readBuffer(stream, msg.data, 0, payloadLength);

                        return msg;
                    }

                    private boolean readMulticastHeader(InputStream stream) throws IOException
                    {
                        // protocol version
                        int protocolVersion = stream.read();
                        if (protocolVersion != NetConstants.PROTOCOL_VERSION)
                        {
                            Log.v(TAG, "Can't read message: unsupported protocol version: %d", protocolVersion);
                            return false;
                        }

                        // app id
                        int appIdLength = stream.read();
                        byte[] appIdBytes = new byte[appIdLength];
                        stream.read(appIdBytes);

                        String appId = UTF8.GetString(appIdBytes, 0, appIdBytes.length);
                        if (!appId.equals(configuration.appIdentifier))
                        {
                            Log.v(TAG, "Can't read message: wrong app id: %s", appId);
                            return false;
                        }

                        return true;
                    }

                    @Override
                    protected void receiveIncomingMessage(NetMessage msg)
                    {
                        NetMulticastSocket.this.receiveIncomingMessage(msg);
                    }

                    @Override
                    protected void handlerMessageError(String message)
                    {
                        Log.e("Error receiving multicast message: %s", message);
                    }
                };
                reader.start();

                AsyncMessageHandler writer = new AsyncMessageHandler()
                {
                    private HackByteArrayOutputStream output = new HackByteArrayOutputStream();

                    @Override
                    protected void runLoop() throws Exception
                    {
                        NetTuple tuple = outgoingMessages.take();

                        SocketAddress address = tuple.address;
                        NetMessage msg = tuple.message;

                        sendMessage(msg, address);
                        msg.recycle();
                    }

                    @Override
                    protected void handlerMessageError(String message)
                    {
                        Log.e("Error receiving multicast message: %s" + message);
                    }

                    protected void sendMessage(NetMessage msg, SocketAddress address) throws IOException
                    {
                        output.reset();
                        writeMessage(output, msg);

                        byte[] buffer = output.internalBuffer();
                        int length = output.size();
                        socket.send(new DatagramPacket(buffer, 0, length, address));
                    }

                    private void writeMessage(OutputStream stream, NetMessage msg) throws IOException
                    {
                        // multicast header
                        writeMulticastHeader(stream);

                        // header
                        int payloadLength = msg.getLength() ;
                        headerBuffer[0] = msg.getMessageType().byteValue();
                        headerBuffer[1] = (byte) ((payloadLength >> 24) & 0xff);
                        headerBuffer[2] = (byte) ((payloadLength >> 16) & 0xff);
                        headerBuffer[3] = (byte) ((payloadLength >> 8) & 0xff);
                        headerBuffer[4] = (byte) (payloadLength & 0xff);

                        stream.write(headerBuffer);

                        // payload
                        stream.write(msg.data, 0, msg.getLength());
                    }

                    private void writeMulticastHeader(OutputStream stream) throws IOException
                    {
                        // protocol version
                        stream.write(NetConstants.PROTOCOL_VERSION);

                        // app id
                        byte[] appIdBytes = UTF8.GetBytes(configuration.appIdentifier);
                        stream.write(appIdBytes.length);
                        stream.write(appIdBytes);
                    }
                };
                writer.start();
            }
        });
    }

    protected abstract void receiveIncomingMessage(NetMessage msg);

    public void stop()
    {
        socket.close();
    }

    public void sendDiscoveryRequest(NetMessage payload)
    {
        NetMessage msg = NetMessagePool.getMessage(NetMessageType.DiscoveryRequest);
        if (payload != null)
        {
            msg.Write(payload.data, 0, payload.getLength());
        }
        NetTuple tuple = new NetTuple(configuration.multicastAddress, msg);
        outgoingMessages.enqueue(tuple);
    }
}
