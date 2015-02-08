using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client as a result to change the weapons auge for
    /// the specified weapon in your weaponary-list.
    /// </remarks>
    /// <id>
    /// 0517
    /// </id>
    internal class CMSG_WEAPONAUGE : RelayPacket
    {
        public CMSG_WEAPONAUGE()
        {
            this.data = new byte[3];
        }

        public byte Index
        {
            get { return this.data[0]; }
        }

        public byte WeaponSlot
        {
            get { return this.data[1]; }
        }

        public byte Slot
        {
            get { return this.data[2]; }
        }

        #region Conversions

        public static explicit operator CMSG_WEAPONAUGE(byte[] p)
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

            CMSG_WEAPONAUGE pkt = new CMSG_WEAPONAUGE();
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