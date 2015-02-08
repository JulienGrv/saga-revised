using Saga.Network.Packets;
using System;

namespace Saga.Authentication.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is invoked when an client selects a other character
    /// from the Characterlist
    ///
    /// CharacterId containts the Unique represented id of the character
    /// Channel contains the selected channel Id
    ///
    /// Last updated on Friday 26, okt 2007.
    /// </remarks>
    public class CMSG_SELECTCHARACTER : RelayPacket
    {
        public CMSG_SELECTCHARACTER()
        {
            this.data = new byte[5];
        }

        public byte Index
        {
            get
            {
                return data[0];
            }
        }

        public byte Channel
        {
            get
            {
                return data[4];
            }
        }

        #region Conversions

        public static explicit operator CMSG_SELECTCHARACTER(byte[] p)
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

            CMSG_SELECTCHARACTER pkt = new CMSG_SELECTCHARACTER();
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