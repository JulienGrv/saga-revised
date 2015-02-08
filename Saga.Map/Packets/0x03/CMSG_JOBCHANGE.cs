using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send by the player as a request to change jobs.
    /// It describes information like which weapon to change on what slot.
    /// How to rename the weapon, if to change the weapon, to which job you
    /// want to change.
    /// </remarks>
    /// <id>
    /// 0307
    /// </id>
    internal class CMSG_CHANGEJOB : RelayPacket
    {
        public CMSG_CHANGEJOB()
        {
            this.data = new byte[5];
        }

        public byte Job
        {
            get { return this.data[0]; }
        }

        public byte ChangeWeapon
        {
            get { return this.data[1]; }
        }

        public byte WeaponSlot
        {
            get { return this.data[2]; }
        }

        public ushort PostFix
        {
            get { return BitConverter.ToUInt16(this.data, 3); }
        }

        #region Conversions

        public static explicit operator CMSG_CHANGEJOB(byte[] p)
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

            CMSG_CHANGEJOB pkt = new CMSG_CHANGEJOB();
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