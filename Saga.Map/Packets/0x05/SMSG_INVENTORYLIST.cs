using Saga.Network.Packets;
using Saga.PrimaryTypes;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This function send the inventory list of the specified player this is apart
    /// of the map-loading function. Should be send once and all other item interaction
    /// is handeld incrementally.
    /// </remarks>
    /// <id>
    /// 0502
    /// </id>
    internal class SMSG_INVENTORYLIST : RelayPacket
    {
        private int index = 0;

        public SMSG_INVENTORYLIST(byte items)
        {
            this.Cmd = 0x0601;
            this.Id = 0x0502;
            this.data = new byte[2 + (items * 67)];
            this.data[1] = items;
        }

        public byte SortType
        {
            set { this.data[0] = value; }
        }

        public void AddItem(Rag2Item item)
        {
            int offset = 2 + (this.index * 67);
            Rag2Item.Serialize(item, this.data, offset);
            this.data[offset + 66] = (byte)this.index;
            this.index++;
        }
    }
}