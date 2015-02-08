using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet contains a list of all posible jobs you are
    /// allowed to switch/change to.
    /// </remarks>
    /// <id>
    /// 0312
    /// </id>
    internal class SMSG_JOBCHANGE : RelayPacket
    {
        public SMSG_JOBCHANGE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0312;
            this.data = new byte[12];
        }

        public void Add(byte value)
        {
            this.data[++this.data[0]] = value;
        }
    }
}