using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sets a position and yaw. Probably
    /// WideMovementStop function?
    /// </remarks>
    /// <id>
    /// 031A
    /// </id>
    internal class SMSG_WIDEMOVEMENTSTOP : RelayPacket
    {
        public SMSG_WIDEMOVEMENTSTOP()
        {
            this.Cmd = 0x0601;
            this.Id = 0x031A;
            this.data = new byte[20];
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

        public ushort Yaw1
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 16, 2); }
        }

        public ushort Yaw2
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 18, 2); }
        }
    }
}