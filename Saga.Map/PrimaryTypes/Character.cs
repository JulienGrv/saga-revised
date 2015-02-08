using Saga.Map;
using Saga.Map.Client;
using Saga.Map.Definitions.Misc;
using Saga.Map.Utils.Definitions.Misc;
using Saga.Packets;
using Saga.Structures;
using Saga.Structures.Collections;
using Saga.Tasks;
using System;
using System.Collections.Generic;

namespace Saga.PrimaryTypes
{
    public delegate void RangeEventHandler(Character regionObject);

    /// <summary>
    /// Character class used for by the client
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [Serializable]
    public sealed partial class Character : Actor
    {
        //Character Members

        #region Character Constructor / Deconstructor

        public Character(uint CharacterId)
        {
            this.TickLogged = Environment.TickCount;
            this.LASTBREATH_TICK = Environment.TickCount;
            this.LASTHP_TICK = Environment.TickCount;
            //this.LASTLP_TICK = Environment.TickCount;
            this.LASTSP_TICK = Environment.TickCount;
            this.ModelId = CharacterId;
            this.CharacterJobLevel = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            this.FaceDetails = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            this.job = 1;
            this.Cexp = 1;
            this.Jexp = 1;
            this.jlvl = 1;
            this._level = 1;
            this._status.CurrentHp += Singleton.CharacterConfiguration.CalculateMaximumHP(this);
            this._status.CurrentSp += Singleton.CharacterConfiguration.CalculateMaximumSP(this);
            this._status.MaxHP += Singleton.CharacterConfiguration.CalculateMaximumHP(this);
            this._status.MaxSP += Singleton.CharacterConfiguration.CalculateMaximumSP(this);
        }

        /// <summary>
        /// Initialize a new character
        /// </summary>
        /// <param name="client"></param>
        public Character(Client client, uint CharacterId, uint SessionId)
        {
            this.client = client;
            this.TickLogged = Environment.TickCount;
            this.LASTBREATH_TICK = Environment.TickCount;
            this.LASTHP_TICK = Environment.TickCount;
            //this.LASTLP_TICK = Environment.TickCount;
            this.LASTSP_TICK = Environment.TickCount;
            this.id = SessionId;
            this.ModelId = CharacterId;
            this.CharacterJobLevel = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            this.FaceDetails = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            this.job = 1;
            this.Cexp = 1;
            this.Jexp = 1;
            this.jlvl = 1;
            this._level = 1;
            this._status.CurrentSp += Singleton.CharacterConfiguration.CalculateMaximumSP(this);
            this._status.CurrentHp += Singleton.CharacterConfiguration.CalculateMaximumHP(this);
            this._status.MaxSP += Singleton.CharacterConfiguration.CalculateMaximumSP(this);
            this._status.MaxHP += Singleton.CharacterConfiguration.CalculateMaximumHP(this);
        }

        #endregion Character Constructor / Deconstructor

        #region Character Internal Members

        //Addition 562
        [NonSerialized()]
        internal float partysprecoveramount;

        [NonSerialized()]
        internal object Tag;

        [NonSerialized()]
        internal Client client;

        [NonSerialized()]
        internal TradeSession TradeSession;

        [NonSerialized()]
        internal int UpdateReason = 0;

        [NonSerialized()]
        internal uint _ShowLoveDialog;

        [NonSerialized()]
        internal uint _Event;

        [NonSerialized()]
        internal uint _lastcastedskill = 0;

        [NonSerialized()]
        internal int _lastcastedtick = 0;

        [NonSerialized()]
        internal byte chatmute = 0;

        [NonSerialized()]
        internal byte blockhprecovery = 0;

        [NonSerialized()]
        internal byte blocksprecovery = 0;

        [NonSerialized()]
        internal CoolDownCollection cooldowncollection = new CoolDownCollection();

        /// <summary>
        /// GmLevel of the character
        /// </summary>
        [NonSerialized()]
        private byte _gmlevel = 0;

        /// <summary>
        /// Name of the character
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// Amount of character-experience (overall gameplay experience)
        /// </summary>
        private uint _cexp = 0;

        /// <summary>
        /// Amount of job-experience (speciafic for the current job)
        /// </summary>
        private uint _jexp = 0;

