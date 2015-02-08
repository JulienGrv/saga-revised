using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Some packet related to job changing.
    /// </remarks>
    /// <id>
    /// 0D0E
    /// </id>
    internal class SMSG_PARTYMEMBERU2 : RelayPacket
    {
        public SMSG_PARTYMEMBERU2()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D0E;
            this.data = new byte[9];
        }

        public byte Index
        {
            set { this.data[0] = value; }
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public byte Unknown
        {
            set { this.data[5] = value; }
        }

        public uint Skill
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 6, 2); }
        }
    }
}