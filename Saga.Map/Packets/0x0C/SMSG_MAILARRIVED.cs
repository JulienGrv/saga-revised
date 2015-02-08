using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is to indicate a relative new packet arrival.
    /// </remarks>
    /// <id>
    /// 0C0B
    /// </id>
    internal class SMSG_MAILARRIVED : RelayPacket
    {
        public SMSG_MAILARRIVED()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C0B;
            this.data = new byte[9];
        }

        public uint Amount
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

        public byte U1
        {
            set
            {
                this.data[4] = value;
            }
        }

        public uint U2
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 4);
            }
        }
    }
}