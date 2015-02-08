using Saga.Map;
using Saga.Map.Definitions.Misc;
using Saga.Shared.Definitions;
using Saga.Tasks;
using System;
using System.Collections.Generic;

namespace Saga.PrimaryTypes
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [Serializable()]
    public abstract class Actor : MapObject, ISelectAble
    {
        #region Protected Internal

        public byte stance = 3;
        protected internal byte _level;

        [NonSerialized()]
        protected internal MapObject _target = null;

        [NonSerialized()]
        protected internal uint _targetid = 0;

        protected internal Addition _additions = new Addition();
        protected internal BattleStatus _status = new BattleStatus();

        #endregion Protected Internal

        #region Public Properties

        public MapObject Target
        {
            get
            {
                return _target;
            }
            protected internal set
            {
                _target = value;
            }
        }

        public uint SelectedTarget
        {
            get
            {
                return _targetid;
            }
        }

        public byte Level
        {
            get
            {
                return _level;
            }
        }

        public BattleStatus Status
        {
            get
            {
                return _status;
            }
        }

        public bool IsMoving
        {
            get
            {
                return (stance == 4 || stance == 5);
            }
        }

        #endregion Public Properties

        #region Internal Methods

        protected internal void UpdateAdditions()
        {
            //PROCESS ADDITION
            if (this._additions._skipcheck == false)
            {
                List<AdditionState> appliedadditions = this._additions.timed_additions;
                for (int i = 0; i < appliedadditions.Count; i++)
                {
                    AdditionState state = appliedadditions[i];

                    if (state.IsExpired)
                    {
                        Singleton.Additions.DeapplyAddition(state.Addition, this);
                        Common.Skills.SendDeleteStatus(this, state.Addition);
                        appliedadditions.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        state.Update(this, Target);
                    }
                }

                if (_status.Updates > 0)
                {
                    Character character = this as Character;
                    if (character != null)
                    {
                        LifeCycle.Update(character);
                        _status.Updates = 0;
                    }
                }
            }
        }

        #endregion Internal Methods

        #region ISelectAble Members

        /// <summary>
        /// Get's or set's the current HP
        /// </summary>
        public ushort HP
        {
            get
            {
                return _status.CurrentHp;
            }
            set
            {
                _status.CurrentHp = value;
                _status.Updates |= 1;
            }
        }

        /// <summary>
        /// Get's or set's the Maximum HP
        /// </summary>
        public ushort HPMAX
        {
            get
            {
                return (ushort)_status.MaxHP;
            }
            set
            {
                _status.MaxHP = value;
                _status.Updates |= 1;
            }
        }

        /// <summary>
        /// Get's or set's the Current SP
        /// </summary>
        public ushort SP
        {
            get
            {
                return _status.CurrentSp;
            }
            set
            {
                _status.CurrentSp = value;
                _status.Updates |= 1;
            }
        }

        /// <summary>
        /// Get's or set's the Maximum SP
        /// </summary>
        public ushort SPMAX
        {
            get
            {
                return (ushort)_status.MaxSP;
            }
            set
            {
                _status.MaxSP = value;
                _status.Updates |= 1;
            }
        }

        #endregion ISelectAble Members

        #region MapObject Members

        public override void OnLoad()
        {
            base.OnLoad();
        }

        public override void OnRegister()
        {
            base.OnRegister();
        }

        public override void OnClick(Character target)
        {
            Common.Actions.SelectActor(target, this);
        }

        #endregion MapObject Members

        public override void OnSkillUsedByTarget(MapObject source, SkillBaseEventArgs e)
        {
            bool isdamage = e.Result == Saga.SkillBaseEventArgs.ResultType.Block || e.Result == Saga.SkillBaseEventArgs.ResultType.Critical || e.Result == Saga.SkillBaseEventArgs.ResultType.Normal;
            if (e.Damage > 0)
            {
                bool HasDied = (isdamage && e.Damage >= this.HP && this.stance != 7);
                if (HasDied)
                {
                    this.HP = 0;
                    e.Target.OnDie(e.Sender);
                    e.Sender.OnEnemyDie(e.Target);

                    lock (this)
                    {
                        this.stance = 7;
                        Common.Actions.UpdateStance(this);
                        Console.WriteLine("Target dead");
                    }
                }
                else
                {
                    this.HP -= (ushort)e.Damage;
                }
            }

            base.OnSkillUsedByTarget(source, e);
        }

        public override void ShowObject(Character character)
        {
        }
    }
}