using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    public class SMSG_CHARACTERLOGIN : RelayPacket
    {
        public SMSG_CHARACTERLOGIN()
        {
            this.data = new byte[8];
            this.Cmd = 0x0701;
            this.Id = 0xFF02;
        }

        public uint CharacterId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

        public uint Session
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4);
            }
        }
    }
}