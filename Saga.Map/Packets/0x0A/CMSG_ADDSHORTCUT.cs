using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is depreciated since CB2. The client still preserves the packet however.
    /// due this reason this function is purely implacated for show.
    /// </remarks>
    /// <id>
    /// 0A01
    /// </id>
    internal class CMSG_ADDSHORTCUT : RelayPacket
    {
        public CMSG_ADDSHORTCUT()
        {
            this.data = new byte[11];
        }

        public byte ShortCutType
        {
            get { return this.data[0]; }
        }

        public byte Slot
        {
            get { return this.data[1]; }
        }

        public uint ShortcutId
        {
            get { return BitConverter.ToUInt32(this.data, 2); }
        }

        #region Conversions

        public static explicit operator CMSG_ADDSHORTCUT(byte[] p)
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

            CMSG_ADDSHORTCUT pkt = new CMSG_ADDSHORTCUT();
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