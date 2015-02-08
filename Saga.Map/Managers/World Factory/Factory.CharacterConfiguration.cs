using Saga.Configuration;
using Saga.Map;
using Saga.Map.Configuration;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace Saga.Factory
{
    public class CharacterConfiguration : FactoryBase
    {
        #region Ctor/Dtor

        public CharacterConfiguration()
        {
        }

        ~CharacterConfiguration()
        {
            this.maps = null;
        }

        #endregion Ctor/Dtor

        #region Internal Members

        /// <summary>
        /// Lookuptable to contain the base character configuration per class
        /// </summary>
        public Dictionary<byte, Info> maps;

        //Contains all startup information for norman race
        private NormanDefaultConfiguration normanConfiguration = new NormanDefaultConfiguration();

        //Contains all startup information for ellr race
        //Note: Ellr are currently not supported but future references
        private IDefaultCharacterSettings ellrConfiguration;

        //Contains all startup information for dimago race
        //Note: Dimago are currently not supported but future references
        private IDefaultCharacterSettings dimagoConfiguration;

        #endregion Internal Members

        #region Protected Methods

        protected override void Initialize()
        {
            maps = new Dictionary<byte, Info>();
        }

        protected override void Load()
        {
            CharacterConfigurationSettings section = (CharacterConfigurationSettings)ConfigurationManager.GetSection("Saga.Factory.CharacterConfiguration");
            if (section != null)
            {
                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("CharacterConfigurationFactory", "Loading character information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }
            }
            else
            {
                WriteWarning("XmlSections", "Section Saga.Factory.CharacterConfiguration");
            }
        }

        protected override void ParseAsCsvStream(Stream stream, FactoryBase.ReportProgress ProgressReport)
        {
            using (StreamReader c = new StreamReader(stream))
            {
                c.ReadLine();
                while (c.Peek() > 0)
                {
                    //REPORT PROGRESS
                    ProgressReport.Invoke();
                    String row = c.ReadLine();
                    String[] fields = row.Split(',');

                    maps.Add(

                        //ASSOCIATE MAP-ID WITH ZONE INSTANCE
                        byte.Parse(fields[1], NumberFormatInfo.InvariantInfo),

                        //CREATE NEW CHARACTER INFO
                        new Info
                        (
                            ushort.Parse(fields[2], NumberFormatInfo.InvariantInfo),
                            ushort.Parse(fields[3], NumberFormatInfo.InvariantInfo),
                            ushort.Parse(fields[4], NumberFormatInfo.InvariantInfo)
                        )
                    );
                }
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Checks if the specified character-configuration is hosted by the server.
        /// </summary>
        /// <param name="id">Job of the character</param>
        /// <returns></returns>
        public bool IsCharacterConfigurationFound(byte id)
        {
            return maps.ContainsKey(id);
        }

        /// <summary>
        /// Try to get the specified character configuration.
        /// </summary>
        /// <param name="id">Job of the character</param>
        /// <param name="configuration">Found configuration element</param>
        /// <returns></returns>
        public bool TryGetCharacterConfiguration(byte id, out Info configuration)
        {
            return maps.TryGetValue(id, out configuration);
        }

        /// <summary>
        /// Appplies the character configuration to the specified character.
        /// </summary>
        /// <remarks>
        /// Character configuration is job based. So we need to determine the
        /// base stats.
        /// </remarks>
        /// <param name="character"></param>
        public void ApplyConfiguration(Character character)
        {
            Saga.Factory.CharacterConfiguration.Info info;
            if (Singleton.CharacterConfiguration.TryGetCharacterConfiguration(character.job, out info))
            {
                character._status.BaseMaxPAttack = info.PhysicalAttack;
                character._status.BaseMaxRAttack = info.PhysicalRangedAttack;
                character._status.BaseMaxMAttack = info.MagicalAttack;
            }
        }

        #endregion Public Methods

        #region Public Methods

        /// <summary>
        /// Calculates the Maximum SP of a given character.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public ushort CalculateMaximumSP(Character target)
        {
            if (target.job == 5)
                return 210;
            else
                return 110;
        }

        /// <summary>
        /// Calculates the maximum HP of a given character.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public ushort CalculateMaximumHP(Character target)
        {
            ushort result = Convert.ToUInt16(200 + ((target._level - 1) * 40));
            return result;
        }

        #endregion Public Methods

        #region Public Properties

        ///<summary>
        ///Contains all startup information for norman race
        /// </summary>
        public IDefaultCharacterSettings Normans
        {
            get
            {
                return normanConfiguration;
            }
        }

        ///<summary>
        ///Contains all startup information for ellr race
        ///</summary>
        ///<remarks>
        ///Note: Ellr are currently not supported but future references
        ///</remarks>
        public IDefaultCharacterSettings Ellr
        {
            get
            {
                return ellrConfiguration;
            }
        }

        ///<summary>
        ///Contains all startup information for Dimago race
        ///</summary>
        ///<remarks>
        ///Note: Dimago are currently not supported but future references
        ///</remarks>
        public IDefaultCharacterSettings Dimago
        {
            get
            {
                return dimagoConfiguration;
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
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_CHARACTERCONFIGURATION; }
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
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_CHARACTERCONFIGURATION; }
        }

        #endregion Protected Properties

        #region Nested Classes/Structures

        /// <summary>
        /// Contains the character configuration
        /// </summary>
        public class Info
        {
            /// <summary>
            /// Maximum Physical Attack
            /// </summary>
            public ushort PhysicalAttack = 0;

            /// <summary>
            /// Maximum Ranged Attack
            /// </summary>
            public ushort PhysicalRangedAttack = 0;

            /// <summary>
            /// Maximum Magical Attack
            /// </summary>
            public ushort MagicalAttack = 0;

            /// <summary>
            /// Creates a new information entry
            /// </summary>
            /// <param name="PhysicalAttack">Maximum p. attack</param>
            /// <param name="PhysicalRangedAttack">Maximum p. r. attack</param>
            /// <param name="MagicalAttack">Maximum m. attack</param>
            public Info(ushort PhysicalAttack, ushort PhysicalRangedAttack, ushort MagicalAttack)
            {
                this.PhysicalAttack = PhysicalAttack;
                this.PhysicalRangedAttack = PhysicalRangedAttack;
                this.MagicalAttack = MagicalAttack;
            }
        }

        public interface IDefaultCharacterSettings
        {
            WorldCoordinate DefaultLocation { get; }

            WorldCoordinate DefaultSaveLocation { get; }

            bool create(out Character c, CharCreationArgument e);
        }

        private class NormanDefaultConfiguration : IDefaultCharacterSettings
        {
            public WorldCoordinate DefaultLocation
            {
                get
                {
                    return new WorldCoordinate(new Point(-17208f, 9944f, 108f), 11);
                }
            }

            public WorldCoordinate DefaultSaveLocation
            {
                get
                {
                    //No default save location
                    return new WorldCoordinate(new Point(0, 0, 0), 0);
                }
            }

            WorldCoordinate IDefaultCharacterSettings.DefaultLocation
            {
                get
                {
                    return DefaultLocation;
                }
            }

            WorldCoordinate IDefaultCharacterSettings.DefaultSaveLocation
            {
                get
                {
                    return DefaultSaveLocation;
                }
            }

            bool IDefaultCharacterSettings.create(out Character character, CharCreationArgument e)
            {
                //---------------------------------------------------------------------------
                //Create temp character to contain our changes
                //---------------------------------------------------------------------------
                Character tempChar = new Character(null, 0, 0);
                tempChar.Name = e.CharName;
                tempChar.FaceDetails = e.FaceDetails;

                //---------------------------------------------------------------------------
                //Set race spefic positions
                //---------------------------------------------------------------------------
                //Set default last location from character configuration
                tempChar.lastlocation = DefaultSaveLocation;
                //Set default save location from character configuration
                tempChar.savelocation = DefaultSaveLocation;
                //Set default starting location from character configuration
                tempChar.map = DefaultLocation.map;
                tempChar.Position = DefaultLocation.coords;

                //---------------------------------------------------------------------------
                //Add inventory items
                //---------------------------------------------------------------------------
                Rag2Item item;
                if (Singleton.Item.TryGetItemWithCount(4075, 10, out item))
                    tempChar.container.Add(item);
                if (Singleton.Item.TryGetItemWithCount(16061, 1, out item))
                    tempChar.container.Add(item);
                if (Singleton.Item.TryGetItemWithCount(4074, 1, out item))
                    tempChar.container.Add(item);
                if (Singleton.Item.TryGetItemWithCount(4076, 1, out item))
                    tempChar.container.Add(item);
                if (Singleton.Item.TryGetItemWithCount(4252, 1, out item))
                    tempChar.container.Add(item);
                if (Singleton.Item.TryGetItemWithCount(4253, 1, out item))
                    tempChar.container.Add(item);

                //---------------------------------------------------------------------------
                //Add a weapon to your character
                //---------------------------------------------------------------------------
                tempChar.weapons[0] = new Weapon();
                tempChar.weapons[0]._weaponname = e.WeaponName;
                tempChar.weapons[0]._weapontype = 1;
                tempChar.weapons[0]._augeskill = 150001;
                tempChar.weapons[0]._weaponlevel = 1;
                tempChar.weapons[0]._suffix = e.WeaponAffix;
                tempChar.weapons[0]._active = 1;
                if (Singleton.Weapons.TryGetWeaponInfo(tempChar.weapons[0]._weapontype, tempChar.weapons[0]._weaponlevel, out tempChar.weapons[0].Info))
                    tempChar.weapons[0]._durabillity = (ushort)tempChar.weapons[0].Info.max_durabillity;

                //---------------------------------------------------------------------------
                //Create a character by adding it to the database.
                //If the character is created than add the new skills to the database
                //---------------------------------------------------------------------------
                if (Singleton.Database.TransactionInsert(tempChar, e.UserId))
                {
                    Singleton.Database.InsertNewSkill(tempChar.ModelId, 1407401, 1);
                    Singleton.Database.InsertNewSkill(tempChar.ModelId, 1406901, 1);
                    character = tempChar;
                    return tempChar.ModelId > 0;
                }
                else
                {
                    character = tempChar;
                    return false;
                }
            }
        }

        #endregion Nested Classes/Structures
    }
}