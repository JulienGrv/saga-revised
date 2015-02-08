using Saga.Network.Packets;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This function indicated a movement start of the given actor
    /// to start moving to the given position. The actor is here specified
    /// as a actor of the player class.
    /// </remarks>
    /// <id>
    /// 0305
    /// </id>
    internal class SMSG_MOVEMENTSTART : RelayPacket
    {
        public SMSG_MOVEMENTSTART()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0305;
            this.data = new byte[44];
            this.data[34] = 0;
        }

        public uint SourceActorID
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

        public float AccelerationX
        {
            set { FloatToArray(value, this.data, 16); }
        }

        public float AccelerationY
        {
            set { FloatToArray(value, this.data, 20); }
        }

        public float AccelerationZ
        {
            set { FloatToArray(value, this.data, 24); }
        }

        public Rotator Yaw
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.rotation), 0, this.data, 28, 2);
                Array.Copy(BitConverter.GetBytes(value.unknown), 0, this.data, 30, 2);
            }
        }

        public ushort Speed
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 32, 2); }
        }

        public uint Delay0
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 35, 4); }
        }

        public byte MovementType
        {
            set { this.data[39] = value; }
        }

        public uint TargetActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 40, 4); }
        }

        /*
        public uint Delay1
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 40, 4); }
        }
         * */
    }
}