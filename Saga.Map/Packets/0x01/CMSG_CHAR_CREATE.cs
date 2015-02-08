using Saga.Network;
using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Shared.PacketLib.Map
{
    internal class CMSG_CHAR_CREATE : RelayPacket
    {
        public CMSG_CHAR_CREATE()
        {
            this.data = new byte[76];
            this.Id = 0x0105;
        }

        public uint UserId
        {
            get
            {
                return BitConverter.ToUInt32(data, 0);
            }
        }

        public string Name
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 4, 34);
            }
        }

        public byte Eye
        {
            get
            {
                return this.data[39];
            }
        }

        public byte EyeColor
        {
            get
            {
                return this.data[40];
            }
        }

        public byte EyeBrowse
        {
            get
            {
                return this.data[41];
            }
        }

        public byte Hair
        {
            get
            {
                return this.data[42];
            }
        }

        public byte HairColor
        {
            get
            {
                return this.data[43];
            }
        }

        public string WeaponName
        {
            get
            {
                return Encoding.BigEndianUnicode.GetString(this.data, 50, 24).TrimEnd((char)0);
            }
        }

        public ushort WeaponAffix
        {
            get
            {
                return BitConverter.ToUInt16(data, 74);
            }
        }

        #region Conversions

        public static explicit operator CMSG_CHAR_CREATE(byte[] p)
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

            CMSG_CHAR_CREATE pkt = new CMSG_CHAR_CREATE();
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