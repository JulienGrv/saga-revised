using Saga.Network.Packets;
using System;
using System.Net;

namespace Saga.Packets
{
    public class SMSG_NETWORKADRESSIP : RelayPacket
    {
        public SMSG_NETWORKADRESSIP()
        {
            this.data = new byte[4];
            this.Cmd = 0x0401;
            this.Id = 0xFF01;
        }

        public IPAddress ConnectionFrom
        {
            set
            {
                Array.Copy(value.GetAddressBytes(), 0, this.data, 0, 4);
            }
        }
    }
}