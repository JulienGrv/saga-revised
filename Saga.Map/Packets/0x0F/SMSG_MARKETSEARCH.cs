using Saga.Network.Packets;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <remarks>
    /// This contains all the search results from the attemted search. Most of the
    /// stuff there. According to the old saga's code the bytes after the itemid would be
    /// the name of the item used for sortations.
    /// </remarks>
    /// <id>
    /// 1101
    /// </id>
    internal class SMSG_MARKETSEARCH : RelayPacket
    {
        public SMSG_MARKETSEARCH()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1101;
            this.data = new byte[2];
        }

        public byte Unknown
        {
            set { this.data[0] = value; }
        }

        public byte Count
        {
            set
            {
                this.data[1] = value;
                Array.Resize<byte>(ref this.data, 2 + value * 112);
            }
        }

        public void Add(MarketItemArgument c)
        {
            int index = this.data.Length - 112;
            Rag2Item.Serialize(c.item, this.data, index);
            Array.Copy(BitConverter.GetBytes(c.id), 0, this.data, index + 66, 4);
            UnicodeEncoding.Unicode.GetBytes(c.sender, 0, c.sender.Length, this.data, index + 70);
            Array.Copy(BitConverter.GetBytes(c.price), 0, this.data, index + 104, 4); //Price
            Array.Copy(BitConverter.GetBytes(c.id), 0, this.data, index + 108, 4);
        }
    }
}