using Saga.PrimaryTypes;
using System;

namespace Saga.Structures.Collections
{
    [Serializable()]
    public class WeaponCollection
    {
        /// <summary>
        /// Container for the amount of unlocked weapons
        /// </summary>
        private byte unlockedweapon = 1;

        /// <summary>
        /// Defines a active weapon index (0 for Left hand, 1 for right hand)
        /// </summary>
        private byte activeweaponindex = 0;

        /// <summary>
        /// Defines the weapon index of the left hand
        /// </summary>
        private byte primaryweaponindex = 0;

        /// <summary>
        /// Defines the weapon index of the right hand
        /// </summary>
        private byte secondairyweaponindex = 255;

        /// <summary>
        /// Container for the weapons (maximum count is 5)
        /// </summary>
        private Weapon[] weapons = new Weapon[5];

        /// <summary>
        /// Get's or Sets a weapon at the specified index
        /// </summary>
        /// <param name="index">Zero based index where to set the weapon</param>
        /// <returns>Weapon</returns>
        public Weapon this[int index]
        {
            get
            {
                return weapons[index];
            }
            internal set
            {
                weapons[index] = value;
            }
        }

        /// <summary>
        /// Get's or set's the number of unlocked weapon slots (maximum 5)
        /// </summary>
        public byte UnlockedWeaponSlots
        {
            get
            {
                return unlockedweapon;
            }
            internal set
            {
                unlockedweapon = value;
            }
        }

        /// <summary>
        /// Get's or set's the index of the primary weapon
        /// </summary>
        /// <remarks>
        /// Set this value to 255 to use hands. A value between 0 - 5 will
        /// try to get the weapon at the specified index.
        /// </remarks>
        public byte PrimaryWeaponIndex
        {
            get
            {
                return primaryweaponindex;
            }
            internal set
            {
                primaryweaponindex = value;
            }
        }

        /// <summary>
        /// Get's or set's the index of the seccondairy weapon
        /// </summary>
        /// <remarks>
        /// Set this value to 255 to use hands. A value between 0 - 5 will
        /// try to get the weapon at the specified index.
        /// </remarks>
        public byte SeconairyWeaponIndex
        {
            get
            {
                return secondairyweaponindex;
            }
            internal set
            {
                secondairyweaponindex = value;
            }
        }

        /// <summary>
        /// Get's or sets the active weapon index
        /// </summary>
        /// <remarks>
        /// Use 0 for using the left hand and 1 for using the right hand.
        /// </remarks>
        public byte ActiveWeaponIndex
        {
            get
            {
                return activeweaponindex;
            }
            internal set
            {
                activeweaponindex = value;
            }
        }

        /// <summary>
        /// Checks if the supplied weapon slot is the active slot
        /// </summary>
        /// <param name="slot">Zero-based index</param>
        /// <returns>True if the weapon is active</returns>
        public bool IsActiveSlot(byte slot)
        {
            int ActiveWeaponIndex = (activeweaponindex == 1) ? secondairyweaponindex : primaryweaponindex;
            return ActiveWeaponIndex == slot;
        }

        public byte GetCurrentWeaponType()
        {
            int ActiveWeaponIndex = (activeweaponindex == 1) ? secondairyweaponindex : primaryweaponindex;
            Weapon current = weapons[activeweaponindex];
            if (current != null && current._active == 1)
            {
                return current._weapontype;
            }

            return 0;
        }
    }
}