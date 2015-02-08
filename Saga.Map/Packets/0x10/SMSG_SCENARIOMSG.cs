using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_SCENARIOMSG : RelayPacket
    {
        public SMSG_SCENARIOMSG()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1002;
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