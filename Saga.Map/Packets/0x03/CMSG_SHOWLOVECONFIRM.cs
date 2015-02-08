using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send by the player as a confirms a showlove request
    /// female/male
    /// </remarks>
    /// <id>
    /// 030D
    /// </id>
    internal class CMSG_SHOWLOVECONFIRM : RelayPacket
    {
        public CMSG_SHOWLOVECONFIRM()
        {
            this.data = new byte[0];
        }

        public byte Response
        {
            get { return this.data[0]; }
        }

        #region Conversions

        public static explicit operator CMSG_SHOWLOVECONFIRM(byte[] p)
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

            CMSG_SHOWLOVECONFIRM pkt = new CMSG_SHOWLOVECONFIRM();
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