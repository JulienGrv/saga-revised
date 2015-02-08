using Saga.PrimaryTypes;
using Saga.Quests;
using Saga.Scripting.Interfaces;

namespace Saga.Templates
{
    internal class ActionObject : MapItem, OpenBox
    {
        #region Base Members

        public override byte IsHighlighted(Character target)
        {
            return (byte)(QuestBase.IsActionObjectActivated(this.ModelId, target) ? 1 : 0);
        }

        public override byte IsInteractable(Character target)
        {
            return (byte)(QuestBase.IsActionObjectActivated(this.ModelId, target) ? 1 : 0);
        }

        #endregion Base Members

        #region OpenBox Members

        public void OnOpenBox(Character sender)
        {
            LootCollection collection;
            this.GetLootCollection(sender, out collection);
            collection.Open(sender, this as MapObject);
        }

        #endregion OpenBox Members

        #region ILootable Members

        public bool GetLootCollection(Character target, out LootCollection collection)
        {
            collection = LootCollection.Create(this, target);
            return true;
        }

        #endregion ILootable Members
    }
}