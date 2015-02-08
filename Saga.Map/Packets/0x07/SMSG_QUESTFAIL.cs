using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_QUESTFAIL : RelayPacket
    {
        public SMSG_QUESTFAIL()
        {
            this.Cmd = 0x0601;
            this.Id = 0x070A;
            this.data = new byte[8];
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte CancelReason
        {
            set { this.data[4] = value; }
        }
    }
}