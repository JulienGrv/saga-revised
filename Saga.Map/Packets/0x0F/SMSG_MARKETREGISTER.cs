using Saga.Network.Packets;
using Saga.PrimaryTypes;
using System;

namespace Saga.Packets
{
    /// <remarks>
    /// This will add the specified item to the list of owned items.
    /// </remarks>
    /// <id>
    /// 1104
    /// </id>
    internal class SMSG_MARKETREGISTER : RelayPacket
    {
        public SMSG_MARKETREGISTER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1104;
            this.data = new byte[76];
        }

        public byte Result
        {
            set
            {
                this.data[0] = value;
            }
        }

        public Rag2Item Item
        {
            set
            {
                Rag2Item.Serialize(value, this.data, 1);
            }
        }

        public uint Zeny
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 67, 4);
            }
        }

        public DateTime ExpireDate
        {
            set
            {
                this.data[71] = (byte)Math.Max(0, (value - DateTime.Now).TotalHours);
            }
        }

        public uint AuctionID
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 72, 4);
            }
        }
    }
}