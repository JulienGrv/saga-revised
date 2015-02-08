using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_TRADERESULT2 : RelayPacket
    {
        public SMSG_TRADERESULT2()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0809;
            this.data = new byte[4];
        }

        public uint Reason
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}