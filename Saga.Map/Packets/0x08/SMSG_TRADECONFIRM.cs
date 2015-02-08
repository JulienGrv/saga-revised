using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to confirm the trade is okay.
    /// </remarks>
    /// <id>
    /// 0807
    /// </id>
    internal class SMSG_TRADECONFIRM : RelayPacket
    {
        public SMSG_TRADECONFIRM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0807;
            this.data = new byte[0];
        }
    }
}