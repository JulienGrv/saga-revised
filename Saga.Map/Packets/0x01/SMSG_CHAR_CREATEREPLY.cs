using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Map
{
    internal class SMSG_CHAR_CREATE : RelayPacket
    {
        public SMSG_CHAR_CREATE()
        {
            this.data = new byte[5];
            this.Id = 0x0103;
        }

        public byte Result
        {
            set
            {
                this.data[0] = value;
            }
        }

        public uint CharatcerId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4);
            }
        }
    }
}