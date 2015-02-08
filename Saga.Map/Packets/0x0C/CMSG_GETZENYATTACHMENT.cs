using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sent by the user to indicate he or she wants
    /// to obtain the zeny attachment of the selected mailitem.
    /// </remarks>
    /// <id>
    /// 0C05
    /// </id>
    internal class CMSG_GETZENYATTACHMENT : RelayPacket
    {
        public CMSG_GETZENYATTACHMENT()
        {
            this.data = new byte[4];
        }

        public uint MailId
        {
            get { return BitConverter.ToUInt32(this.data, 0); }
        }

        public uint Zeny
        {
            get { return BitConverter.ToUInt32(this.data, 4); }
        }

        #region Conversions

        public static explicit operator CMSG_GETZENYATTACHMENT(byte[] p)
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

            CMSG_GETZENYATTACHMENT pkt = new CMSG_GETZENYATTACHMENT();
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