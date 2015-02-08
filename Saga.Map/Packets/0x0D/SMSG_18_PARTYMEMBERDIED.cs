using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Packet that indicates the player has died.
    /// </remarks>
    /// <id>
    /// 0D18
    /// </id>
    internal class SMSG_PARTYMEMBERDIED : RelayPacket
    {
        public SMSG_PARTYMEMBERDIED()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D18;
            this.data = new byte[5];
        }

        public byte Index
        {
            set { this.data[0] = value; }
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }
    }
}