using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Add a special skill on the specified slot.
    /// </summary>
    /// <remarks>
    /// This packet is sent as an incrementeal update on the special skill
    /// list. This will add an skill on the specified slot on the
    /// special skilllist. In total there are 16 available slots.
    /// </remarks>
    /// <id>
    /// 0918
    /// </id>
    internal class SMSG_SETSPECIALSKILL : RelayPacket
    {
        public SMSG_SETSPECIALSKILL()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0918;
            this.data = new byte[6];
        }

        public byte Slot
        {
            set { this.data[0] = value; }
        }

        public uint SkillID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public byte Result
        {
            set { this.data[5] = value; }
        }
    }
}