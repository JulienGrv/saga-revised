using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet that indicates to change
    /// the auge of the weapon.
    /// </remarks>
    /// <id>
    /// 050E
    /// </id>
    internal class SMSG_SHOWWEAPON : RelayPacket
    {
        public SMSG_SHOWWEAPON()
        {
            this.Cmd = 0x0601;
            this.Id = 0x050E;
            this.data = new byte[8];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint AugeID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }
    }
}