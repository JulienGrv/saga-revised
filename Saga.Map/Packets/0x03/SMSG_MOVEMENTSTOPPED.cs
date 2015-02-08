using Saga.Network.Packets;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This function is used to indacte the given actor stopped moving.
    /// </remarks>
    /// <id>
    /// 0306
    /// </id>
    internal class SMSG_MOVEMENTSTOPPED : RelayPacket
    {
        public SMSG_MOVEMENTSTOPPED()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0306;
            this.data = new byte[31];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public float X
        {
            set { FloatToArray(value, this.data, 4); }
        }

        public float Y
        {
            set { FloatToArray(value, this.data, 8); }
        }

        public float Z
        {
            set { FloatToArray(value, this.data, 12); }
        }

        public Rotator yaw
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.rotation), 0, this.data, 16, 2);
                Array.Copy(BitConverter.GetBytes(value.unknown), 0, this.data, 18, 2);
            }
        }

        public ushort speed
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 20, 2); }
        }

        public uint DelayTime
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 23, 4); }
        }

        public uint TargetActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 27, 4); }
        }
    }
}