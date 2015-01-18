using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.PacketLib;
using Saga.Network.Packets;

namespace Saga.Packets
{

    internal class SMSG_TRADEZENY : RelayPacket
    {
        public SMSG_TRADEZENY()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0805;
            this.data = new byte[4];
        }

        public uint Zeny
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}
