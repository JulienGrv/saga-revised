using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.PacketLib;
using Saga.Network.Packets;

namespace Saga.Packets
{

    internal class SMSG_QUESTCANCEL : RelayPacket
    {
        public SMSG_QUESTCANCEL()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0704;
            this.data = new byte[8];
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte CancelReason
        {
            set { this.data[4] = value; }
        }

    }


}
