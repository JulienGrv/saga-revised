using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet indicates that the user has saved their new
    /// position.
    /// </remarks>
    /// <id>
    /// 0611
    /// </id>
    internal class SMSG_KAFTRASAVEPOSITION : RelayPacket
    {
        public SMSG_KAFTRASAVEPOSITION()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0611;
            this.data = new byte[1];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }
    }
}