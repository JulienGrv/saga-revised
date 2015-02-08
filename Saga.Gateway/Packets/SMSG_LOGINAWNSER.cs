using Saga.Network.Packets;

namespace Saga.Gateway.Packets
{
    public class SMSG_LOGINAWNSER : RelayPacket
    {
        public SMSG_LOGINAWNSER()
        {
            this.data = new byte[39];
        }

        public byte LoginError
        {
            set
            {
                this.data[36] = value;
            }
        }
    }
}