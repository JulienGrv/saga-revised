using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is used to send to the user after she has invoked a
    /// request to open the drop-list for the dead target. The client
    /// blocks all other trafic.
    /// </remarks>
    /// <id>
    /// 0518
    /// </id>
    internal class SMSG_NPCDROPLISTRESULT : RelayPacket
    {
        public SMSG_NPCDROPLISTRESULT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0518;
            this.data = new byte[5];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte Result
        {
            set { this.data[4] = value; }
        }
    }
}