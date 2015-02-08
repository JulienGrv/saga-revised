using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet shows a set of submenu items.
    /// </remarks>
    /// <id>
    /// 0603
    /// </id>
    internal class SMSG_NPCMENU : RelayPacket
    {
        public SMSG_NPCMENU()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0603;
            this.data = new byte[2];
        }

        public byte ButtonID
        {
            set { this.data[0] = value; }
        }

        public byte MenuID
        {
            set { this.data[1] = value; }
        }
    }
}