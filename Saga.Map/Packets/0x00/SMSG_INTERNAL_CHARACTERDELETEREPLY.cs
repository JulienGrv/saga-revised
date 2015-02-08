using Saga.Network.Packets;

namespace Saga.Packets
{
    internal class SMSG_INTERNAL_CHARACTERDELETEREPLY : RelayPacket
    {
        public SMSG_INTERNAL_CHARACTERDELETEREPLY()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0106;
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