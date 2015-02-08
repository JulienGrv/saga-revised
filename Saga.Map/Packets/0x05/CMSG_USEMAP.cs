using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send by the player as a result of he/she attempts to use
    /// a map to reveal a hidden section of the map.
    /// </remarks>
    /// <id>
    /// 0517
    /// </id>
    internal class CMSG_USEMAP : RelayPacket
    {
        public CMSG_USEMAP()
        {
            this.data = new byte[7];
        }

        public byte Index
        {
            get { return this.data[0]; }
        }

        #region Conversions

        public static explicit operator CMSG_USEMAP(byte[] p)
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

            CMSG_USEMAP pkt = new CMSG_USEMAP();
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