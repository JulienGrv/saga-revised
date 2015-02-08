using Saga.Network.Packets;

namespace Saga.Packets
{
    internal class SMSG_PONG : RelayPacket
    {
        public SMSG_PONG()
        {
            this.Cmd = 0x0701;
            this.Id = 0x000A;
            this.data = new byte[0];
        }
    }
}