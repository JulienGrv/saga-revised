using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent over as an reply indicating if an error occures
    /// when retrieving the item.
    /// </remarks>
    /// <id>
    /// 0C04
    /// </id>
    internal class SMSG_MAILITEMAWNSER : RelayPacket
    {
        public SMSG_MAILITEMAWNSER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C04;
            this.data = new byte[1];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }
    }
}