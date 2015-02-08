using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This function returns the required money to transfer jobs. The formulla to change
    /// jobs is something along the lines of:
    ///
    /// ((joblvl - 5) * 300) + ( nSummedSkillLvl - nSkills  ) * 100
    /// </remarks>
    /// <id>
    /// 0323
    /// </id>
    internal class SMSG_SENDMONEYFORJOB : RelayPacket
    {
        public SMSG_SENDMONEYFORJOB()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0323;
            this.data = new byte[4];
        }

        public uint Zeny
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}