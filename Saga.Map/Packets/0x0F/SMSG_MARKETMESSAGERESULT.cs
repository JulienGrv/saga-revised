using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <remarks>
    /// Note quite sure what this packet really does. But seems to work in
    /// conjunction with owner-based message retrieving.
    /// </remarks>
    /// <id>
    /// 1106
    /// </id>
    internal class SMSG_MARKETMESSAGERESULT : RelayPacket
    {
        public SMSG_MARKETMESSAGERESULT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1106;
            this.data = new byte[1];
        }
    }
}