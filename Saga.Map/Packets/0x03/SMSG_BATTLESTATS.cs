using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to send your battle stats. These battlestats consist
    /// out of the resistances, defense, flee.
    /// </remarks>
    /// <id>
    /// 0310
    /// </id>
    internal class SMSG_BATTLESTATS : RelayPacket
    {
        public SMSG_BATTLESTATS()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0310;
            this.data = new byte[40];
        }

        public ushort HolyResistance
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 2); }
        }

        public ushort DarkResistance
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 2, 2); }
        }

        public ushort FireResistance
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 2); }
        }

        public ushort IceResistance
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 6, 2); }
        }

        public ushort WindResistance
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 8, 2); }
        }

        public ushort CurseResistance
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 10, 2); }
        }

        public ushort SpiritResistance
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 12, 2); }
        }

        public ushort GhostResistance
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 14, 2); }
        }

        public ushort PhysicalEvasion
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 16, 2); }
        }

        public ushort PhysicalRangedEvasion
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 18, 2); }
        }

        public ushort MagicalEvasion
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 20, 2); }
        }

        public ushort PhysicalDefense
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 22, 2); }
        }

        public ushort PhysicalRangedDefense
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 24, 2); }
        }

        public ushort MagicalDefense
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 26, 2); }
        }

        public ushort PhysicalAttackMax
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 28, 2); }
        }

        public ushort PhysicalRangedAttackMax
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 30, 2); }
        }

        public ushort MagicalAttackMax
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 32, 2); }
        }

        public ushort PhysicalAttackMin
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 34, 2); }
        }

        public ushort PhysicalRangedAttackMin
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 36, 2); }
        }

        public ushort MagicalAttackMin
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 38, 2); }
        }
    }
}