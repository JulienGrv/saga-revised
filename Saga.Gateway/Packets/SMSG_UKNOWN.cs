using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Gateway
{
    public class SMSG_UKNOWN : Packet
    {
        public SMSG_UKNOWN()
        {
            this.data = new byte[1];
            this.Id = 0x0205;
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