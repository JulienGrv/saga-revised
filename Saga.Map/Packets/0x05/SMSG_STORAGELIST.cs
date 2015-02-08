using Saga.Network.Packets;
using Saga.PrimaryTypes;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is used to show the storage list to the player. This is sent
    /// everytime the player opens his storage.
    /// </remarks>
    /// <id>
    /// 0503
    /// </id>
    internal class SMSG_STORAGELIST : RelayPacket
    {
        public SMSG_STORAGELIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0503;
            this.data = new byte[6];
        }

        public byte SortType
        {
            set { this.data[0] = value; }
        }

        public void AddItem(Rag2Item item, int ItemIndex)
        {
            int offset = this.data.Length;
            Array.Resize<byte>(ref this.data, offset + 67);
            Rag2Item.Serialize(item, this.data, offset);
            this.data[offset + 66] = (byte)ItemIndex;
            this.data[1]++;
        }
    }
}