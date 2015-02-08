using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// What this packet does is not yet know. We believe it is needed
    /// to be able to see party members on the minimap.
    /// </remarks>
    /// <id>
    /// 0D06
    /// </id>
    internal class SMSG_PARTYUNKNOWN : RelayPacket
    {
        public SMSG_PARTYUNKNOWN()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D06;
            this.data = new byte[20];
        }

        public byte Unsure
        {
            set
            {
                this.data[0] = value;
            }
        }

        public uint ActorID
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4);
            }
        }

        public byte Unknown
        {
            set
            {
                this.data[5] = value;
            }
        }

        public byte Race
        {
            set
            {
                this.data[6] = value;
            }
        }

        public byte Map
        {
            set
            {
                this.data[7] = value;
            }
        }

        public ushort HP
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 8, 2);
            }
        }

        public ushort HPmax
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 10, 2);
            }
        }

        public ushort SP
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 12, 2);
            }
        }

        public ushort SPmax
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 14, 2);
            }
        }

        public byte LP
        {
            set
            {
                this.data[16] = value;
            }
        }

        public byte CharLvl
        {
            set
            {
                this.data[17] = value;
            }
        }

        public byte Job
        {
            set
            {
                this.data[18] = value;
            }
        }

        public byte JobLvl
        {
            set
            {
                this.data[19] = value;
            }
        }
    }
}