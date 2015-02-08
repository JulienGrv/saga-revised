using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_SKILLADD : RelayPacket
    {
        public SMSG_SKILLADD()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0901;
            this.data = new byte[7];
        }

        //USE SLOT 0 FOR BATTLESKILL
        //USE SLOT 1 FOR SPECIALLITY
        public byte Slot
        {
            set { this.data[0] = value; }
        }

        public uint SkillId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }
    }
}