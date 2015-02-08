using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet that indicates wheter or not a upgrade has been sucessfull.
    /// </remarks>
    /// <id>
    /// 0527
    /// </id>
    internal class SMSG_USESUPPLEMENTSTONE : RelayPacket
    {
        public SMSG_USESUPPLEMENTSTONE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0527;
            this.data = new byte[9];
        }

        public byte InventoryId
        {
            set { this.data[0] = value; }
        }

        public byte Container
        {
            set { this.data[1] = value; }
        }

        public byte ContainerSlot
        {
            set { this.data[2] = value; }
        }

        public byte EnchantmentSlot
        {
            set { this.data[3] = value; }
        }

        public uint Skill
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }

        public byte Result
        {
            set { this.data[8] = value; }
        }
    }
}