using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the player as he/she requests to move a item.
    /// Movement in this context is expressed as Inventory -> Storage and vice versa.
    /// Or Inventory to storage and Vice Versa.
    /// </remarks>
    /// <id>
    /// 050B
    /// </id>
    internal class CMSG_MOVEITEM : RelayPacket
    {
        public CMSG_MOVEITEM()
        {
            this.data = new byte[5];
        }

        public byte Unknown
        {
            get { return this.data[0]; }
        }

        public byte MovementType
        {
            get { return this.data[1]; }
        }

        public byte SourceIndex
        {
            get { return this.data[2]; }
        }

        public byte DestinationIndex
        {
            get { return this.data[3]; }
        }

        public byte Amount
        {
            get { return this.data[4]; }
        }

        #region Conversions

        public static explicit operator CMSG_MOVEITEM(byte[] p)
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

            CMSG_MOVEITEM pkt = new CMSG_MOVEITEM();
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