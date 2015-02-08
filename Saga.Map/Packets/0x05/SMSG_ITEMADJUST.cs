using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Level = 1, EXP = 2, Durability = 3, Amount = 4, Active = 5, Tradable = 7
    /// </summary>
    /// <remarks>
    /// This function adjust a item, a item could be adjusted for exp, lvl, duration.
    /// This packet is used by repair (blacksmith) functions, and equipment/weaponary
    /// duration loss.
    /// </remarks>
    /// <id>
    /// 0513
    /// </id>
    internal class SMSG_ITEMADJUST : RelayPacket
    {
        public SMSG_ITEMADJUST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0513;
            this.data = new byte[6];
        }

        public byte Container
        {
            set { this.data[0] = value; }
        }

        public byte Function
        {
            set { this.data[1] = value; }
        }

        public byte UpdateReason
        {
            set { this.data[2] = value; }
        }

        public byte Slot
        {
            set { this.data[3] = value; }
        }

        public uint Value
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 2); }
        }
    }
}