using Saga.Network;
using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent as indication by the player that he/she wishes to change
    /// the weapon suffix of the specified weapon.
    /// </remarks>
    /// <id>
    /// 0515
    /// </id>
    internal class CMSG_WEAPONCHANGESUFFIX2 : RelayPacket
    {
        public CMSG_WEAPONCHANGESUFFIX2()
        {
            this.data = new byte[27];
        }

        public byte SlotId
        {
            get { return this.data[0]; }
        }

        public string WeaponName
        {
            get { return SpecialEncoding.Read(ref this.data, 1, 24); }
        }

        public ushort Suffix
        {
            get { return BitConverter.ToUInt16(this.data, 25); }
        }

        #region Conversions

        public static explicit operator CMSG_WEAPONCHANGESUFFIX2(byte[] p)
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

            CMSG_WEAPONCHANGESUFFIX2 pkt = new CMSG_WEAPONCHANGESUFFIX2();
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