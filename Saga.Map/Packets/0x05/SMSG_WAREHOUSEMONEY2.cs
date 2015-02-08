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
    /// 051A
    /// </id>
    internal class SMSG_WAREHOUSEMONEY2 : RelayPacket
    {
        public SMSG_WAREHOUSEMONEY2()
        {
            this.Cmd = 0x0601;
            this.Id = 0x051A;
            this.data = new byte[4];
        }
    }
}