using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_SCENARIOEVENTEND : RelayPacket
    {
        public SMSG_SCENARIOEVENTEND()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1009;
            this.data = new byte[4];
        }

        public uint ActorId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }
    }
}