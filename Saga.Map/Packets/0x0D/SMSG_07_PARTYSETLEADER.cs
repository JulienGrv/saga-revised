using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to all party members to indicate the party
    /// is switching party leader.
    /// </remarks>
    /// <id>
    /// 0D07
    /// </id>
    internal class SMSG_PARTYSETLEADER : RelayPacket
    {
        public SMSG_PARTYSETLEADER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D07;
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