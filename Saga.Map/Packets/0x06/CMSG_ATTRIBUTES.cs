using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client to indicate he or she want
    /// know wants to get some additional target information
    /// about their monster.
    /// </remarks>
    /// <id>
    /// 0608
    /// </id>
    internal class CMSG_ATTRIBUTE : RelayPacket
    {
        public CMSG_ATTRIBUTE()
        {
            this.data = new byte[5];
        }

        public uint ActorID
        {
            get { return BitConverter.ToUInt32(this.data, 0); }
        }

        public byte Unknown
        {
            get { return this.data[4]; }
        }

        #region Conversions

        public static explicit operator CMSG_ATTRIBUTE(byte[] p)
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

            CMSG_ATTRIBUTE pkt = new CMSG_ATTRIBUTE();
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