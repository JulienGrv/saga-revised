using Saga.PrimaryTypes;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Saga.Structures
{
    /// <summary>
    /// Collection for Rag2Items
    /// </summary>
    [Serializable()]
    public class Rag2Collection : IEnumerable
    {
        #region Private Members

        /// <summary>
        /// Lookup table to find the items
        /// </summary>
        private Dictionary<byte, Rag2Item> dictonairy = new Dictionary<byte, Rag2Item>();

        /// <summary>
        /// Capacity saying how many items it can contain
        /// </summary>
        private byte capacity = 50;

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Adds a item to the list
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>Index of the item</returns>
        public int Add(Rag2Item item)
        {
            int i = FindFirstFreeIndex();
            this.dictonairy.Add((byte)i, item);
            return i;
        }

        /// <summary>
        /// Removes a item
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True if the item was removed</returns>
        public bool Remove(Rag2Item item)
        {
            foreach (KeyValuePair<byte, Rag2Item> pair in this.dictonairy)
            {
                if (pair.Value == item)
                {
                    this.dictonairy.Remove(pair.Key);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes a item at the specified index
        /// </summary>
        /// <param name="index">Index where to remove</param>
        /// <returns>True if the item was removed</returns>
        public bool RemoveAt(int index)
        {
            return this.dictonairy.Remove((byte)index);
        }

        /// <summary>
        /// Gets a item at the specified index
        /// </summary>
        /// <param name="index">zero-based index</param>
        /// <returns>Item which was found</returns>
        public Rag2Item this[int index]
        {
            get
            {
                Rag2Item item;
                if (this.dictonairy.TryGetValue((byte)index, out item))
                    return item;
                return null;
            }
            set
            {
                this.dictonairy[(byte)index] = value;
            }
        }

        /// <summary>
        /// Get's the number of items in the collection
        /// </summary>
        public int Count
        {
            get
            {
                return this.dictonairy.Count;
            }
        }

        /// <summary>
        /// Get's or Set's the capacity of the collection
        /// </summary>
        public int Capacity
        {
            get
            {
                return this.capacity;
            }
            set
            {
                this.capacity = (byte)value;
            }
        }

        /// <summary>
        /// Locates the first free index.
        /// </summary>
        /// <returns>First zero-based free index</returns>
        public int FindFirstFreeIndex()
        {
            int capacity = this.capacity;
            for (int i = 0; i < capacity; i++)
            {
                Rag2Item item;
                if (dictonairy.TryGetValue((byte)i, out item) == false)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Locates all free indexes in the list
        /// </summary>
        /// <returns>A list of free indexes</returns>
        public List<int> FindFreeIndexes()
        {
            int capacity = this.capacity;
            List<int> list = new List<int>();
            for (int i = 0; i < capacity; i++)
            {
                Rag2Item item;
                if (dictonairy.TryGetValue((byte)i, out item) == false)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        /// <summary>
        /// Get's the item count of items with the supplied menu
        /// </summary>
        /// <param name="item">Itemid to find</param>
        /// <returns>Number of matching items</returns>
        public int GetItemCount(uint item)
        {
            int count = 0;
            foreach (KeyValuePair<byte, Rag2Item> pair in dictonairy)
            {
                if (pair.Value.info.item == item)
                    count += pair.Value.count;
            }
            return count;
        }

        /// <summary>
        /// Finds item indexes at the supplied menu
        /// </summary>
        /// <param name="item">Itemid to find</param>
        /// <returns>Collection of matching indexes</returns>
        public IEnumerable<int> FindAllItems(uint item)
        {
            foreach (KeyValuePair<byte, Rag2Item> pair in dictonairy)
            {
                if (pair.Value.info.item == item)
                    yield return pair.Key;
            }
        }

        /// <summary>
        /// Finds all items
        /// </summary>
        /// <returns>Collection of keyvaluepairs</returns>
        public IEnumerable<KeyValuePair<byte, Rag2Item>> GetAllItems()
        {
            foreach (KeyValuePair<byte, Rag2Item> pair in dictonairy)
            {
                yield return pair;
            }
        }

        /// <summary>
        /// Finds all items matching the predicate
        /// </summary>
        /// <returns>Collection of keyvaluepairs</returns>
        public IEnumerable<KeyValuePair<byte, Rag2Item>> GetAllItems(Predicate<KeyValuePair<byte, Rag2Item>> match)
        {
            foreach (KeyValuePair<byte, Rag2Item> pair in dictonairy)
            {
                if (match.Invoke(pair) == true)
                    yield return pair;
            }
        }

        /// <summary>
        /// Get the number of free stacks for a given item
        /// </summary>
        /// <param name="item">Itemid to match</param>
        /// <returns>Number of free stacks</returns>
        public int FreeStacks(uint item)
        {
            int count = 0;
            foreach (KeyValuePair<byte, Rag2Item> pair in dictonairy)
            {
                if (pair.Value.info.item == item)
                    count += (pair.Value.info.max_stack - pair.Value.count);
            }
            return count;
        }

        #endregion Public Members

        #region IEnumerable<Rag2Item> Members

        /// <summary>
        /// Gets all items in the collection
        /// </summary>
        /// <returns>Collection of items</returns>
        public IEnumerator<Rag2Item> GetEnumerator()
        {
            foreach (KeyValuePair<byte, Rag2Item> item in this.dictonairy)
                yield return item.Value;
        }

        #endregion IEnumerable<Rag2Item> Members

        #region IEnumerable Members

        /// <summary>
        /// Gets all items in the collection
        /// </summary>
        /// <returns>Collection of items</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}