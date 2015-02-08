using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Login
{
    public class SMSG_FINDCHARACTERDETAILS : RelayPacket
    {
        public SMSG_FINDCHARACTERDETAILS()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0003;
            this.data = new byte[4];
        }

        public uint CharacterId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }
    }
}