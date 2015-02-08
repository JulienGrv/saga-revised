using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Changes from stance
    /// </summary>
    /// <remarks>
    /// This packet is sent to the client to confirm to change stances e.d. skills
    /// like baynonet stance request use to send to send this. If this packet is not
    /// send the skill will hang and you aren't able to do other skills.
    /// </remarks>
    /// <id>
    /// 0909
    /// </id>
    internal class SMSG_SKILLTOGLE : RelayPacket
    {
        public SMSG_SKILLTOGLE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0909;
            this.data = new byte[6];
        }

        public byte SkillType
        {
            set { this.data[0] = value; }
        }

        public uint SkillID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public bool Toggle
        {
            set { this.data[5] = (byte)((value) ? 13 : 12); }
        }
    }
}