using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_QUESTNPCNOTE : RelayPacket
    {
        public SMSG_QUESTNPCNOTE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x070E;
            this.data = new byte[8];
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint StepId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }
    }
}