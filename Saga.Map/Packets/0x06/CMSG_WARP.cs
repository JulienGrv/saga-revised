using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent when the user has selected a warp location.
    /// </remarks>
    /// <id>
    /// 0617
    /// </id>
    internal class CMSG_WARP : RelayPacket
    {
        public CMSG_WARP()
        {
            this.data = new byte[2];
        }

        public ushort WarpId
        {
            get { return this.data[0]; }
        }

        #region Conversions

        public static explicit operator CMSG_WARP(byte[] p)
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

            CMSG_WARP pkt = new CMSG_WARP();
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