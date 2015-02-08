using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client to indicate he or she has confirmed or denied the
    /// as a respons on the question the client asked.
    /// </remarks>
    /// <id>
    /// 0605
    /// </id>
    internal class CMSG_PERSONALREQUEST : RelayPacket
    {
        public CMSG_PERSONALREQUEST()
        {
            this.data = new byte[5];
        }

        public uint Unknown
        {
            get { return BitConverter.ToUInt32(this.data, 0); }
        }

        public byte Result
        {
            get { return this.data[4]; }
        }

        #region Conversions

        public static explicit operator CMSG_PERSONALREQUEST(byte[] p)
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

            CMSG_PERSONALREQUEST pkt = new CMSG_PERSONALREQUEST();
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