        /// <summary>
        /// The gender of your character
        /// </summary>
        public byte gender;

        /// <summary>
        /// The race of your character
        /// </summary>
        public byte race;

        /// <summary>
        /// Defines the last updated position tick
        /// </summary>
        [NonSerialized()]
        internal int LastPositionTick = 0;

        /// <summary>
        /// Defines the last sp tick
        /// </summary>
        [NonSerialized()]
        internal int LASTSP_TICK = 0;

        /// <summary>
        /// Defines the last hp tick
        /// </summary>
        [NonSerialized()]
        internal int LASTHP_TICK = 4000;

        /// <summary>
        /// Defines the last lp tick
        /// </summary>
        //[NonSerialized()]
        //internal int LASTLP_TICK = 0;

        /// <summary>
        /// Defines the last breath tick
        /// </summary>
        [NonSerialized()]
        internal int LASTBREATH_TICK = 0;

        /// <summary>
        /// ???
        /// </summary>
        [NonSerialized()]
        internal int TickLogged = 0;

        #endregion Character Internal Members

        #region Character Public Members (Variables)

        #region Character - Base stats

        /// <summary>
        /// Get's the characters name
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            internal set
            {
                name = value;
            }
        }

        /// <summary>
        /// Get's the character experience.
        /// </summary>
        public uint Cexp
        {
            get
            {
                return _cexp;
            }
            internal set
            {
                _cexp = value;
            }
        }

        /// <summary>
        /// Get's the Job experience of the current job
        /// </summary>
        public uint Jexp
        {
            get
            {
                return _jexp;
            }
            internal set
            {
                _jexp = value;
            }
        }

        /// <summary>
        /// Get's the gender
        /// </summary>
        public byte Gender
        {
            get
            {
                return gender;
            }
            internal set
            {
                gender = value;
            }
        }

        /// <summary>
        /// Current job of the character
        /// </summary>
        public byte job = 5;

        /// <summary>
        /// Current map of the charracter
        /// </summary>
        public byte map;

        /// <summary>
        /// Job-level of the character
        /// </summary>
        public byte jlvl = 0;

        /// <summary>
        /// Byte array containing the face details of the character
        /// </summary>
        public byte[] FaceDetails;

        /// <summary>
        /// Boolean incidacting if the character is diving or not.
        /// </summary>
        [NonSerialized()]
        internal bool IsDiving = false;

        /// <summary>
        /// Boolean indicating if the character is in battle position
        /// </summary>
        [NonSerialized()]
        internal bool ISONBATTLE = false;

        /// <summary>
        /// The amount of zeny of the character
        /// </summary>
        public uint ZENY = 10000;

        #endregion Character - Base stats

        #region Character - Rates

        [NonSerialized()]
        public double _CexpModifier = 1;

        [NonSerialized()]
        public double _JexpModifier = 1;

        [NonSerialized()]
        public double _WexpModifier = 1;

        [NonSerialized()]
        public double _DropRate = 0;

        #endregion Character - Rates

        #region Character - Location

        /// <summary>
        /// Save location
        /// </summary>
        public WorldCoordinate savelocation;

        /// <summary>
        /// Last known location
        /// </summary>
        public WorldCoordinate lastlocation;

        #endregion Character - Location

        #region Character - Misc

        /// <summary>
        /// Internally used as a container for all of your weapons
        /// friends.
        /// </summary>
        internal WeaponCollection weapons = new WeaponCollection();

        /// <summary>
        /// Internally used as a container for all of your jobs with the associated
        /// job levels. They form milestones of your jobs.
        /// </summary>
        internal byte[] CharacterJobLevel = new byte[15];

        /// <summary>
        /// Internally used as a container for all of your registered
        /// friends.
        /// </summary>
        internal List<string> _friendlist = new List<string>();

        /// <summary>
        /// Internally used as a container for all of your registered
        /// characteracters on your blacklist.
        /// </summary>
        /// <remarks>
        /// Because the blacklist also contains a definition for reason
        /// why you put him there we are using a KeyValuePair. So in order
        /// to perform a name based lookup use a anoumous predicate.
        /// </remarks>
        internal List<KeyValuePair<string, byte>> _blacklist = new List<KeyValuePair<string, byte>>();

