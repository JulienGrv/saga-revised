using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Gateway
{
    public class SMSG_UNKNOWN3 : Packet
    {
        public SMSG_UNKNOWN3()
        {
            this.data = new byte[160];
            this.Id = 0x0203;
        }
    }
}