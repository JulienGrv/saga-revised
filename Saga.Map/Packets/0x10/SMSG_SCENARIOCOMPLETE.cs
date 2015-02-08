using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_SCENARIOCOMPLETE : RelayPacket
    {
        public SMSG_SCENARIOCOMPLETE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1005;
            this.data = new byte[4];
        }

        public uint Scenario
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }
    }
}