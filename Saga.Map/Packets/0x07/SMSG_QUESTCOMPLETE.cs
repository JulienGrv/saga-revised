using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_QUESTCOMPLETE : RelayPacket
    {
        public SMSG_QUESTCOMPLETE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0707;
            this.data = new byte[4];
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}