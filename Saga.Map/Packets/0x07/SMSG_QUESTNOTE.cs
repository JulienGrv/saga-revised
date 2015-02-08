using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent in conjunction with npc note
    /// this indicates that the following steps are beeing
    /// logged in a history.
    /// </remarks>
    /// <id>
    /// 070D
    /// </id>
    internal class SMSG_QUESTNOTE : RelayPacket
    {
        public SMSG_QUESTNOTE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x070D;
            this.data = new byte[5];
            this.Unknown = 1;
        }

        public uint QuestID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte Unknown
        {
            set { this.data[4] = value; }
        }
    }
}