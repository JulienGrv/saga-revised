using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Not quite sure what it does
    /// </remarks>
    /// <id>
    /// 0C03
    /// </id>
    internal class SMSG_MAILERROR : RelayPacket
    {
        public SMSG_MAILERROR()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C03;
            this.data = new byte[590];
        }

        public byte Unknown
        {
            set
            {
                this.data[0] = value;
                //this.data[521] = value;
            }
        }
    }
}