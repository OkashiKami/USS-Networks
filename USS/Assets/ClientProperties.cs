using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

[Serializable] public class ClientProperties
{
    public bool IsSet;
    public bool IsConnected;
    public int attempts;

    public static Socket socket;
    public IPEndPoint endpoint;


    public ClientProperties(bool debug = false)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2665);
        Thread InitThread = new Thread(new ThreadStart(Init));
        InitThread.IsBackground = true;
        InitThread.Start();
    }
    internal void Init()
    {
        IsSet = true;

        retry:
        if (attempts > 0)
        {
            Master_Client.inst.Log("Attempts {0}", attempts);
        }

        try
        {
            Master_Client.inst.Log("Connecting...");
            socket.Connect(endpoint);
            Thread.Sleep(2000);
            attempts = 0;
            Master_Client.inst.Log("Connected!");
            IsConnected = true;

            new Thread(new ThreadStart(RECEIVER)).Start();
            new Thread(new ThreadStart(TestSend)).Start();
        }
        catch
        {

            if (attempts < 10)
            {
                Master_Client.inst.Log("Can't connect to master server retrying");
                attempts += 1;
                goto retry;
            }
            IsConnected = false;
            Master_Client.inst.Log("Connection to server failed");
        }

    }

    private void RECEIVER()
    {
        Master_Client.inst.Log("Receiver started...");
        while (IsConnected)
        {
            try
            {
                byte[] buff = new byte[socket.ReceiveBufferSize];
                socket.Receive(buff);
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(buff);
                object data = bf.Deserialize(ms);
                if (data != null)
                {
                    Master_Client.inst.Log("Data received from client");
                    Process((USS_Shared.Packet)data);
                }
            }
            catch { }
        }
    }

    private void TestSend()
    {
        try
        {
            Start:
            USS_Shared.Packet data = new USS_Shared.Packet(USS_Shared.Packet.PacketType.TEST);
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, data);
            byte[] buff = ms.ToArray();
            socket.Send(buff);
            if (buff.Length > 0)
            {
                Master_Client.inst.Log("Test data sent!");
            }
            else Master_Client.inst.Log("No test data sent!");
            goto Start;
        }
        catch
        {
            IsConnected = false;
            Master_Client.inst.Log("Client Disconnected!");
        }
    }
    public static void Send(USS_Shared.Packet data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, data);
        byte[] buff = ms.ToArray();
        socket.Send(buff);
        if (buff.Length > 0)
        {
            Master_Client.inst.Log("Message sent!");
        }
        else Master_Client.inst.Log("No data sent!");
    }

    private void Process(USS_Shared.Packet nPacket)
    {
        Master_Client.inst.Log("Processing Packet ({0})", nPacket.uuid);
        if (!string.IsNullOrEmpty(nPacket.message))
            Master_Client.inst.Log("Client: {0}", nPacket.message);
        if (nPacket.data != null)
        {
            if (nPacket.TYPE != USS_Shared.Packet.PacketType.TEST)
            {
                Master_Client.inst.Log("Data Type: {0}", nPacket.data.GetType());
                Master_Client.inst.Log("This Object needs to be type casted... Type casting...");
            }
            else Master_Client.inst.Log("Test Received!");
        }
    }
}