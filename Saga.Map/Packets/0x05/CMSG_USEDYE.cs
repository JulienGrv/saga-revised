using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send by the player as a result of he/she attempts to use
    /// a dye item.
    /// </remarks>
    /// <id>
    /// 0519
    /// </id>
    internal class CMSG_USEDYEITEM : RelayPacket
    {
        public CMSG_USEDYEITEM()
        {
            this.data = new byte[7];
        }

        public byte Index
        {
            get { return this.data[0]; }
        }

        public byte Container
        {
            get { return this.data[1]; }
        }

        public byte Slot
        {
            get { return this.data[2]; }
        }

        #region Conversions

        public static explicit operator CMSG_USEDYEITEM(byte[] p)
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

            CMSG_USEDYEITEM pkt = new CMSG_USEDYEITEM();
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