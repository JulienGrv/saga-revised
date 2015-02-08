using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <id>
    /// 0E08
    /// </id>
    internal class SMSG_ERROR : RelayPacket
    {
        public SMSG_ERROR()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0E00;
            this.data = new byte[4];
        }

        public uint ErrorCode
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}