using Saga.Network.Packets;

namespace Saga.Authentication.Packets
{
    public class SMSG_SETSESSION : Packet
    {
        public SMSG_SETSESSION()
        {
            this.Id = 0x0001;
            this.data = new byte[0];
        }
    }
}