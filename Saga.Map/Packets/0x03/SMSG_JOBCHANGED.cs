using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is used to indicate the given actor has changed their jobs. This
    /// packet is expected to send to player after it invokes a request to change their
    /// current job class.
    /// </remarks>
    /// <id>
    /// 0311
    /// </id>
    internal class SMSG_JOBCHANGED : RelayPacket
    {
        public SMSG_JOBCHANGED()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0311;
            this.data = new byte[6];
        }

        public byte Job
        {
            set { this.data[0] = value; }
        }

        public byte Result
        {
            set { this.data[1] = value; }
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 2, 4); }
        }
    }
}