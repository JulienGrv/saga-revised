using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Login
{
    public class CMSG_SESSIONREQUEST : RelayPacket
    {
        public CMSG_SESSIONREQUEST()
        {
            this.data = new byte[0];
        }

        #region Conversions

        public static explicit operator CMSG_SESSIONREQUEST(byte[] p)
        {
            CMSG_SESSIONREQUEST pkt = new CMSG_SESSIONREQUEST();
            pkt.data = new byte[p.Length - 10];
            pkt.session = BitConverter.ToUInt32(p, 2);
            Array.Copy(p, 6, pkt.id, 0, 2);
            Array.Copy(p, 10, pkt.data, 0, p.Length - 10);
            return pkt;
        }

        #endregion Conversions
    }
}