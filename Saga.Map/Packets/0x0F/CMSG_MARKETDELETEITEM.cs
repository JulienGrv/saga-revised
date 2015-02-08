using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to the player when a person want to delete an
    /// item from the market. Note that he/she can only delete his own items.
    /// </remarks>
    /// <id>
    /// 0F05
    /// </id>
    internal class CMSG_MARKETDELETEITEM : RelayPacket
    {
        public CMSG_MARKETDELETEITEM()
        {
            this.data = new byte[4];
        }

        public uint ItemId
        {
            get
            {
                return BitConverter.ToUInt32(this.data, 0);
            }
        }

        #region Conversions

        public static explicit operator CMSG_MARKETDELETEITEM(byte[] p)
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

            CMSG_MARKETDELETEITEM pkt = new CMSG_MARKETDELETEITEM();
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