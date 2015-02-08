using Saga.Configuration;
using Saga.Map.Configuration;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Saga.Factory
{
    public class Spells : FactoryBase
    {
        protected static BooleanSwitch refenceWarningsAsErrors = new BooleanSwitch("SkillReferenceWarningsAsErrors", "Treat warnings as errors in the refence files", "1");

        #region Ctor/Dtor

        public Spells()
        {
        }

        #endregion Ctor/Dtor

        #region Internal Members

        private Dictionary<uint, SkillHandler> methods = new Dictionary<uint, SkillHandler>();
        public Dictionary<uint, Info> spells;

        internal SkillHandler PhysicalAttack;
        internal SkillHandler MagicalAttack;
        internal SkillHandler RangedAttack;

        #endregion Internal Members

        #region Protected Methods

        protected override void Initialize()
        {
            methods = new Dictionary<uint, SkillHandler>();
            spells = new Dictionary<uint, Info>();
            PhysicalAttack = new SkillHandler(BasePhysicalAttack);
            MagicalAttack = new SkillHandler(MagicalAttack);
            RangedAttack = new SkillHandler(RangedAttack);
        }

        protected override void Load()
        {
            SpellSettings section = (SpellSettings)ConfigurationManager.GetSection("Saga.Factory.Spells");
            if (section != null)
            {
                try
                {
                    string file = Saga.Structures.Server.SecurePath(section.Reference);
                    if (File.Exists(file))
                        ParseReferenceAsCsvStream(File.OpenRead(file));
                    else
                        WriteError("SpellFactory", "Missing reference file: {0}", section.Reference);
                }
                catch (IOException e)
                {
                    WriteError("SpellFactory", "Cannot open file {0}", e.Message);
                }
                catch (Exception e)
                {
                    WriteError("SpellFactory", "Unknown exception on {0} {1}", e.Source, e.Message);
                }

                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("SpellFactory", "Loading spell information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }

                WriteLine("SpellFactory", "Clear method lookup table");
                methods.Clear();
            }
            else
            {
                WriteWarning("XmlSections", "Section {0} was not found", "Saga.Factory.Spells");
            }
        }

        protected override void ParseAsXmlStream(Stream stream, FactoryBase.ReportProgress ProgressReport)
        {
            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                Info current = null;
                string value = null;
                while (reader.Read())
                {
                    try
                    {
                        ProgressReport.Invoke();
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name.ToUpperInvariant() == "SKILL") current = new Info();
                                value = null;
                                break;

                            case XmlNodeType.Text:
                                value = reader.Value;
                                break;

                            case XmlNodeType.EndElement:
                                string[] values;
                                switch (reader.Name.ToUpperInvariant())
                                {
                                    case "SKILL": goto Add;
                                    case "SKILLID": current.skillid = uint.Parse(value, NumberFormatInfo.InvariantInfo); current.skill = FindSkillHandler(current.skillid, methods); break;
                                    case "SKILLTYPE": current.skilltype = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "MAXSKILLEXP": current.maximumexperience = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "GROWLEVEL": current.maximumgrowlevel = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "MINRANGE": current.minimumrange = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "MAXRANGE": current.maximumrange = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "TARGET": current.target = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "CASTTIME": current.casttime = int.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "DELAY": current.delay = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "SP": current.SP = int.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "SPECIAL": current.special = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "SPECIALLVREQUIREMENT": current.specialJlvl = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "RACE": current.race = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "STANCE": current.stance = ConsoleUtils.ParseToUintArray(value); break;
                                    case "ADDITION": current.addition = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "HATEONCAST": current.requiredlp = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "HATE": current.hate = short.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "ATTACKTYPE": current.attacktype = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "ELEMENTTYPE": current.elementtype = byte.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                    case "WEAPONREQUIREMENT":
                                        values = value.Split(',');
                                        current.requiredWeapons = new byte[]
                                    {
                                        byte.Parse(values[0], NumberFormatInfo.InvariantInfo),  //HAND
                                        byte.Parse(values[1], NumberFormatInfo.InvariantInfo),  //SHORTSWORD
                                        byte.Parse(values[2], NumberFormatInfo.InvariantInfo),  //LONGSWORD
                                        byte.Parse(values[3], NumberFormatInfo.InvariantInfo),  //SWORDSTICK
                                        byte.Parse(values[4], NumberFormatInfo.InvariantInfo),  //DAMPTFLINTE
                                        byte.Parse(values[5], NumberFormatInfo.InvariantInfo),  //BOW
                                        byte.Parse(values[6], NumberFormatInfo.InvariantInfo),  //DAMPTSCHWERTZ
                                        byte.Parse(values[7], NumberFormatInfo.InvariantInfo),  //KATANA
                                        byte.Parse(values[8], NumberFormatInfo.InvariantInfo),  //SPECIALIST
                                    };
                                        break;

                                    case "JOBREQUIREMENT":
                                        values = value.Split(',');
                                        current.requiredJobs = new byte[]
                                    {
                                        byte.Parse(values[0], NumberFormatInfo.InvariantInfo),  //NOVICE
                                        byte.Parse(values[1], NumberFormatInfo.InvariantInfo),  //SWORDSMAN
                                        byte.Parse(values[3], NumberFormatInfo.InvariantInfo),  //RECRUIT
                                        byte.Parse(values[2], NumberFormatInfo.InvariantInfo),  //THIEF
                                        byte.Parse(values[4], NumberFormatInfo.InvariantInfo),  //ENCHANTER
                                        byte.Parse(values[5], NumberFormatInfo.InvariantInfo),  //CLOWN
                                        byte.Parse(values[6], NumberFormatInfo.InvariantInfo),  //KNIGHT
                                        byte.Parse(values[7], NumberFormatInfo.InvariantInfo),  //ASSASIN
                                        byte.Parse(values[8], NumberFormatInfo.InvariantInfo),  //SPECIALIST
                                        byte.Parse(values[9], NumberFormatInfo.InvariantInfo),  //SAGE
                                        byte.Parse(values[10], NumberFormatInfo.InvariantInfo), //GAMBLER
                                        byte.Parse(values[11], NumberFormatInfo.InvariantInfo), //FALCATA
                                        byte.Parse(values[12], NumberFormatInfo.InvariantInfo), //FPRSYTHIE
                                        byte.Parse(values[13], NumberFormatInfo.InvariantInfo), //NEMOPHILA
                                        byte.Parse(values[14], NumberFormatInfo.InvariantInfo)  //VEILCHENBLAU
                                    };
                                        break;
                                }
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new SystemException(string.Format("File caused a error at fileline: {0}", reader.LineNumber), e);
                    }
                    continue;
                Add:
                    try
                    {
                        current.skill = FindSkillHandler(current.skillid, methods);
                        this.spells.Add(current.skillid, current);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Duplicate Id detected {0}", current.skillid);
                    }
                }
            }
        }

        protected virtual void ParseReferenceAsCsvStream(System.IO.Stream stream)
        {
            using (StreamReader c = new StreamReader(stream))
            {
                c.ReadLine();
                while (c.Peek() > 0)
                {
                    CommaDelimitedString fields = CommaDelimitedString.Parse(c.ReadLine());

                    uint SkillId = 0;
                    bool IsValidInteger = uint.TryParse(fields[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out SkillId);
                    try
                    {
                        SkillHandler Handler = CoreService.Find<SkillHandler>(fields[1]);
                        if (Handler != null && IsValidInteger)
                        {
                            methods.Add(SkillId, Handler);
                        }
                        else
                        {
                            if (refenceWarningsAsErrors.Enabled)
                                WriteError("SkillManager", "Spell not loaded: {0} {1}", fields[0], fields[1]);
                            else
                                WriteWarning("SkillManager", "Spell not loaded: {0} {1}", fields[0], fields[1]);
                        }
                    }
                    catch (Exception e)
                    {
                        if (refenceWarningsAsErrors.Enabled)
                            WriteError("SkillManager", "Unhandeld exception {1} {0}", e.Message, e.Source);
                        else
                            WriteWarning("SkillManager", "Unhandeld exception {1} {0}", e.Message, e.Source);
                    }
                }
            }
        }

        /// <summary>
        /// This function performs a lookup against the skilltable to find the instanted
        /// callback if no additional callback is found a reference to the default skill
        /// is returned.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="methodlookup"></param>
        /// <returns></returns>
        protected SkillHandler FindSkillHandler(uint id, Dictionary<uint, SkillHandler> methodlookup)
        {
            SkillHandler Info;
            if (methodlookup.TryGetValue(id, out Info) == true)
            {
                return Info;
            }
            else
            {
                return Default;
            }
        }

        protected virtual void Default(SkillBaseEventArgs arguments)
        {
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Checks if a the specified spell is implanted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsSpellImplamented(uint id)
        {
            return spells.ContainsKey(id);
        }

        /// <summary>
        /// Tries to obtain the spellinformation.
        /// </summary>
        /// <remarks>
        /// These type of lookups should be prefered over a lookup like:
        /// if( Singleton.SpellManager.IsSpellImplamented(); )
        /// {
        ///     Singleton.SpellManager.TryCastSkill(...);
        /// }
        /// </remarks>
        /// <param name="id">id of the skill</param>
        /// <param name="spellInfo">output for spellinfo /param>
        /// <returns>True on success</returns>
        public bool TryGetSpell(uint skill, out Info spellInfo)
        {
            return this.spells.TryGetValue(skill, out spellInfo);
        }

        /// <summary>
        /// Checks if a skill can be used.
        /// </summary>
        /// <param name="character">Character to check on</param>
        /// <param name="info">Information to check</param>
        /// <returns>True if skill can be used</returns>
        public bool CanUse(Character character, Info info)
        {
            int WeaponType = 0;
            int Job = character.job - 1;

            int WeaponIndex = (character.weapons.ActiveWeaponIndex == 1) ? character.weapons.SeconairyWeaponIndex : character.weapons.PrimaryWeaponIndex;
            if (WeaponIndex < character.weapons.UnlockedWeaponSlots)
            {
                WeaponType = character.weapons[WeaponIndex]._weapontype;
            }

            bool jobAble = info.requiredJobs[Job] == 1;
            bool weaponAble = info.requiredWeapons[WeaponType] == 1;
            return jobAble && weaponAble;
        }

        #endregion Public Methods

        #region Protected Properties

        protected override string Notification
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_SPELLS; }
        }

        protected override string ReadyState
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_SPELLS; }
        }

        #endregion Protected Properties

        #region Nested Classes/Structures

        /// <summary>
        /// Supplies information about the specified skill
        /// </summary>
        [Serializable()]
        public class Info
        {
            public uint skillid;
            public byte skilltype;
            public uint maximumexperience;
            public byte maximumgrowlevel;
            public uint minimumrange;
            public uint maximumrange;
            public byte target;
            public int casttime;
            public uint delay;
            public byte attacktype;
            public byte elementtype;
            public int SP;
            public byte race;
            public byte special;
            public byte specialJlvl;
            public byte[] requiredJobs = new byte[15];
            public byte[] requiredWeapons = new byte[9];
            public uint addition;
            public uint[] stance = new uint[6];
            public byte requiredlp;
            public short hate = 0;

            [NonSerialized()]
            internal SkillHandler skill;

            public bool IsInRangeOf(int distance)
            {
                return (distance + 200) >= minimumrange && (distance - 200) <= maximumrange;
            }

            public bool IsTarget(MapObject source, MapObject target)
            {
                bool result = false;
                if ((this.target & 1) == 1)
                    result |= target.id == source.id;
                if ((this.target & 2) == 2)
                    result |= MapObject.IsMonster(target) || MapObject.IsMapItem(target);
                if ((this.target & 4) == 4)
                    result |= MapObject.IsNotMonster(target) || MapObject.IsMonster(target) || MapObject.IsPlayer(target);
                return result;
            }
        }

        /// <summary>
        /// Delegate containing the method signature to invoke a spell.
        /// </summary>
        /// <param name="sender">Source who calls the spell</param>
        /// <param name="target">Target who the spell is casted on</param>
        /// <param name="arguments">Arguments</param>
        protected internal delegate void SkillHandler(SkillBaseEventArgs arguments);

        #endregion Nested Classes/Structures

        #region Nested Skills

        private static void BasePhysicalAttack(SkillBaseEventArgs bargument)
        {
            int Lvldiff;
            SkillUsageEventArgs.SkillMatrix matrix;
            Actor asource = bargument.Sender as Actor;
            Actor atarget = bargument.Target as Actor;

            if (asource != null && atarget != null && bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                matrix = arguments.GetPhysicalSkillMatrix(asource, atarget);
                Lvldiff = arguments.GetCappedLevelDifference(matrix);
                matrix[4, 3] += (Lvldiff * 120);
                matrix[0, 1] = 0; matrix[1, 1] = 0;

                if (arguments.IsMissed(matrix) || arguments.IsBlocked(matrix))
                {
                    return;
                }
                else
                {
                    arguments.CanCheckEquipmentDurabillity = true;
                    arguments.CanCheckWeaponDurabillity = true;
                    arguments.Damage = arguments.GetDamage(matrix);
                    arguments.Damage = arguments.GetDefenseReduction(matrix, arguments.Damage);
                    arguments.IsCritical(matrix);
                }
            }
            else
            {
                bargument.Failed = true;
            }
        }

        private static void BaseRangedAttack(SkillBaseEventArgs bargument)
        {
            int Lvldiff;
            SkillUsageEventArgs.SkillMatrix matrix;
            Actor asource = bargument.Sender as Actor;
            Actor atarget = bargument.Target as Actor;

            if (asource != null && atarget != null && bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                matrix = arguments.GetRangedSkillMatrix(asource, atarget);
                Lvldiff = arguments.GetCappedLevelDifference(matrix);
                matrix[4, 3] += (Lvldiff * 120);
                matrix[0, 1] = 0; matrix[1, 1] = 0;

                if (arguments.IsMissed(matrix) || arguments.IsBlocked(matrix))
                {
                    return;
                }
                else
                {
                    arguments.CanCheckEquipmentDurabillity = true;
                    arguments.CanCheckWeaponDurabillity = true;
                    arguments.Damage = arguments.GetDamage(matrix);
                    arguments.Damage = arguments.GetDefenseReduction(matrix, arguments.Damage);
                    arguments.IsCritical(matrix);
                }
            }
            else
            {
                bargument.Failed = true;
            }
        }

        private static void BaseMagicalAttack(SkillBaseEventArgs bargument)
        {
            int Lvldiff;
            SkillUsageEventArgs.SkillMatrix matrix;
            Actor asource = bargument.Sender as Actor;
            Actor atarget = bargument.Target as Actor;

            if (asource != null && atarget != null && bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                matrix = arguments.GetMagicalSkillMatrix(asource, atarget);
                Lvldiff = arguments.GetCappedLevelDifference(matrix);
                matrix[4, 3] += (Lvldiff * 120);
                matrix[0, 1] = 0; matrix[1, 1] = 0;

                if (arguments.IsMissed(matrix) || arguments.IsBlocked(matrix))
                {
                    return;
                }
                else
                {
                    arguments.CanCheckEquipmentDurabillity = true;
                    arguments.CanCheckWeaponDurabillity = true;
                    arguments.Damage = arguments.GetDamage(matrix);
                    arguments.Damage = arguments.GetDefenseReduction(matrix, arguments.Damage);
                    arguments.IsCritical(matrix);
                }
            }
            else
            {
                bargument.Failed = true;
            }
        }

        #endregion Nested Skills
    }
}