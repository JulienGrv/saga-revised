using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet set the weaponname and suffix
    /// </remarks>
    /// <id>
    /// 0520
    /// </id>
    internal class SMSG_WEAPONNEWCHANGESUFFIX : RelayPacket
    {
        public SMSG_WEAPONNEWCHANGESUFFIX()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0520;
            this.data = new byte[28];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }

        public byte SlotId
        {
            set { this.data[1] = value; }
        }

        public string WeaponName
        {
            set { Encoding.Unicode.GetBytes(value, 0, Math.Min(value.Length, 11), this.data, 2); }
        }

        public ushort Suffix
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 26, 2); }
        }
    }
}