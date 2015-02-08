using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is broadcasted to all players in a party to update the sp of a current
    /// player. This mostly seems to be sent after regeneration.
    /// </remarks>
    /// <id>
    /// 0D0A
    /// </id>
    internal class SMSG_PARTYMEMBERSPINFO : RelayPacket
    {
        public SMSG_PARTYMEMBERSPINFO()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D0A;
            this.data = new byte[9];
        }

        public byte Index
        {
            set { this.data[0] = value; }
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public ushort SpMax
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 2); }
        }

        public ushort Sp
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 7, 2); }
        }
    }
}