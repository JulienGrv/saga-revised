using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client to indicate he or she want
    /// to use the item on the submenu.
    /// </remarks>
    /// <id>
    /// 0604
    /// </id>
    internal class CMSG_NPCMENU : RelayPacket
    {
        /// <summary>
        /// Initialized a new CMSG_NPCMENU packet
        /// </summary>
        public CMSG_NPCMENU()
        {
            this.data = new byte[2];
        }

        /// <summary>
        /// Get's the selected button id (usally instance of DialogType)
        /// </summary>
        public byte ButtonID
        {
            get { return this.data[0]; }
        }

        /// <summary>
        /// Get's the selected menuid
        /// </summary>
        public byte MenuID
        {
            get { return this.data[1]; }
        }

        #region Conversions

        public static explicit operator CMSG_NPCMENU(byte[] p)
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

            CMSG_NPCMENU pkt = new CMSG_NPCMENU();
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