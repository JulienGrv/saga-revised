using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Login
{
    public class SMSG_DELETECHARACTER : RelayPacket
    {
        public SMSG_DELETECHARACTER()
        {
            this.data = new byte[2];
            this.Cmd = 0x0401;
            this.Id = 0x0106;
        }

        public byte Index
        {
            set
            {
                this.data[0] = value;
            }
        }

        public byte Result
        {
            set
            {
                this.data[1] = value;
            }
        }
    }
}