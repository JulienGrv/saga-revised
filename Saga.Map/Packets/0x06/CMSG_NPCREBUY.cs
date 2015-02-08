using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client to indicate he or she want
    /// to rebuy the item they sold before.
    /// </remarks>
    /// <id>
    /// 0613
    /// </id>
    internal class CMSG_NPCREBUY : RelayPacket
    {
        public CMSG_NPCREBUY()
        {
            this.data = new byte[2];
        }

        public byte Container
        {
            get { return this.data[0]; }
        }

        public byte Index
        {
            get { return this.data[1]; }
        }

        public uint Amount
        {
            get { return this.data[3]; }
        }

        #region Conversions

        public static explicit operator CMSG_NPCREBUY(byte[] p)
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

            CMSG_NPCREBUY pkt = new CMSG_NPCREBUY();
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