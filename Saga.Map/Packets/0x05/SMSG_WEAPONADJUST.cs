using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Level = 1, Exp = 2, Dura = 3, Activation = 6
    /// </summary>
    /// <remarks>
    /// This packet adjusts the weapon, the weapon can be adjusted for various reasons:
    /// Exp, durabillity, Weapon. The value is pinpointed in affection of the pinpointed
    /// function (reason to upate).
    /// </remarks>
    /// <id>
    /// 0516
    /// </id>
    internal class SMSG_WEAPONADJUST : RelayPacket
    {
        public SMSG_WEAPONADJUST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0516;
            this.data = new byte[6];
        }

        public byte Function
        {
            set { this.data[0] = value; }
        }

        public byte Slot
        {
            set { this.data[1] = value; }
        }

        public uint Value
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 2, 4); }
        }
    }
}