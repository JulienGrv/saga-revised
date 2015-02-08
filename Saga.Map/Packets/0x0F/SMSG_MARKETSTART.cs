using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <remarks>
    /// This packet will invoke the client to show the market dialog. Where you
    /// can register items and or buy new ones. When the market is started the
    /// items are not automaticly displayed.
    /// </remarks>
    /// <id>
    /// 1109
    /// </id>
    internal class SMSG_MARKETSTART : RelayPacket
    {
        public SMSG_MARKETSTART()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1109;
            this.data = new byte[5];
        }

        public byte Unknown
        {
            set { this.data[0] = value; }
        }

        public uint Actor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }
    }
}