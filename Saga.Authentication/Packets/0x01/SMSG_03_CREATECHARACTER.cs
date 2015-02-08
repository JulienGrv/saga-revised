using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Login
{
    public class SMSG_CREATECHARACTER : RelayPacket
    {
        public SMSG_CREATECHARACTER()
        {
            this.data = new byte[1];
            this.Cmd = 0x0401;
            this.Id = 0x0103;
        }

        public byte Result
        {
            set
            {
                this.data[0] = value;
            }
        }
    }
}