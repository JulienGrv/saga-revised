using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client when he/she requests his/hers own
    /// comment in the auction house.
    /// search.
    /// </remarks>
    /// <id>
    /// 0F08
    /// </id>
    internal class CMSG_MARKETOWNERCOMMENT : RelayPacket
    {
        public CMSG_MARKETOWNERCOMMENT()
        {
            this.data = new byte[0];
        }

        #region Conversions

        public static explicit operator CMSG_MARKETOWNERCOMMENT(byte[] p)
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

            CMSG_MARKETOWNERCOMMENT pkt = new CMSG_MARKETOWNERCOMMENT();
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