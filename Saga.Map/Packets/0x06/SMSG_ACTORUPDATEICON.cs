using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This updates the current npc's icon. The icon in a bit flag enumation:
    /// 1 - Official Quest
    /// 2 - Personal Quest
    /// 3 - Both quests
    /// 4 - Dead
    /// </remarks>
    /// <id>
    /// 060E
    /// </id>
    internal class SMSG_ACTORUPDATEICON : RelayPacket
    {
        public SMSG_ACTORUPDATEICON()
        {
            this.Cmd = 0x0601;
            this.Id = 0x060E;
            this.data = new byte[5];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte Icon
        {
            set { this.data[4] = value; }
        }
    }
}