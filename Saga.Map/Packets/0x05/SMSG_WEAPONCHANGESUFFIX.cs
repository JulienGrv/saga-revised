using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet adjusts the weapon's suffix.
    /// </remarks>
    /// <id>
    /// 0524
    /// </id>
    internal class SMSG_WEAPONCHANGESUFFIX : RelayPacket
    {
        public SMSG_WEAPONCHANGESUFFIX()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0524;
            this.data = new byte[4];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }

        public byte Slot
        {
            set { this.data[1] = value; }
        }

        public ushort Suffix
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 2, 2); }
        }
    }
}