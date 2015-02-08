using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet appears is broadcasted to all players in a party to
    /// indicate party settings have changed
    /// </remarks>
    /// <id>
    /// 0D08
    /// </id>
    internal class SMSG_PARTYMODE : RelayPacket
    {
        public SMSG_PARTYMODE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D08;
            this.data = new byte[7];
        }

        public byte LootShare
        {
            set { this.data[0] = value; }
        }

        public byte ExpShare
        {
            set { this.data[1] = value; }
        }

        public byte Unknown
        {
            set { this.data[2] = value; }
        }

        public uint LootMaster
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 3, 4); }
        }
    }
}