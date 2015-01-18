using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.PacketLib;
using System.IO;
using Saga.PacketLib;
using Saga.Network.Packets;

namespace Saga.Packets
{

    [CLSCompliant(false)]
    public class SMSG_SCENARIOEVENT : RelayPacket
    {
        public SMSG_SCENARIOEVENT()
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
