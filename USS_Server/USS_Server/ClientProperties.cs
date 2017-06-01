using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using USS_Shared;

namespace USS_Server
{
    [Serializable]
    public class ClientProperties
    {
        public bool IsConnected;
        private static Socket socket;
        public ClientProperties(Socket incomming_client)
        {
            socket = incomming_client;
            Master_Server.inst.Log("Client Connected!");
            Master_Server.inst.Log("Starting client receiver...");
            IsConnected = socket.Connected;

            new Thread(new ThreadStart(RECEIVER)).Start();
            new Thread(new ThreadStart(TestSend)).Start();

        }

        private void RECEIVER()
        {
            Master_Server.inst.Log("Receiver started...");
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
                        Master_Server.inst.Log("Data received from client");
                        Process((Packet)data);
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
                    Master_Server.inst.Log("Test data sent!");
                }
                else Master_Server.inst.Log("No test data sent!");
                goto Start;
            }
            catch
            {
                IsConnected = false;
                Master_Server.inst.Log("Client Disconnected!");
            }
        }
        public static void Send(Packet data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, data);
            byte[] buff = ms.ToArray();
            socket.Send(buff);
            if (buff.Length > 0)
            {
                Master_Server.inst.Log("Message sent!");
            }
            else Master_Server.inst.Log("No data sent!");
        }

        private void Process(Packet nPacket)
        {
            Master_Server.inst.Log("Processing Packet ({0})", nPacket.uuid);
            if (!string.IsNullOrEmpty(nPacket.message))
                Master_Server.inst.Log("Client: {0}", nPacket.message);
            if (nPacket.data != null)
            {
                if (nPacket.TYPE != Packet.PacketType.TEST)
                {
                    Master_Server.inst.Log("Data Type: {0}", nPacket.data.GetType());
                    Master_Server.inst.Log("This Object needs to be type casted... Type casting...");
                }
                else Master_Server.inst.Log("Test Received!");
            }
        }
    }
}
