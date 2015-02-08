using Saga.Network.Packets;
using Saga.PrimaryTypes;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sent the drop list of the user. Packet should be
    /// renamed to SMSG_DROPLIST.
    /// </remarks>
    /// <id>
    /// 0601
    /// </id>
    internal class SMSG_SENDINVENTORY : RelayPacket
    {
        private int offset = 0;

        public SMSG_SENDINVENTORY(int items)
        {
            this.Cmd = 0x0601;
            this.Id = 0x0601;
            this.data = new byte[5 + (items * 67)];
            this.data[4] = (byte)items;
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public void AddItem(Rag2Item item, int index)
        {
            Rag2Item.Serialize(item, this.data, 5 + (this.offset * 67));
            this.data[5 + 66 + (this.offset * 67)] = (byte)index;
            this.offset++;
        }
    }
}