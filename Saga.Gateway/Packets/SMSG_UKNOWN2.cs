using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Gateway
{
    public class SMSG_UKNOWN2 : Packet
    {
        public SMSG_UKNOWN2()
        {
            this.data = new byte[1];
            this.Id = 0x0206;
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