        /// <summary>
        /// Byte containing the zone information of all learned maps
        /// </summary>
        internal byte[] ZoneInformation = new byte[256];

        internal List<byte> ParticipatedEvents = new List<byte>();

        [NonSerialized()]
        internal PartySession sessionParty = null;

        internal Rag2Collection container = new Rag2Collection();
        internal Rag2Collection STORAGE = new Rag2Collection();
        internal Rag2Item[] Equipment = new Rag2Item[16];
        internal List<Rag2Item> REBUY = new List<Rag2Item>();
        internal Skill[] SpecialSkills = new Skill[16];
        internal List<Skill> learnedskills = new List<Skill>();
        internal CharacterStats stats = new CharacterStats();
        internal uint ActiveDialog = 0;
        internal uint TakenDamage = 0;
        internal byte LastEquipmentDuraLoss = 0;

        internal Saga.Quests.Objectives.ObjectiveList QuestObjectives =
            new Saga.Quests.Objectives.ObjectiveList();

        #endregion Character - Misc

        #endregion Character Public Members (Variables)

        #region Character Public Members (Properties)

        public WorldCoordinate SavePosition
        {
            get
            {
                return this.savelocation;
            }
            set
            {
                this.savelocation = value;
            }
        }

        /// <summary>
        /// Get's the character level
        /// </summary>
        public byte Clvl
        {
            get
            {
                return this._level;
            }
        }

        /// <summary>
        /// Get's the job level
        /// </summary>
        public byte Jlvl
        {
            get
            {
                return this.jlvl;
            }
        }

        /// <summary>
        /// Get's the gm level
        /// </summary>
        public byte GmLevel
        {
            get
            {
                return this._gmlevel;
            }
            internal set
            {
                _gmlevel = value;
            }
        }

        #endregion Character Public Members (Properties)

        #region Character Public Methods

        public bool FindRequiredRootSkill(uint RootSkill)
        {
            Predicate<Skill> Query = delegate(Skill skill)
            {
                uint SkillId = (uint)((skill.Id / 100) * 100);
                if (RootSkill == SkillId) return true;
                return false;
            };

            return this.learnedskills.FindIndex(Query) > -1;
        }

        /// <summary>
        /// Calculates the current position
        /// </summary>
        /// <returns></returns>
        public Point CalculatePosition()
        {
            if (stance == 4 || stance == 5)
            {
                int t_diff = Environment.TickCount - LastPositionTick;
                float diff = t_diff * 0.1F;
                Point Loc = this.Position;

                double rad = Saga.Structures.Yaw.ToRadiants(Yaw.rotation);
                Loc.x += (float)(diff * Math.Cos(rad));
                Loc.y += (float)(diff * Math.Sin(rad));

                return Loc;
            }
            else
            {
                return this.Position;
            }
        }

        public uint ComputeAugeSkill()
        {
            int NewWeaponIndex = (this.weapons.ActiveWeaponIndex == 1) ? this.weapons.SeconairyWeaponIndex : this.weapons.PrimaryWeaponIndex;
            if (NewWeaponIndex < this.weapons.UnlockedWeaponSlots)
                return this.weapons[NewWeaponIndex]._augeskill;
            return 0;
        }

        #endregion Character Public Methods

        //Character Events (Special)

        #region Character Events

        [NonSerialized()]
        public RangeEventHandler PartyMemberAppearsInRange;

