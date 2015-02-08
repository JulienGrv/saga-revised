using Saga.Map;
using Saga.PrimaryTypes;
using Saga.Shared.Definitions;
using Saga.Structures;
using System;
using System.Collections.Generic;

namespace Saga.Templates
{
    internal class Book : MapItem, OpenBook
    {
        #region OpenBook Members

        private Dictionary<uint, Rag2Collection> loottable = new Dictionary<uint, Rag2Collection>();

        public void OnOpenBook(Character sender)
        {
            Rag2Collection items = new Rag2Collection();
            foreach (Rag2Item c in Singleton.Itemdrops.FindItemDropsById(this.ModelId, sender._DropRate))
                items.Add(c);
            this.loottable.Add(sender.id, items);
        }

        #endregion OpenBook Members

        #region Disposible Members

        public void Dispose(Character target)
        {
            loottable.Remove(target.id);
        }

        #endregion Disposible Members

        #region ILootable Members

        public Rag2Collection GetLootList(Character target)
        {
            Rag2Collection items;
            if (!loottable.TryGetValue(target.id, out items))
            {
                items = new Rag2Collection();
            }

            return items;
        }

        private uint _LootLeader = 0;

        public uint LootLeader
        {
            get { return _LootLeader; }
        }

        #endregion ILootable Members

        public uint ActorId
        {
            get { return this.id; }
        }

        #region ILootable Members

        public bool GetLootCollection(Character target, out LootCollection collection)
        {
            throw new NotImplementedException();
        }

        #endregion ILootable Members
    }
}