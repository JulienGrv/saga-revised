using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sends out the current hateinfo of a monster towards a player.
    /// Once the player starts a fight the client broadcasts a message every tick to
    /// request the hate. This is done to save the server processing power and memory
    /// of keeping track al the hate.
    /// </remarks>
    /// <id>
    /// 0609
    /// </id>
    internal class SMSG_HATEINFO : RelayPacket
    {
        public SMSG_HATEINFO()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0609;
            this.data = new byte[6];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public ushort Hate
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 2); }
        }
    }
}