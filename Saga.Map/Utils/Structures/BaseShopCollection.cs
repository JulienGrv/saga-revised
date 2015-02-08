using Saga.Map;
using Saga.PrimaryTypes;
using Saga.Templates;
using System.Collections.Generic;

namespace Saga.Structures.Collections
{
    /// <summary>
    /// Abstract class servers a base shop collection. This class
    /// is not intented to be overriden by other classes outside this
    /// assenbly.
    /// </summary>
    public abstract class BaseShopCollection
    {
        /// <summary>
        /// List of Shoppairs
        /// </summary>
        protected internal List<ShopPair> list;

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseShopCollection()
        {
            list = new List<ShopPair>();
        }

        /// <summary>
        /// Adds a infinite stock item to the list
        /// </summary>
        /// <param name="ItemId">Itemid to lookup</param>
        public void AddGoods(uint ItemId)
        {
            Rag2Item item;
            if (Singleton.Item.TryGetItemWithCount(ItemId, 0, out item))
            {
                ShopPair pair = new ShopPair(item, true);
                list.Add(pair);
            }
        }

        /// <summary>
        /// Adds a stockable item to the list
        /// </summary>
        /// <param name="ItemId">Itemid to lookup</param>
        /// <param name="count">Number of items to stock</param>
        public void AddGoods(uint ItemId, byte count)
        {
            Rag2Item item;
            if (Singleton.Item.TryGetItemWithCount(ItemId, count, out item))
            {
                ShopPair pair = new ShopPair(item, false);
                list.Add(pair);
            }
        }

        /// <summary>
        /// Internal used structure shoppair
        /// </summary>
        protected internal sealed class ShopPair
        {
            /// <summary>
            /// Creates a new shoppair
            /// </summary>
            /// <param name="item">item to stock</param>
            /// <param name="NoStock">True if the item has infinite stock</param>
            public ShopPair(Rag2Item item, bool NoStock)
            {
                this.item = item;
                this.NoStock = NoStock;
            }

            /// <summary>
            /// Item associated with the shop
            /// </summary>
            public Rag2Item item;

            /// <summary>
            /// Boolean saying if the shop can be stocked
            /// </summary>
            public bool NoStock = true;
        }

        /// <summary>
        /// When overriden this will open the shoplist
        /// </summary>
        /// <param name="character">Character to open the list</param>
        /// <param name="basenpc">Npc who shows the list</param>
        public virtual void Open(Character character, BaseNPC basenpc)
        {
        }
    }
}