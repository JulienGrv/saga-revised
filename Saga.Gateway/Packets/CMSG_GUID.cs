using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Gateway
{
    public class CMSG_GUID : Packet
    {
        public CMSG_GUID()
        {
            this.data = new byte[20];
            this.Id = 0x0202;
        }

        public string Key
        {
            get
            {
                return PacketLib.Other.Conversions.ByteToHexString(this.data);
            }
            set
            {
                byte[] tmp = PacketLib.Other.Conversions.HexStringToBytes(value);
                System.Diagnostics.Trace.Assert(tmp.Length == 20, "Incorrect key length\r\nKey:" + value);
                Array.Copy(tmp, 0, this.data, 0, 20);
            }
        }

        public static explicit operator byte[](CMSG_GUID p)
        {
            byte[] tmp = new byte[p.Size];
            Array.Copy(BitConverter.GetBytes(p.Size), 0, tmp, 0, 2);
            Array.Copy(BitConverter.GetBytes(p.session), 0, tmp, 2, 4);
            Array.Copy(p.id, 0, tmp, 6, 2);
            Array.Copy(p.data, 0, tmp, 10, p.data.Length);
            return tmp;
        }

        public static explicit operator CMSG_GUID(byte[] p)
        {
            CMSG_GUID pkt = new CMSG_GUID();
            pkt.data = new byte[p.Length - 10];
            pkt.session = BitConverter.ToUInt32(p, 2);
            Array.Copy(p, 6, pkt.id, 0, 2);
            Array.Copy(p, 10, pkt.data, 0, p.Length - 10);
            return pkt;
        }
    }
}