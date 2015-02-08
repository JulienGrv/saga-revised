using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to specified player to indicate to release
    /// resources for the associated item-resource on the client. As a result
    /// it deletes the item. The item can be deleted with numerous of reason,
    /// item traded or sold to a npc
    /// </remarks>
    /// <id>
    /// 0507
    /// </id>
    internal class SMSG_DELETEITEM : RelayPacket
    {
        public SMSG_DELETEITEM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0507;
            this.data = new byte[3];
        }

        public byte Container
        {
            set { this.data[0] = value; }
        }

        public byte UpdateReason
        {
            set { this.data[1] = value; }
        }

        public byte Index
        {
            set { this.data[2] = value; }
        }
    }
}