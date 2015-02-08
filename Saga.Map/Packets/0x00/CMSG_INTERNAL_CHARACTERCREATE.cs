using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    internal class CMSG_INTERNAL_CHARACTERCREATE : RelayPacket
    {
        /*
        // This packet is invoked when an client creates an new character
        // with the following details.
        //
        // Name         Name of the character
        // Eye          A byte representing the Eye
        // EyeColor     A byte representing the EyeColor
        // EyeBrows     A byte representing the EyeBrows
        // Hair         A byte representing the Hair-style
        // HairColor    A byte representing the Hair-color
        // WeaponName   A string containing the weapon's name
        // WeaponAffix  A byte representating the weapon-affix
        //
        // Last updated on Friday 26, okt 2007.
        */

        public CMSG_INTERNAL_CHARACTERCREATE()
        {
            this.data = new byte[76];
            this.Id = 0x0105;
        }

        public uint UserId
        {
            get
            {
                return BitConverter.ToUInt32(this.data, 0);
            }
        }

        public string Name
        {
            get
            {
                return Encoding.BigEndianUnicode.GetString(this.data, 4, 34).TrimEnd('\0');
            }
        }

        public byte[] FaceDetails
        {
            get
            {
                byte[] buffer = new byte[11];
                Array.Copy(this.data, 39, buffer, 0, 11);
                return buffer;
            }
        }

        public string WeaponName
        {
            get
            {
                return Encoding.Unicode.GetString(this.data, 50, 24).TrimEnd((char)0);
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

        public static explicit operator CMSG_INTERNAL_CHARACTERCREATE(byte[] p)
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

            CMSG_INTERNAL_CHARACTERCREATE pkt = new CMSG_INTERNAL_CHARACTERCREATE();
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