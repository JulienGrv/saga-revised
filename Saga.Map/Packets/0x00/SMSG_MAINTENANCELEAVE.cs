using Saga.Network.Packets;

namespace Saga.Packets
{
    internal class SMSG_MAINTENANCELEAVE : RelayPacket
    {
        public SMSG_MAINTENANCELEAVE()
        {
            this.Cmd = 0x0701;
            this.Id = 0x000E;
            this.data = new byte[0];
        }
    }
}