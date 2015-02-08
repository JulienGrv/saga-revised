using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Login
{
    public class SMSG_LOGIN : RelayPacket
    {
        public SMSG_LOGIN()
        {
            this.Cmd = 0x0501;
            this.Id = 0x0001;
            this.data = new byte[6];
        }

        public byte GmLevel
        {
            set
            {
                this.data[0] = value;
            }
        }

        public byte Gender
        {
            set
            {
                this.data[1] = value;
            }
        }

        public uint CharacterId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 2, 4);
            }
        }
    }
}