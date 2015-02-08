using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Shared.PacketLib.Login
{
    public class SMSG_INTERNAL_CHARACTERCREATE : RelayPacket
    {
        public SMSG_INTERNAL_CHARACTERCREATE()
        {
            this.Cmd = 0x0501;
            this.Id = 0x0105;
            this.data = new byte[76];
        }

        public uint CharacterId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

        public string Name
        {
            set
            {
                Encoding.BigEndianUnicode.GetBytes(value, 0, Math.Min(value.Length, 17), this.data, 4);
            }
        }

        public byte Eye
        {
            set
            {
                this.data[39] = value;
            }
        }

        public byte EyeColor
        {
            set
            {
                this.data[40] = value;
            }
        }

        public byte EyeBrowse
        {
            set
            {
                this.data[41] = value;
            }
        }

        public byte Hair
        {
            set
            {
                this.data[42] = value;
            }
        }

        public byte HairColor
        {
            set
            {
                this.data[43] = value;
            }
        }

        public string WeaponName
        {
            set
            {
                Encoding.Unicode.GetBytes(value, 0, Math.Min(value.Length, 12), this.data, 50);
            }
        }

        public ushort WeaponAffix
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 74, 2);
            }
        }
    }
}