using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// No information yet. To be believed to do something related to the old guard functionallity.
    /// Which will show you some 'hot' places or provide information about them.
    /// </remarks>
    /// <id>
    /// 0607
    /// </id>
    internal class SMSG_NPCASKLOCATIONSRC : RelayPacket
    {
        public SMSG_NPCASKLOCATIONSRC()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0607;
            this.data = new byte[28];
        }

        public uint DialogScript
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

        public byte Result
        {
            set
            {
                this.data[4] = value;
            }
        }

        public uint NpcId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 4);
            }
        }

        public float X
        {
            set
            {
                FloatToArray(value / 1000, this.data, 9);
            }
        }

        public float Y
        {
            set
            {
                FloatToArray(value / 1000, this.data, 13);
            }
        }

        public float Z
        {
            set
            {
                FloatToArray(value / 1000, this.data, 17);
            }
        }

        public byte Result2
        {
            set
            {
                this.data[21] = value;
            }
        }
    }
}