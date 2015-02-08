using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to request new love comitment.
    /// </remarks>
    /// <id>
    /// 031E
    /// </id>
    internal class SMSG_RESPONSESHOWLOVE : RelayPacket
    {
        public SMSG_RESPONSESHOWLOVE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x031E;
            this.data = new byte[1];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }
    }
}