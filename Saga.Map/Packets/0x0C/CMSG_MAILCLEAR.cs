using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sent by the user to indicate he or she wants to cancel sending the pending
    /// and retrieve the items. The items are sended back using a new mail item towards the
    /// user.
    /// </remarks>
    /// <id>
    /// 0C0A
    /// </id>
    internal class CMSG_MAILCLEAR : RelayPacket
    {
        public CMSG_MAILCLEAR()
        {
            this.data = new byte[4];
        }

        public uint MailId
        {
            get { return BitConverter.ToUInt32(this.data, 0); }
        }

        #region Conversions

        public static explicit operator CMSG_MAILCLEAR(byte[] p)
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

            CMSG_MAILCLEAR pkt = new CMSG_MAILCLEAR();
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