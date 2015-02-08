using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_SCENARIOSTEPUPDATE : RelayPacket
    {
        public SMSG_SCENARIOSTEPUPDATE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1006;
            this.data = new byte[11];
        }

        public uint Scenario
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

        public uint Scenario2
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4);
            }
        }

        public byte SubStep
        {
            set { this.data[8] = value; }
        }

        public byte Amount
        {
            set { this.data[9] = value; }
        }
    }
}