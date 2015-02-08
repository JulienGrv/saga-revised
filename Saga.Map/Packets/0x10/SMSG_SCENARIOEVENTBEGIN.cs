using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_SCENARIOEVENTBEGIN : RelayPacket
    {
        public SMSG_SCENARIOEVENTBEGIN()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1007;
            this.data = new byte[8];
        }

        public uint Event
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

        public uint ActorId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4);
            }
        }
    }
}