using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to the client as an reply indicating if you sent
    /// the mail.
    /// </remarks>
    /// <id>
    /// 0C02
    /// </id>
    internal class SMSG_MAILSENDAWNSER : RelayPacket
    {
        public SMSG_MAILSENDAWNSER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C02;
            this.data = new byte[1];
        }

        public byte Result
        {
            set
            {
                this.data[0] = value;
            }
        }
    }
}