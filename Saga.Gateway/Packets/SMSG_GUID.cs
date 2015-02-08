using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Gateway
{
    public class SMSG_GUID : Packet
    {
        public SMSG_GUID()
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
    }
}