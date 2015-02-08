using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Notifies that the skill failed
    /// </summary>
    /// <remarks>
    /// This packet is sent to the client as a reply to their request
    /// to use a skill which has failed.
    /// </remarks>
    /// <id>
    /// 0907
    /// </id>
    internal class SMSG_OFFENSIVESKILLFAILED : RelayPacket
    {
        public SMSG_OFFENSIVESKILLFAILED()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0907;
            this.data = new byte[9];
            SkillType = 0x01;
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
    }
}