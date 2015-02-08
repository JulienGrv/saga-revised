using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent over as an reply indicating if the mail item
    /// could be canceled.
    /// </remarks>
    /// <id>
    /// 0C0A
    /// </id>
    internal class SMSG_MAILCANCELAWNSER : RelayPacket
    {
        public SMSG_MAILCANCELAWNSER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C08;
            this.data = new byte[1];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }
    }
}