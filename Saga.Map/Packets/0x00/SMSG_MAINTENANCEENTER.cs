using Saga.Network.Packets;

namespace Saga.Packets
{
    internal class SMSG_MAINTENANCEENTER : RelayPacket
    {
        public SMSG_MAINTENANCEENTER()
        {
            this.Cmd = 0x0701;
            this.Id = 0x000F;
            this.data = new byte[0];
        }
    }
}