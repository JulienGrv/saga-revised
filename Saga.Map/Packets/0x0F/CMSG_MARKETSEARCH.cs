using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client when he/she requests a new
    /// search.
    /// </remarks>
    /// <id>
    /// 0F01
    /// </id>
    internal class CMSG_MARKETSEARCH : RelayPacket
    {
        public CMSG_MARKETSEARCH()
        {
            this.data = new byte[42];
        }

        public ushort Unknown
        {
            get
            {
                return BitConverter.ToUInt16(this.data, 0);
            }
        }

        public byte ItemClass
        {
            get
            {
                return this.data[2];
            }
        }

        public byte ItemType
        {
            get
            {
                return this.data[3];
            }
        }

        public byte GetSearchMode
        {
            get
            {
                return this.data[4];
            }
        }

        public string SearchString
        {
            get
            {
                return UnicodeEncoding.Unicode.GetString(this.data, 5, 32).TrimEnd((char)0);
            }
        }

        public byte MinCLv
        {
            get
            {
                return this.data[39];
            }
        }

        public byte MaxCLv
        {
            get
            {
                return this.data[40];
            }
        }

        public byte Unknown2
        {
            get
            {
                return this.data[41];
            }
        }

        #region Conversions

        public static explicit operator CMSG_MARKETSEARCH(byte[] p)
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

            CMSG_MARKETSEARCH pkt = new CMSG_MARKETSEARCH();
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