using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to all players in a party to indicate a existing party
    /// members has quite the party.
    /// </remarks>
    /// <id>
    /// 0D03
    /// </id>
    internal class SMSG_PARTYQUIT : RelayPacket
    {
        public SMSG_PARTYQUIT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D03;
            this.data = new byte[6];
        }

        public byte Index
        {
            set { this.data[0] = value; }
        }

        public byte Reason
        {
            set { this.data[1] = value; }
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 2, 4); }
        }
    }
}