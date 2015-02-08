using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send to every player surrounding the specified player
    /// to indicate he/she has updated their current actor state. For example
    /// player a was sitting and is now going to lay.
    /// </remarks>
    /// <id>
    /// 0309
    /// </id>
    internal class SMSG_ACTORCHANGESTATE : RelayPacket
    {
        public SMSG_ACTORCHANGESTATE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0309;
            this.data = new byte[10];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte State
        {
            set { this.data[4] = value; }
        }

        public byte Stance
        {
            set { this.data[5] = value; }
        }

        public uint TargetActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 6, 4); }
        }
    }
}