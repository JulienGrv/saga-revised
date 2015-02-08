using Saga.Network.Packets;
using Saga.Structures;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send to sent new character information to yourself and people who
    /// can see you. This packet contains information as your name, gender, virtually
    /// anything to render yourself.
    /// </remarks>
    /// <id>
    /// 0302
    /// </id>
    internal class SMSG_CHARACTERINFO : RelayPacket
    {
        public SMSG_CHARACTERINFO()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0302;
            this.data = new byte[123];
            this.data[118] = 0x00;  //INVISIBLE
            this.data[119] = 0x00;  //NOTHING OBVIOUS
            this.data[120] = 0x00;
            this.data[121] = 0x00;
            this.data[122] = 0x00;
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte Gender
        {
            set { this.data[4] = value; }
        }

        public float X
        {
            set { FloatToArray(value, this.data, 5); }
        }

        public float Y
        {
            set { FloatToArray(value, this.data, 9); }
        }

        public float Z
        {
            set { FloatToArray(value, this.data, 13); }
        }

        public Rotator yaw
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.rotation), 0, this.data, 17, 2);
                Array.Copy(BitConverter.GetBytes(value.unknown), 0, this.data, 19, 2);
            }
        }

        public string Name
        {
            set { Encoding.Unicode.GetBytes(value, 0, Math.Min(16, value.Length), this.data, 21); }
        }

        public byte race
        {
            set { this.data[55] = value; }
        }

        public byte[] face
        {
            set { Array.Copy(value, 0, this.data, 56, value.Length); }
        }

        #region JUNK

        public uint[] EquipsA
        {
            set { Array.Copy(value, 0, this.data, 71, value.Length); }
        }

        public uint[] EquipsB
        {
            set { Array.Copy(value, 0, this.data, 91, value.Length); }
        }

        #endregion JUNK

        public void SetHeadTop(uint itemid, byte dye)
        {
            Array.Copy(BitConverter.GetBytes(itemid), 0, this.data, 67, 4);
            this.data[71] = dye;
        }

        public void SetHeadMiddle(uint itemid, byte dye)
        {
            Array.Copy(BitConverter.GetBytes(itemid), 0, this.data, 72, 4);
            this.data[76] = dye;
        }

        public void SetHeadBottom(uint itemid, byte dye)
        {
            Array.Copy(BitConverter.GetBytes(itemid), 0, this.data, 77, 4);
            this.data[81] = dye;
        }

        public void SetShield(uint itemid, byte dye)
        {
            Array.Copy(BitConverter.GetBytes(itemid), 0, this.data, 82, 4);
            this.data[86] = dye;
        }

        public uint AugeSkillID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 87, 4); }
        }

        public void SetShirt(uint itemid, byte dye)
        {
            Array.Copy(BitConverter.GetBytes(itemid), 0, this.data, 91, 4);
            this.data[95] = dye;
        }

        public void SetPants(uint itemid, byte dye)
        {
            Array.Copy(BitConverter.GetBytes(itemid), 0, this.data, 96, 4);
            this.data[100] = dye;
        }

        public void SetGloves(uint itemid, byte dye)
        {
            Array.Copy(BitConverter.GetBytes(itemid), 0, this.data, 101, 4);
            this.data[105] = dye;
        }

        public void SetFeet(uint itemid, byte dye)
        {
            Array.Copy(BitConverter.GetBytes(itemid), 0, this.data, 106, 4);
            this.data[110] = dye;
        }

        public void SetBack(uint itemid, byte dye)
        {
            Array.Copy(BitConverter.GetBytes(itemid), 0, this.data, 111, 4);
            this.data[115] = dye;
        }

        public byte Stance
        {
            set { this.data[116] = value; }
        }

        public byte Job
        {
            set { this.data[117] = value; }
        }

        public byte U1
        {
            set { this.data[120] = value; }
        }

        public byte SwordDrawn
        {
            set { this.data[86] = value; }
        }

        public byte NumberOfAdditions
        {
            set { this.data[123] = value; }
        }

        public void SetWeapon(uint addition, uint duration)
        {
            int offset = this.data.Length;
            Array.Resize<byte>(ref this.data, offset + 8);
            Array.Copy(BitConverter.GetBytes(addition), 0, this.data, offset, 4);
            Array.Copy(BitConverter.GetBytes(duration), 0, this.data, offset + 4, 4);
            this.data[123]++;
        }
    }
}