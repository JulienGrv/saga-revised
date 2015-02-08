using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is broadcasted to all players in a party to update the characters base level
    /// player. This packet should be sent when obtaining a new character level.
    /// </remarks>
    /// <id>
    /// 0D0B
    /// </id>
    internal class SMSG_PARTYMEMBERCLVL : RelayPacket
    {
        public SMSG_PARTYMEMBERCLVL()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D0B;
            this.data = new byte[6];
        }

        public byte Index
        {
            set { this.data[0] = value; }
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public byte Lp
        {
            set { this.data[5] = value; }
        }
    }
}