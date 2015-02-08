using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to indicate the party should be dismissed.
    /// There is also party quiting which leads to if you're only one left
    /// inside the party the party should be dismissed.
    /// </remarks>
    /// <id>
    /// 0D16
    /// </id>
    internal class SMSG_PARTYDISMISSED : RelayPacket
    {
        public SMSG_PARTYDISMISSED()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D16;
            this.data = new byte[0];
        }
    }
}