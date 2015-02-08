using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_QUESTCONFIRM : RelayPacket
    {
        public SMSG_QUESTCONFIRM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0703;
            this.data = new byte[4];
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}