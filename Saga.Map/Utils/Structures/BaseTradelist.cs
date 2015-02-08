using Saga.PrimaryTypes;
using Saga.Templates;
using System.Collections.Generic;

namespace Saga.Map.Utils.Structures
{
    public abstract class BaseTradelist
    {
        protected internal byte clvl;

        public virtual void Open(Character character, BaseNPC basenpc, uint buttonId)
        {
        }
    };

    public class Tradelist
    {
        protected internal List<Rag2Item> GetSupplementlist =
            new List<Rag2Item>();

        protected internal List<Rag2Item> GetProductionlist =
            new List<Rag2Item>();

        public Rag2Item GetProduction(int index)
        {
            return (index < GetProductionlist.Count) ? GetProductionlist[index] : null;
        }

        public Rag2Item GetSupplement(int index)
        {
            return (index < GetSupplementlist.Count) ? GetSupplementlist[index] : null;
        }

        public void AddSupplement(uint ItemId, byte count)
        {
            Rag2Item item;
            if (Singleton.Item.TryGetItemWithCount(ItemId, count, out item))
            {
                GetSupplementlist.Add(item);
            }
        }

        public void AddProduct(uint ItemId, byte count)
        {
            Rag2Item item;
            if (Singleton.Item.TryGetItemWithCount(ItemId, count, out item))
            {
                GetProductionlist.Add(item);
            }
        }
    }
}