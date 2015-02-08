using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Invokes a skill from item
    /// </summary>
    /// <remarks>
    /// This packet is sent to the user to indicate a skill has been
    /// invoked throughout an item.
    /// </remarks>
    /// <id>
    /// 0913
    /// </id>
    internal class SMSG_ITEMTOGGLE : RelayPacket
    {
        public SMSG_ITEMTOGGLE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0913;
            this.data = new byte[22];
        }

        public byte SkillType
        {
            set { this.data[0] = value; }
        }

        public byte SkillMessage
        {
            set { this.data[1] = value; }
        }

        public byte Container
        {
            set { this.data[2] = value; }
        }

        public byte Index
        {
            set { this.data[3] = value; }
        }

        public uint SkillID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 8, 4); }
        }

        public uint TargetActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 12, 4); }
        }

        public uint Value
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 16, 4); }
        }
    }
}