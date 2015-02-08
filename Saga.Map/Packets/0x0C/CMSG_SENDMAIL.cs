using Saga.Network;
using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sent by the user to indicate he or she wants
    /// to wants to send a new mail.
    /// </remarks>
    /// <id>
    /// 0C02
    /// </id>
    internal class CMSG_SENDMAIL : RelayPacket
    {
        public CMSG_SENDMAIL()
        {
        }

        public string Name
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 0, 34).TrimEnd('\0');
            }
        }

        public byte HasItem
        {
            get
            {
                return this.data[34];
            }
        }

        public byte Slot
        {
            get
            {
                return this.data[35];
            }
        }

        public byte StackCount
        {
            get
            {
                return this.data[36];
            }
        }

        public uint Zeny
        {
            get
            {
                return BitConverter.ToUInt32(this.data, 40);
            }
        }

        public string Topic
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 44, 38).TrimEnd('\0');
            }
        }

        public string Content
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 86, 400).TrimEnd('\0');
            }
        }

        #region Conversions

        public static explicit operator CMSG_SENDMAIL(byte[] p)
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

            CMSG_SENDMAIL pkt = new CMSG_SENDMAIL();
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