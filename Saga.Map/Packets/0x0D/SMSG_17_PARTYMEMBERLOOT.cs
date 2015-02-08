using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is broadcasted to all players in a party whenever he/she obtains a
    /// item he/she looted.
    /// </remarks>
    /// <id>
    /// 0D17
    /// </id>
    internal class SMSG_PARTYMEMBERLOOT : RelayPacket
    {
        public SMSG_PARTYMEMBERLOOT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D17;
            this.data = new byte[9];
        }

        public byte Index
        {
            set { this.data[0] = value; }
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public uint ItemId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 4); }
        }
    }
}