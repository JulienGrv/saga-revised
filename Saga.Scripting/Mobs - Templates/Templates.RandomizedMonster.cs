using Saga.Map;
using Saga.Map.Librairies;
using Saga.PrimaryTypes;
using Saga.Shared.Definitions;
using Saga.Structures;
using Saga.Tasks;
using System;

namespace Saga.Templates
{
    public class RandomizedMonster : Monster, IArtificialIntelligence
    {
        #region Public Members

        public override void OnSpawn()
        {
            lock (this)
            {
                Lifespan.lasttick = Environment.TickCount;
                base.OnSpawn();
            }
        }

        /// <summary>
        /// Occurs when the speciafiec character killed
        /// our monsters
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override void OnDie(MapObject target)
        {
            //Stop movement
            lock (this)
            {
                LifespanAI.Unsubscribe(this);
                this.stance = 7;
            }

            base.OnDie(target);
        }

        /// <summary>
        /// Subscribes the monsters ai if an the monster is summoned in a crowded
        /// area.
        /// </summary>
        public override void OnRegister()
        {
            //First register the mob
            base.OnRegister();

            //Check if start moving the mob

            if (Regiontree.GetCharacterCount(this) > 0)
            {
                Lifespan.lasttick = Environment.TickCount;
                LifespanAI.Subscribe(this);
            }
        }

        public override void OnDeregister()
        {
            LifespanAI.Unsubscribe(this);
            base.OnDeregister();
        }

        /// <summary>
        /// Subscribes the monsters moving ai if an
        /// character appears.
        /// </summary>
        /// <param name="character"></param>
        public override void Appears(Character character)
        {
            //ENABLE THE AI
            if (!LifespanAI.IsSubscribed(this))
            {
                Lifespan.lasttick = Environment.TickCount;
                LifespanAI.Subscribe(this);
            }
        }

        /// <summary>
        /// Unsubscribes the monsters moving ai if the region's player
        /// count is 0.
        /// </summary>
        /// <param name="character"></param>
        public override void Disappear(Character character)
        {
            //DISABLE THE AI
            if (Regiontree.GetCharacterCount(this) == 0)
            {
                LifespanAI.Unsubscribe(this);
                this.Position = this.DestPosition;
            }

            base.Disappear(character);
        }

        #endregion Public Members

        #region IArtificialIntelligence

        void IArtificialIntelligence.Process()
        {
            //Update all additions
            UpdateAdditions();
            if (this.stance == 7)
            {
                return;
            }

            //REquire a minimum of 1 seccond
            int t_diff = Environment.TickCount - Lifespan.lasttick;
            if (t_diff > 1000 && IsMoveable)
            {
                switch (this.stance)
                {
                    case 4:
                        OnIdleWalk(t_diff);
                        break;

                    case 5:
                        OnChaseTarget(t_diff);
                        break;

                    default:
                        //Stand still for  7 secconds
                        if (this.Target == null)
                        {
                            OnThinkNextAction(t_diff);
                        }
                        else
                        {
                            OnRevalidateRange();
                        }
                        break;
                }
            }
        }

        #endregion IArtificialIntelligence

        #region AI Helper Functions

        private void OnRevalidateRange()
        {
            Point walkTo;
            uint minrange = this.skills != null ? this.skills.MinRange : 0;
            uint maxrange = this.skills != null ? this.skills.MaxRange : 150;
            if (Point.GetNonSquaredDistance3D(this.Position, this.Target.Position) >= maxrange)
            {
                walkTo = GetPositionForEnemy(this._target.Position, minrange);
                StartRunning(walkTo);
            }

            Lifespan.lasttick = Environment.TickCount;
        }

        private void OnThinkNextAction(int t_diff)
        {
            if (t_diff > 7000)
            {
                Point walkTo;
                if (IsAggresive)
                {
                    uint minrange = this.skills != null ? this.skills.MinRange : 0;
                    Character aggresiveTarget = GetLowestEnemy();
                    if (aggresiveTarget == null)
                    {
                        walkTo = GetRandomCoords();
                    }
                    else
                    {
                        this.SwitchTarget(aggresiveTarget);
                        walkTo = GetPositionForEnemy(this._target.Position, minrange);
                        Tasks.BattleThread.Subscribe(this);
                    }
                }
                else
                {
                    //Pick an random coord
                    walkTo = GetRandomCoords();
                }

                StartMovement(walkTo);
                Lifespan.lasttick = Environment.TickCount;
            }
        }

        private void OnChaseTarget(int t_diff)
        {
            UpdatePostion(t_diff);
            if (this.Target == null)
            {
                this.stance = 3;
            }

            //Always walk to taget to minimum attack range
            uint minrange = this.skills != null ? this.skills.MinRange : 0;
            this.DestPosition = GetPositionForEnemy(this._target.Position, minrange);
            if (!HasReachedDestination() && this._target != null)
            {
                StartRunning(this.DestPosition);
            }
            else
            {
                double distance2 = Vector.GetDistance2D(this.Position, this.RespawnOrigin);
                if (distance2 > 4000)
                {
                    OnEnemyOutOfRange(_target as Character);
                }
            }

            //Update time
            Lifespan.lasttick = Environment.TickCount;
        }

        private void OnIdleWalk(int t_diff)
        {
            //Update position
            UpdatePostion(t_diff);

            //If we reached the goal stand still
            if (HasReachedDestination())
            {
                this.Position = this.DestPosition;
                this.stance = 3;
            }

            //Update time
            Lifespan.lasttick = Environment.TickCount;
        }

        #endregion AI Helper Functions

        #region Constructor/Deconstructor

        public RandomizedMonster()
        {
            Lifespan.lasttick = Environment.TickCount;
            Lifespan = new LifespanAI.Lifespan();
        }

        #endregion Constructor/Deconstructor
    }
}