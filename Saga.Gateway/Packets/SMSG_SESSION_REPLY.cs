using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Login
{
    public class SMSG_SESSIONREQUEST : RelayPacket
    {
        public SMSG_SESSIONREQUEST()
        {
            this.data = new byte[0];
            this.Cmd = 0x0401;
            this.Id = 0xFF03;
        }
    }
}