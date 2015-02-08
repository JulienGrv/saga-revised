using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Gateway
{
    public class SMSG_IDENTIFY : Packet
    {
        public SMSG_IDENTIFY()
        {
            this.data = new byte[0];
            this.Id = 0x0204;
        }
    }
}