using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is used by the by the client as a request to repair the
    /// specified equipements or weaponary.
    /// </remarks>
    /// <id>
    /// 0508
    /// </id>
    internal class CMSG_REPAIRITEM : RelayPacket
    {
        public CMSG_REPAIRITEM()
        {
            this.data = new byte[1];
        }

        public byte Amount
        {
            get { return this.data[0]; }
        }

        private int offset = 1;

        public void ReadEquipmentInfo(out byte container, out byte index)
        {
            index = this.data[this.offset];
            container = this.data[this.offset + 1];
            this.offset += 2;
        }

        #region Conversions

        public static explicit operator CMSG_REPAIRITEM(byte[] p)
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

            CMSG_REPAIRITEM pkt = new CMSG_REPAIRITEM();
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