        [NonSerialized()]
        public RangeEventHandler PartyMemberDisappears;

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The natural hp recovery is invoked every seccond
        ///
        /// The natural sp & hp recovery are affected by additional stats/bonusses. Therefor
        /// we'll process HPRECOVERY only when the latest tick is greater than  1000 minus
        /// HP_RECOVERYRATE. The same storrie applies to SPRECOVERY, we'll only process that
        /// if the lastest tick is greater than 4000 minus SPRECOVERYRATE.
        ///
        /// Note: delta tick means the ellapsed time difference specified in millisecconds
        /// between tick start and tick end. So 4000 makes 4 secconds and 1000 makes 1 seccond.
        /// </remarks>
        /// <notes>
        /// TODO: Add official regenaration hp bonus
        ///
        /// Regeneration details of 26th dec patch:
        /// clvl 34 - 1657MHP -> 15HP per seccond
        /// clvl 38 - 1870MHP -> 16HP per seccond
        /// </notes>
        public bool OnRegenerateHP()
        {
            if (this.ISONBATTLE == false && this.HP < this.HPMAX && Environment.TickCount - LASTHP_TICK > (1000 - this._status.HpRecoveryRate))
            {
                LASTHP_TICK = Environment.TickCount;
                ushort REGENHP_BONUS = (ushort)(15 + _status.HpRecoveryQuantity);
                ushort newHP = (ushort)(this.HP + REGENHP_BONUS);
                this.HP = (newHP < this.HPMAX) ? newHP : this.HPMAX;
                this._status.Updates |= 1;

                return true;
            } return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The natural sp recovery is invoked every 4 secconds
        ///
        /// The natural sp & hp recovery are affected by additional stats/bonusses. Therefor
        /// we'll process HPRECOVERY only when the latest tick is greater than  1000 minus
        /// HP_RECOVERYRATE. The same storrie applies to SPRECOVERY, we'll only process that
        /// if the lastest tick is greater than 4000 minus SPRECOVERYRATE.
        ///
        /// Note: delta tick means the ellapsed time difference specified in millisecconds
        /// between tick start and tick end. So 4000 makes 4 secconds and 1000 makes 1 seccond.
        /// </remarks>
        public bool OnRegenerateSP()
        {
            if (this.SP < this.SPMAX && Environment.TickCount - LASTSP_TICK > (4000 - this._status.SpRecoveryRate))
            {
                LASTSP_TICK = Environment.TickCount;
                ushort REGENSP_BONUS = (ushort)(((double)this.SPMAX * 0.35) + _status.SpRecoveryQuantity);
                ushort newSP = (ushort)(this.SP + REGENSP_BONUS);
                this.SP = (newSP < this.SPMAX) ? newSP : this.SPMAX;
                this._status.Updates |= 1;

                return true;
            } return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The natural breath cheacking is invoked every seccond.
        ///
        /// Youre beath capacity equals 10% of your MHP per seccond multiplied
        /// by the delta_t. Time difference between the first and the last tick.
        ///
        /// First when breathing we'll substract 1 LC per seccond which represents
        /// your breath capacity. Once your LC drops down to 0 we'll invoked for damage
        /// packets if the player is still diving.
        /// </remarks>
        public bool OnBreath()
        {
            int delta_t = Environment.TickCount - LASTBREATH_TICK;
            LASTBREATH_TICK = Environment.TickCount;

            if (IsDiving == true)
            {
                if (delta_t > 1000)
                {
                    if (_status.CurrentOxygen > 0)
                    {
                        _status.CurrentOxygen--;
                        this._status.Updates |= 1;
                    }
                    else
                    {
                        ushort damage = (ushort)((double)this.HPMAX * 0.0001 * delta_t);
                        damage = (this.HP > damage) ? damage : this.HP;
                        this.HP -= damage;
                        this._status.Updates |= 1;
                        CommonFunctions.SendOxygenTakeDamage(this, damage);
                    }

                    return true;
                }
            }
            else
            {
                if (delta_t > 1000 - _status.OxygenRecoveryRate)
                {
                    bool LCRegen = _status.CurrentOxygen < _status.MaximumOxygen;
                    if (LCRegen == true) _status.CurrentOxygen += 10;
                    this._status.Updates |= 1;
                    return LCRegen;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Occurs when reducing the LP for reduction
        /// </summary>
        /// <returns>True if LP is reduced</returns>
        //public bool OnLPReduction()
        //{
        //	if(LASTLP_TICK<_status.LASTLP_ADD)
        //		LASTLP_TICK=_status.LASTLP_ADD;
        //    int delta_t = Environment.TickCount - LASTLP_TICK;
        //    if (delta_t > 60000)
        //    {
        //        LASTLP_TICK = Environment.TickCount;
        //        if (_status.CurrentLp > 0)
        //        {
        //            _status.CurrentLp--;
        //            this._status.Updates |= 1;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public override void OnSkillUsedByTarget(MapObject source, SkillBaseEventArgs e)
        {
            //If durabillity could be checked
            if (e.CanCheckEquipmentDurabillity)
                Common.Durabillity.DoEquipment(this.Target as Character, e.Damage);

            //Set sword drawn
            if (e.SpellInfo.hate > 0)
                this.ISONBATTLE = true;

            //Use base skill
            base.OnSkillUsedByTarget(source, e);
        }

        /// <summary>
        /// Occurs when the character dies
        /// </summary>
        /// <returns>Returns always true</returns>
        public bool OnDie()
        {
            //Pronounce my dead to other people
            if (this.sessionParty != null)
                foreach (Character target in this.sessionParty.GetCharacters())
                {
                    if (target.id == this.id) continue;
                    if (target.client.isloaded == true)
                    {
                        SMSG_PARTYMEMBERDIED spkt = new SMSG_PARTYMEMBERDIED();
                        spkt.SessionId = target.id;
                        spkt.Index = 1;
                        spkt.ActorId = this.id;
                        target.client.Send((byte[])spkt);
                    }
                }

            // SUBSCRIBE FOR RESPAWN
            this.ISONBATTLE = false;
            Respawns.Subscribe(this);
            return true;
        }

        public override void OnEnemyDie(MapObject enemy)
        {
            if (MapObject.IsNpc(enemy))
            {
                Quests.QuestBase.UserEliminateTarget(enemy.ModelId, this);
            }
        }

        #endregion Character Events

        //Interface Memebers

        public override void Appears(Character character)
        {
            if (character.sessionParty != null && this.sessionParty != null)
            {
                if (character.sessionParty._Characters.Contains(this))
                {
                    character.PartyMemberAppearsInRange(this);
                    this.PartyMemberAppearsInRange(character);
                }
            }
        }

        public override void Disappear(Character character)
        {
            if (character.sessionParty != null && this.sessionParty != null)
            {
                if (character.sessionParty._Characters.Contains(this))
                {
                    character.PartyMemberDisappears(this);
                    this.PartyMemberDisappears(character);
                }
            }
        }

        public override void ShowObject(Character character)
        {
            //HELPER VARIABLES
            Rag2Item item;

            //STRUCTIRIZE GENERAL INFORMATION
            SMSG_CHARACTERINFO spkt = new SMSG_CHARACTERINFO();
            spkt.race = 0;
            spkt.Gender = this.gender;
            spkt.Name = this.Name;
            spkt.X = this.Position.x;
            spkt.Y = this.Position.y;
            spkt.Z = this.Position.z;
            spkt.ActorID = this.id;
            spkt.face = this.FaceDetails;
            spkt.AugeSkillID = this.ComputeAugeSkill();
            spkt.yaw = this.Yaw;
            spkt.Job = this.job;
            spkt.Stance = this.stance;

            //STRUCTURIZE EQUIPMENT INFORMATION
            item = this.Equipment[0];
            if (item != null && item.active > 0) spkt.SetHeadTop(item.info.item, item.dyecolor);

            item = this.Equipment[1];
            if (item != null && item.active > 0) spkt.SetHeadMiddle(item.info.item, item.dyecolor);

            item = this.Equipment[2];
            if (item != null && item.active > 0) spkt.SetHeadBottom(item.info.item, item.dyecolor);

            item = this.Equipment[14];
            if (item != null && item.active > 0) spkt.SetShield(item.info.item, item.dyecolor);

            item = this.Equipment[8];
            if (item != null && item.active > 0) spkt.SetBack(item.info.item, item.dyecolor);

            item = this.Equipment[3];
            if (item != null && item.active > 0) spkt.SetShirt(item.info.item, item.dyecolor);

            item = this.Equipment[4];
            if (item != null && item.active > 0) spkt.SetPants(item.info.item, item.dyecolor);

            item = this.Equipment[5];
            if (item != null && item.active > 0) spkt.SetGloves(item.info.item, item.dyecolor);

            item = this.Equipment[6];
            if (item != null && item.active > 0) spkt.SetFeet(item.info.item, item.dyecolor);

            foreach (AdditionState state in this._additions)
                spkt.SetWeapon(state.Addition, state.Lifetime);

            lock (character)
            {
                spkt.SessionId = character.id;
                character.client.Send((byte[])spkt);

                if (this._Event > 0)
                {
                    SMSG_SCENARIOEVENTBEGIN spkt2 = new SMSG_SCENARIOEVENTBEGIN();
                    spkt2.Event = this._Event;
                    spkt2.ActorId = this.id;
                    spkt2.SessionId = character.id;
                    character.client.Send((byte[])spkt2);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Character name:{1} id:{0}", this.id, this.name);
        }
    }

    [Serializable()]
    public class CharacterStats
    {
        private static Stats _BASE = new Stats(5, 3, 3, 2, 0);
        public Stats CHARACTER = new Stats();
        public Stats EQUIPMENT = new Stats();
        public Stats ENCHANTMENT = new Stats();
        public ushort REMAINING = 0;

        public Stats BASE
        {
            get
            {
                return _BASE;
            }
        }

        public ushort Strength
        {
            get
            {
                return (ushort)(
                       _BASE.strength +             //ADD STATS FROM THE BASE TEMPLATE
                       CHARACTER.strength +         //ADD STATS CHOOSEN BY THE CHARACTER
                       EQUIPMENT.strength +         //ADD STATS GIVEN BY EQUIPMENT
                       ENCHANTMENT.strength        //ADD STATS GIVEN BY ENCHANTMENTS, BUFFS, SKILLS
                );
            }
        }

        public ushort Dexterity
        {
            get
            {
                return (ushort)(
                       _BASE.dexterity +            //ADD STATS FROM THE BASE TEMPLATE
                       CHARACTER.dexterity +        //ADD STATS CHOOSEN BY THE CHARACTER
                       EQUIPMENT.dexterity +        //ADD STATS GIVEN BY EQUIPMENT
                       ENCHANTMENT.dexterity       //ADD STATS GIVEN BY ENCHANTMENTS, BUFFS, SKILLS
                );
            }
        }

        public ushort Concentration
        {
            get
            {
                return (ushort)(
                       _BASE.concentration +        //ADD STATS FROM THE BASE TEMPLATE
                       CHARACTER.concentration +    //ADD STATS CHOOSEN BY THE CHARACTER
                       EQUIPMENT.concentration +    //ADD STATS GIVEN BY EQUIPMENT
                       ENCHANTMENT.concentration    //ADD STATS GIVEN BY ENCHANTMENTS, BUFFS, SKILLS
                );
            }
        }

        public ushort Intelligence
        {
            get
            {
                return (ushort)(
                       _BASE.intelligence +                 //ADD STATS FROM THE BASE TEMPLATE
                       CHARACTER.intelligence +             //ADD STATS CHOOSEN BY THE CHARACTER
                       EQUIPMENT.intelligence +             //ADD STATS GIVEN BY EQUIPMENT
                       ENCHANTMENT.intelligence             //ADD STATS GIVEN BY ENCHANTMENTS, BUFFS, SKILLS
                );
            }
        }

        public ushort Luck
        {
            get
            {
                return (ushort)(
                       _BASE.luck +                 //ADD STATS FROM THE BASE TEMPLATE
                       CHARACTER.luck +             //ADD STATS CHOOSEN BY THE CHARACTER
                       EQUIPMENT.luck +             //ADD STATS GIVEN BY EQUIPMENT
                       ENCHANTMENT.luck             //ADD STATS GIVEN BY ENCHANTMENTS, BUFFS, SKILLS
                );
            }
        }

        [Serializable()]
        public class Stats
        {
            public ushort strength = 0;               //CONTAINER FOR STRENGTH STAT
            public ushort dexterity = 0;              //CONTAINER FOR DEXTERITY STAT
            public ushort intelligence = 0;           //CONTAINER FOR INTELLIGENCE STAT
            public ushort concentration = 0;          //CONTAINER FOR CONCENTRATION STAT
            public ushort luck = 0;                   //CONTAINER FOR LUCK STAT

            public Stats()
            {
            }

            public Stats(ushort strength, ushort dexterity, ushort intelligence, ushort concentration, ushort luck)
            {
                this.strength = strength;
                this.dexterity = dexterity;
                this.intelligence = intelligence;
                this.concentration = concentration;
                this.luck = luck;
            }

            public static implicit operator ushort[](Stats f)
            {
                return new ushort[]
                {
                    f.strength,
                    f.dexterity,
                    f.intelligence,
                    f.concentration,
                    f.luck
                };
            }
        }
    }
}