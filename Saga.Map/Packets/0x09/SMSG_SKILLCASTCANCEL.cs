using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Changes from stance
    /// </summary>
    /// <remarks>
    /// This packet is sent to the user and all surrounding characters to indicate
    /// the cast has failed. A cast fail can happen because of a skill that causes
    /// to fail casts, or the character suddenly moves.
    /// </remarks>
    /// <id>
    /// 0906
    /// </id>
    internal class SMSG_SKILLCASTCANCEL : RelayPacket
    {
        public SMSG_SKILLCASTCANCEL()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0906;
            this.data = new byte[11];
        }

        public byte SkillType
        {
            set { this.data[0] = value; }
        }

        public uint SkillID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 4); }
        }

        public ushort Unknown
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 9, 2); }
        }
    }
}