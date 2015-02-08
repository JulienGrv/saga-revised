using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to indicate if the party invivitation is
    /// agreed or denied.
    /// </remarks>
    /// <id>
    /// 0D02
    /// </id>
    internal class SMSG_PARTYINVITATIONRESULT : RelayPacket
    {
        public SMSG_PARTYINVITATIONRESULT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D02;
            this.data = new byte[1];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }
    }
}