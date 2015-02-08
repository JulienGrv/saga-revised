using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send by the player to indicate he/she wished to
    /// discard the specified item. Note Quest Items cannot be discarded
    /// if the quest is active.
    /// </remarks>
    /// <id>
    /// 0507
    /// </id>
    internal class CMSG_DELETEITEM : RelayPacket
    {
        public CMSG_DELETEITEM()
        {
            this.data = new byte[7];
        }

        public byte Container
        {
            get { return this.data[0]; }
        }

        public byte Index
        {
            get { return this.data[1]; }
        }

        public uint ItemId
        {
            get { return BitConverter.ToUInt32(this.data, 2); }
        }

        public byte Amount
        {
            get { return this.data[6]; }
        }

        #region Conversions

        public static explicit operator CMSG_DELETEITEM(byte[] p)
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

            CMSG_DELETEITEM pkt = new CMSG_DELETEITEM();
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