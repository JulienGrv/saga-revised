using Saga.Network.Packets;
using Saga.PrimaryTypes;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet shows a equipment trading list. This is a single list of where multiple items can
    /// be gained in contradiction with our other list which is only 1 item.
    /// </remarks>
    /// <id>
    /// 0615
    /// </id>
    internal class SMSG_SUPPLYLIST : RelayPacket
    {
        public SMSG_SUPPLYLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0615;
            this.data = new byte[3423];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint SupplyID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }

        public void SetProducts(Rag2Item list)
        {
            int offset = 119 + (this.data[17] * 66);
            Rag2Item.Serialize(list, this.data, offset);
            this.data[17]++;
        }

        public void SetMatrial(Rag2Item list)
        {
            int offset = 1769 + (this.data[18] * 66);
            Rag2Item.Serialize(list, this.data, offset);
            this.data[18]++;
        }
    }
}