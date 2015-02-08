using Saga.Map;
using Saga.Packets;
using Saga.Structures;
using System;
using System.Diagnostics;

namespace Saga.PrimaryTypes
{
    /// <summary>
    /// Base object all actors inherit.
    /// </summary>
    [Serializable()]
    public abstract class MapObject : IActorid
    {
        #region Private Members

        /// <summary>
        /// Stores the position
        /// </summary>
        private Point position;

        /// <summary>
        /// Contains the session id of the item
        /// </summary>
        private uint _id;

        /// <summary>
        /// Contains the region id of the item
        /// </summary>
        private uint _region;

        /// <summary>
        /// Contains the yaw of the item (rotation)
        /// </summary>
        private Rotator _Yaw;

        /// <summary>
        /// Contains a boolean weather the item can respawn or not
        /// </summary>
        private bool _canrespawn = true;

        #endregion Private Members

        #region Protected Internal Members

        /// <summary>
        /// Contains extra storage (in character this means the characterid) while with
        /// normal npc's this means modelid
        /// </summary>
        protected internal uint _ModelId;

        /// <summary>
        /// Contains the zone of the item
        /// </summary>
        internal Zone _currentzone;

        #endregion Protected Internal Members

        #region Public Members

        /// <summary>
        /// Get's the zone of the mapobject
        /// </summary>
        public Zone currentzone
        {
            get
            {
                return _currentzone;
            }
            internal set
            {
                _currentzone = value;
            }
        }

        /// <summary>
        /// Get's the yaw of the mapobject
        /// </summary>
        public Rotator Yaw
        {
            get
            {
                return _Yaw;
            }
            set
            {
                _Yaw = value;
            }
        }

