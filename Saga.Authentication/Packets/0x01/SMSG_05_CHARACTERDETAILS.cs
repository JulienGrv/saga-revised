using Saga.Network.Packets;
using System;

namespace Saga.Authentication.Packets
{
    public class SMSG_CHARACTERDETAILS : RelayPacket
    {
        public SMSG_CHARACTERDETAILS()
        {
            this.data = new byte[105];
            this.Cmd = 0x0401;
            this.Id = 0x0105;
        }

        #region Face-informations

        public byte Eye
        {
            set
            {
                this.data[4] = value;
            }
        }

        public byte Eyecolor
        {
            set
            {
                this.data[5] = value;
            }
        }

        public byte Eyebrows
        {
            set
            {
                this.data[6] = value;
            }
        }

        public byte Hair
        {
            set
            {
                this.data[7] = value;
            }
        }

        public byte Haircolor
        {
            set
            {
                this.data[8] = value;
            }
        }

        #endregion Face-informations

        #region Equipment

        public uint HeadTop
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 18, 4);
            }
        }

        public uint HeadMiddle
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 22, 4);
            }
        }

        public uint HeadBottom
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 26, 4);
            }
        }

        public uint Body
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 30, 4);
            }
        }

        public uint Legs
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 34, 4);
            }
        }

        public uint Pants
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 38, 4);
            }
        }

        public uint Hands
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 42, 4);
            }
        }

        public uint Belt
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 46, 4);
            }
        }

        public uint Back
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 50, 4);
            }
        }

        public uint LeftFinger
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 54, 4);
            }
        }

        public uint RightFinger
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 56, 4);
            }
        }

        public uint Ammo
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 60, 4);
            }
        }

        public uint Necklace
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 64, 4);
            }
        }

        public uint Earring
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 68, 4);
            }
        }

        public uint LeftHand
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 72, 4);
            }
        }

        public uint RightHand
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 76, 4);
            }
        }

        #endregion Equipment

        #region Misc

        public void SetRawBytes(byte[] buffer)
        {
            Array.Copy(buffer, this.data, 105);
        }

        public uint JobExperience
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

        public uint AugeSkill
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 100, 4);
            }
        }

        public byte SaveMap
        {
            set
            {
                this.data[104] = value;
            }
        }

        #endregion Misc
    }
}