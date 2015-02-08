using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet updates the amount of zeny a shopping npc has. This packet should
    /// be send on every item your buy/sell. And should be broadcasted to every player
    /// who references the shop as a target.
    /// </remarks>
    /// <id>
    /// 060A
    /// </id>
    internal class SMSG_SHOPZENYUPDATE : RelayPacket
    {
        public SMSG_SHOPZENYUPDATE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x060A;
            this.data = new byte[8];
        }

        public uint Zeny
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint Actor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }
    }
}