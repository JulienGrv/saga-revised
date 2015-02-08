using Saga.Network.Packets;
using Saga.PrimaryTypes;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to the specified player to add a item-resource to
    /// the specified container. Container 2 speciafies the inventory, while container 3
    /// speciafies the storage.
    /// </remarks>
    /// <id>
    /// 0506
    /// </id>
    internal class SMSG_ADDITEM : RelayPacket
    {
        public SMSG_ADDITEM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0506;
            this.data = new byte[69];
        }

        public byte Container
        {
            set { this.data[0] = value; }
        }

        public byte UpdateReason
        {
            set { this.data[1] = value; }
        }

        public void SetItem(Rag2Item item, int Index)
        {
            Rag2Item.Serialize(item, this.data, 2);
            this.data[68] = (byte)Index;
        }
    }
}