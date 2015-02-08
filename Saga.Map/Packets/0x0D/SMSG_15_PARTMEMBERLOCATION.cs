using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sents out the new x and y position on the minimap
    /// to display for a certain character.
    /// </remarks>
    /// <id>
    /// 0D15
    /// </id>
    internal class SMSG_PARTYMEMBERLOCATION : RelayPacket
    {
        public SMSG_PARTYMEMBERLOCATION()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D15;
            this.data = new byte[13];
        }

        public byte Index
        {
            set { this.data[0] = value; }
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public float X
        {
            set { FloatToArray(value, this.data, 9); }
        }

        public float Y
        {
            set { FloatToArray(value, this.data, 5); }
        }
    }
}