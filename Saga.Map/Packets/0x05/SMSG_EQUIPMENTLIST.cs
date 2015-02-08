using Saga.Network.Packets;
using Saga.PrimaryTypes;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sends the equipment list in total to the specified player. This packet should be
    /// send as part of the map-loading function.
    /// </remarks>
    /// <id>
    /// 0512
    /// </id>
    internal class SMSG_EQUIPMENTLIST : RelayPacket
    {
        public SMSG_EQUIPMENTLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0512;
            this.data = new byte[1088];
        }

        public void AddItem(Rag2Item item, byte Active, int ItemIndex)
        {
            int index = ItemIndex * 68;
            Rag2Item.Serialize(item, this.data, index);
            this.data[index + 67] = item.active;
        }
    }
}