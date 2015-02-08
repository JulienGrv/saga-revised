using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_QUESTSUBSTEPUPDATE : RelayPacket
    {
        public SMSG_QUESTSUBSTEPUPDATE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0709;
            this.data = new byte[11];
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint StepID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }

        public byte Unknown
        {
            set { this.data[8] = value; }
        }

        public byte SubStep
        {
            set { this.data[9] = value; }
        }

        public byte Amount
        {
            set { this.data[10] = value; }
        }
    }
}