using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent as indication by the player that he/she wishes sort their
    /// storage list (Items in warehouse).
    /// </remarks>
    /// <id>
    /// 0505
    /// </id>
    internal class CMSG_SORTSTORAGELIST : RelayPacket
    {
        public CMSG_SORTSTORAGELIST()
        {
            this.data = new byte[1];
        }

        public byte SortType
        {
            get { return this.data[0]; }
        }

        #region Conversions

        public static explicit operator CMSG_SORTSTORAGELIST(byte[] p)
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

            CMSG_SORTSTORAGELIST pkt = new CMSG_SORTSTORAGELIST();
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