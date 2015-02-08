using Saga.Network.Packets;
using System;

namespace Saga.Shared.PacketLib.Map
{
    internal class CMSG_CHAR_GET : RelayPacket
    {
        /*
        // This packet is invoked when an client selects a other character
        // from the Characterlist
        //
        // Index contains an zero-based offset that specifies the character
        //
        // Last updated on Friday 26, okt 2007.
        */

        public CMSG_CHAR_GET()
        {
            this.data = new byte[1];
        }

        public byte Index
        {
            get
            {
                return this.data[0];
            }
            set
            {
                this.data[0] = value;
            }
        }

        #region Conversions

        public static explicit operator CMSG_CHAR_GET(byte[] p)
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

            CMSG_CHAR_GET pkt = new CMSG_CHAR_GET();
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