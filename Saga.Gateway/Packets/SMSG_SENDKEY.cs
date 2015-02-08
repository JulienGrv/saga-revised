using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Gateway
{
    public class SMSG_SENDKEY : Packet
    {
        public SMSG_SENDKEY()
        {
            this.data = new byte[550];
            this.Id = 0x0201;
        }

        public byte[] Key
        {
            get
            {
                return new byte[20];
            }
            set
            {
                if (value.Length < data.Length - 256)
                    Array.Copy(value, 0, data, 256, value.Length);
            }
        }

        public byte Collumns
        {
            get
            {
                return this.data[512];
            }
            set
            {
                this.data[512] = value;
            }
        }

        public byte Rounds
        {
            get
            {
                return this.data[516];
            }
            set
            {
                this.data[516] = value;
            }
        }

        public byte Direction
        {
            get
            {
                return this.data[520];
            }
            set
            {
                this.data[520] = value;
            }
        }
    }
}