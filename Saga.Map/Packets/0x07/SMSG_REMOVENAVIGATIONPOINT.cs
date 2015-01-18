using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.PacketLib;
using Saga.Network.Packets;

namespace Saga.Packets
{

    internal class SMSG_REMOVENAVIGATIONPOINT : RelayPacket
    {
        public SMSG_REMOVENAVIGATIONPOINT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x070C;
            this.data = new byte[4];
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}
