using Saga.Map;
using Saga.PrimaryTypes;
using Saga.Shared.Definitions;
using Saga.Tasks;
using System;

namespace Saga.Templates
{
    internal class FrozenMonster : Monster, IArtificialIntelligence
    {
        #region Public Members

        public override void OnSpawn()
        {
            base.OnSpawn();
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
            LifespanAI.Unsubscribe(this);
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

        #region Private Members

        void IArtificialIntelligence.Process()
        {
            UpdateAdditions();
        }

        #endregion Private Members

        #region Constructor/Deconstructo

        public FrozenMonster()
        {
            Lifespan = new LifespanAI.Lifespan();
        }

        #endregion Constructor/Deconstructo
    }
}