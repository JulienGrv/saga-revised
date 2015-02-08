using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is used to set a specified enchantement to
    /// the specified weapon.
    /// </remarks>
    /// <id>
    /// 050F
    /// </id>
    [CLSCompliant(false)]
    public class SMSG_ENCHANTMENT : RelayPacket
    {
        /// <summary>
        /// Initialized a new SMSG_ENCHANTMENT packet
        /// </summary>
        public SMSG_ENCHANTMENT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x050F;
            this.data = new byte[8];
        }

        /// <summary>
        /// Unknown setting
        /// </summary>
        public byte Unknown1
        {
            set
            {
                this.data[0] = value;
            }
        }

        /// <summary>
        /// Weapon slot
        /// </summary>
        public byte Unknown2
        {
            set
            {
                this.data[1] = value;
            }
        }

        /// <summary>
        /// Inventory slot item
        /// </summary>
        public byte Weaponslot
        {
            set
            {
                this.data[2] = value;
            }
        }

        /// <summary>
        /// New auge skill
        /// </summary>
        public uint SkillId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 3, 4); }
        }
    }
}