package lunarapp.com.lunartestapp;

import android.content.SharedPreferences;
import android.os.Handler;
import android.os.Looper;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.lunar.network.NetClientPeer;
import com.lunar.network.NetConnectionStatus;
import com.lunar.network.NetMessage;
import com.lunar.network.NetPeer;
import com.lunar.network.NetPeerConfiguration;

import java.io.IOException;

public class MainActivity extends ActionBarActivity
{
    private Handler handler;

    private EditText editHost;
    private EditText editPort;
    private TextView textStatus;

    private NetPeer peer;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        SharedPreferences prefs = getSharedPreferences("com.lunar.prefs", MODE_PRIVATE);

        textStatus = (TextView) findViewById(R.id.textStatus);
        setStatus("Disconnected");

        editHost = (EditText) findViewById(R.id.editHost);
        String host = prefs.getString("host", null);
        if (host != null)
        {
            editHost.setText(host);
        }

        editPort = (EditText) findViewById(R.id.editPort);
        int port = prefs.getInt("port", -1);
        if (port != -1)
        {
            editPort.setText(Integer.toString(port));
        }

        Button connectButton = (Button) findViewById(R.id.buttonConnect);
        connectButton.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                String host = editHost.getText().toString();
                String portStr = editPort.getText().toString();

                try
                {
                    int port = Integer.parseInt(portStr);

                    SharedPreferences prefs = getSharedPreferences("com.lunar.prefs", MODE_PRIVATE);
                    SharedPreferences.Editor editor = prefs.edit();
                    editor.putString("host", host);
                    editor.putInt("port", port);
                    editor.apply();

                    connect(host, port);

                    handler = new Handler(Looper.getMainLooper());
                    handler.postDelayed(loopRunnable, 20);

                }
                catch (NumberFormatException e)
                {
                    Toast.makeText(MainActivity.this, "Bad port", Toast.LENGTH_LONG).show();
                }
                catch (Exception e)
                {
                    Toast.makeText(MainActivity.this, "Can't connect", Toast.LENGTH_LONG).show();
                    e.printStackTrace();
                }
            }
        });

        Button sendButton = (Button) findViewById(R.id.buttonSend);
        sendButton.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                if (peer != null)
                {
                    sendMessage();
                }
            }
        });
    }

    private void setStatus(String status)
    {
        textStatus.setText(status);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu)
    {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item)
    {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings)
        {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    private void connect(String host, int port) throws IOException
    {
        NetPeerConfiguration config = new NetPeerConfiguration("test");
        NetPeer peer = new NetClientPeer(config);
        peer.start();
        peer.sendDiscoveryRequest();

        setStatus("Connecting");

        this.peer = peer;
    }

    private void sendMessage()
    {
        NetMessage msg = peer.createMessage();
        msg.Write("test");
        peer.sendMessage(msg);
    }

    private void runLoop()
    {
        NetMessage msg;
        while ((msg = peer.readMessage()) != null)
        {
            handleMessage(msg);
        }
    }

    private void handleMessage(NetMessage msg)
    {
        try
        {
            switch (msg.getMessageType())
            {
                case Data:
                {
                    System.out.println("Message received: " + msg.getLength());
                    break;
                }

                case StatusChanged:
                {
                    NetConnectionStatus status = NetConnectionStatus.valueOf(msg.ReadByte());
                    if (status == NetConnectionStatus.Connected)
                    {
                        setStatus("Connected");
                    }
                    else if (status == NetConnectionStatus.Disconnected)
                    {
                        setStatus("Disconnected");
                    }
                    break;
                }

                case DiscoveryResponse:
                {
                    System.out.println("Discovery response from " + msg.getRemoteAddress());
                    break;
                }
            }
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
    }

    private Runnable loopRunnable = new Runnable()
    {
        @Override
        public void run()
        {
            runLoop();
            handler.postDelayed(loopRunnable, 20);
        }
    };
}
