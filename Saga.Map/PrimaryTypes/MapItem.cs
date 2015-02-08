using Saga.Map;
using Saga.Map.Definitions.Misc;
using Saga.Packets;
using Saga.Quests;
using System;

namespace Saga.PrimaryTypes
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public abstract class MapItem : MapObject
    {
        #region Public Members

        /// <summary>
        /// Evemt called when clicking on the item
        /// </summary>
        /// <param name="target">Target who clicks on the object</param>
        public virtual void OnClicked(Character target)
        {
            Console.WriteLine(this.ModelId);
        }

        /// <summary>
        /// Get's if the item is interactable for the given actor
        /// </summary>
        /// <param name="target">Target to check against</param>
        /// <returns>1 If the item is interactable</returns>
        public virtual byte IsInteractable(Character target)
        {
            return 0;
        }

        /// <summary>
        /// Get's if the item can be highlighted to show a tooltip
        /// </summary>
        /// <param name="target">Target to check against</param>
        /// <returns>1 If the item can be highlighted</returns>
        public virtual byte IsHighlighted(Character target)
        {
            return 0;
        }

        #endregion Public Members

        protected void OnCheckQuest(Character target)
        {
            foreach (QuestBase c in target.QuestObjectives)
            {
                c.CheckQuest(target);
            }
        }

        protected void OnCheckMail(Character target)
        {
            SMSG_MAILLIST spkt = new SMSG_MAILLIST();
            spkt.SessionId = target.id;
            spkt.SourceActor = target.id;

            foreach (Mail c in Singleton.Database.GetInboxMail(target))
                spkt.AddMail(c);

            target.client.Send((byte[])spkt);
        }

        public override void ShowObject(Character character)
        {
            SMSG_ITEMINFO spkt = new SMSG_ITEMINFO();
            spkt.ActorID = this.id;
            spkt.NPCID = this.ModelId;
            spkt.X = this.Position.x;
            spkt.Y = this.Position.y;
            spkt.Z = this.Position.z;
            spkt.Yaw = this.Yaw;
            spkt.SessionId = character.id;
            spkt.IsActive = this.IsHighlighted(character);
            spkt.CanInteract = this.IsInteractable(character);
            character.client.Send((byte[])spkt);
        }
    }
}