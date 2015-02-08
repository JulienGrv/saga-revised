using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Login
{
    public class SMSG_FINDCHARACTERS : RelayPacket
    {
        public SMSG_FINDCHARACTERS()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0002;
            this.data = new byte[4];
        }

        public uint UserId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }
    }
}