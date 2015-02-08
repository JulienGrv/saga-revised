using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to the user to indicate the
    /// party mode of the users party is changed.
    /// </remarks>
    /// <id>
    /// 0E06
    /// </id>
    internal class CMSG_PARTYMODE : RelayPacket
    {
        public CMSG_PARTYMODE()
        {
            this.data = new byte[6];
        }

        public byte LootShare
        {
            get
            {
                return this.data[0];
            }
        }

        public byte ExpShare
        {
            get
            {
                return this.data[1];
            }
        }

        public byte Unknown
        {
            get
            {
                return this.data[2];
            }
        }

        public uint LootMaster
        {
            get
            {
                return BitConverter.ToUInt32(this.data, 3);
            }
        }

        #region Conversions

        public static explicit operator CMSG_PARTYMODE(byte[] p)
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

            CMSG_PARTYMODE pkt = new CMSG_PARTYMODE();
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