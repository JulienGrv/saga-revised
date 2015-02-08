using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sends the extended character stats. This packet contains
    /// information about base stats, bonusses.
    /// </remarks>
    /// <id>
    /// 030F
    /// </id>
    internal class SMSG_EXTSTATS : RelayPacket
    {
        public SMSG_EXTSTATS()
        {
            this.Cmd = 0x0601;
            this.Id = 0x030F;
            this.data = new byte[42];
        }

        public ushort[] base_stats_1
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value[0]), 0, this.data, 0, 2);
                Array.Copy(BitConverter.GetBytes(value[1]), 0, this.data, 2, 2);
                Array.Copy(BitConverter.GetBytes(value[2]), 0, this.data, 4, 2);
                Array.Copy(BitConverter.GetBytes(value[3]), 0, this.data, 6, 2);
                Array.Copy(BitConverter.GetBytes(value[4]), 0, this.data, 8, 2);
            }
        }

        public ushort[] base_stats_2
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value[0]), 0, this.data, 10, 2);
                Array.Copy(BitConverter.GetBytes(value[1]), 0, this.data, 12, 2);
                Array.Copy(BitConverter.GetBytes(value[2]), 0, this.data, 14, 2);
                Array.Copy(BitConverter.GetBytes(value[3]), 0, this.data, 16, 2);
                Array.Copy(BitConverter.GetBytes(value[4]), 0, this.data, 18, 2);
            }
        }

        public ushort[] base_stats_jobs
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value[0]), 0, this.data, 20, 2);
                Array.Copy(BitConverter.GetBytes(value[1]), 0, this.data, 22, 2);
                Array.Copy(BitConverter.GetBytes(value[2]), 0, this.data, 24, 2);
                Array.Copy(BitConverter.GetBytes(value[3]), 0, this.data, 26, 2);
                Array.Copy(BitConverter.GetBytes(value[4]), 0, this.data, 28, 2);
            }
        }

        public ushort[] base_stats_bonus
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value[0]), 0, this.data, 30, 2);
                Array.Copy(BitConverter.GetBytes(value[1]), 0, this.data, 32, 2);
                Array.Copy(BitConverter.GetBytes(value[2]), 0, this.data, 34, 2);
                Array.Copy(BitConverter.GetBytes(value[3]), 0, this.data, 36, 2);
                Array.Copy(BitConverter.GetBytes(value[4]), 0, this.data, 38, 2);
            }
        }

        public ushort statpoints
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 40, 2); }
        }
    }
}