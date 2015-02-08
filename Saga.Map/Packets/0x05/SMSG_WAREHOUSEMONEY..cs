using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet creates a new weapon
    /// </remarks>
    /// <id>
    /// 0519
    /// </id>
    internal class SMSG_WAREHOUSEMONEY : RelayPacket
    {
        public SMSG_WAREHOUSEMONEY()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0519;
            this.data = new byte[4];
        }
    }
}