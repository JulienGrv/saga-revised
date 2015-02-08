using Saga.Network.Packets;

namespace Saga.Packets
{
    internal class SMSG_QUESTREWITEM : RelayPacket
    {
        public SMSG_QUESTREWITEM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0708;
            this.data = new byte[0];
        }
    }
}