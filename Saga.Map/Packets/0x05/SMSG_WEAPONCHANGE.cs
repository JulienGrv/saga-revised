using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to your character to indicate a weaonchange has occured.
    /// </remarks>
    /// <id>
    /// 051E
    /// </id>
    internal class SMSG_WEAPONCHANGE : RelayPacket
    {
        public SMSG_WEAPONCHANGE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x051E;
            this.data = new byte[10];
        }

        public byte Reason
        {
            set { this.data[0] = value; }
        }

        public uint Auge
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public byte WeaponType
        {
            set { this.data[5] = value; }
        }

        public byte Index
        {
            set { this.data[6] = value; }
        }

        public ushort Suffix
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 7, 2); }
        }

        public byte Active
        {
            set { this.data[9] = value; }
        }
    }
}