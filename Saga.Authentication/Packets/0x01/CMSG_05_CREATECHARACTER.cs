using Saga.Network;
using Saga.Network.Packets;
using System;

namespace Saga.Authentication.Packets
{
    public class CMSG_CREATECHARACTER : RelayPacket
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

        public CMSG_CREATECHARACTER()
        {
            this.data = new byte[72];
            this.Id = 0x0105;
        }

        public string Name
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 0, 34).TrimEnd(' ');
            }
        }

        public byte Eye
        {
            get
            {
                return this.data[35];
            }
        }

        public byte EyeColor
        {
            get
            {
                return this.data[36];
            }
        }

        public byte EyeBrowse
        {
            get
            {
                return this.data[37];
            }
        }

        public byte Hair
        {
            get
            {
                return this.data[38];
            }
        }

        public byte HairColor
        {
            get
            {
                return this.data[39];
            }
        }

        public string WeaponName
        {
            get
            {
                return SpecialEncoding.Read(ref this.data, 46, 24).TrimEnd('\0');
            }
        }

        public ushort WeaponAffix
        {
            get
            {
                return BitConverter.ToUInt16(data, 70);
            }
        }

        #region Conversions

        public static explicit operator CMSG_CREATECHARACTER(byte[] p)
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

            CMSG_CREATECHARACTER pkt = new CMSG_CREATECHARACTER();
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