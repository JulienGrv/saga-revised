using Saga.Network.Packets;
using Saga.PrimaryTypes;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet sends the current weaponary list and
    /// their information. This function is send as beeing apart of the
    /// map-load function. All other weapon updates should be incremental.
    /// </remarks>
    /// <id>
    /// 0510
    /// </id>
    internal class SMSG_WEAPONLIST : RelayPacket
    {
        public SMSG_WEAPONLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0510;
            this.data = new byte[375];
        }

        private int i = 0;

        public void AddWeapon(Weapon weapon)
        {
            if (weapon != null && i <= 300)
                Weapon.Serialize(weapon, this.data, i);
            i += 75;
        }
    }
}