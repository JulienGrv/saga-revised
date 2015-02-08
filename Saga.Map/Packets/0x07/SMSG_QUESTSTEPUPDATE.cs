using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_QUESTSTEPUPDATE : RelayPacket
    {
        public SMSG_QUESTSTEPUPDATE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0706;
            this.data = new byte[14];
            this.data[8] = 2;
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint StepID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }

        public uint NextStepID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 9, 4); }
        }

        public byte Progress
        {
            set { this.data[13] = value; }
        }
    }
}