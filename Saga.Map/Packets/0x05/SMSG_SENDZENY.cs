using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent by the client to set the amount of zeny. Note
    /// that the specified value is a absolute value and not a incremental
    /// value.
    /// </remarks>
    /// <id>
    /// 0515
    /// </id>
    internal class SMSG_SENDZENY : RelayPacket
    {
        public SMSG_SENDZENY()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0515;
            this.data = new byte[4];
        }

        public uint Zeny
        {
            set { Array.Copy(BitConverter.GetBytes(value), this.data, 4); }
            get { return BitConverter.ToUInt32(this.data, 4); }
        }
    }
}