using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Some packet related target
    /// </remarks>
    /// <id>
    /// 0D13
    /// </id>
    internal class SMSG_PARTYMEMBERU3 : RelayPacket
    {
        public SMSG_PARTYMEMBERU3()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D13;
            this.data = new byte[10];
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

        public uint TargetId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 6, 4); }
        }
    }
}