using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sent by the user to indicate he or she wants to see the full contents
    /// of the requested mail found in their inbox
    /// </remarks>
    /// <id>
    /// 0C03
    /// </id>
    internal class CMSG_GETMAIL : RelayPacket
    {
        public CMSG_GETMAIL()
        {
            this.data = new byte[4];
        }

        public uint MailId
        {
            get { return BitConverter.ToUInt32(this.data, 0); }
        }

        #region Conversions

        public static explicit operator CMSG_GETMAIL(byte[] p)
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

            CMSG_GETMAIL pkt = new CMSG_GETMAIL();
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