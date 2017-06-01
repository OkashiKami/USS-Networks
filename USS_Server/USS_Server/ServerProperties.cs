using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace USS_Server
{
    public class ServerProperties
    {
        public static bool isListening;
        public static Socket socket;
        public static IPEndPoint endpoint;

        internal static void New()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2665);
            isListening = false;

            new Thread(new ThreadStart(Init)).Start();
        }
        internal static void Init()
        {
            Master_Server.inst.Log("Binding to server endpoint...");
            socket.Bind(endpoint);
            if (socket.IsBound)
            {
                Master_Server.inst.Log("Socket bound!");
                socket.Listen(100);
                isListening = true;

                while (isListening)
                {
                    Master_Server.inst.Log("Waiting for client");
                    Socket client = socket.Accept();
                    Master_Server.Clients.Add(new ClientProperties(client));
                }
            }
        }
    }
}