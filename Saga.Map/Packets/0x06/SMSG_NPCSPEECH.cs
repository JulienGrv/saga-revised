using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet makes a npc output the requested speech script (dialoge)
    /// </remarks>
    /// <id>
    /// 0605
    /// </id>
    internal class SMSG_NPCSPEECH : RelayPacket
    {
        public SMSG_NPCSPEECH()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0605;
            this.data = new byte[8];
        }

        public uint Script
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }
    }
}