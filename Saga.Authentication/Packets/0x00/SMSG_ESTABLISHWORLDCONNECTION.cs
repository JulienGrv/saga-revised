using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    public class SMSG_ESTABLISHWORLDCONNECTION : RelayPacket
    {
        public SMSG_ESTABLISHWORLDCONNECTION()
        {
            this.Cmd = 0x0002;
            this.Id = 0x0002;
            this.data = new byte[10];
        }

        public byte[] IpAddr
        {
            set
            {
                Array.Copy(value, 0, this.data, 0, value.Length);
            }
        }

        public int Port
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 6, 4);
            }
        }
    }
}