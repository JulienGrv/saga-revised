using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is depreciated since CB2. The client still preserves the packet however.
    /// due this reason this function is purely implacated for show.
    /// </remarks>
    /// <id>
    /// 0A02
    /// </id>
    internal class SMSG_DELETESHORTCUT : RelayPacket
    {
        public SMSG_DELETESHORTCUT()
        {
            this.data = new byte[1];
        }

        public byte Slot
        {
            set { this.data[0] = value; }
        }
    }
}