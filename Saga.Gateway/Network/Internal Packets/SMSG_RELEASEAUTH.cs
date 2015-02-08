using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    public class SMSG_RELEASEAUTH : RelayPacket
    {
        public SMSG_RELEASEAUTH()
        {
            this.data = new byte[4];
            this.Cmd = 0x0401;
            this.Id = 0xFF02;
        }

        public uint Session
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }
    }

    public class SMSG_HEADERTOKEN : RelayPacket
    {
        public SMSG_HEADERTOKEN()
        {
            this.data = new byte[0];
            this.Cmd = 0x0401;
            this.Id = 0xFFFF;
        }
    }
}