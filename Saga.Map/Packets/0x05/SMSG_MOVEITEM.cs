using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This function is used to swap a item. In this given context "to swap"
    /// the item indicates a event where the user equips/unequips a item.
    /// </remarks>
    /// <id>
    /// 0514
    /// </id>
    internal class SMSG_MOVEITEM : RelayPacket
    {
        public SMSG_MOVEITEM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0514;
            this.data = new byte[3];
        }

        public byte MovementType
        {
            set { this.data[0] = value; }
        }

        public byte SourceIndex
        {
            set { this.data[1] = value; }
        }

        public byte DestinationIndex
        {
            set { this.data[2] = value; }
        }
    }
}