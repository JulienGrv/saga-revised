using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to the client as an reply indicating if you could
    /// retrieve the zeny fromthe mail.
    /// </remarks>
    /// <id>
    /// 0C05
    /// </id>
    internal class SMSG_MAILZENYAWNSER : RelayPacket
    {
        public SMSG_MAILZENYAWNSER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C05;
            this.data = new byte[1];
        }

        public bool Failed
        {
            set { this.data[0] = (value == true) ? (byte)1 : (byte)0; }
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }
    }
}