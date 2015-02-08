using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// No information available.
    /// Still figuring out what it does and when it get's send
    /// </remarks>
    /// <id>
    /// 061A
    /// </id>
    internal class SMSG_CHANGEATKTYPE : RelayPacket
    {
        public SMSG_CHANGEATKTYPE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x061A;
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