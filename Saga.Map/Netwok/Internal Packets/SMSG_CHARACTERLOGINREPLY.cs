using Saga.Network.Packets;

namespace Saga.Packets
{
    internal class SMSG_CHARACTERLOGINREPLY : RelayPacket
    {
        public SMSG_CHARACTERLOGINREPLY()
        {
            this.data = new byte[1];
            this.Cmd = 0x0701;
            this.Id = 0xFF02;
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