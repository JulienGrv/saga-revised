using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Invokes a packet to do some animiation. We should confirm it's packets
    /// useabillity and capacities.
    /// </remarks>
    /// <id>
    /// 0613
    /// </id>
    internal class SMSG_ACTORANIMATION : RelayPacket
    {
        public SMSG_ACTORANIMATION()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0613;
            this.data = new byte[9];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint Animation
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }
    }
}