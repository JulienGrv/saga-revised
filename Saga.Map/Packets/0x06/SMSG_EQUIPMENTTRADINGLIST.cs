using Saga.Network.Packets;
using Saga.PrimaryTypes;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packets sents a list of items that can be traded. Still need to implament
    /// this so both trading windows can be used.
    /// </remarks>
    /// <id>
    /// 0617
    /// </id>
    internal class SMSG_EQUIPMENTTRADINGLIST : RelayPacket
    {
        public SMSG_EQUIPMENTTRADINGLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0617;
            this.data = new byte[5061];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint SupplyID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 7, 4); }
        }

        public void AddItem(Rag2Item product, Rag2Item supply, Rag2Item supply2)
        {
            int count = this.data[4];
            int newcount = count + 1;
            if (newcount > 25) return;

            if (product != null)
            {
                Rag2Item.Serialize(product, this.data, 0x6F + count * 66);
                Array.Copy(BitConverter.GetBytes(count), 0, this.data, 11 + count * 4, 4);
                this.data[4]++;
            }
            if (supply != null)
            {
                Rag2Item.Serialize(supply, this.data, 0x6E1 + count * 66);
                this.data[5]++;
            }
            if (supply2 != null)
            {
                Rag2Item.Serialize(supply2, this.data, 0xD53 + count * 66);
                this.data[6]++;
            }
        }

        #region Conversions

        public static explicit operator byte[](SMSG_EQUIPMENTTRADINGLIST p)
        {
            /*
            // Creates a new byte with the length of data
            // plus 4. The first size bytes are used like
            // [PacketSize][PacketId][PacketBody]
            //
            // Where Packet Size equals the length of the
            // Packet body, Packet Identifier, Packet Size
            // Container.
            */

            if (p.data[6] == 0) Array.Resize<byte>(ref p.data, 3411);
            byte[] tmp = new byte[p.Size];
            Array.Copy(BitConverter.GetBytes(p.Size), 0, tmp, 0, 2);
            Array.Copy(BitConverter.GetBytes(p.session), 0, tmp, 2, 4);
            Array.Copy(p.cmd, 0, tmp, 6, 2);
            Array.Copy(BitConverter.GetBytes(p.Size - 10), 0, tmp, 10, 2);
            Array.Copy(p.id, 0, tmp, 12, 2);
            Array.Copy(p.data, 0, tmp, 14, p.data.Length);
            return tmp;
        }

        #endregion Conversions
    }
}