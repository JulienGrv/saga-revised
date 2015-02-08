using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Sends a list of special skills to the character
    /// </summary>
    /// <remarks>
    /// This packet sends a list of all saved special skills to
    /// the character.
    /// </remarks>
    /// <id>
    /// 060C
    /// </id>
    internal class SMSG_LISTSPECIALSKILLS : RelayPacket
    {
        public SMSG_LISTSPECIALSKILLS()
        {
            this.Cmd = 0x0601;
            this.Id = 0x090C;
            this.data = new byte[144];
        }

        public void AddSkill(uint skillid, uint skillexp, byte slot)
        {
            if (skillid > 0)
            {
                int offset = slot * 9;
                Array.Copy(BitConverter.GetBytes(skillid), 0, this.data, offset, 4);
                Array.Copy(BitConverter.GetBytes(skillexp), 0, this.data, offset + 4, 4);
                this.data[offset + 8] = slot;
            }
        }
    }
}