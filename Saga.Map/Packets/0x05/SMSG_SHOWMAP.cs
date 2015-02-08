using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to the player to reveal a unrevealed piece of region on
    /// there map. After the unrevealed region is learned it is always known and should be
    /// send over with with the mapload function.
    /// </remarks>
    /// <id>
    /// 0522
    /// </id>
    internal class SMSG_SHOWMAP : RelayPacket
    {
        /// <summary>
        /// Initializes a new SMSG_SHOWMAP packet
        /// </summary>
        public SMSG_SHOWMAP()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0522;
            this.data = new byte[3];
        }

        /// <summary>
        /// Error reason
        /// </summary>
        public byte Reason
        {
            set { this.data[0] = value; }
        }

        /// <summary>
        /// Map of the zone
        /// </summary>
        public byte Map
        {
            set { this.data[1] = value; }
        }

        /// <summary>
        /// Bitflag of visible zones
        /// </summary>
        public byte Zone
        {
            set { this.data[2] = value; }
        }
    }
}