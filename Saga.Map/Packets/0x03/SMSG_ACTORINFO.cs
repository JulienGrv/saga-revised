using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This is used to send the acotr information for the specified Actor.
    /// </remarks>
    /// <id>
    /// 0303
    /// </id>
    internal class SMSG_ACTORINFO : RelayPacket
    {
        public SMSG_ACTORINFO()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0303;
            this.data = new byte[73];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public string Name
        {
            set { Encoding.Unicode.GetBytes(value, 0, Math.Min(value.Length, 16), this.data, 4); }
        }

        public float X
        {
            set { FloatToArray(value, this.data, 38); }
        }

        public float Y
        {
            set { FloatToArray(value, this.data, 42); }
        }

        public float Z
        {
            set { FloatToArray(value, this.data, 46); }
        }

        public uint yaw
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 50, 4); }
        }

        public byte race
        {
            set
            {
                this.data[54] = value;
            }
        }

        public byte[] face
        {
            set
            {
                Array.Copy(value, 0, this.data, 55, Math.Min(value.Length, 11));
            }
        }

        public byte PrimaryWeaponByIndex
        {
            set
            {
                this.data[66] = (byte)value;
            }
        }

        public byte SeccondairyWeaponByIndex
        {
            set
            {
                this.data[67] = (byte)value;
            }
        }

        public byte ActiveWeaponIndex
        {
            set
            {
                this.data[68] = value;
            }
        }

        public byte InventoryContainerSize
        {
            set
            {
                this.data[69] = value;
            }
        }

        public byte StorageContainerSize
        {
            set
            {
                this.data[70] = value;
            }
        }

        public byte UnlockedWeaponCount
        {
            set
            {
                this.data[71] = value;
            }
        }

        public byte Unknown
        {
            set
            {
                this.data[72] = value;
            }
        }

        [Obsolete("Slots is not supported anymore", true)]
        public byte[] slots
        {
            set
            {
                Array.Copy(value, 0, this.data, 68, Math.Min(value.Length, 5));
            }
        }
    }
}