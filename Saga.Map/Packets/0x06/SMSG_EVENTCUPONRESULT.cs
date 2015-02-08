using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is supposed to show some failure/success message after you selected a item
    /// that is sent using SMSG_EVENTINFO2 packet.
    /// </remarks>
    /// <id>
    /// 061F
    /// </id>
    internal class SMSG_EVENTCUPONRESULT : RelayPacket
    {
        public SMSG_EVENTCUPONRESULT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x061F;
            this.data = new byte[1];
        }

        public byte Result
        {
            set
            {
                this.data[0] = value;
            }
        }
    }
}