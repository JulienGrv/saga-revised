using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet indactes the respawn maps for when you die/use the promise stone.
    /// The IsSaveLocationSet indicate if the user can be warped to their save
    /// locations when they die.
    ///
    /// The from map indicates the save-position.
    /// The to-map indicates the catheleya-position.
    /// </remarks>
    /// <id>
    /// 031C
    /// </id>
    internal class SMSG_RETURNMAPLIST : RelayPacket
    {
        public SMSG_RETURNMAPLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x031C;
            this.data = new byte[3];
        }

        public byte IsSaveLocationSet
        {
            set { this.data[0] = value; }
        }

        public byte FromMap
        {
            set { this.data[1] = value; }
        }

        public byte ToMap
        {
            set { this.data[2] = value; }
        }
    }
}