using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    /// Level = 1, EXP = 2, Durability = 3, Amount = 4, Active = 5, Tradable = 7
    /// </summary>
    /// <remarks>
    /// This packet is used to update the item in the the
    /// specified container most common usage is the update
    /// the items count for various reaons.
    ///
    /// </remarks>
    /// <id>
    /// 0513
    /// </id>
    internal class SMSG_UPDATEITEM : RelayPacket
    {
        public SMSG_UPDATEITEM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0513;
            this.data = new byte[6];
        }

        public byte Container
        {
            set { this.data[0] = value; }
        }

        public byte UpdateType
        {
            set { this.data[1] = value; }
        }

        public byte UpdateReason
        {
            set { this.data[2] = value; }
        }

        public byte Index
        {
            set { this.data[3] = value; }
        }

        public byte Amount
        {
            set { this.data[4] = value; }
        }

        public byte Unknown
        {
            set { this.data[5] = value; }
        }
    }
}