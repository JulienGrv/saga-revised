using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent when removing a special skill from the specified
    /// slot. This packet can be sent during the skillmaster.
    /// </remarks>
    /// <id>
    /// 090E
    /// </id>
    internal class CMSG_REMOVESPECIALSKILL : RelayPacket
    {
        public CMSG_REMOVESPECIALSKILL()
        {
            this.data = new byte[5];
        }

        public byte Slot
        {
            get { return this.data[0]; }
        }

        public uint SkillID
        {
            get { return BitConverter.ToUInt32(this.data, 1); }
        }

        #region Conversions

        public static explicit operator CMSG_REMOVESPECIALSKILL(byte[] p)
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

            CMSG_REMOVESPECIALSKILL pkt = new CMSG_REMOVESPECIALSKILL();
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