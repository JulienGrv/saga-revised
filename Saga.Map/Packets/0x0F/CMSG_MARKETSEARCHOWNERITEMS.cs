using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client when he/she requests his/hers own
    /// items to retrieve. This packet is normally only retrieved once.
    /// search.
    /// </remarks>
    /// <id>
    /// 0F03
    /// </id>
    internal class CMSG_MARKETSEARCHOWNERITEMS : RelayPacket
    {
        public CMSG_MARKETSEARCHOWNERITEMS()
        {
            this.data = new byte[0];
        }

        #region Conversions

        public static explicit operator CMSG_MARKETSEARCHOWNERITEMS(byte[] p)
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

            CMSG_MARKETSEARCHOWNERITEMS pkt = new CMSG_MARKETSEARCHOWNERITEMS();
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