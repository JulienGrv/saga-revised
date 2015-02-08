using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <id>
    /// 0525
    /// </id>
    internal class SMSG_USEDYEITEM : RelayPacket
    {
        public SMSG_USEDYEITEM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0525;
            this.data = new byte[7];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }

        public uint ItemId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public byte Container
        {
            set { this.data[5] = value; }
        }

        public byte Equipment
        {
            set { this.data[6] = value; }
        }
    }
}