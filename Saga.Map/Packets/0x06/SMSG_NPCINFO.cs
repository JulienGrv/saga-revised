using Saga.Network.Packets;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet makes a new npc appear.
    /// Currently there are 1 unknown field.
    /// </remarks>
    /// <id>
    /// 060B
    /// </id>
    internal class SMSG_NPCINFO : RelayPacket
    {
        public SMSG_NPCINFO(uint count)
        {
            this.Cmd = 0x0601;
            this.Id = 0x060B;
            this.data = new byte[36];
            this.unknown = 1;
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint NPCID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }

        public float X
        {
            set { FloatToArray(value, this.data, 8); }
        }

        public float Y
        {
            set { FloatToArray(value, this.data, 12); }
        }

        public float Z
        {
            set { FloatToArray(value, this.data, 16); }
        }

        public Rotator Yaw
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.rotation), 0, this.data, 20, 2);
                Array.Copy(BitConverter.GetBytes(value.unknown), 0, this.data, 22, 2);
            }
        }

        public byte unknown
        {
            set { this.data[24] = value; }
        }

        public ushort HP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 25, 2); }
        }

        public ushort MaxHP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 27, 2); }
        }

        public ushort SP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 29, 2); }
        }

        public ushort MaxSP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 31, 2); }
        }

        public byte Icon
        {
            set { this.data[33] = value; }
        }

        public byte IsAggresive
        {
            set { this.data[34] = value; }
        }

        [Obsolete("Use icon", true)]
        public void SetAdditionalStatus(params byte[] aStats)
        {
            if ((34 + (4 * aStats.Length)) != this.data.Length) return;
            for (int i = 0; i < aStats.Length; i++) this.data[33 + (i * 4)] = aStats[i];
        }
    }
}