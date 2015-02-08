using Saga.Enumarations;
using Saga.Map;
using System;

namespace Saga.PrimaryTypes
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [Serializable()]
    public class Rag2Item : ICloneable
    {
        #region Public Members

        /// <summary>
        /// Required clvl to use the item
        /// </summary>
        public byte clvl = 0;

        /// <summary>
        /// Durabillity of the item
        /// </summary>
        /// <remarks>
        /// This is only used by equipment. Other items like usable items don't
        /// seem to be effected by this.
        /// </remarks>
        public int durabillty = 0;

        /// <summary>
        /// The stacked count of the item.
        /// </summary>
        public int count = 1;

        /// <summary>
        /// Dye color of the item.
        /// </summary>
        public byte dyecolor = 0;

        /// <summary>
        /// If a item is tradeable or not.
        /// </summary>
        /// <remarks>
        /// When dealing with equipment the context of this boolean changes into
        /// sealed/unsealed.
        /// </remarks>
        public bool tradeable = true;

        /// <summary>
        /// Defines if a item is active or not
        /// </summary>
        /// <remarks>
        /// This settings seems to be only important with equipment. Equipment that
        /// can no longer be used because of job switches the equipment will get
        /// deactivated.
        /// </remarks>
        public byte active = 0;

        /// <summary>
        /// Collection of enchantments.
        /// </summary>
        /// <remarks>
        /// One enchantment is reserved for parasite stones the other enchantment is
        /// reserved for alterstones.
        /// </remarks>
        public uint[] Enchantments = new uint[2];

        /// <summary>
        /// Collection of shared information between other instances of a rag2item class
        /// with the same itemid.
        /// </summary>
        public Saga.Factory.ItemsFactory.ItemInfo info;

        #endregion Public Members

        #region Internal Members

        internal void Activate(AdditionContext apply, Character character)
        {
            if (apply == AdditionContext.Applied)
                Singleton.Additions.ApplyAddition(info.option_id, character);
            else if (apply == AdditionContext.Deapplied)
                Singleton.Additions.DeapplyAddition(info.option_id, character);
            for (int i = 0; i < 2; i++)
            {
                Factory.Spells.Info info1;
                Factory.Additions.Info info2;
                uint skill = Enchantments[i];
                if (skill > 0)
                {
                    if (Singleton.SpellManager.TryGetSpell(skill, out info1)
                    && Singleton.Additions.TryGetAddition(info1.addition, out info2))
                    {
                        info2.Do(character, character, apply);
                    }
                }
            }
        }

        #endregion Internal Members

        #region ICloneable Members

        /// <summary>
        /// Clones the current item
        /// </summary>
        /// <returns>a cloned object</returns>
        public object Clone()
        {
            Rag2Item item = new Rag2Item();
            item.durabillty = this.durabillty;
            item.clvl = this.clvl;
            item.info = this.info;
            return item;
        }

        /// <summary>
        /// Clones the current item with the specified count
        /// </summary>
        /// <returns>a cloned object</returns>
        public Rag2Item Clone(int count)
        {
            Rag2Item item = new Rag2Item();
            item.durabillty = this.durabillty;
            item.clvl = this.clvl;
            item.count = count;
            item.info = this.info;
            return item;
        }

        #endregion ICloneable Members

        #region Serialization

        public static uint CalculateRepairCosts(double Price, double CharacterLvl, double MaximumDurabillity)
        {
            double y = (double)CharacterLvl;
            double x = (double)Price;
            double BasePrice;
            BasePrice = (x * 0.05) + (x * 0.0005 * y);
            BasePrice = BasePrice + (BasePrice * (-1 / (1 + (0.5 * (MaximumDurabillity - 1))) + 1));
            return (uint)(Math.Ceiling(BasePrice) - 1);
        }

        /// <summary>
        /// Method serializes a existing rag2item to byte code.
        /// </summary>
        /// <remarks>
        /// This byte code is regonized by the network trafic. And therefor
        /// it's used in both packet sending as in database storage. Using
        /// this central method.
        /// </remarks>
        /// <param name="item">Item to serialize</param>
        /// <param name="buffer">desination array to contain the bytes</param>
        /// <param name="offset">starting offset where to serialize</param>
        public static void Serialize(Rag2Item item, byte[] buffer, int offset)
        {
            //OTHER STUFF
            Array.Copy(BitConverter.GetBytes(item.info.item), 0, buffer, offset + 0, 4);
            //Array.Copy(BitConverter.GetBytes(0), 0, this.data, index + 4, 4);
            //Array.Copy(BitConverter.GetBytes(0), 0, this.data, index + 8, 4);
            //Encoding.Unicode.GetBytes(name, 0, Math.Min(name.Length, 16), this.data, index + 12);
            //Array.Copy(BitConverter.GetBytes(0), 0, this.data, index + 45, 4);

            buffer[offset + 48] = item.dyecolor;
            buffer[offset + 49] = item.clvl;
            buffer[offset + 50] = (item.tradeable == false) ? (byte)1 : (byte)0;
            Array.Copy(BitConverter.GetBytes(item.durabillty), 0, buffer, offset + 51, 2);
            buffer[offset + 53] = (byte)item.count;
            Array.Copy(BitConverter.GetBytes(item.Enchantments[0]), 0, buffer, offset + 54, 4);
            Array.Copy(BitConverter.GetBytes(item.Enchantments[1]), 0, buffer, offset + 58, 4);
            //this.data[index + 66] = (byte)ItemIndex;
            //buffer[offset + 67] = 1;
        }

        /// <summary>
        /// Method deserializes byte code to a rag2item.
        /// </summary>
        /// <param name="item">Out rag2item container</param>
        /// <param name="buffer">byte array with supplied data</param>
        /// <param name="offset">starting offset where to deserialize</param>
        /// <returns>True if deserializing succeeds</returns>
        public static bool Deserialize(out Rag2Item item, byte[] buffer, int offset)
        {
            uint id = BitConverter.ToUInt32(buffer, offset);
            byte dyecolor = buffer[offset + 48];
            byte clvl = buffer[offset + 49];
            bool tradeable = (buffer[offset + 50] == 1) ? false : true;
            ushort dura = BitConverter.ToUInt16(buffer, offset + 51);
            byte itemcount = buffer[offset + 53];
            uint AlterStone = BitConverter.ToUInt32(buffer, offset + 54);
            uint ParasiteStone = BitConverter.ToUInt32(buffer, offset + 58);

            bool result = Singleton.Item.TryGetItem(id, out item);
            if (result)
            {
                item.tradeable = tradeable;
                item.durabillty = dura;
                item.dyecolor = dyecolor;
                item.clvl = clvl;
                item.count = itemcount;
                item.Enchantments[0] = AlterStone;
                item.Enchantments[1] = ParasiteStone;
            }

            return result;
        }

        #endregion Serialization

        #region Base Members

        /// <summary>
        /// Returns wether the item references are equal
        /// </summary>
        /// <param name="obj">Object to check against</param>
        /// <returns>True if object is the same</returns>
        public override bool Equals(object obj)
        {
            if (obj is Rag2Item)
            {
                Rag2Item item = obj as Rag2Item;
                return ReferenceEquals(this.info, item.info);
            }
            return false;
        }

        /// <summary>
        /// Returns the hashcode of the object
        /// </summary>
        /// <returns>Hashcode as an integer</returns>
        public override int GetHashCode()
        {
            return (int)this.info.item;
        }

        #endregion Base Members
    }
}