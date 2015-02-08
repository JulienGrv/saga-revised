using Saga.IO;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Templates;
using System;
using System.Collections.Generic;
using System.IO;

namespace Saga.Map.Utils.Structures
{
    public class TradelistContainer
    {
        #region Private Members

        protected internal uint TradeMenu
            = 0;

        protected internal Dictionary<uint, BaseTradelist> dict
            = new Dictionary<uint, BaseTradelist>();

        #endregion Private Members

        #region Public Methods

        public void Open(Character character, BaseNPC basenpc)
        {
            character.Tag = this;
            SMSG_SUPPLYMENU spkt = new SMSG_SUPPLYMENU();
            spkt.MenuId = this.TradeMenu;
            spkt.SessionId = character.id;
            character.client.Send((byte[])spkt);
        }

        public SingleTradelist AddSingleContainer(uint key)
        {
            SingleTradelist tradelist = new SingleTradelist();
            dict.Add(key, tradelist);
            return tradelist;
        }

        public GroupedTradelist AddGroupedContainer(uint key)
        {
            GroupedTradelist tradelist = new GroupedTradelist();
            dict.Add(key, tradelist);
            return tradelist;
        }

        public static TradelistContainer FromFile(string filename)
        {
            TradelistContainer _container = new TradelistContainer();
            Tradelist currentlist = null;

            if (File.Exists(filename))
                using (TraderReader reader = TraderReader.Open(filename))
                    while (reader.Read())
                    {
                        _container.TradeMenu = reader.Menu;
                        switch (reader[reader.Trade].node)
                        {
                            case TraderReader.NodeTypeSpeciafier.Grouped:
                                goto AddGroupList;
                            case TraderReader.NodeTypeSpeciafier.Single:
                                goto AddSinglelist;
                        }
                        return null;
                    AddGroupList:
                        GroupedTradelist gtrade = GetSafeGroup(_container, reader.Trade);
                        currentlist = GetSafeList(gtrade, reader.Group - 1);
                        goto AddItemCount;
                    AddSinglelist:
                        SingleTradelist strade = GetSafeSingle(_container, reader.Trade);
                        currentlist = strade.list;
                        goto AddItemCount;
                    AddItemCount:
                        switch (reader.Type)
                        {
                            case TraderReader.NodeType.Production:
                                currentlist.AddProduct(reader.ItemId, reader.ItemCount);
                                break;

                            case TraderReader.NodeType.Supplement:
                                currentlist.AddSupplement(reader.ItemId, reader.ItemCount);
                                break;
                        }
                    }

            return _container;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Helper funtion
        /// </summary>
        private static Tradelist GetSafeList(SingleTradelist l)
        {
            return l.list;
        }

        /// <summary>
        /// Helper funtion
        /// </summary>
        private static Tradelist GetSafeList(GroupedTradelist l, int Group)
        {
            try
            {
                for (int i = l.list.Count; i < Group + 1; i++)
                {
                    l.AddList();
                }

                return l.list[Group];
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error get {0}", Group), ex);
            }
        }

        /// <summary>
        /// Helper funtion
        /// </summary>
        private static GroupedTradelist GetSafeGroup(TradelistContainer _container, uint TradeId)
        {
            BaseTradelist blist = null;
            _container.dict.TryGetValue(TradeId, out blist);
            GroupedTradelist glist = blist as GroupedTradelist;

            if (glist != null)
            {
                return glist;
            }
            else
            {
                glist = new GroupedTradelist();
                _container.dict[TradeId] = glist;
                return glist;
            }
        }

        /// <summary>
        /// Helper funtion
        /// </summary>
        private static SingleTradelist GetSafeSingle(TradelistContainer _container, uint TradeId)
        {
            BaseTradelist blist = null;
            _container.dict.TryGetValue(TradeId, out blist);
            SingleTradelist slist = blist as SingleTradelist;
            if (slist != null)
            {
                return slist;
            }
            else
            {
                slist = new SingleTradelist();
                _container.dict[TradeId] = slist;
                return slist;
            }
        }

        #endregion Private Methods
    }
}