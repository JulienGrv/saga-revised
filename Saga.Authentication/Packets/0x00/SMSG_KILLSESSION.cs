using Saga.Network.Packets;
using System;

namespace Saga.Authentication.Packets
{
    public class SMSG_KILLSESSION : RelayPacket
    {
        public SMSG_KILLSESSION()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0004;
            this.data = new byte[4];
        }

        public uint Session
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }
    }
}