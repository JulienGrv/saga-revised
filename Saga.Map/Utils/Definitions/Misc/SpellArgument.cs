using Saga.Enumarations;
using Saga.Map;
using Saga.PrimaryTypes;
using System;

namespace Saga
{
    public abstract class SkillBaseEventArgs : EventArgs
    {
        #region Private Members

        private MapObject target;
        private MapObject sender;
        private SkillContext context;
        private ResultType result;
        protected bool failed;
        protected Factory.Spells.Info info;
        private uint damage;

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Describes in what context the skill is used
        /// </summary>
        public SkillContext Context
        {
            get
            {
                return context;
            }
        }

        /// <summary>
        /// The actor that invokes the skill
        /// </summary>
        public MapObject Sender
        {
            get
            {
                return sender;
            }
        }

        /// <summary>
        /// The actor on which the skill is applied
        /// </summary>
        public MapObject Target
        {
            get
            {
                return target;
            }
        }

        /// <summary>
        /// Boolean to indicate the skill failed
        /// </summary>
        public bool Failed
        {
            get
            {
                return failed;
            }
            set
            {
                failed = value;
            }
        }

        /// <summary>
        /// Addition information that is lookedup
        /// </summary>
        public Factory.Spells.Info SpellInfo
        {
            get
            {
                return info;
            }
        }

        /// <summary>
        /// Gets or sets if the skill should check for weapon durabillity loss
        /// </summary>
        public virtual bool CanCheckWeaponDurabillity
        {
            get
            {
                return false;
            }
            set
            {
                throw new SystemException("Cannot set weapon durabillity check");
            }
        }

        /// <summary>
        /// Gets or sets if the skill should check for equipment durabillity loss
        /// </summary>
        public virtual bool CanCheckEquipmentDurabillity
        {
            get
            {
                return false;
            }
            set
            {
                //do nothing
            }
        }

        /// <summary>
        /// Gets or sets the damage inflicted by the skill
        /// </summary>
        public uint Damage
        {
            get
            {
                return damage;
            }
            set
            {
                damage = value;
            }
        }

        /// <summary>
        /// Gets or sets if the skill result
        /// </summary>
        public ResultType Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }

        #endregion Public Members

        #region Public Methods

        public abstract bool Use();

        #endregion Public Methods

        #region Constructor / Deconstructor

        protected SkillBaseEventArgs(SkillContext context, MapObject sender, MapObject target)
        {
            this.context = context;
            this.sender = sender;
            this.target = target;

            if (!(this is SkillUsageEventArgs || this is ItemSkillUsageEventArgs || this is SkillToggleEventArgs))
            {
                throw new SystemException("Class inheritance by other classes than the default are not allowed");
            }
        }

        #endregion Constructor / Deconstructor

        #region Nested Classes/Structs

        /// <summary>
        /// Result type defines what type of message should be shown on the
        /// client.
        /// </summary>
        public enum ResultType
        {
            /// <summary>
            /// System message: normal attack
            /// </summary>
            Normal,

            /// <summary>
            /// System message: critical attack
            /// </summary>
            Critical,

            /// <summary>
            /// System message: attack missed
            /// </summary>
            Miss,

            /// <summary>
            /// System message: attack blocked
            /// </summary>
            Block,

            /// <summary>
            /// System message: item used
            /// </summary>
            Item = 5,

            /// <summary>
            /// System message: healed
            /// </summary>
            Heal = 6,

            /// <summary>
            /// System message: no damage
            /// </summary>
            NoDamage
        }

