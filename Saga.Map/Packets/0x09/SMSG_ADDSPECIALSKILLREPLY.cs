using Saga.Network.Packets;

namespace Saga.Packets
{
    internal class SMSG_ADDSPECIALSKILLREPLY : RelayPacket
    {
        public SMSG_ADDSPECIALSKILLREPLY()
        {
            this.Cmd = 0x0601;
            this.Id = 0x091D;
            this.data = new byte[1];
        }

        public byte result
        {
            set
            {
                this.data[0] = value;
            }
        }
    }
}