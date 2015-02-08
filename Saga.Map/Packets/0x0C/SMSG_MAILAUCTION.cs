using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to the client as an reply on the auction house causing a new mail
    /// item to be added to your maillist. Note that this packet is incremental.
    /// </remarks>
    /// <id>
    /// 0C0B
    /// </id>
    internal class SMSG_MAILAUCTION : RelayPacket
    {
        public SMSG_MAILAUCTION()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C0B;
            this.data = new byte[] {
	            0x01, 	0x00, 	0x00, 	0x00, 	0x02, 	0x01, 	0x00, 	0x00, 	0x00
            };
        }
    }
}