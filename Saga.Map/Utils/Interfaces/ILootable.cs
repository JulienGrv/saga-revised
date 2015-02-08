using Saga.PrimaryTypes;

namespace Saga.Shared.Definitions
{
    /// <summary>
    /// Interface for object which should have be considered lootable.
    /// </summary>
    /// <remarks>
    /// Make your object inherit from this interfact if you wish to add
    /// the posibility to make the object lootable. Without this inheriting
    /// from this interface the system will fail to find the actor.
    ///
    /// If you wish to directly open the loot collection you can use collection.Open()
    /// Internally the system uses the "CanOpen" method to check wether a collection
    /// can be opened with checking the party settings.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    ///using System;
    ///using System.Collections.Generic;
    ///using System.Text;
    ///using Saga.Quests;
    ///using Saga.PrimaryTypes;
    ///using Saga.Scripting.Interfaces;
    ///using Saga.Structures;
    ///namespace Saga.Templates
    ///{
    ///    class ActionObject : MapItem, OpenBox
    ///    {
    ///        public override byte IsHighlighted(Character target)
    ///        {
    ///            return (byte)(QuestBase.IsActionObjectActivated(this.ModelId, target) ? 1 : 0);
    ///        }
    ///
    ///        public override byte IsInteractable(Character target)
    ///        {
    ///            return (byte)(QuestBase.IsActionObjectActivated(this.ModelId, target) ? 1 : 0);
    ///        }
    ///
    ///        public void OnOpenBox(Character sender)
    ///        {
    ///            LootCollection collection;
    ///            current.GetLootCollection(character, collection);
    ///            collection.Open(character, target as MapObject);
    ///        }
    ///
    ///        public bool GetLootCollection(Character target, out LootCollection collection)
    ///        {
    ///            collection = LootCollection.Create(this, target);
    ///            return true;
    ///        }
    ///    }
    ///}
    /// ]]>
    /// </example>
    public interface ILootable
    {
        bool GetLootCollection(Character target, out LootCollection collection);
    }
}