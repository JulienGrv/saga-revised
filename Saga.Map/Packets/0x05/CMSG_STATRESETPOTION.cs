using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent as indication by the player that he/she wishes to reset
    /// their assigned stat points.
    /// </remarks>
    /// <id>
    /// 051A
    /// </id>
    internal class CMSG_STATRESETPOTION : RelayPacket
    {
        public CMSG_STATRESETPOTION()
        {
            this.data = new byte[1];
        }

        public byte SlotId
        {
            get { return this.data[0]; }
        }

        #region Conversions

        public static explicit operator CMSG_STATRESETPOTION(byte[] p)
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

            CMSG_STATRESETPOTION pkt = new CMSG_STATRESETPOTION();
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