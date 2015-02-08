using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to player who used be able to see the specified
    /// player but now can't see him anymore as a result of moving. It releases any
    /// assiciated resources related to the player (client-side).
    /// </remarks>
    /// <id>
    /// 0304
    /// </id>
    internal class SMSG_ACTORDELETE : RelayPacket
    {
        /// <summary>
        /// Initialized a new SMSG_ACTORDELETE packet.
        /// </summary>
        public SMSG_ACTORDELETE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0304;
            this.data = new byte[4];
        }

        /// <summary>
        /// ActorId to delete
        /// </summary>
        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}