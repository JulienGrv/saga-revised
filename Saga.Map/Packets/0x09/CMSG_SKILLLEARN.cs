using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client after
    /// invoking a new skillbook item. The index is the
    /// index of the item in the inventory.
    /// </remarks>
    /// <id>
    /// 090C
    /// </id>
    internal class CMSG_SKILLLEARN : RelayPacket
    {
        public CMSG_SKILLLEARN()
        {
            this.data = new byte[1];
        }

        public byte Index
        {
            get { return this.data[0]; }
        }

        #region Conversions

        public static explicit operator CMSG_SKILLLEARN(byte[] p)
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

            CMSG_SKILLLEARN pkt = new CMSG_SKILLLEARN();
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