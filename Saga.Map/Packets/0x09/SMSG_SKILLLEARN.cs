using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    /// Learns a skill
    /// </summary>
    /// <remarks>
    /// This packet is sent to the user to sent as a result whether he or she
    /// has learnt the skill or an error occured.
    /// </remarks>
    /// <id>
    /// 091A
    /// </id>
    internal class SMSG_SKILLLEARN : RelayPacket
    {
        public SMSG_SKILLLEARN()
        {
            this.Cmd = 0x0601;
            this.Id = 0x091A;
            this.data = new byte[1];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }
    }
}