using Saga.Map;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;

namespace Saga.Factory
{
    public class ItemsDrops : FactoryBase
    {
        #region Ctor/Dtor

        public ItemsDrops()
        {
        }

        ~ItemsDrops()
        {
            this.item_drops = null;
        }

        #endregion Ctor/Dtor

        #region Internal Members

        public Dictionary<uint, DropInfo[]> item_drops;

        #endregion Internal Members

        #region Public Methods

        private Random random = new Random();

        public IEnumerable<Rag2Item> FindItemDropsById(uint id, double droprate)
        {
            foreach (KeyValuePair<uint, Rag2Item> itempair in Common.Special.GetLootList(id))
            {
                if (CheckAgainstDroprate(itempair.Value, itempair.Key, droprate))
                    yield return itempair.Value;
            }
        }

        public bool CheckAgainstDroprate(Rag2Item item, uint droprate, double reducement)
        {
            int b = 9999 - (int)((double)random.Next(600, 1000) * (double)(reducement + Singleton.experience.Modifier_Drate));
            int c = b > 2000 ? b : 2000;

            int i = random.Next(0, c);
            int a = i;

            if (i < droprate)
            {
                int increment = 4000;
                while (item.count < item.info.max_stack)
                {
                    a += increment;
                    c += increment;

                    int d = random.Next(a, c);
                    if (d < droprate)
                    {
                        item.count++;
                    }
                    else
                    {
                        break;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Public Methods

        #region Protected Properties

        /// <summary>
        /// Get the notification string.
        /// </summary>
        /// <remarks>
        /// We used notification strings from the resource files. This way it's easier
        /// for us to make a multilanguagable server. And a golden rule in C# is that
        /// strings are slow, so rather have it instanted once by the resource file than
        /// reallocting a new string for every progress report.
        /// </remarks>
        protected override string Notification
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION__ITEMDROPS; }
        }

        /// <summary>
        /// Get the readystate string.
        /// </summary>
        /// <remarks>
        /// We used readystate strings from the resource files. This way it's easier
        /// for us to make a multilanguagable server. And a golden rule in C# is that
        /// strings are slow, so rather have it instanted once by the resource file than
        /// reallocting a new string for every progress report.
        /// </remarks>
        protected override string ReadyState
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_ITEMDROPS; }
        }

        #endregion Protected Properties

        #region Nested Classes/Structures

        public class DropInfo
        {
            public uint item;
            public uint rate;

            public DropInfo(uint item, uint rate)
            {
                this.item = item;
                this.rate = rate;
            }
        }

        #endregion Nested Classes/Structures
    }
}