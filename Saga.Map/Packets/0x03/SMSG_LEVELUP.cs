using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// This packet idicates when a user has reached a new job-level. This
    /// triggers a animation on the client that the specified player
    /// has reached a job level up.
    /// </summary>
    /// <remarks>
    /// LevelType speciafies for what type of level up.
    /// 1 determines a base level up
    /// 2 determines a job level up
    /// </remarks>
    /// <id>
    /// 030C
    /// </id>
    internal class SMSG_LEVELUP : RelayPacket
    {
        public SMSG_LEVELUP()
        {
            this.Cmd = 0x0601;
            this.Id = 0x030C;
            this.data = new byte[8];
        }

        public byte LevelType
        {
            set { this.data[0] = value; }
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public byte Levels
        {
            set { this.data[5] = value; }
        }
    }
}