using Saga.Network.Packets;
using System.IO;

namespace Saga.Shared.Network
{
    internal class PacketBuilder<T> where T : Packet, new()
    {
        public static void GeneratePacket(byte[] bytes)
        {
        }

        public static void GeneratePacket(Stream s)
        {
        }
    }
}