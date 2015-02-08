using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to the client to indicate if the trade failed
    /// or was a success.
    /// </remarks>
    /// <id>
    /// 0801
    /// </id>
    internal class SMSG_TRADERESULT : RelayPacket
    {
        public SMSG_TRADERESULT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0801;
            this.data = new byte[5];
        }

        public uint ActorId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte Reason
        {
            set { this.data[4] = value; }
        }
    }
}