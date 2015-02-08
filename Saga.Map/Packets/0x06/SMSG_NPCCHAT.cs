using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet makes the npc start a dialog.
    /// </remarks>
    /// <id>
    /// 0602
    /// </id>
    internal class SMSG_NPCCHAT : RelayPacket
    {
        public SMSG_NPCCHAT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0602;
            this.data = new byte[11];
            this.Unknown = 0;
            this.Unknown2 = 0;
        }

        public byte Unknown
        {
            set { this.data[0] = value; }
        }

        public uint Script
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 4); }
        }

        public byte Unknown2
        {
            set { this.data[10] = value; }
        }

        public byte[] Icons
        {
            set
            {
                int len = this.data.Length;
                Array.Resize<byte>(ref this.data, len + value.Length);
                for (int i = 0; i < value.Length; i++)
                    this.data[len + i] = value[i];
                this.data[9] += (byte)value.Length;
            }
        }
    }
}