using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent as a result of moving your weapon. In this context
    /// to move your weapon it does not mean, to swap the primairy with seccondairy weapon,
    /// no it means to change the index of the primairy or seccondairy weapon.
    /// </remarks>
    /// <id>
    /// 050B
    /// </id>
    internal class CMSG_WEAPONMOVE : RelayPacket
    {
        public CMSG_WEAPONMOVE()
        {
            this.data = new byte[1];
        }

        public byte Index
        {
            get { return this.data[0]; }
        }

        public sbyte WeaponSlot
        {
            get { return (sbyte)this.data[1]; }
        }

        public byte Slot
        {
            get { return this.data[2]; }
        }

        #region Conversions

        public static explicit operator CMSG_WEAPONMOVE(byte[] p)
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

            CMSG_WEAPONMOVE pkt = new CMSG_WEAPONMOVE();
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