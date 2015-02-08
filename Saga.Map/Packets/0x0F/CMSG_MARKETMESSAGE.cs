using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client when he/she wants to set the comment
    /// </remarks>
    /// <id>
    /// 0F06
    /// </id>
    internal class CMSG_MARKETMESSAGE : RelayPacket
    {
        public CMSG_MARKETMESSAGE()
        {
            this.data = new byte[4];
        }

        public string Comment
        {
            get
            {
                return UnicodeEncoding.Unicode.GetString(this.data, 1, this.data.Length - 1).TrimEnd((char)0);
            }
        }

        #region Conversions

        public static explicit operator CMSG_MARKETMESSAGE(byte[] p)
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

            CMSG_MARKETMESSAGE pkt = new CMSG_MARKETMESSAGE();
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