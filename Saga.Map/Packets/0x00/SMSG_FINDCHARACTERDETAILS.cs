using Saga.Network.Packets;
using Saga.PrimaryTypes;
using System;

namespace Saga.Packets
{
    internal class SMSG_FINDCHARACTERDETAILS : RelayPacket
    {
        public SMSG_FINDCHARACTERDETAILS()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0003;
            this.data = new byte[109];
        }

        public uint CharacterId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

        public uint JobExperience
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4);
            }
        }

        public byte[] FaceDetails
        {
            set
            {
                Array.Copy(value, 0, this.data, 8, 11);
            }
        }

        public Rag2Item HeadTop
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 19, 4);
                this.data[23] = value.dyecolor;
            }
        }

        public Rag2Item HeadMiddle
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 24, 4);
                this.data[28] = value.dyecolor;
            }
        }

        public Rag2Item HeadBottom
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 29, 4);
                this.data[33] = value.dyecolor;
            }
        }

        public Rag2Item Shirt
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 34, 4);
                this.data[38] = value.dyecolor;
            }
        }

        public Rag2Item Legs
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 39, 4);
                this.data[43] = value.dyecolor;
            }
        }

        public Rag2Item Gloves
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 44, 4);
                this.data[48] = value.dyecolor;
            }
        }

        public Rag2Item Shoes
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 49, 4);
                this.data[53] = value.dyecolor;
            }
        }

        public Rag2Item Belt
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 54, 4);
                this.data[58] = value.dyecolor;
            }
        }

        public Rag2Item Back
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 59, 4);
                this.data[63] = value.dyecolor;
            }
        }

        public Rag2Item LeftFinger
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 64, 4);
                this.data[68] = value.dyecolor;
            }
        }

        public Rag2Item RightFinger
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 69, 4);
                this.data[73] = value.dyecolor;
            }
        }

        public Rag2Item Necklace
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 74, 4);
                this.data[78] = value.dyecolor;
            }
        }

        public Rag2Item Earring
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 79, 4);
                this.data[83] = value.dyecolor;
            }
        }

        public Rag2Item Ammo
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 84, 4);
                this.data[88] = value.dyecolor;
            }
        }

        public Rag2Item LeftShield
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 89, 4);
                this.data[93] = value.dyecolor;
            }
        }

        public Rag2Item RightShield
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.info.item), 0, this.data, 94, 4);
                this.data[98] = value.dyecolor;
            }
        }

        public uint AugeSkill
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 104, 4);
            }
        }

        public byte SaveMap
        {
            set
            {
                this.data[108] = value;
            }
        }
    }
}