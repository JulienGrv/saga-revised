using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to the user to resize the
    /// number of unlocked weapons.After this is unlocked
    /// you can use weapons on the slots less than maximum count.
    /// </remarks>
    /// <id>
    /// 050D
    /// </id>
    internal class SMSG_WEAPONMAX : RelayPacket
    {
        public SMSG_WEAPONMAX()
        {
            this.Cmd = 0x0601;
            this.Id = 0x050D;
            this.data = new byte[2];
        }

        public byte Result
        {
            set
            {
                this.data[0] = value;
            }
        }

        public byte Count
        {
            set
            {
                this.data[1] = value;
            }
        }
    }
}