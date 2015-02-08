using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// No remarks
    /// </summary>
    /// <remarks>
    /// This packet is broadcasted to all players in the regional range who
    /// are able to see the player. This packet indicates that the user is
    /// switching equipment.
    ///
    /// For example player a wares a hat and now switched to bunny ears. This
    /// packet is then send to notify other actors you're switching gears.///
    /// </remarks>
    /// <id>
    /// 030D
    /// </id>
    internal class SMSG_CHANGEEQUIPMENT : RelayPacket
    {
        public SMSG_CHANGEEQUIPMENT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x030D;
            this.data = new byte[10];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte Slot
        {
            set { this.data[4] = value; }
        }

        public uint ItemID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 4); }
        }

        public byte Dye
        {
            set { this.data[9] = value; }
        }
    }
}