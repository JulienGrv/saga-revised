using Saga.Configuration;
using Saga.Enumarations;
using Saga.Map;
using Saga.Map.Configuration;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace Saga.Factory
{
    /// <summary>
    /// Factory to supply weapon information
    /// </summary>
    public class Weaponary : FactoryBase
    {
        #region Ctor/Dtor

        /// <summary>
        /// Initializes a new weaponary factory
        /// </summary>
        public Weaponary()
        {
        }

        #endregion Ctor/Dtor

        #region Internal Members

        /// <summary>
        /// Lookup table to supply to find weapon information.
        /// </summary>
        /// <remarks>
        /// The values entered are index by weapontype/level
        /// </remarks>
        public Dictionary<uint, Dictionary<uint, Info>> weapons;

        #endregion Internal Members

        #region Protected Methods

        /// <summary>
        /// Initializes all member variables
        /// </summary>
        protected override void Initialize()
        {
            weapons = new Dictionary<uint, Dictionary<uint, Info>>();
        }

        /// <summary>
        /// Loads all listed files that contains weaponary information
        /// </summary>
        protected override void Load()
        {
            WeaponarySettings section = (WeaponarySettings)ConfigurationManager.GetSection("Saga.Factory.Weaponary");
            if (section != null)
            {
                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("WeaponFactory", "Loading weapon information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }
            }
            else
            {
                WriteWarning("XmlSections", "Section {0} was not found", "Saga.Factory.Weaponary");
            }
        }

        /// <summary>
        /// Default included event that invokes a csv based stream.
        /// </summary>
        /// <param name="stream">Stream to read data from</param>
        /// <param name="ProgressReport">Class to report the state of reading</param>
        protected override void ParseAsCsvStream(Stream stream, FactoryBase.ReportProgress ProgressReport)
        {
            using (StreamReader c = new StreamReader(stream))
            {
                c.ReadLine();
                while (c.Peek() > 0)
                {
                    ProgressReport.Invoke();
                    String row = c.ReadLine();
                    String[] fields = row.Split(',');

                    uint weapontype = uint.Parse(fields[0], NumberFormatInfo.InvariantInfo);

                    Dictionary<uint, Info> temp;
                    bool isnew = weapons.TryGetValue(weapontype, out temp);
                    if (isnew == false) temp = new Dictionary<uint, Info>();

                    uint key = uint.Parse(fields[1], NumberFormatInfo.InvariantInfo);
                    uint maxdura = uint.Parse(fields[2], NumberFormatInfo.InvariantInfo);
                    ushort minshortatk = ushort.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                    ushort maxshortatk = ushort.Parse(fields[4], NumberFormatInfo.InvariantInfo);
                    ushort minrangeatk = ushort.Parse(fields[5], NumberFormatInfo.InvariantInfo);
                    ushort maxrangeatk = ushort.Parse(fields[6], NumberFormatInfo.InvariantInfo);
                    ushort minmagicatk = ushort.Parse(fields[7], NumberFormatInfo.InvariantInfo);
                    ushort maxmagicatk = ushort.Parse(fields[8], NumberFormatInfo.InvariantInfo);
                    uint weaponskill = uint.Parse(fields[10], NumberFormatInfo.InvariantInfo);
                    uint unknown = uint.Parse(fields[9], NumberFormatInfo.InvariantInfo);

                    temp.Add(key, new Info(
                            maxdura, minshortatk, maxshortatk,
                            minrangeatk, maxrangeatk, minmagicatk,
                            maxmagicatk, weaponskill, unknown
                            ));

                    if (isnew == false)
                        weapons.Add(weapontype, temp);
                }
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Checks if the weapon information is available
        /// </summary>
        /// <param name="type">Weapontype of the request weapon</param>
        /// <param name="level">Level of the supplied weapon</param>
        /// <returns>True if the weaponinformation was found</returns>
        public bool HasWeaponInfo(uint type, uint level)
        {
            try
            {
                Info tmp = this.weapons[type][level];
                return true;
            }
            catch (System.NullReferenceException)
            {
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to get the weapon information.
        /// </summary>
        /// <param name="type">Weapontype of the request weapon</param>
        /// <param name="level">Level of the request weapon</param>
        /// <param name="weaponinfo">Weapon information</param>
        /// <returns>True if the weaponinformation was found</returns>
        public bool TryGetWeaponInfo(uint type, uint level, out Info weaponinfo)
        {
            try
            {
                weaponinfo = this.weapons[type][level];
                return true;
            }
            catch (System.NullReferenceException)
            {
                weaponinfo = new Info();
                return false;
            }
            catch (KeyNotFoundException)
            {
                weaponinfo = new Info();
                return false;
            }
        }

        /// <summary>
        /// Assisting method that changes the weapon's capabillities.
        /// </summary>
        /// <remarks>
        /// When changing the weapons information don't forget update the battle status.
        /// If the battlestatus of the actor is not updated there will be synchronisation
        /// issues.
        /// </remarks>
        /// <param name="job">Job of the new change</param>
        /// <param name="suffix">Suffix after the change</param>
        /// <param name="selectedWeapon">Weapon where to apply the change</param>
        /// <returns>True if changing succeeds</returns>
        public bool ChangeWeapon(byte job, ushort suffix, Weapon selectedWeapon)
        {
            switch ((JobType)job)
            {
                case JobType.Enchanter:
                    selectedWeapon._type = (ushort)WeaponType.Swordstick;
                    selectedWeapon._augeskill = 150029;
                    selectedWeapon._weapontype = (byte)WeaponType.Swordstick;
                    break;

                case JobType.Swordsman:
                    selectedWeapon._type = (ushort)WeaponType.LongSword;
                    selectedWeapon._augeskill = 150015;
                    selectedWeapon._weapontype = (byte)WeaponType.LongSword;
                    break;

                case JobType.Thief:
                    selectedWeapon._type = (ushort)WeaponType.ShortSword;
                    selectedWeapon._augeskill = 150001;
                    selectedWeapon._weapontype = (byte)WeaponType.ShortSword;
                    break;

                case JobType.Recruit:
                    selectedWeapon._type = (ushort)WeaponType.Damptflinte;
                    selectedWeapon._augeskill = 150043;
                    selectedWeapon._weapontype = (byte)WeaponType.Damptflinte;
                    break;

                case JobType.Clown:
                    selectedWeapon._type = (ushort)WeaponType.ShortSword;
                    selectedWeapon._augeskill = 150001;
                    selectedWeapon._weapontype = (byte)WeaponType.ShortSword;
                    break;

                case JobType.Novice:
                    selectedWeapon._type = (ushort)WeaponType.ShortSword;
                    selectedWeapon._augeskill = 150001;
                    selectedWeapon._weapontype = (byte)WeaponType.ShortSword;
                    break;
            }

            //Restructure weapon
            selectedWeapon._durabillity = (selectedWeapon._durabillity > selectedWeapon.Info.max_durabillity) ? (ushort)selectedWeapon.Info.max_durabillity : selectedWeapon._durabillity;
            selectedWeapon.Slots = new uint[selectedWeapon.Slots.Length];
            selectedWeapon._suffix = suffix;
            Singleton.Weapons.TryGetWeaponInfo(selectedWeapon._type, selectedWeapon._weaponlevel, out selectedWeapon.Info);
            return true;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Gets the maximum supported weapon level
        /// </summary>
        public int MaximumWeaponLevel
        {
            get
            {
                return (int)Singleton.experience.MaxWLVL;
            }
        }

        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        /// Get the notification string.
        /// </summary>
        /// <remarks>
        /// We used notification strings from the resource files. This way it's easier
        /// for us to make a multilanguagable server. And a golden rule in C# is that
        /// strings are slow, so rather have it instanted once by the resource file than
        /// reallocting a new string for every progress report.
        /// </remarks>
        protected override string Notification
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_WEAPONARY; }
        }

        /// <summary>
        /// Get the readystate string.
        /// </summary>
        /// <remarks>
        /// We used readystate strings from the resource files. This way it's easier
        /// for us to make a multilanguagable server. And a golden rule in C# is that
        /// strings are slow, so rather have it instanted once by the resource file than
        /// reallocting a new string for every progress report.
        /// </remarks>
        protected override string ReadyState
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_WEAPONARY; }
        }

        #endregion Protected Properties

        #region Nested Classes/Structures

        /// <summary>
        /// Container for weapon information as used by the weaponary factory.
        /// </summary>
        public struct Info
        {
            /// <summary>
            /// Maximum durabillity for the weapon
            /// </summary>
            public uint max_durabillity;

            /// <summary>
            /// Minimum physical attack damage
            /// </summary>
            public ushort min_short_attack;

            /// <summary>
            /// Maximum physical attack damage
            /// </summary>
            public ushort max_short_attack;

            /// <summary>
            /// Minimum ranged attack damage
            /// </summary>
            public ushort min_range_attack;

            /// <summary>
            /// Maximum ranged attack damage
            /// </summary>
            public ushort max_range_attack;

            /// <summary>
            /// Minimum magical attack damage
            /// </summary>
            public ushort min_magic_attack;

            /// <summary>
            /// Maximum magical attack damage
            /// </summary>
            public ushort max_magic_attack;

            /// <summary>
            /// Required skill to use the weapon
            /// </summary>
            public uint weapon_skill;

            /// <summary>
            /// Skill that is associated to be used with the weapon
            /// </summary>
            public uint unknown;

            /// <summary>
            /// Initialized the structure with basic it's information
            /// </summary>
            /// <param name="max_durabillity">Maximum durabillity for the weapon</param>
            /// <param name="min_short_attack">Minimum physical attack damage</param>
            /// <param name="max_short_attack">Maximum physical attack damage</param>
            /// <param name="min_range_attack">Minimum ranged attack damage</param>
            /// <param name="max_range_attack">Maximum ranged attack damage</param>
            /// <param name="min_magic_attack">Minimum magical attack damage</param>
            /// <param name="max_magic_attack">Maximum magical attack damage</param>
            /// <param name="weapon_skill">Required skill to use the weapon</param>
            /// <param name="unknown">Skill that is associated to be used with the weapon</param>
            public Info(uint max_durabillity, ushort min_short_attack, ushort max_short_attack, ushort min_range_attack, ushort max_range_attack, ushort min_magic_attack, ushort max_magic_attack, uint weapon_skill, uint unknown)
            {
                this.max_durabillity = max_durabillity;
                this.min_short_attack = min_short_attack;
                this.max_short_attack = max_short_attack;
                this.min_range_attack = min_range_attack;
                this.max_range_attack = max_range_attack;
                this.min_magic_attack = min_magic_attack;
                this.max_magic_attack = max_magic_attack;
                this.weapon_skill = weapon_skill;
                this.unknown = unknown;
            }
        }

        #endregion Nested Classes/Structures
    }
}