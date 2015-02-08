using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client when he or she wishes to
    /// enchant their armor using a supplement stone.
    /// </remarks>
    /// <id>
    /// 051B
    /// </id>
    internal class CMSG_USESUPPLEMENTSTONE : RelayPacket
    {
        public CMSG_USESUPPLEMENTSTONE()
        {
            this.data = new byte[4];
        }

        public byte IventoryId
        {
            get { return this.data[0]; }
        }

        public byte Container
        {
            get { return this.data[1]; }
        }

        public byte ContainerSlot
        {
            get { return this.data[2]; }
        }

        public byte EnchantmentSlot
        {
            get { return this.data[3]; }
        }

        #region Conversions

        public static explicit operator CMSG_USESUPPLEMENTSTONE(byte[] p)
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

            CMSG_USESUPPLEMENTSTONE pkt = new CMSG_USESUPPLEMENTSTONE();
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