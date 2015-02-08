using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet forces your to terminiate conversation with the npc.
    /// At the moment this is only used in conjunction with the warper npc
    /// where you need to leave your current npc convo before beeing warped.
    /// </remarks>
    /// <id>
    /// 0621
    /// </id>
    internal class SMSG_LEAVENPC : RelayPacket
    {
        public SMSG_LEAVENPC()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0621;
            this.data = new byte[1];
        }

        public byte Reason
        {
            set
            {
                this.data[0] = value;
            }
        }
    }
}