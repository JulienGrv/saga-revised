using Saga.Network.Packets;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <remarks>
    /// This contains a list of items used to be owned by the current user.
    /// </remarks>
    /// <id>
    /// 1103
    /// </id>
    internal class SMSG_MARKETOWNERSEARCH : RelayPacket
    {
        public SMSG_MARKETOWNERSEARCH()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1103;
            this.data = new byte[2];
        }

        public byte Unknown
        {
            set { this.data[0] = value; }
        }

        public void Add(MarketItemArgument item)
        {
            int index = this.data.Length;
            Array.Resize<byte>(ref this.data, 2 + (++this.data[1] * 75));
            Rag2Item.Serialize(item.item, this.data, index + 0);
            Array.Copy(BitConverter.GetBytes(item.price), 0, this.data, index + 66, 4);
            this.data[index + 70] = (byte)Math.Max(0, (item.expires - DateTime.Now).TotalHours);
            Array.Copy(BitConverter.GetBytes(item.id), 0, this.data, index + 71, 4);
        }
    }
}