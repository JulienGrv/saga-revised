using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Gateway
{
    public class CMSG_SENDKEY : Packet
    {
        public CMSG_SENDKEY()
        {
            this.data = new byte[550];
            this.Id = 0x0201;
        }

        public byte[] Key
        {
            get
            {
                byte[] tmp = new byte[16];
                Array.Copy(data, 256, tmp, 0, 16);
                return tmp;
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

        public static explicit operator byte[](CMSG_SENDKEY p)
        {
            byte[] tmp = new byte[p.Size];
            Array.Copy(BitConverter.GetBytes(p.Size), 0, tmp, 0, 2);
            Array.Copy(BitConverter.GetBytes(p.session), 0, tmp, 2, 4);
            Array.Copy(p.id, 0, tmp, 6, 2);
            Array.Copy(p.data, 0, tmp, 10, p.data.Length);
            return tmp;
        }

        public static explicit operator CMSG_SENDKEY(byte[] p)
        {
            CMSG_SENDKEY pkt = new CMSG_SENDKEY();
            pkt.data = new byte[p.Length - 10];
            pkt.session = BitConverter.ToUInt32(p, 2);
            Array.Copy(p, 6, pkt.id, 0, 2);
            Array.Copy(p, 10, pkt.data, 0, p.Length - 10);
            return pkt;
        }
    }
}