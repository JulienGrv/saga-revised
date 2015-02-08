using Saga.Network.Packets;
using Saga.PrimaryTypes;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to user to create a new weapon. The new
    /// weapon will be created on the specified index.
    /// </remarks>
    /// <id>
    /// 050B
    /// </id>
    internal class SMSG_WEAPONCREATE : RelayPacket
    {
        public SMSG_WEAPONCREATE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x050B;
            this.data = new byte[76];
        }

        public byte Index
        {
            set
            {
                this.data[0] = value;
            }
        }

        public Weapon weapon
        {
            set
            {
                if (value != null)
                    Weapon.Serialize(value, this.data, 1);
            }
        }
    }
}