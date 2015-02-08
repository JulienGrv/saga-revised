using Saga.Network.Packets;

namespace Saga.Packets
{
    internal class SMSG_KICKSESSION : RelayPacket
    {
        public SMSG_KICKSESSION()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0010;
            this.data = new byte[0];
        }
    }
}