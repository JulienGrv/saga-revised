using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// List of ailable skills
    /// </summary>
    /// <remarks>
    /// Sends over a list of available skills to the character. This can be
    /// active, passive or supportive skills.
    /// </remarks>
    /// <id>
    /// 090A
    /// </id>
    internal class SMSG_BATTLESKILL : RelayPacket
    {
        public SMSG_BATTLESKILL(int NumberOfSkills)
        {
            this.Cmd = 0x0601;
            this.Id = 0x090A;
            this.data = new byte[1 + (NumberOfSkills * 9)];
            this.data[0] = (byte)NumberOfSkills;
        }

        public byte current = 0;

        public void AddSkill(uint skillid, uint skillexp)
        {
            int offset = 1 + (current * 9);
            Array.Copy(BitConverter.GetBytes(skillid), 0, this.data, offset, 4);
            Array.Copy(BitConverter.GetBytes(skillexp), 0, this.data, offset + 4, 4);
            this.data[offset + 8] = current;
            current++;
        }
    }
}