        /// <summary>
        /// Get's the session id of the mapobject
        /// </summary>
        public uint id
        {
            get
            {
                return _id;
            }
            internal set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Get's the session id of the mapobject
        /// </summary>
        public uint ModelId
        {
            get
            {
                return _ModelId;
            }
            internal set
            {
                _ModelId = value;
            }
        }

        /// <summary>
        /// Get's the session id of the mapobject
        /// </summary>
        public uint region
        {
            get
            {
                return _region;
            }
            internal set
            {
                _region = value;
            }
        }

        /// <summary>
        /// Get's whether the mob can respawn
        /// </summary>
        public bool CanRespawn
        {
            get
            {
                return _canrespawn;
            }
            internal set
            {
                _canrespawn = value;
            }
        }

        /// <summary>
        /// Get's or set's position of the object
        /// </summary>
        public Point Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        #endregion Public Members

        #region Public Methods

        /// <summary>
        /// Internal function to show the object
        /// </summary>
        /// <param name="character">Character who to show the packet</param>
        public abstract void ShowObject(Character character);

        /// <summary>
        /// Internal function to hide the object
        /// </summary>
        /// <param name="character"></param>
        public virtual void HideObject(Character character)
        {
            try
            {
                //Actor disappears
                SMSG_ACTORDELETE spkt2 = new SMSG_ACTORDELETE();
                spkt2.ActorID = this.id;
                spkt2.SessionId = character.id;
                character.client.Send((byte[])spkt2);
            }
            catch (Exception)
            {
                Trace.TraceError("Error sending delete actor");
            }
        }

        /// <summary>
        /// Registers a charcter on the associated actor manager
        /// </summary>
        public virtual void OnRegister()
        {
            this.currentzone.Regiontree.Subscribe(this);
            Trace.WriteLine("DEBUG", string.Format("MapObject registered: {0:X8} {1}", this.region, this.ToString()));
        }

        /// <summary>
        /// Deregisters a character to the associated actor manager
        /// </summary>
        public virtual void OnDeregister()
        {
            this.currentzone.Regiontree.Unsubscribe(this);
        }

        /// <summary>
        /// Event called when a actor appears
        /// </summary>
        /// <param name="character"></param>
        public virtual void Appears(Character character)
        {
        }

        /// <summary>
        /// Event called when a actor disappears
        /// </summary>
        public virtual void Disappear(Character character)
        {
        }

        /// <summary>
        /// Event called when spawning the actor
        /// </summary>
        public virtual void OnSpawn()
        {
        }

        /// <summary>
        /// Event called when casting a spell
        /// </summary>
        /// <param name="source">Source actor using the skill</param>
        /// <param name="e">Spell argument</param>
        public virtual void OnSkillUsedByTarget(MapObject source, SkillBaseEventArgs e)
        {
        }

        /// <summary>
        /// Event called when actor dies
        /// </summary>
        /// <param name="d">Actor who's responsible for the death</param>
        public virtual void OnDie(MapObject d)
        {
        }

        /// <summary>
        /// Occurs when seeing a enemy die
        /// </summary>
        /// <param name="enemy"></param>
        public virtual void OnEnemyDie(MapObject enemy)
        {
        }

        /// <summary>
        /// Event called when the actor is beeing clicked
        /// </summary>
        public virtual void OnClick(Character target)
        {
        }

        /// <summary>
        /// Event called when the actor is loaded
        /// </summary>
        public virtual void OnLoad()
        {
        }

        /// <summary>
        /// Event called when the actor is initialized
        /// </summary>
        /// <param name="startpoint"></param>
        public virtual void OnInitialize(Point startpoint)
        {
            this.position = startpoint;
            this.position.z += 15;
        }

        #endregion Public Methods

        #region Public Static Methods

        /// <summary>
        /// Checks if the supplied mapobject is a character instance
        /// </summary>
        /// <param name="instance">Instance to check</param>
        /// <returns>True if the object is a player</returns>
        public static bool IsPlayer(MapObject instance)
        {
            return instance.id < Regiontree.PlayerBorder;
        }

        /// <summary>
        /// Checks if the supplied mapobject is a mapitem instance
        /// </summary>
        /// <param name="instance">Instance to check</param>
        /// <returns>True if the object is a mapitem</returns>
        public static bool IsMapItem(MapObject instance)
        {
            return instance.id < Regiontree.MapItemBorder;
        }

        /// <summary>
        /// Checks if the supplied mapobject is a npc instance
        /// </summary>
        /// <param name="instance">Instance to check</param>
        /// <returns>True if the object is a npc (this includes monsters)</returns>
        public static bool IsNpc(MapObject instance)
        {
            return instance.id >= Regiontree.MapItemBorder;
        }

        /// <summary>
        /// Checks if the supplied mapobject is a monster instance
        /// </summary>
        /// <param name="instance">Instance to check</param>
        /// <returns>True if the object is a monster</returns>
        public static bool IsMonster(MapObject instance)
        {
            return IsNpc(instance) && instance.ModelId >= Regiontree.NpcIndexBorder;
        }

        /// <summary>IActorid
        /// Checks if the supplied mapobject is a regulair npc instance
        /// </summary>
        /// <param name="instance">Instance to check</param>
        /// <returns>True if the object is a  regulair npc</returns>
        public static bool IsNotMonster(MapObject instance)
        {
            return IsNpc(instance) && (instance.ModelId < Regiontree.NpcIndexBorder);
        }

        #endregion Public Static Methods

        #region Updates

        /// <summary>
        /// Event for locking updates for the current object
        /// </summary>
        public virtual void Lock()
        {
        }

        /// <summary>
        /// Event for releasing updates for the current object
        /// </summary>
        public virtual void Release()
        {
        }

        /// <summary>
        /// Event for sending updates to the specified character
        /// </summary>
        /// <param name="character"></param>
        public virtual void Flush(Character character)
        {
        }

        #endregion Updates

        #region IActorid Members

        uint IActorid.Id
        {
            get { return this._id; }
        }

        #endregion IActorid Members

        public override string ToString()
        {
            return string.Format("MapObject id:{0}", this.ModelId);
        }
    }
}