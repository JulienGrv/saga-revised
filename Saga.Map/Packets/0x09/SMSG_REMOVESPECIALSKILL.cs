using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Removes an previously learned special skill
    /// </summary>
    /// <remarks>
    /// This packet is sent to the user to indicate to remove a special skill
    /// from the special skill array.
    /// </remarks>
    /// <id>
    /// 0919
    /// </id>
    internal class SMSG_REMOVESPECIALSKILL : RelayPacket
    {
        public SMSG_REMOVESPECIALSKILL()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0919;
            this.data = new byte[5];
        }

        public uint SkillID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte Result
        {
            set { this.data[4] = value; }
        }
    }
}