        #endregion Nested Classes/Structs
    }

    public sealed class SkillUsageEventArgs : SkillBaseEventArgs
    {
        #region Private Members

        private bool WeaponDurabillityLoss = true;
        private bool EquipmentDurabillityLoss = true;
        internal bool TargetHasDied = false;

        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Gets the skill level of the skill.
        /// </summary>
        public int SkillLevel
        {
            get
            {
                return (int)(SkillId % 100);
            }
        }

        /// <summary>
        /// Gets the addition saved called by the skill.
        /// </summary>
        public uint Addition
        {
            get
            {
                return info.addition;
            }
        }

        /// <summary>
        /// Gets the skillid used by the skill
        /// </summary>
        public uint SkillId
        {
            get
            {
                return info.skillid;
            }
        }

        /// <summary>
        /// Gets or sets if the skill should check for weapon durabillity loss
        /// </summary>
        public override bool CanCheckWeaponDurabillity
        {
            get
            {
                return this.WeaponDurabillityLoss;
            }
            set
            {
                this.WeaponDurabillityLoss = value;
            }
        }

        /// <summary>
        /// Gets or sets if the skill should check for equipment durabillity loss
        /// </summary>
        public override bool CanCheckEquipmentDurabillity
        {
            get
            {
                return this.EquipmentDurabillityLoss;
            }
            set
            {
                this.EquipmentDurabillityLoss = value;
            }
        }

        #endregion Public Properties

        #region Constructors / Deconstructors

        private SkillUsageEventArgs(MapObject sender, MapObject target)
            : base(SkillContext.SkillUse, sender, target) { }

        public static SkillUsageEventArgs Create(uint skill, MapObject sender, MapObject target)
        {
            //SkillUsageEventArgs skill = new SkillUsageEventArgs();

            SkillUsageEventArgs skillusage = new SkillUsageEventArgs(sender, target);
            if (Singleton.SpellManager.TryGetSpell(skill, out skillusage.info))
                return skillusage;
            else
                return null;
        }

        public static bool Create(uint skill, MapObject sender, MapObject target, out SkillUsageEventArgs argument)
        {
            argument = Create(skill, sender, target);
            return argument != null;
        }

        #endregion Constructors / Deconstructors

        #region Public Methods

        public override bool Use()
        {
            try
            {
                //Always return true
                this.Failed = false;
                this.Damage = 0;
                this.Result = ResultType.NoDamage;

                //Use the scripted skill
                if (this.info.skill != null) this.info.skill.Invoke(this);

                //Reset miss corect
                if (this.Damage == 0 && !(this.Result == ResultType.Heal || this.Result == ResultType.NoDamage || this.Result == ResultType.Item))
                {
                    Result = ResultType.Miss;
                    this.failed = false;
                }

                this.Target.OnSkillUsedByTarget(this.Sender, this);

                return !this.Failed;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError(e.ToString());
                return false;
            }
        }

        #endregion Public Methods

        #region Public Members

        public SkillMatrix GetDefaultSkillMatrix(Actor source, Actor target)
        {
            switch (this.SpellInfo.attacktype)
            {
                case 1: return GetPhysicalSkillMatrix(source, target);
                case 2: return GetRangedSkillMatrix(source, target);
                case 3: return GetMagicalSkillMatrix(source, target);
                default: return GetPhysicalSkillMatrix(source, target);
            }
        }

        /// <summary>
        /// Creates a skill matrix which can be used to perform complex
        /// calculations based upon ranged damage.
        /// </summary>
        /// <param name="source">Source who invokes the skill</param>
        /// <param name="target">Target on who the skill is called</param>
        /// <returns>Ranged skillmatrix</returns>
        public SkillMatrix GetRangedSkillMatrix(Actor source, Actor target)
        {
            SkillMatrix matrix = new SkillMatrix();
            if (source != null)
            {
                matrix.matrix[0, 0] = (int)source._status.MinRAttack;
                matrix.matrix[0, 1] = (int)source._status.MinWRAttack;
                matrix.matrix[0, 2] = (int)source._status.BaseMinRAttack;

                matrix.matrix[1, 0] = (int)source._status.MaxRAttack;
                matrix.matrix[1, 1] = (int)source._status.MaxWRAttack;
                matrix.matrix[1, 2] = (int)source._status.BaseMaxRAttack;

                matrix.matrix[2, 0] = (int)source._status.DefenceRanged;
                matrix.matrix[2, 1] = (int)source._status.BlockrateRanged;
                matrix.matrix[2, 2] = (int)source._status.BaseRCritrate;
                matrix.matrix[2, 3] = (int)source._status.BaseRHitrate;
                matrix.matrix[4, 0] = (int)source._status.BaseREvasionrate;
                matrix.matrix[4, 1] = source._level;
            }
            if (target != null)
            {
                matrix.matrix[3, 0] = (int)target._status.DefenceRanged;
                matrix.matrix[3, 1] = (int)target._status.BlockrateRanged;
                matrix.matrix[3, 2] = (int)target._status.BaseRCritrate;
                matrix.matrix[4, 2] = target._level;
                matrix.matrix[4, 3] = (int)target._status.BaseREvasionrate;
                matrix.matrix[3, 3] = (int)target._status.BaseRHitrate;
            }

            return matrix;
        }

        /// <summary>
        /// Creates a skill matrix which can be used to perform complex
        /// calculations based upon physical damage.
        /// </summary>
        /// <param name="source">Source who invokes the skill</param>
        /// <param name="target">Target on who the skill is called</param>
        /// <returns>Physical skillmatrix</returns>
        public SkillMatrix GetPhysicalSkillMatrix(Actor source, Actor target)
        {
            SkillMatrix matrix = new SkillMatrix();
            if (source != null)
            {
                matrix.matrix[0, 0] = (int)source._status.MinPAttack;
                matrix.matrix[0, 1] = (int)source._status.MinWPAttack;
                matrix.matrix[0, 2] = (int)source._status.BaseMinPAttack;

                matrix.matrix[1, 0] = (int)source._status.MaxPAttack;
                matrix.matrix[1, 1] = (int)source._status.MaxWPAttack;
                matrix.matrix[1, 2] = (int)source._status.BaseMaxRAttack;

                matrix.matrix[2, 0] = (int)source._status.DefencePhysical;
                matrix.matrix[2, 1] = (int)source._status.BlockratePhysical;
                matrix.matrix[2, 2] = (int)source._status.BasePCritrate;
                matrix.matrix[2, 3] = (int)source._status.BasePHitrate;
                matrix.matrix[4, 0] = (int)source._status.BasePEvasionrate;
                matrix.matrix[4, 1] = (int)source._level;
            }
            if (target != null)
            {
                matrix.matrix[3, 0] = (int)target._status.DefencePhysical;
                matrix.matrix[3, 1] = (int)target._status.BlockratePhysical;
                matrix.matrix[3, 2] = (int)target._status.BasePCritrate;
                matrix.matrix[4, 2] = (int)target._level;
                matrix.matrix[4, 3] = (int)target._status.BasePEvasionrate;
                matrix.matrix[3, 3] = (int)target._status.BasePHitrate;
            }

            return matrix;
        }

        /// <summary>
        /// Creates a skill matrix which can be used to perform complex
        /// calculations based upon magical damage.
        /// </summary>
        /// <param name="source">Source who invokes the skill</param>
        /// <param name="target">Target on who the skill is called</param>
        /// <returns>Magical skillmatrix</returns>
        public SkillMatrix GetMagicalSkillMatrix(Actor source, Actor target)
        {
            SkillMatrix matrix = new SkillMatrix();
            if (source != null)
            {
                matrix.matrix[0, 0] = (int)source._status.MinMAttack;
                matrix.matrix[0, 1] = (int)source._status.MinWMAttack;
                matrix.matrix[0, 2] = (int)source._status.BaseMinMAttack;

                matrix.matrix[1, 0] = (int)source._status.MaxMAttack;
                matrix.matrix[1, 1] = source._status.MaxWMAttack;
                matrix.matrix[1, 2] = (int)source._status.BaseMaxRAttack;

                matrix.matrix[2, 0] = (int)source._status.DefenceMagical;
                matrix.matrix[2, 1] = (int)source._status.BlockrateMagical;
                matrix.matrix[2, 2] = (int)source._status.BaseMCritrate;
                matrix.matrix[2, 3] = (int)source._status.BaseMHitrate;
                matrix.matrix[4, 0] = (int)source._status.BaseMEvasionrate;
                matrix.matrix[4, 1] = source._level;
            }
            if (target != null)
            {
                matrix.matrix[3, 0] = (int)target._status.DefenceMagical;
                matrix.matrix[3, 1] = (int)target._status.BlockrateMagical;
                matrix.matrix[3, 2] = (int)target._status.BasePCritrate;
                matrix.matrix[4, 2] = target._level;
                matrix.matrix[4, 3] = (int)target._status.BasePEvasionrate;
                matrix.matrix[3, 3] = (int)target._status.BasePHitrate;
            }

            return matrix;
        }

        /// <summary>
        /// Checks if the skill missed.
        /// </summary>
        /// <param name="matrix">Skill matrix used to peform the calculation</param>
        /// <returns>True if skill missed</returns>
        public bool IsMissed(SkillMatrix matrix)
        {
            int Evasion = Saga.Utils.Generator.Random(500, 1200);
            if ((Evasion + (matrix[2, 3]) / 2) < 50 + matrix[4, 3])
            {
                Result = ResultType.Miss;
                this.failed = false;
                return true;
            }

            return false;
        }

        public void UpdateCancelAddition(uint addition, uint lifetime, uint cancelAddition, Actor actor)
        {
            if (Common.Skills.HasAddition(actor, cancelAddition))
                Common.Skills.DeleteAddition(actor, cancelAddition);
            Common.Skills.CreateAddition(actor, addition, lifetime);
        }

        /// <summary>
        /// Checks if the skill has missed.
        /// </summary>
        /// <param name="matrix">Skill matrix used to peform the calculation</param>
        /// <returns>True if skill missed</returns>
        public bool IsBlocked(SkillMatrix matrix)
        {
            int Block = Saga.Utils.Generator.Random(0, 1000);
            if (Block + (matrix[2, 3] / 2) < 50 + matrix[3, 1])
            {
                Result = ResultType.Block;
                this.failed = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the skill was a critical hit.
        /// </summary>
        /// <param name="matrix">Skill matrix used to peform the calculation</param>
        /// <returns>True if is critical</returns>
        public bool IsCritical(SkillMatrix matrix)
        {
            int Block = Saga.Utils.Generator.Random(0, 1000);
            if (Block < 80 + matrix[2, 2])
            {
                this.Result = ResultType.Critical;
                this.Damage = (uint)((double)this.Damage * 1.3);
                this.failed = false;
                return true;
            }

            this.Result = ResultType.Normal;
            return false;
        }

        /// <summary>
        /// Calculates skill damanged based upon the skill matrix
        /// </summary>
        /// <param name="matrix">Skill matrix used to peform the calculation</param>
        /// <returns>True if is critical</returns>
        public uint GetDamage(SkillMatrix matrix)
        {
            uint Damage = 0;
            int min = matrix[0, 0] + matrix[0, 1] + matrix[0, 2] + matrix[0, 3];
            int max = matrix[1, 0] + matrix[1, 1] + matrix[1, 2] + matrix[1, 3];
            int mean = max;

            for (int i = 0; i < 4; i++)
            {
                double deviations = Math.Abs((double)(Saga.Utils.Generator.Random(min, max) - mean));
                Damage += (uint)Math.Pow(deviations, 2);
            }

            Damage /= 4;
            return (uint)(min + Math.Sqrt((double)Damage));
        }

        /// <summary>
        /// Calculates new damage with defense reducation substracted from it.
        /// </summary>
        /// <param name="matrix">Skill matrix used to peform the calculation</param>
        /// <param name="damage">Damage</param>
        /// <returns>Damange with reduction</returns>
        public uint GetDefenseReduction(SkillMatrix matrix, uint damage)
        {
            return (uint)(damage - (((double)matrix[3, 0] / (double)1000) * damage));
        }

        /// <summary>
        /// Gets the capped level difference
        /// </summary>
        /// <param name="matrix">Skill matrix used to peform the calculation</param>
        /// <returns>Level difference</returns>
        public int GetCappedLevelDifference(SkillMatrix matrix)
        {
            return Math.Min(5, Math.Max(0, matrix[4, 2] - matrix[4, 1]));
        }

        /// <summary>
        /// Gets the capped level difference
        /// </summary>
        /// <param name="matrix">Skill matrix used to peform the calculation</param>
        /// <returns>Level difference</returns>
        public int GetLevelDifference(SkillMatrix matrix)
        {
            return Math.Abs(matrix[4, 2] - matrix[4, 1]);
        }

        /// <summary>
        /// Does a hp effect.
        /// </summary>
        public void HpEffect(int value, Actor source)
        {
            Regiontree tree = source.currentzone.Regiontree;
            foreach (Character current in tree.SearchActors(source, Saga.Enumarations.SearchFlags.Characters))
                Common.Skills.SendSkillEffect(current, source, this.info.addition, 1, (uint)value);
        }

        /// <summary>
        /// Does a sp effect.
        /// </summary>
        public void SpEffect(int value, Actor source)
        {
            Regiontree tree = source.currentzone.Regiontree;
            foreach (Character current in tree.SearchActors(source, Saga.Enumarations.SearchFlags.Characters))
                Common.Skills.SendSkillEffect(current as Character, source, this.info.addition, 2, (uint)value);
        }

        /// <summary>
        /// Does a lp effect.
        /// </summary>
        public void LpEffect(int value, Actor source)
        {
            Regiontree tree = source.currentzone.Regiontree;
            foreach (Character current in tree.SearchActors(source, Saga.Enumarations.SearchFlags.Characters))
                Common.Skills.SendSkillEffect(current as Character, source, this.info.addition, 3, (uint)value);
        }

        /// <summary>
        /// Does a random LP Increase.
        /// </summary>
        /// <param name="source">Actor who's LP to increase</param>
        public void DoLpIncrease(Actor source)
        {
            if (source.Status.CurrentLp < 7 && //Check if lp is already full
                    Saga.Utils.Generator.Random(0, 99) < 80) //80% chance to gain one lp point
            {
                source.Status.CurrentLp++;
            }
        }

        #endregion Public Members

        #region Nested Classes/Structs

        /// <summary>
        /// Classifies a skill matrix. This class supplies as glue for scriptable
        /// skills by run-time compiled skills.
        /// </summary>
        public sealed class SkillMatrix
        {
            /// <summary>
            /// 5 by 4 Matrx internal matrix which holds the values
            /// of the specified class.
            /// </summary>
            public int[,] matrix = new int[5, 4];

            /// <summary>
            /// Gets or sets the values in the matrix specified by the Left and Right index.
            /// </summary>
            /// <param name="leftIndex">1st component of the matrix</param>
            /// <param name="rightIndex">2nd component of the matrix</param>
            /// <returns>Value at the specified index</returns>
            public int this[int leftIndex, int rightIndex]
            {
                get
                {
                    return matrix[leftIndex, rightIndex];
                }
                set
                {
                    matrix[leftIndex, rightIndex] = value;
                }
            }
        }

        #endregion Nested Classes/Structs
    }

    public sealed class SkillToggleEventArgs : SkillBaseEventArgs
    {
        #region Private Members

        private ResultType result;
        private uint damage;

        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Gets the skill level of the skill.
        /// </summary>
        public int SkillLevel
        {
            get
            {
                return (int)(SkillId % 100);
            }
        }

        /// <summary>
        /// Gets the addition saved called by the skill.
        /// </summary>
        public uint Addition
        {
            get
            {
                return info.addition;
            }
        }

        /// <summary>
        /// Gets the skillid used by the skill
        /// </summary>
        public uint SkillId
        {
            get
            {
                return info.skillid;
            }
        }

        /// <summary>
        /// Gets or sets the damage inflicted by the skill
        /// </summary>
        public new uint Damage
        {
            get
            {
                return damage;
            }
            set
            {
                damage = value;
            }
        }

        /// <summary>
        /// Gets or sets if the skill failed
        /// </summary>
        public new bool Failed
        {
            get
            {
                return failed;
            }
            set
            {
                failed = value;
            }
        }

        /// <summary>
        /// Gets or sets if the skill result
        /// </summary>
        public new ResultType Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }

        #endregion Public Properties

        #region Constructors / Deconstructors

        private SkillToggleEventArgs(MapObject sender, MapObject target)
            : base(SkillContext.SkillToggle, sender, target) { }

        public static SkillToggleEventArgs Create(uint skill, MapObject sender, MapObject target)
        {
            //SkillUsageEventArgs skill = new SkillUsageEventArgs();

            SkillToggleEventArgs skillusage = new SkillToggleEventArgs(sender, target);
            if (Singleton.SpellManager.TryGetSpell(skill, out skillusage.info))
                return skillusage;
            else
                return null;
        }

        public static bool Create(uint skill, MapObject sender, MapObject target, out SkillToggleEventArgs argument)
        {
            argument = Create(skill, sender, target);
            return argument != null;
        }

        #endregion Constructors / Deconstructors

        #region Public Methods

        public override bool Use()
        {
            try
            {
                if (this.info.SP > 0 && MapObject.IsPlayer(this.Sender))
                {
                    Character sTarget = (Character)this.Sender;
                    long newsp = sTarget._status.CurrentSp - this.info.SP;
                    if (newsp < 0) newsp = 0;
                    sTarget._status.CurrentSp = (ushort)newsp;
                    sTarget._status.Updates = 1;
                }

                //Always return true
                this.Failed = false;
                this.damage = 0;
                this.result = ResultType.NoDamage;

                //Use the scripted skill
                if (this.info.skill != null) this.info.skill.Invoke(this);
                this.Target.OnSkillUsedByTarget(this.Sender, this);
                return !this.Failed;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError(e.ToString());
                return false;
            }
        }

        public void Toggle(Actor actor, uint addition)
        {
            if (Common.Skills.HasAddition(actor, addition))
                Common.Skills.DeleteStaticAddition(actor, addition);
            else
                Common.Skills.CreateAddition(actor, addition);
        }

        #endregion Public Methods

        #region Public Members

        /// <summary>
        /// Does a hp effect.
        /// </summary>
        public void HpEffect(int value, Actor source)
        {
            Regiontree tree = source.currentzone.Regiontree;
            foreach (Character current in tree.SearchActors(source, Saga.Enumarations.SearchFlags.Characters))
                Common.Skills.SendSkillEffect(current, source, this.info.addition, 1, (uint)value);
        }

        /// <summary>
        /// Does a sp effect.
        /// </summary>
        public void SpEffect(int value, Actor source)
        {
            Regiontree tree = source.currentzone.Regiontree;
            foreach (Character current in tree.SearchActors(source, Saga.Enumarations.SearchFlags.Characters))
                Common.Skills.SendSkillEffect(current as Character, source, this.info.addition, 2, (uint)value);
        }

        /// <summary>
        /// Does a lp effect.
        /// </summary>
        public void LpEffect(int value, Actor source)
        {
            Regiontree tree = source.currentzone.Regiontree;
            foreach (Character current in tree.SearchActors(source, Saga.Enumarations.SearchFlags.Characters))
                Common.Skills.SendSkillEffect(current as Character, source, this.info.addition, 3, (uint)value);
        }

        /// <summary>
        /// Does a random LP Increase.
        /// </summary>
        /// <param name="source">Actor who's LP to increase</param>
        public void DoLpIncrease(Actor source)
        {
            if (source.Status.CurrentLp < 7 && //Check if lp is already full
                    Saga.Utils.Generator.Random(0, 99) < 80) //80% chance to gain one lp point
            {
                source.Status.CurrentLp++;
            }
        }

        #endregion Public Members
    }

    public sealed class ItemSkillUsageEventArgs : SkillBaseEventArgs
    {
        #region Private Members

        private ResultType result;
        private uint damage;
        private Factory.ItemsFactory.ItemInfo iteminfo;
        internal bool TargetHasDied = false;

        #endregion Private Members

        #region Public Methods

        public override bool Use()
        {
            try
            {
                if (this.info.SP > 0 && MapObject.IsPlayer(this.Sender))
                {
                    Character sTarget = (Character)this.Sender;
                    long newsp = sTarget._status.CurrentSp - this.info.SP;
                    if (newsp < 0) newsp = 0;
                    sTarget._status.CurrentSp = (ushort)newsp;
                    sTarget._status.Updates = 1;
                }

                //Always return true
                this.Failed = false;
                this.damage = 0;
                this.result = ResultType.NoDamage;

                //Use the scripted skill
                if (this.info.skill != null) this.info.skill.Invoke(this);
                this.Target.OnSkillUsedByTarget(this.Sender, this);

                //Is damgae skill
                bool isdamage = this.result == ResultType.Block || this.result == ResultType.Critical || this.result == ResultType.Normal;
                if (this.damage > 0 && (MapObject.IsMonster(this.Target) || MapObject.IsPlayer(this.Target)))
                {
                    Actor btarget = this.Target as Actor;
                    this.TargetHasDied |= (isdamage && this.damage >= btarget.HP && btarget.stance != 7);
                    if (this.TargetHasDied)
                    {
                        btarget._status.CurrentHp = 0;
                        btarget.stance = 7;
                        this.Target.OnDie(this.Sender);
                        this.Sender.OnEnemyDie(this.Target);
                    }
                    else
                    {
                        btarget._status.CurrentHp -= (ushort)this.damage;
                        btarget._status.Updates |= 1;
                    }
                }
                else if (damage == 0 && (this.result == ResultType.NoDamage || this.result == ResultType.Item))
                {
                    this.result = ResultType.Miss;
                }

                return !this.Failed;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError(e.ToString());
                return false;
            }
        }

        #endregion Public Methods

        #region Public Properties

        public Factory.ItemsFactory.ItemInfo ItemInfo
        {
            get
            {
                return iteminfo;
            }
        }

        /// <summary>
        /// Gets or sets if the skill failed
        /// </summary>
        public new bool Failed
        {
            get
            {
                return failed;
            }
            set
            {
                failed = value;
            }
        }

        /// <summary>
        /// Gets or sets the damage inflicted by the skill
        /// </summary>
        public new uint Damage
        {
            get
            {
                return damage;
            }
            set
            {
                damage = value;
            }
        }

        /// <summary>
        /// Gets the addition saved called by the skill.
        /// </summary>
        public uint Addition
        {
            get
            {
                return info.addition;
            }
        }

        /// <summary>
        /// Gets or sets if the skill result
        /// </summary>
        public new ResultType Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }

        #endregion Public Properties

        #region Constructor / Deconstructors

        private ItemSkillUsageEventArgs(MapObject sender, MapObject target)
            : base(SkillContext.ItemUsage, sender, target) { }

        public static ItemSkillUsageEventArgs Create(Rag2Item item, MapObject sender, MapObject target)
        {
            ItemSkillUsageEventArgs skillusage = new ItemSkillUsageEventArgs(sender, target);
            skillusage.iteminfo = item.info;

            if (item == null)
                return null;
            else if (!Singleton.SpellManager.TryGetSpell(item.info.skill, out skillusage.info))
                return null;
            else
                return skillusage;
        }

        public static bool Create(Rag2Item item, MapObject sender, MapObject target, out ItemSkillUsageEventArgs argument)
        {
            argument = Create(item, sender, target);
            return argument != null;
        }

        #endregion Constructor / Deconstructors
    }
}