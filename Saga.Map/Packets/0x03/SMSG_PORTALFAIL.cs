using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent when portal information could not be found.
    /// direction.
    /// </remarks>
    /// <id>
    /// 031B
    /// </id>
    internal class SMSG_PORTALFAIL : RelayPacket
    {
        public SMSG_PORTALFAIL()
        {
            this.Cmd = 0x0601;
            this.Id = 0x031B;
            this.data = new byte[1];
        }

        public byte Reason
        {
            set { this.data[0] = value; }
        }
    }
}