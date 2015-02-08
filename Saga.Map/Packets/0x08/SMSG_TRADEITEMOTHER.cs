using Saga.Network.Packets;
using Saga.PrimaryTypes;

namespace Saga.Packets
{
    internal class SMSG_TRADEITEMOTHER : RelayPacket
    {
        public SMSG_TRADEITEMOTHER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0804;
            this.data = new byte[67];
        }

        public byte Tradeslot
        {
            set { this.data[0] = value; }
        }

        public Rag2Item Item
        {
            set { Rag2Item.Serialize(value, this.data, 1); }
        }
    }
}