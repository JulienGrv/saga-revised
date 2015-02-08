using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This function is sent by the player to indicate he/she has updated her
    /// stats.
    /// </remarks>
    /// <id>
    /// 0309
    /// </id>
    internal class CMSG_STATUPDATE : RelayPacket
    {
        public CMSG_STATUPDATE()
        {
            this.data = new byte[8];
        }

        public ushort Strength
        {
            get { return BitConverter.ToUInt16(this.data, 0); }
        }

        public ushort Dextericty
        {
            get { return BitConverter.ToUInt16(this.data, 2); }
        }

        public ushort Concentration
        {
            get { return BitConverter.ToUInt16(this.data, 6); }
        }

        public ushort Intellect
        {
            get { return BitConverter.ToUInt16(this.data, 4); }
        }

        public ushort PointsLeft
        {
            get { return BitConverter.ToUInt16(this.data, 8); }
        }

        #region Conversions

        public static explicit operator CMSG_STATUPDATE(byte[] p)
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

            CMSG_STATUPDATE pkt = new CMSG_STATUPDATE();
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