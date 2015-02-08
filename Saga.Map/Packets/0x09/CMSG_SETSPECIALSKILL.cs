using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent when setting a special skill from the specified
    /// slot. This packet can be sent during the skillmaster.
    /// </remarks>
    /// <id>
    /// 090D
    /// </id>
    internal class CMSG_SETSPECIALSKILL : RelayPacket
    {
        public CMSG_SETSPECIALSKILL()
        {
            this.data = new byte[9];
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

        public static explicit operator CMSG_SETSPECIALSKILL(byte[] p)
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

            CMSG_SETSPECIALSKILL pkt = new CMSG_SETSPECIALSKILL();
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