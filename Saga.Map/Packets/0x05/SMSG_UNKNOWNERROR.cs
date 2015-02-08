using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// No information
    /// </remarks>
    /// <id>
    /// 0521
    /// </id>
    internal class SMSG_UNKNOWNERROR : RelayPacket
    {
        public SMSG_UNKNOWNERROR()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0521;
            this.data = new byte[1];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }
    }
}