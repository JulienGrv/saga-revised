using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is broadcasted to all players in a party to update the job lvel
    /// of your character.
    /// </remarks>
    /// <id>
    /// 0D0D
    /// </id>
    internal class SMSG_PARTYMEMBERJLVL : RelayPacket
    {
        public SMSG_PARTYMEMBERJLVL()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D0D;
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

        public byte Jvl
        {
            set { this.data[5] = value; }
        }
    }
}