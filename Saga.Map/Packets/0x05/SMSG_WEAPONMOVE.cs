using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is the result of the weaponmove packet send by the player.
    /// It indicates how to change current weapon.
    /// </remarks>
    /// <id>
    /// 0508
    /// </id>
    internal class SMSG_WEAPONMOVE : RelayPacket
    {
        public SMSG_WEAPONMOVE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0508;
            this.data = new byte[3];
        }

        public byte Unknown1
        {
            set
            {
                this.data[0] = value;
            }
        }

        public sbyte Unknown2
        {
            set
            {
                this.data[1] = (byte)value;
            }
        }

        public byte Weaponslot
        {
            set
            {
                this.data[2] = value;
            }
        }
    }
}