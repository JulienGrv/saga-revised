using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent when the user confirms to trade
    /// her goods against the selected goods.
    /// </remarks>
    /// <id>
    /// 060D
    /// </id>
    internal class CMSG_SUPPLYEXCHANGE : RelayPacket
    {
        public CMSG_SUPPLYEXCHANGE()
        {
            this.data = new byte[5];
        }

        public uint ActorId
        {
            get { return BitConverter.ToUInt32(this.data, 0); }
        }

        public byte ButtonId
        {
            get { return this.data[4]; }
        }

        #region Conversions

        public static explicit operator CMSG_SUPPLYEXCHANGE(byte[] p)
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

            CMSG_SUPPLYEXCHANGE pkt = new CMSG_SUPPLYEXCHANGE();
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