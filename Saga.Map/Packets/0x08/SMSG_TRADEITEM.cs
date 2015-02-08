using Saga.Network.Packets;

namespace Saga.Packets
{
    internal class SMSG_TRADEITEM : RelayPacket
    {
        public SMSG_TRADEITEM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0803;
            this.data = new byte[4];
        }

        public byte Tradeslot
        {
            set { this.data[0] = value; }
        }

        public byte ItemSlot
        {
            set { this.data[1] = value; }
        }

        public byte Count
        {
            set { this.data[2] = value; }
        }

        public byte Status
        {
            set { this.data[3] = value; }
        }
    }
}