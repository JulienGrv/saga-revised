using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent as a return message on the client rigth after 0x0709 (CMSG_QUESTSTART)
    /// has been invoked. Posible it's been used for other purposes as well.
    /// </remarks>
    /// <id>
    /// 0526
    /// </id>
    internal class SMSG_USEQUESTITEM : RelayPacket
    {
        public SMSG_USEQUESTITEM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0526;
            this.data = new byte[2];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }

        public byte Index
        {
            set { this.data[1] = value; }
        }
    }
}