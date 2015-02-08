using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet appears to be broadcasted when you switch a nap/zone.
    /// </remarks>
    /// <id>
    /// 0D14
    /// </id>
    internal class SMSG_PARTYMEMBERMAPCHANGE : RelayPacket
    {
        public SMSG_PARTYMEMBERMAPCHANGE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D14;
            this.data = new byte[8];
        }

        public byte Index
        {
            set { this.data[0] = value; }
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public byte Unknown
        {
            set { this.data[5] = value; }
        }

        public byte Unknown2
        {
            set { this.data[6] = value; }
        }

        public byte Zone
        {
            set { this.data[7] = value; }
        }
    }
}