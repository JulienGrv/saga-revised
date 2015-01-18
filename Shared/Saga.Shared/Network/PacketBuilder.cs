using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Saga.Network.Packets;

namespace Saga.Shared.Network
{
    class PacketBuilder<T> where T : Packet, new()
    {

        
        public static void GeneratePacket(byte[] bytes)
        {
        }

        public static void GeneratePacket(Stream s)
        {
        }




    }
}
