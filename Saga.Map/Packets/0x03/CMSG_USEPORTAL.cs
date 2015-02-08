using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send by the player as a result of the player using a
    /// portal. And wished to be warped to a other map.
    /// </remarks>
    /// <id>
    /// 0306
    /// </id>
    internal class CMSG_USEPORTAL : RelayPacket
    {
        public CMSG_USEPORTAL()
        {
            this.data = new byte[0];
        }

        public byte PortalID
        {
            get { return (byte)(this.data[0] - 1); }
        }

        #region Conversions

        public static explicit operator CMSG_USEPORTAL(byte[] p)
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

            CMSG_USEPORTAL pkt = new CMSG_USEPORTAL();
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