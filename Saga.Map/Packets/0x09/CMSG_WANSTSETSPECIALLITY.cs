using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet indicates that the user should has added a new
    /// special skill. When adding a special skill to your list it
    /// requires you to have 250 rufi.
    ///
    /// If you do not have it your request will be denied using the
    /// responding packt.
    /// </remarks>
    /// <id>
    /// 0911
    /// </id>
    internal class CMSG_WANTSETSPECIALLITY : RelayPacket
    {
        public CMSG_WANTSETSPECIALLITY()
        {
            this.data = new byte[6];
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

        public static explicit operator CMSG_WANTSETSPECIALLITY(byte[] p)
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

            CMSG_WANTSETSPECIALLITY pkt = new CMSG_WANTSETSPECIALLITY();
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