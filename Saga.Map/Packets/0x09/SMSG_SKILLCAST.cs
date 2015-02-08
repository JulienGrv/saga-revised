using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// This will sent an skill effect to the client.
    /// </summary>
    /// <remarks>
    /// This packet is sent to all surrounding characters to indicate
    /// the mob or character is in casting stage.
    /// </remarks>
    /// <id>
    /// 0904
    /// </id>
    internal class SMSG_SKILLCAST : RelayPacket
    {
        public SMSG_SKILLCAST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0904;
            this.data = new byte[15];
        }

        public byte SkillType
        {
            set { this.data[0] = value; }
        }

        public uint SkillID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 4); }
        }

        public uint TargetActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 9, 4); }
        }

        public ushort Unknown
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 13, 2); }
        }
    }
}