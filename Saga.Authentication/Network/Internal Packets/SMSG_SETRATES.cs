using Saga.Network.Packets;

namespace Saga.Packets
{
    public class SMSG_SETRATES : RelayPacket
    {
        public SMSG_SETRATES()
        {
            this.data = new byte[8];
            this.Cmd = 0x0701;
            this.Id = 0xFF03;
        }

        public byte IsAdDisplayed
        {
            set
            {
                this.data[0] = value;
            }
        }
    }
}