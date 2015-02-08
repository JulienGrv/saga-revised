using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Invokes an skill.
    /// </summary>
    /// <remarks>
    /// This packet is sent towards clients as a indication the selected
    /// user has performed a skill.
    /// </remarks>
    /// <id>
    /// 0908
    /// </id>
    internal class SMSG_OFFENSIVESKILL : RelayPacket
    {
        public SMSG_OFFENSIVESKILL()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0908;
            this.data = new byte[21];
            Unknown2 = 0xFF;
        }

        public byte SkillType
        {
            set { this.data[0] = value; }
        }

        public byte IsCritical
        {
            set { this.data[1] = value; }
        }

        public uint SkillID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 2, 4); }
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 6, 4); }
        }

        public uint TargetActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 10, 4); }
        }

        public uint Damage
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 14, 4); }
        }

        public ushort Unknown1
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 18, 2); }
        }

        public byte Unknown2
        {
            set { this.data[20] = value; }
        }
    }
}