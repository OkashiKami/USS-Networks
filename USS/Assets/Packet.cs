using System;

namespace USS_Shared
{
    [Serializable]
    public class Packet
    {
        public string uuid;
        public string message;
        public object data;
        public enum PacketType { TEST, NORMAL, }
        public PacketType TYPE;

        public Packet(PacketType TYPE)
        {
            this.TYPE = TYPE;
        }

        internal static void Process(Packet nPacket)
        {
            Console.WriteLine("Processing Packet ({0})", nPacket.uuid);
            if (!string.IsNullOrEmpty(nPacket.message))
               Console.WriteLine("Client: {0}", nPacket.message);
            if (nPacket.data != null)
            {
                if (nPacket.TYPE != PacketType.TEST)
                {
                    Console.WriteLine("Data Type: {0}", nPacket.data.GetType());
                    Console.WriteLine("This Object needs to be type casted... Type casting...");
                }
                else Console.WriteLine("Test Received!");
            }
        }
    }
}
