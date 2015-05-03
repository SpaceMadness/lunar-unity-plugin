package com.lunar.extensions.unity;

import com.lunar.debug.Assert;
import com.lunar.debug.Log;
import com.lunar.network.AddressUtils;
import com.lunar.network.NetClientPeer;
import com.lunar.network.NetMessage;
import com.lunar.network.NetMessageType;
import com.lunar.network.NetPeerConfiguration;

import java.net.InetSocketAddress;
import java.net.SocketAddress;

import static com.lunar.debug.Assert.*;

public class UnityClientPeer extends NetClientPeer
{
    private static final int ADDRESS_BUFFER_SIZE = 4 + 2; /* IP address + port number */

    private static final int RESULT_FAILURE = -1;
    private static final int RESULT_SUCCESS = 0;

    private final byte[] addressBuffer;

    private String lastErrorMessage;

    public UnityClientPeer(String appId)
    {
        super(new NetPeerConfiguration(appId));

        addressBuffer = new byte[ADDRESS_BUFFER_SIZE];
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Native interface

    public int nativeStart()
    {
        try
        {
            start();
            return RESULT_SUCCESS;
        }
        catch (Exception e)
        {
            handleException(e);
        }

        return RESULT_FAILURE;
    }

    public int nativeClose()
    {
        try
        {
            close();
            return RESULT_SUCCESS;
        }
        catch (Exception e)
        {
            handleException(e);
        }

        return RESULT_FAILURE;
    }

    public int nativeConnect(int address, int port)
    {
        try
        {
            SocketAddress remoteAddress = AddressUtils.createAddress(address, port);
            connect(remoteAddress);

            return RESULT_SUCCESS;
        }
        catch (Exception e)
        {
            handleException(e);
        }

        return RESULT_FAILURE;
    }

    public int nativeSendMessage(byte type, int bitLength, byte[] buffer)
    {
        try
        {
            NetMessageType msgType = NetMessageType.valueOf(type);
            if (msgType == null)
            {
                Log.e("Invalid message type: %s", Integer.toString(type));
                return RESULT_FAILURE;
            }

            NetMessage msg = createMessage(msgType);
            msg.setPayload(buffer, bitLength);
            sendMessage(msg);

            return RESULT_SUCCESS;
        }
        catch (Exception e)
        {
            handleException(e);
        }

        return RESULT_FAILURE;
    }

    public int nativeSendDiscoveryRequest(byte[] payloadBytes)
    {
        try
        {
            NetMessage payload = createMessage();
            payload.Write(payloadBytes);
            super.sendDiscoveryRequest(payload);
            payload.recycle();

            return RESULT_SUCCESS;
        }
        catch (Exception e)
        {
            handleException(e);
        }

        return RESULT_FAILURE;
    }

    public int nativeReadMessageHeader()
    {
        try
        {
            NetMessage msg = peekMessage();
            if (msg != null)
            {
                int type = msg.getMessageType().byteValue();
                Assert.range(type, 0, 255);

                int payloadLength = msg.getLength();
                Assert.range(payloadLength, 0, 16777215);

                return (type << 24) | payloadLength;
            }
        }
        catch (Exception e)
        {
            handleException(e);
        }

        return RESULT_FAILURE;
    }

    public byte[] nativeReadMessagePayload()
    {
        NetMessage msg = readMessage();
        return msg != null ? getMessageData(msg) : null;
    }

    public String nativeLastErrorMessage()
    {
        String message = lastErrorMessage;
        lastErrorMessage = null;
        return message;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Helpers

    private void handleException(Exception e)
    {
        e.printStackTrace(); // FIXME: log exception
        lastErrorMessage = e.getMessage();
    }

    private byte[] getMessageData(NetMessage msg)
    {
        /* For discovery response messages we need to add the host's remote address */
        if (msg.getMessageType() == NetMessageType.DiscoveryResponse)
        {
            byte[] payload = msg.getPayload();
            byte[] data = payload != null && payload.length > 0 ?
                    new byte[ADDRESS_BUFFER_SIZE + payload.length] :
                    addressBuffer;

            InetSocketAddress remoteAddress = msg.getRemoteAddress();
            assertNotNull(remoteAddress);

            byte[] addressBytes = remoteAddress.getAddress().getAddress();
            assertEquals(4, addressBytes.length);

            short port = (short) remoteAddress.getPort();

            // reverse network address bytes order
            data[0] = addressBytes[3];
            data[1] = addressBytes[2];
            data[2] = addressBytes[1];
            data[3] = addressBytes[0];

            data[4] = (byte) ((port >> 8) & 0xff);
            data[5] = (byte) (port & 0xff);

            if (payload != null && payload.length > 0)
            {
                System.arraycopy(payload, 0, data, ADDRESS_BUFFER_SIZE, payload.length);
            }

            return data;
        }

        return msg.getPayload();
    }
}
