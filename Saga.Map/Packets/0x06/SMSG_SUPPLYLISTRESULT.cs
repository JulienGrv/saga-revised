using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This show a fail/succes message if the trade is sucessfully.
    /// </remarks>
    /// <id>
    /// 0618
    /// </id>
    internal class SMSG_SUPPLYLISTRESULT : RelayPacket
    {
        public SMSG_SUPPLYLISTRESULT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0618;
            this.data = new byte[2];
        }

        public byte Reason1
        {
            set
            {
                this.data[0] = value;
            }
        }

        public byte Reason2
        {
            set
            {
                this.data[1] = value;
            }
        }
    }
}