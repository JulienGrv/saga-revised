using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <remarks>
    /// This packet is send to notify the user he has successfully bought the item or
    /// a unknown database error has appeared.
    /// </remarks>
    /// <id>
    /// 1102
    /// </id>
    internal class SMSG_MARKETBUY : RelayPacket
    {
        public SMSG_MARKETBUY()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1102;
            this.data = new byte[1];
        }

        public byte Reason
        {
            set
            {
                this.data[0] = value;
            }
        }
    }
}