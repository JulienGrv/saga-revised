using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet defines the the result of the performed heal operation.
    /// </remarks>
    /// <id>
    /// 0316
    /// </id>
    internal class SMSG_HEALRESULT : RelayPacket
    {
        public SMSG_HEALRESULT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0316;
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