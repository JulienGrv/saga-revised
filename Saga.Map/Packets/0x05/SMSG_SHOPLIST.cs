using Saga.Network.Packets;
using Saga.PrimaryTypes;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet shows the shoplist with available items which can be bought by
    /// the player. The shoplist are used by npc by either the type of a shop, catheleya
    /// or skillmaster.
    /// </remarks>
    /// <id>
    /// 0511
    /// </id>
    internal class SMSG_SHOPLIST : RelayPacket
    {
        private int index = 0;

        public SMSG_SHOPLIST(byte items)
        {
            this.Cmd = 0x0601;
            this.Id = 0x0511;
            this.data = new byte[11 + (items * 68)];
            this.data[0] = items;
        }

        public SMSG_SHOPLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0511;
            this.data = new byte[11];
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 4); }
        }

        public uint Zeny
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public void AddItem(Rag2Item item, bool nostock)
        {
            int offset = 11 + (this.index * 68);
            Rag2Item.Serialize(item, this.data, offset);
            this.data[offset + 66] = (byte)this.index;
            this.data[offset + 67] = (byte)(nostock == true ? 1 : 0);
            this.index++;
        }

        public void AddItem(Rag2Item item, bool nostock, int index)
        {
            int offset = this.data.Length;
            Array.Resize<byte>(ref this.data, offset + 68);
            Rag2Item.Serialize(item, this.data, offset);
            this.data[0]++;
            this.data[offset + 66] = (byte)index;
            this.data[offset + 67] = (byte)(nostock == true ? 1 : 0);
        }
    }
}