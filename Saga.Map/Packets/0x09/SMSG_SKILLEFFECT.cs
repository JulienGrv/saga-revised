using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// This will sent an skill effect to the client.
    /// </summary>
    /// <remarks>
    /// The skill effect can cause three affects HP, LP, SP. This packet
    /// is largly still unknown as haven't seen two effects working at the
    /// same time.
    /// </remarks>
    /// <id>
    /// 091F
    /// </id>
    internal class SMSG_SKILLEFFECT : RelayPacket
    {
        public SMSG_SKILLEFFECT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x091F;
            this.data = new byte[15];
            Unknown1 = 0x01;
            Unknown3 = 0xFF;
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte Unknown1
        {
            set { this.data[4] = value; }
        }

        public uint Unknown2
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 4); }
        }

        public byte Function
        {
            set { this.data[9] = value; }
        }

        public uint Amount
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 10, 4); }
        }

        public byte Unknown3
        {
            set { this.data[14] = value; }
        }
    }
}