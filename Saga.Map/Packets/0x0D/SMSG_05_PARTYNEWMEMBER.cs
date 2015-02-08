using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to all party members to indicate a new party
    /// member should be added to their list.
    /// </remarks>
    /// <id>
    /// 0D05
    /// </id>
    internal class SMSG_PARTYNEWMEMBER : RelayPacket
    {
        public SMSG_PARTYNEWMEMBER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D05;
            this.data = new byte[44];
        }

        public byte Index
        {
            set { this.data[0] = value; }
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public byte Unknown
        {
            set { this.data[5] = value; }
        }

        public string Name
        {
            set { Encoding.Unicode.GetBytes(value, 0, Math.Min(value.Length, 16), this.data, 6); }
        }
    }
}