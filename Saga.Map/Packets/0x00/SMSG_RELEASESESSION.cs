using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    internal class SMSG_RELEASESESSION : RelayPacket
    {
        public SMSG_RELEASESESSION()
        {
            this.Cmd = 0x0701;
            this.Id = 0x000B;
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