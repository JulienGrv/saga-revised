using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client when he/she cancels/interups
    /// her casted skill. For example when you move and your skill is
    /// casted you'll cancel your skills
    /// </remarks>
    /// <id>
    /// 0904
    /// </id>
    internal class CMSG_SKILLCASTCANCEL : RelayPacket
    {
        public CMSG_SKILLCASTCANCEL()
        {
            this.data = new byte[5];
        }

        public byte SkillType
        {
            get { return this.data[0]; }
        }

        public uint SkillID
        {
            get { return BitConverter.ToUInt32(this.data, 1); }
        }

        #region Conversions

        public static explicit operator CMSG_SKILLCASTCANCEL(byte[] p)
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

            CMSG_SKILLCASTCANCEL pkt = new CMSG_SKILLCASTCANCEL();
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