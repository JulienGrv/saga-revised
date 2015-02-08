using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Changes from stance
    /// </summary>
    /// <remarks>
    /// This packet is sent to the user to indicate a skill should be removed from
    /// the client's skilllis.
    /// </remarks>
    /// <id>
    /// 0902
    /// </id>
    internal class SMSG_SKILLREMOVE : RelayPacket
    {
        public SMSG_SKILLREMOVE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0902;
            this.data = new byte[6];
        }

        public byte Unknown
        {
            set { this.data[0] = value; }
        }

        public uint SkillId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public byte Unknown2
        {
            set { this.data[0] = value; }
        }
    }
}