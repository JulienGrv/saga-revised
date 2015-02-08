using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_SCENARIOSTEPCOMPLETE : RelayPacket
    {
        public SMSG_SCENARIOSTEPCOMPLETE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1003;
            this.data = new byte[8];
        }

        public uint Step
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

        public uint NextStep
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4);
            }
        }
    }
}