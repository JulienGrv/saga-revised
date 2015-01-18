using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.PacketLib;
using Saga.Network.Packets;

namespace Saga.Packets
{

    [CLSCompliant(false)]
    public class SMSG_WEAPONUNLOCK : RelayPacket
    {
        public SMSG_WEAPONUNLOCK()
        {
            this.Cmd = 0x0601;
            this.Id = 0x051B;
            this.data = new byte[] {
            	0x02, 	0x4D, 	0xAD, 	0x01, 	0x00, 	0x00, 	0x00, 	0x00, 	0x00, 	0x65, 	0x00, 	0x00, 	0x00, 	0x00, 	0x00, 	0x00,
	            0x00
            };
        }
    }
}
