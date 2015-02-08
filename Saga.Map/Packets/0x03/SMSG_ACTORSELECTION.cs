using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This is packet is send by to the specified actor to indicate he/she
    /// should select the specified player. This pop-ups the selection stuff,
    /// there should also be a equiavelent for deselecting.
    /// </remarks>
    /// <id>
    /// 030B
    /// </id>
    internal class SMSG_ACTORSELECTION : RelayPacket
    {
        public SMSG_ACTORSELECTION()
        {
            this.Cmd = 0x0601;
            this.Id = 0x030B;
            this.data = new byte[16];
        }

        public uint SourceActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public ushort HP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 2); }
        }

        public ushort MaxHP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 6, 2); }
        }

        public ushort SP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 8, 2); }
        }

        public ushort MaxSP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 10, 2); }
        }

        public uint TargetActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 12, 4); }
        }
    }
}