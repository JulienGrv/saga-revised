using Saga.Map;
using System;
using System.Diagnostics;
using System.Text;

namespace Saga.PrimaryTypes
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [Serializable()]
    public class Weapon
    {
        #region Internal Members

        /// <summary>
        /// Weapon level of the weapon
        /// </summary>
        internal byte _weaponlevel;

        /// <summary>
        /// Weapon type of the weapon
        /// </summary>
        internal byte _weapontype;

        /// <summary>
        /// Activation state of the weapon
        /// </summary>
        internal byte _active = 0;

        /// <summary>
        /// Type weapon
        /// </summary>
        internal ushort _type;

        /// <summary>
        /// Durabillity of the weapon
        /// </summary>
        internal ushort _durabillity;

        /// <summary>
        /// Suffix of the weapon
        /// </summary>
        internal ushort _suffix;

        /// <summary>
        /// Augeskill of the weapon
        /// </summary>
        internal uint _augeskill;

        /// <summary>
        /// Experience of the weapon
        /// </summary>
        internal uint _experience;

        /// <summary>
        /// Fusion stone of the weapon
        /// </summary>
        internal uint _fusion;

        /// <summary>
        /// Alterstone slots of the weapon (max 8)
        /// </summary>
        internal uint[] Slots = new uint[8];

        /// <summary>
        /// Weapon namae
        /// </summary>
        internal string _weaponname = string.Empty;

        /// <summary>
        /// Weapon info loaded from our weaponary manager
        /// </summary>
        [NonSerialized()]
        internal Saga.Factory.Weaponary.Info Info;

        #endregion Internal Members

        #region Serialization

        /// <summary>
        /// Serialized the specified weapon info binairy bytes.
        /// </summary>
        /// <param name="weapon">Weapon to serialize</param>
        /// <param name="buffer">Buffer containing the bytes (requires min. 75 bytes)</param>
        /// <param name="offset">Offset starting to serialize</param>
        [DebuggerNonUserCode()]
        public static void Serialize(Weapon weapon, byte[] buffer, int offset)
        {
            try
            {
                Encoding.Unicode.GetBytes(weapon._weaponname, 0, Math.Min(11, weapon._weaponname.Length), buffer, offset);
                //----------
                buffer[offset + 24] = weapon._weaponlevel;
                Array.Copy(BitConverter.GetBytes(weapon._experience), 0, buffer, offset + 25, 4);
                buffer[offset + 29] = weapon._weapontype;
                //-----------
                Array.Copy(BitConverter.GetBytes(weapon._durabillity), 0, buffer, offset + 30, 2);
                Array.Copy(BitConverter.GetBytes(weapon._suffix), 0, buffer, offset + 32, 2);
                //-----------
                Array.Copy(BitConverter.GetBytes(weapon._augeskill), 0, buffer, offset + 34, 4);
                Array.Copy(BitConverter.GetBytes(weapon._fusion), 0, buffer, offset + 38, 4);
                //------------
                Array.Copy(BitConverter.GetBytes(weapon.Slots[0]), 0, buffer, offset + 42, 4);
                Array.Copy(BitConverter.GetBytes(weapon.Slots[1]), 0, buffer, offset + 46, 4);
                Array.Copy(BitConverter.GetBytes(weapon.Slots[2]), 0, buffer, offset + 50, 4);
                Array.Copy(BitConverter.GetBytes(weapon.Slots[3]), 0, buffer, offset + 54, 4);
                //------------
                Array.Copy(BitConverter.GetBytes(weapon.Slots[4]), 0, buffer, offset + 58, 4);
                Array.Copy(BitConverter.GetBytes(weapon.Slots[5]), 0, buffer, offset + 62, 4);
                Array.Copy(BitConverter.GetBytes(weapon.Slots[6]), 0, buffer, offset + 66, 4);
                Array.Copy(BitConverter.GetBytes(weapon.Slots[7]), 0, buffer, offset + 70, 4);
                buffer[offset + 74] = weapon._active;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Deserialized the specified buffer into a weapon
        /// </summary>
        /// <param name="buffer">Buffer containing the data to deserialize</param>
        /// <param name="offset">Offset starting to deserialize</param>
        /// <returns>Weapon</returns>
        [DebuggerNonUserCode()]
        public static Weapon Deserialize(byte[] buffer, int offset)
        {
            Weapon weapon = new Weapon();
            weapon._weaponname = UnicodeEncoding.Unicode.GetString(buffer, offset, 24);
            //-----------
            weapon._weaponlevel = buffer[offset + 24];
            weapon._experience = BitConverter.ToUInt32(buffer, offset + 25);
            weapon._weapontype = buffer[offset + 29];
            //-----------
            weapon._durabillity = BitConverter.ToUInt16(buffer, offset + 30);
            weapon._suffix = BitConverter.ToUInt16(buffer, offset + 32);
            //-----------
            weapon._augeskill = BitConverter.ToUInt32(buffer, offset + 34);
            weapon._fusion = BitConverter.ToUInt32(buffer, offset + 38);
            //-----------
            weapon.Slots[0] = BitConverter.ToUInt32(buffer, offset + 42);
            weapon.Slots[1] = BitConverter.ToUInt32(buffer, offset + 46);
            weapon.Slots[2] = BitConverter.ToUInt32(buffer, offset + 50);
            weapon.Slots[3] = BitConverter.ToUInt32(buffer, offset + 54);
            //-----------
            weapon.Slots[4] = BitConverter.ToUInt32(buffer, offset + 58);
            weapon.Slots[5] = BitConverter.ToUInt32(buffer, offset + 62);
            weapon.Slots[6] = BitConverter.ToUInt32(buffer, offset + 66);
            weapon.Slots[7] = BitConverter.ToUInt32(buffer, offset + 70);
            weapon._active = buffer[offset + 74];

            if (Singleton.Weapons.TryGetWeaponInfo(weapon._weapontype, weapon._weaponlevel, out weapon.Info))
                return weapon;
            else
                return null;
        }

        #endregion Serialization
    }
}