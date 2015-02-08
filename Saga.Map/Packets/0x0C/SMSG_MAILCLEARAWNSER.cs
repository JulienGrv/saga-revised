using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent over as an reply indicating if the mail item
    /// could be cleared
    /// </remarks>
    /// <id>
    /// 0C0A
    /// </id>
    internal class SMSG_MAILCLEARAWNSER : RelayPacket
    {
        public SMSG_MAILCLEARAWNSER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C0A;
            this.data = new byte[1];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }
    }
}