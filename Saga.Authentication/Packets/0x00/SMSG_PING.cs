using Saga.Network.Packets;

namespace Saga.Authentication.Packets
{
    public class SMSG_PING : RelayPacket
    {
        public SMSG_PING()
        {
            this.Cmd = 0x0701;
            this.Id = 0x000E;
            this.data = new byte[4];
        }
    }
}