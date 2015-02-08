using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet updates a actionobject (read as mapobject). They are used in conjunction with
    /// quests to activate a object once the quest is activated.
    /// </remarks>
    /// <id>
    /// 0612
    /// </id>
    internal class SMSG_ITEMUPDATE : RelayPacket
    {
        public SMSG_ITEMUPDATE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0612;
            this.data = new byte[7];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte Active
        {
            set { this.data[4] = value; }
        }

        public byte Active1
        {
            set { this.data[5] = value; }
        }

        public byte Active2
        {
            set { this.data[6] = value; }
        }
    }
}