using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet that indicates wheter or not a upgrade has been sucessfull.
    /// </remarks>
    /// <id>
    /// 0517
    /// </id>
    internal class SMSG_WEAPONUPGRADE : RelayPacket
    {
        public SMSG_WEAPONUPGRADE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0517;
            this.data = new byte[1];
        }

        public byte Result
        {
            set { this.data[0] = value; }
        }
    }
}