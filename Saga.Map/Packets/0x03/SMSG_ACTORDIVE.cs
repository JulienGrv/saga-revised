using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send by the player as a result of either going to dive
    /// underwater or the player was diving and has reached the surface. This
    /// function needs to trigger the rightfull regenarations
    /// </remarks>
    /// <id>
    /// 0320
    /// </id>
    internal class SMSG_ACTORDIVE : RelayPacket
    {
        public SMSG_ACTORDIVE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0320;
            this.data = new byte[5];
        }

        public uint Oxygen
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public byte Direction
        {
            set { this.data[0] = value; }
        }
    }
}