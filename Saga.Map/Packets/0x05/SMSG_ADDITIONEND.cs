using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to delete the addition icon for a given player. The deletion
    /// of this icon may occur due the additions active time expired.
    /// </remarks>
    /// <id>
    /// 051D
    /// </id>
    internal class SMSG_ADDITIONEND : RelayPacket
    {
        public SMSG_ADDITIONEND()
        {
            this.Cmd = 0x0601;
            this.Id = 0x051D;
            this.data = new byte[8];
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint StatusID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }
    }
}