using Saga.Network.Packets;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Sends a new item actor to the character
    /// </summary>
    /// <remarks>
    /// This packet adds a new action object with a known.
    /// Still quite a lot is unknown.
    /// </remarks>
    /// <id>
    /// 060C
    /// </id>
    internal class SMSG_ITEMINFO : RelayPacket
    {
        public SMSG_ITEMINFO()
        {
            this.Cmd = 0x0601;
            this.Id = 0x060C;
            this.data = new byte[37];
            this.unknown = 1;
            this.CanInteract = 0;
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

        public byte CanInteract
        {
            set { this.data[35] = value; }
        }

        public byte IsActive
        {
            set { this.data[36] = value; }
        }
    }
}