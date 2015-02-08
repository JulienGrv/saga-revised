using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sets the positions of your
    /// katra-homepoint
    /// </remarks>
    /// <id>
    /// 0314
    /// </id>
    internal class SMSG_KAFTRAHOMEPOINT : RelayPacket
    {
        public SMSG_KAFTRAHOMEPOINT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0314;
            this.data = new byte[2];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }

        public byte Zone
        {
            set { this.data[1] = value; }
        }
    }
}