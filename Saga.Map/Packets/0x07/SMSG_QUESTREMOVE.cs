using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_QUESTREMOVE : RelayPacket
    {
        public SMSG_QUESTREMOVE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0705;
            this.data = new byte[4];
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}