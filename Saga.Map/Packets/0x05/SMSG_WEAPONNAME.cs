using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet creates a new weapon
    /// </remarks>
    /// <id>
    /// 0520
    /// </id>
    internal class SMSG_WEAPONNAME : RelayPacket
    {
        public SMSG_WEAPONNAME()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0520;
            this.data = new byte[1];
        }
    }
}