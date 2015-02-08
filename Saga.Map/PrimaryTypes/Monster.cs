using Saga.Enumarations;
using Saga.Map;
using Saga.Map.Librairies;
using Saga.Network.Packets;
using Saga.Packets;
using Saga.Shared.Definitions;
using Saga.Structures;
using Saga.Tasks;
using System;
using System.Diagnostics;
using System.IO;

namespace Saga.PrimaryTypes
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public abstract class Monster : BaseMob, ILootable,
        Saga.Tasks.BattleThread.IBattleArtificialIntelligence, IHateable, IArtificialIntelligence
    {
        //Monster Members

        #region Internal Members

        private enum ActionType : byte { Walk = 0, Run = 1, Think = 2 }

        protected internal Point RespawnOrigin;

        //MOB-TEMPLATE VARIABLES
        protected internal ushort _LEVEL = 0;

        protected internal ushort _CEXP = 0;
        protected internal ushort _JEXP = 0;
        protected internal ushort _WEXP = 0;
        protected internal ushort _ASPD = 0;
        protected internal ushort _SIGHTRANGE = 0;
        protected internal ushort _SIZE = 0;
        protected internal ushort _WALKSPEED = 0;
        protected internal ushort _RUNSPEED = 0;
        protected internal int _AIMODE = 0;
        protected LifespanAI.Lifespan Lifespan = new LifespanAI.Lifespan();

        //Artificial Intelligence helpers
        protected Point DestPosition;

        /// <summary>
        /// Hate collection
        /// </summary>
        /// <remarks>
        /// The hate table is used to contain the hate towards a player. The actor
        /// with the highest hate will be targeted as by the monster.
        /// </remarks>
        protected HateCollection hatetable;

        /// <summary>
        /// Damage collection
        /// </summary>
        /// <remarks>
        /// The damage collection is used to give the right amount of exp rewards
        /// to the user inflicting the damage
        /// </remarks>
        protected DamageCollection damagetable;

        #endregion Internal Members

        #region Properties

        public bool IsAggresive
        {
            get
            {
                int mask = 1;
                return (_AIMODE & mask) == mask;
            }
        }

        public bool IsSupporter
        {
            get
            {
                int mask = 2;
                return (_AIMODE & mask) == mask;
            }
        }

        public bool IsCallForHelp
        {
            get
            {
                int mask = 3;
                return (_AIMODE & mask) == mask;
            }
        }

        public bool IsSwitchable
        {
            get
            {
                int mask = 4;
                return (_AIMODE & mask) == mask;
            }
        }

        public bool IsMoveable
        {
            get
            {
                int mask = 8;
                return (_AIMODE & mask) != mask;
            }
        }

        //Interface Members
        public byte LP
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public byte LPMAX
        {
            get
            {
                return 7;
            }
            set
            {
            }
        }

        #endregion Properties

        #region Monster Public Methods

        public Point GetPositionForEnemy(Point a, uint minrange)
        {
            Point Loc = a;
            ushort Yaw = Point.CalculateYaw(a, this.Position);
            double rad = Saga.Structures.Yaw.ToRadiants(Yaw);

            Loc.x += (float)((double)(100 + _SIZE + minrange) * Math.Cos(rad));
            Loc.y += (float)((double)(100 + _SIZE + minrange) * Math.Sin(rad));
            Loc.z = a.z;
            return this.currentzone.GetZ(Loc);
        }

        public Point GetRandomCoords()
        {
            ushort randYaw = (ushort)Managers.WorldTasks._random.Next(0, ushort.MaxValue);
            int distance = Managers.WorldTasks._random.Next(200, 1000);
            Point Loc = RespawnOrigin;

            double rad = Saga.Structures.Yaw.ToRadiants(randYaw);
            Loc.x += (float)(distance * Math.Cos(rad));
            Loc.y += (float)(distance * Math.Sin(rad));

            Loc.z = this.Position.z;
            return this.currentzone.GetZ(Loc);
        }

        public void StartMovement(Point a)
        {
            //Set is moving to true
            this.stance = 4;

            //Set my destination position
            this.DestPosition = a;

            this.Yaw = new Rotator(Point.CalculateYaw(this.Position, a), 0);

            //Generate packet
            SMSG_ACTORMOVEMENTSTART spkt = new SMSG_ACTORMOVEMENTSTART();
            spkt.SourceActorID = this.id;
            //spkt.Stance = this.stance;
            spkt.Speed = (ushort)this.Status.WalkingSpeed;
            spkt.Source(this.Position);
            spkt.Destination(this.DestPosition);
            //this.Position = NextPosition;

            //Send over movement start to all characters in neighbourhood
            Regiontree tree = this.currentzone.Regiontree;
            foreach (Character character in tree.SearchActors(this, SearchFlags.Characters))
                if (Point.IsInSightRangeByRadius(this.Position, character.Position))
                {
                    spkt.SessionId = character.id;
                    character.client.Send((byte[])spkt);
                }
        }

        public void StartRunning(Point a)
        {
            //Set is moving to true
            this.stance = 5;

            //Set my destination position
            this.DestPosition = a;

            //Generate packet
            SMSG_ACTORMOVEMENTSTART spkt = new SMSG_ACTORMOVEMENTSTART();
            spkt.SourceActorID = this.id;
            //spkt.Stance = this.stance;
            spkt.Speed = (ushort)this._RUNSPEED;
            spkt.Source(this.Position);
            spkt.Destination(this.DestPosition);
            //this.Position = NextPosition;

            //Send over movement start to all characters in neighbourhood
            Regiontree tree = this.currentzone.Regiontree;
            foreach (Character character in tree.SearchActors(this, SearchFlags.Characters))
                if (Point.IsInSightRangeByRadius(this.Position, character.Position))
                {
                    spkt.SessionId = character.id;
                    character.client.Send((byte[])spkt);
                }
        }

        public void StopMovement()
        {
            //Set my destination position
            this.DestPosition = this.Position;

            //Generate packet
            SMSG_MOVEMENTSTOPPED spkt = new SMSG_MOVEMENTSTOPPED();
            spkt.ActorID = this.id;
            spkt.speed = (ushort)this.Status.WalkingSpeed;
            spkt.DelayTime = 0;
            spkt.X = this.Position.x;
            spkt.Y = this.Position.y;
            spkt.Z = this.Position.z;
            spkt.yaw = this.Yaw;

            //Send over movement start to all characters in neighbourhood
            Regiontree tree = this.currentzone.Regiontree;
            foreach (Character character in tree.SearchActors(this, SearchFlags.Characters))
                if (Point.IsInSightRangeByRadius(this.Position, character.Position))
                {
                    spkt.SessionId = character.id;
                    character.client.Send((byte[])spkt);
                }

            //Set is moving to false
            this.stance = 3;
        }

        public void UpdatePostion(int tdiff)
        {
            ushort yaw = Point.CalculateYaw(this.Position, this.DestPosition);
            double diff = tdiff * ((double)this.Status.WalkingSpeed / (double)1000);
            Point Loc = this.Position;

            double rad = Saga.Structures.Yaw.ToRadiants(yaw);
            Loc.x += (float)(diff * Math.Cos(rad));
            Loc.y += (float)(diff * Math.Sin(rad));
            Loc.z = this.Position.z;
            this.Position = Loc;
        }

        public bool HasReachedDestination()
        {
            return Point.GetDistance3D(this.Position, this.DestPosition) <= (150 + (ushort)Status.WalkingSpeed + _SIZE);
        }

        #endregion Monster Public Methods

        #region Monster Static Members

        private void ForwardToAll(RelayPacket spkt)
        {
            //Send over movement start to all characters in neighbourhood
            foreach (MapObject c in this.currentzone.GetObjectsInRegionalRange(this))
                if (MapObject.IsPlayer(c))
                {
                    spkt.SessionId = c.id;
                    Character character = c as Character;
                    character.client.Send((byte[])spkt);
                }
        }

        public void ChangeStance(byte stance)
        {
            this.stance = stance;
            SMSG_ACTORCHANGESTATE spkt = new SMSG_ACTORCHANGESTATE();
            spkt.ActorID = this.id;
            spkt.TargetActor = (this._target != null) ? this._target.id : 0;
            spkt.State = 1;
            spkt.Stance = stance;
            ForwardToAll(spkt);
        }

        protected static Random random = new Random();

        public float[] GetRandomePosition()
        {
            float[] pos = new float[3];
            float[] unitvec = new float[3];
            pos[0] = RespawnOrigin.x;
            pos[1] = RespawnOrigin.y;

            unitvec = Vector.GetRandomUnitVector();
            pos = Vector.Add(pos, Vector.ScalarProduct(unitvec, Vector.rand.Next(0, (int)2000)));
            pos[2] = 4000f;
            this.currentzone.Heightmap.GetZ(pos[0], pos[1], out pos[2]);
            return pos;
        }

        #endregion Monster Static Members

        //Base Derived Members

        #region MabObject Members

        /// <summary>
        /// Occurs when a skill is used on this actor
        /// </summary>
        /// <param name="source">Source actor inflicting the skill</param>
        /// <param name="e">Argument with information</param>
        public override void OnSkillUsedByTarget(MapObject source, SkillBaseEventArgs argument)
        {
            try
            {
                if (argument.Context == SkillContext.SkillUse)
                {
                    SkillUsageEventArgs d = (SkillUsageEventArgs)argument;
                    //Add damage
                    if (argument.Result == SkillBaseEventArgs.ResultType.Normal || d.Result == SkillBaseEventArgs.ResultType.Critical)
                        damagetable.Add(source, d.Damage);
                }

                if (argument.SpellInfo.hate > 0)
                {
                    //Add hate
                    hatetable.Add(source, argument.SpellInfo.hate);

                    //Subscribe if not subscribed yet
                    if (this.BattleState.IsSubscribed == false)
                    {
                        SwitchSpeed(_RUNSPEED);
                        Saga.Tasks.BattleThread.Subscribe(this);
                    }

                    //Been Attacked
                    OnBeenAttacked(source);
                }
            }
            catch (Exception)
            {
                Trace.TraceError("Error casting skill on {0} from {1}", this, source);
            }
            finally
            {
                //Always call the base aprent
                base.OnSkillUsedByTarget(source, argument);
            }
        }

        /// <summary>
        /// Occurs then the specified characters goes out of sight.
        /// </summary>
        /// <param name="character">character who goes out of sight</param>
        public override void Disappear(Character character)
        {
            try
            {
                //DISAPPEAR THE TARGET
                if (character == this._target)
                {
                    OnEnemyOutOfRange(this._target as Character);
                }
            }
            catch (Exception)
            {
                Trace.TraceError("Error character disappears");
            }
            finally
            {
                //DISABLE THE HATE
                hatetable.Clear(character);
            }
        }

        /// <summary>
        /// Calculates if the mob is aggresive for the specified
        /// character.
        /// </summary>
        /// <param name="character">Character to calculate</param>
        /// <returns>1 for aggresive</returns>
        public override int ComputeIsAggresive(Character character)
        {
            int mask = 1;
            return (_AIMODE & mask);
        }

        /// <summary>
        /// Occurs when the actor is spawned
        /// </summary>
        public override void OnSpawn()
        {
            //Get the z coordinate
            this.stance = (byte)StancePosition.Reborn;
            this.Position = this.currentzone.GetZ(this.Position);

            //Reload battlestatus
            Singleton.Templates.FillByTemplate(this.ModelId, this);

            //Set a random yaw
            this.Yaw = new Rotator((ushort)random.Next(0, ushort.MaxValue), 0);

            //Reload damage table
            damagetable = new DamageCollection();

            //Dispose loot windo
            if (collection != null) collection.Dispose();
            collection = null;
        }

        /// <summary>
        /// Set's the initial spawn coords of the specified point.
        /// </summary>
        /// <param name="startpoint">Startpoint</param>
        public override void OnInitialize(Point startpoint)
        {
            this.RespawnOrigin = startpoint;

            //Create hate & damage table
            hatetable = new HateCollection();
            damagetable = new DamageCollection();

            //Clear collection table
            collection = null;

            //Load skill rotator
            string file = Server.SecurePath("~/mobskills/{0}.xml", this.ModelId);
            string defaultfile = Server.SecurePath("~/mobskills/default.xml");
            if (File.Exists(file))
                skills = SkillRotatorCollection.CreateFromFile(file);
            else if (File.Exists(defaultfile))
                skills = SkillRotatorCollection.CreateFromFile(defaultfile);
            else
                skills = SkillRotatorCollection.Empty;

            //Call base
            base.OnInitialize(startpoint);
        }

        /// <summary>
        /// Registers the mob
        /// </summary>
        public override void OnRegister()
        {
            this.currentzone.Regiontree.Subscribe(this);
        }

        /// <summary>
        /// Deregisters the mob this occurs when unloading the actor
        /// </summary>
        public override void OnDeregister()
        {
            BattleThread.Unsubscribe(this);
            base.OnDeregister();
        }

        /// <summary>
        /// Occurs when you die
        /// </summary>
        /// <param name="d"></param>
        public override void OnDie(MapObject d)
        {
            if (d != null && MapObject.IsPlayer(d))
                Die((Character)d);

            //Clear all hate
            hatetable = new HateCollection();

            //Stop attacking
            Saga.Tasks.BattleThread.Unsubscribe(this);

            //Subscribe as corpse
            Corpses.Subscribe(this);
        }

        /// <summary>
        /// Occurs when seeing a enemy die
        /// </summary>
        /// <param name="enemy"></param>
        public override void OnEnemyDie(MapObject enemy)
        {
            if (enemy.id == this._target.id)
            {
                this._target = null;
                //this.DestPosition = GenerateRandomPoint();
                Saga.Tasks.BattleThread.Unsubscribe(this);
                SwitchSpeed(_WALKSPEED);
                this.Lifespan.lasttick = Environment.TickCount;
                this.stance = 3;
            }
        }

        #endregion MabObject Members

        //Monster Events

        #region Events

        public void Die(Character character)
        {
            lock (this)
            {
                //Clear current target
                this._target = null;

                character.ISONBATTLE = false;

                //Create loot collection by the killing character
                collection = LootCollection.Create(this, character);

                //Hand out experience of the killing character
                damagetable.GiveExperienceRewards(character, this, this._CEXP, this._JEXP, this._WEXP);

                //Elimination Objective killing character
                //QuestBase.UserEliminateTarget(this.ModelId, character);

                // Subscript myself as a corpse
                StopMovement();
            }
        }

        /// <summary>
        /// Occurs when seeing a enemy
        /// </summary>
        /// <param name="enemy"></param>
        public void OnEnemyOnSight(Character enemy)
        {
            if (IsAggresive && this._target == null)
            {
                //SwitchTarget(enemy);
                //StartRunning(enemy.Position);
                Tasks.BattleThread.Subscribe(this);
            }
        }

        /// <summary>
        /// Processes the recieving of damage
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool OnBeenAttacked(MapObject character)
        {
            //Check to switch targets
            if (this._target == null)
            {
                //Switch target
                SwitchTarget(character);

                //Stop the current movement
                StopMovement();
            }
            else if (IsSwitchable && this._target != character)
            {
                if (random.Next(10, 100) < 60)
                {
                    //Switch target
                    SwitchTarget(character);
                }
            }
            else
            {
                //Stop the current movement
                StopMovement();
            }

            return true;
        }

        public Character GetLowestEnemy()
        {
            Character selection = null;
            int lowestLevel = -1;

            Regiontree tree = this.currentzone.Regiontree;
            foreach (Character chacter in tree.SearchActors(this, SearchFlags.Characters))
            {
                int level = chacter.Level - this.Level;
                double distance = Point.GetDistance3D(chacter.Position, this.Position);
                if (level < 30 && distance < this._SIGHTRANGE)
                {
                    if (lowestLevel == -1)
                    {
                        selection = chacter;
                        lowestLevel = level;
                        continue;
                    }

                    if (level > lowestLevel) continue;

                    selection = chacter;
                    lowestLevel = level;
                }
            }

            return selection;
        }

        /// <summary>
        /// Unsubsibes the battle ai if it loses sight
        /// of the player.
        /// </summary>
        public bool OnEnemyOutOfRange(Character enemy)
        {
            if (this._target != null && enemy == this._target)
            {
                StopMovement();

                lock (this._target)
                {
                    this._target = null;
                }

                this.HP = HPMAX;
                this.SP = SPMAX;
                //this.DestPosition = GenerateRandomPoint();
                Saga.Tasks.BattleThread.Unsubscribe(this);
                StopMovement();
                hatetable.Clear(enemy);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SwitchSpeed(ushort speed)
        {
            lock (this._status)
            {
                this.Status.WalkingSpeed = speed;
            }
        }

        public void SwitchTarget(MapObject objects)
        {
            //Check if we should switch our target
            if (this._target != objects)
            {
                //Switch target
                lock (this)
                {
                    this._target = objects;
                }
            }
        }

        #endregion Events

        #region ILootable Members

        private LootCollection collection = null;

        public bool GetLootCollection(Character target, out LootCollection collection)
        {
            collection = this.collection;
            return (collection != null) ? collection.CanOpen(target) : false;
        }

        #endregion ILootable Members

        #region IHateable Members

        HateCollection IHateable.Hatetable
        {
            get { return hatetable; }
        }

        #endregion IHateable Members

        #region IBattleArtificialIntelligence Members

        protected SkillRotatorCollection skills;

        private BattleThread.IBattlestate _battlestate = new BattleThread.IBattlestate();

        protected BattleThread.IBattlestate BattleState
        {
            get { return _battlestate; }
        }

        BattleThread.IBattlestate BattleThread.IBattleArtificialIntelligence.BattleState
        {
            get { return _battlestate; }
        }

        /// <summary>
        /// Callback function for the IBattleArtificialIntelligence.
        /// This is the leading function that processes any attacks for the current movement.
        /// </summary>
        void Saga.Tasks.BattleThread.IBattleArtificialIntelligence.Process()
        {
            //Unsubsribe if the task if no target is set
            if (this._target == null || this.stance == 7)
            {
                Saga.Tasks.BattleThread.Unsubscribe(this);
                return;
            }

            //Process a new attack only if the aspd has fired
            Character character = this._target as Character;
            if (Environment.TickCount - BattleState.LastUpdate > this._ASPD)
            {
                //Set the last tickcount
                BattleState.LastUpdate = Environment.TickCount;

                if (character != null)
                {
                    //int leveldifference = this.Level - character.Level;
                    bool cannotattack = character.stance == 7; //|| character._status.CannotAttack > 0 || leveldifference > character._status.AttackTresshold);
                    if (cannotattack)
                        return;
                }

                //Compute the distance to determine if it should use a ranged or skill or
                //a close combat skill.
                double distance = Vector.GetDistance2D(this.Position, character.Position);
                uint skill = 0;
                if (skills.FindSkillForRange(distance, out skill))
                {
                    Common.Skills.OffensiveSkillUse(character, this, skill, 3);
                }
            }
        }

        #endregion IBattleArtificialIntelligence Members

        #region IArtificialIntelligence Members

        bool IArtificialIntelligence.IsActivatedOnDemand
        {
            get { return true; }
        }

        LifespanAI.Lifespan IArtificialIntelligence.Lifespan
        {
            get { return Lifespan; }
        }

        void IArtificialIntelligence.Process()
        {
            //Do nothing
        }

        #endregion IArtificialIntelligence Members

        #region Constructor/Deconstructor

        public Monster()
        {
        }

        #endregion Constructor/Deconstructor
    }
}