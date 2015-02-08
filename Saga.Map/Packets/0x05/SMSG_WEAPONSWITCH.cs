using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    /// Switches between weapon
    /// </summary>
    /// <remarks>
    /// This packet is sent to the player to indicate she/he
    /// is switching weapons. To other people in the viewrange
    /// are updated using SMSG_SHOWWEAPON
    /// </remarks>
    /// <id>
    /// 050A
    /// </id>
    internal class SMSG_WEAPONSWITCH : RelayPacket
    {
        public SMSG_WEAPONSWITCH()
        {
            this.Cmd = 0x0601;
            this.Id = 0x050A;
            this.data = new byte[2];
        }

        public byte slotid
        {
            set
            {
                this.data[0] = value;
            }
        }
    }
}