using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by an player to indicate he/she
    /// is agreeing with the trade for the last time. When both parties agree
    /// the trades will be swapped.
    /// </remarks>
    /// <id>
    /// 0806
    /// </id>
    internal class CMSG_TRADECONFIRM : RelayPacket
    {
        public CMSG_TRADECONFIRM()
        {
            this.data = new byte[0];
        }

        #region Conversions

        public static explicit operator CMSG_TRADECONFIRM(byte[] p)
        {
            /*
            // Creates a new byte with the length of data
            // plus 4. The first size bytes are used like
            // [PacketSize][PacketId][PacketBody]
            //
            // Where Packet Size equals the length of the
            // Packet body, Packet Identifier, Packet Size
            // Container.
            */

            CMSG_TRADECONFIRM pkt = new CMSG_TRADECONFIRM();
            pkt.data = new byte[p.Length - 14];
            pkt.session = BitConverter.ToUInt32(p, 2);
            Array.Copy(p, 6, pkt.cmd, 0, 2);
            Array.Copy(p, 12, pkt.id, 0, 2);
            Array.Copy(p, 14, pkt.data, 0, p.Length - 14);
            return pkt;
        }

        #endregion Conversions
    }
}