using Saga.Enumarations;
using Saga.Map.Definitions.Misc;
using Saga.Map.Utils.Definitions.Misc;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;

namespace Saga.Map.Client
{
    /// <content>
    /// This part of the client class contains all packet handlers that
    /// interact with the auction. Currrently all packets here are correctly
    /// handeld. Last updated: 15-03-2008 - 20:01:00.
    /// </content>
    partial class Client
    {
        /// <summary>
        /// Searches in the market with the specified arguments.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_MARKET_SEARCH(CMSG_MARKETSEARCH cpkt)
        {
            //GENERATE MARKET SEARCH QUERY
            MarketSearchArgument Argument = new MarketSearchArgument();
            Argument.item_cat = cpkt.ItemType;
            Argument.item_class = cpkt.ItemClass;
            Argument.max_clvl = cpkt.MaxCLv;
            Argument.min_clvl = cpkt.MinCLv;
            Argument.searchstring = cpkt.SearchString;
            Argument.searchType = cpkt.GetSearchMode;
            Argument.SortBy = cpkt.Unknown2;
            Argument.index = cpkt.Unknown;

            //SEND ALL FOUND ITEMS GIVEN BY THE SEARCH QUERY
            SMSG_MARKETSEARCH spkt = new SMSG_MARKETSEARCH();
            spkt.SessionId = this.character.id;
            spkt.Unknown = 1;

            byte i = 0;
            foreach (MarketItemArgument args in Singleton.Database.SearchMarket(Argument))
            {
                spkt.Count = ++i;
                spkt.Add(args);
            }
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Buys the specified market item.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_MARKET_BUY(CMSG_MARKETBUY cpkt)
        {
            //HELPER VARIABLES
            AuctionArgument item;

            //ITEM ALREADY SOLD
            if (!Singleton.Database.GetItemByAuctionId(cpkt.ItemId, out item))
            {
                SMSG_MARKETBUY spkt = new SMSG_MARKETBUY();
                spkt.SessionId = this.character.id;
                spkt.Reason = 7;
                this.Send((byte[])spkt);
            }
            //NOT ENOUGH MONEY TO PURCHASE
            else if (item.zeny > this.character.ZENY)
            {
                SMSG_MARKETBUY spkt = new SMSG_MARKETBUY();
                spkt.SessionId = this.character.id;
                spkt.Reason = 2;
                this.Send((byte[])spkt);
            }
            //CHECK IF OWN INBOX IF FULL
            else if (Singleton.Database.GetInboxMailCount(this.character.Name) == 20)
            {
                SMSG_MARKETBUY spkt = new SMSG_MARKETBUY();
                spkt.SessionId = this.character.id;
                spkt.Reason = 4;
                this.Send((byte[])spkt);
            }
            //CHECK IF SENDER INBOX IS FULL
            else if (Singleton.Database.GetInboxMailCount(item.name) == 20)
            {
                SMSG_MARKETBUY spkt = new SMSG_MARKETBUY();
                spkt.SessionId = this.character.id;
                spkt.Reason = 5;
                this.Send((byte[])spkt);
            }
            //CASH ITEM DOES NOT EXISTS
            else if (!Singleton.Database.DeleteRegisteredAuctionItem(cpkt.ItemId))
            {
                SMSG_MARKETBUY spkt = new SMSG_MARKETBUY();
                spkt.SessionId = this.character.id;
                spkt.Reason = 7;
                this.Send((byte[])spkt);
            }
            //EVERYTHING OKAY
            else
            {
                //OKAY RESULT
                SMSG_MARKETBUY spkt = new SMSG_MARKETBUY();
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);

                //UPDATE ZENY
                this.character.ZENY -= item.zeny;
                CommonFunctions.UpdateZeny(this.character);

                //Buyer get's item
                MailItem buyer = new MailItem();
                buyer.Recieptent = this.character.Name;
                buyer.item = item.item;
                buyer.Content = "Auction";
                buyer.Topic = "Auction";
                buyer.Timestamp = DateTime.Now;
                buyer.Zeny = 0;

                //Reciever get's money
                MailItem reciever = new MailItem();
                reciever.item = null;
                reciever.Recieptent = item.name;
                reciever.Zeny = item.zeny;
                reciever.Topic = "Auction";
                reciever.Timestamp = DateTime.Now;
                reciever.Content = "Auction";

                //Add mail items
                Singleton.Database.InsertNewMailItem(null, buyer);
                Singleton.Database.InsertNewMailItem(null, reciever);
            }
        }

        /// <summary>
        /// Searches in the market for own items.
        /// </summary>
        private void CM_MARKET_SEARCHOWNERITEMS()
        {
            //SEARCHES FOR ALL OWNER BASED ITEMS
            SMSG_MARKETOWNERSEARCH spkt = new SMSG_MARKETOWNERSEARCH();
            spkt.SessionId = this.character.id;
            foreach (MarketItemArgument args in Singleton.Database.SearchMarketForOwner(this.character))
            {
                spkt.Add(args);
            }
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Searches in the market for you owner comment.
        /// </summary>
        private void CM_MARKET_OWNERCOMMENT(CMSG_MARKETOWNERCOMMENT cpkt)
        {
            if (cpkt.SessionId != this.character.id) return;

            //OBTAIN COMMENT
            string comment = Singleton.Database.FindCommentByPlayerId(this.character.ModelId);
            if (comment == null) comment = string.Empty;

            //SEND THE ATTAINED COMMENT OVER TO THE PLAYER
            SMSG_MARKETMESSAGE spkt = new SMSG_MARKETMESSAGE();
            spkt.Reason = (comment.Length == 0) ? (byte)0x0B : (byte)0;
            spkt.Message = comment;
            spkt.SessionId = this.character.id;
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Searches in the market for comment by the player.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_MARKET_COMMENT(CMSG_MARKETGETCOMMENT cpkt)
        {
            if (cpkt.SessionId != this.character.id) return;

            //OBTAIN COMMENT
            string comment = Singleton.Database.FindCommentById(cpkt.ItemId);
            if (comment == null) comment = string.Empty;

            //SEND THE ATTAINED COMMENT OVER TO THE PLAYER
            SMSG_MARKETCOMMENT spkt = new SMSG_MARKETCOMMENT();
            spkt.SessionId = this.character.id;
            spkt.Reason = (comment.Length == 0) ? (byte)0x0B : (byte)0;
            spkt.Message = comment;
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Changes your own comment.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_MARKET_CHANGECOMMENT(CMSG_MARKETMESSAGE cpkt)
        {
            //SET COMMENT
            uint result = Singleton.Database.UpdateCommentByPlayerId(this.character.ModelId, cpkt.Comment);

            //INDICATES WHETHER COMMENT COULD BE CHANGED
            SMSG_MARKETMESSAGERESULT spkt = new SMSG_MARKETMESSAGERESULT();
            spkt.SessionId = this.character.id;
            this.Send((byte[])spkt);

            //ACTUALLY APPLIES THE COMMENT
            SMSG_MARKETCOMMENT spkt2 = new SMSG_MARKETCOMMENT();
            spkt2.SessionId = this.character.id;
            spkt2.Message = cpkt.Comment;
            this.Send((byte[])spkt2);
        }

        /// <summary>
        /// Deletes a item from the market.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_MARKET_DELETEITEM(CMSG_MARKETDELETEITEM cpkt)
        {
            //HELPER VARIABLES
            AuctionArgument item;
            byte result = 0;

            try
            {
                //Failed retrieving
                if (!Singleton.Database.GetItemByAuctionId(cpkt.ItemId, out item))
                {
                    result = 1;
                }
                //Check my outbox
                else if (Singleton.Database.GetInboxMailCount(item.name) >= 20)
                {
                    result = 1;
                }
                //Failed unregistering
                else if (Singleton.Database.UnregisterMarketItem(cpkt.ItemId) == 0)
                {
                    result = 1;
                }
                else
                {
                    //Buyer get's item
                    MailItem buyer = new MailItem();
                    buyer.Recieptent = item.name;
                    buyer.item = item.item;
                    buyer.Content = "Auction";
                    buyer.Topic = "Auction";
                    buyer.Timestamp = DateTime.Now;
                    buyer.Zeny = 0;

                    //Add mail items
                    Singleton.Database.InsertNewMailItem(null, buyer);
                }
            }
            finally
            {
                SMSG_MARKETDELETEITEM spkt = new SMSG_MARKETDELETEITEM();
                spkt.SessionId = this.character.id;
                spkt.ItemID = cpkt.ItemId;
                spkt.Reason = result;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Registers a item from your inventory onto the market.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_MARKET_REGISTERITEM(CMSG_MARKETREGISTER cpkt)
        {
            SMSG_MARKETREGISTER spkt = new SMSG_MARKETREGISTER();
            byte result = 0;

            try
            {
                Rag2Item item = this.character.container[cpkt.Index];
                if (item == null) return;
                uint requiredZeny = (uint)(50 * cpkt.Days);

                //NOT ENOUGH MONEY TO REGISTER ITEM
                if (requiredZeny > this.character.ZENY)
                {
                    result = 6;
                }
                //CHECK REGISTERED ITEM COUNT
                else if (Singleton.Database.GetOwnerItemCount(this.character) == 20)
                {
                    result = 8;
                }
                //EVERYTHING OKAY
                else
                {
                    //Create mail argument

                    Rag2Item bitem = item.Clone(cpkt.Count);
                    MarketItemArgument argument = new MarketItemArgument();
                    argument.item = bitem;
                    argument.expires = DateTime.Now.AddDays(cpkt.Days);
                    argument.cat = (byte)item.info.categorie;
                    argument.price = cpkt.Price;
                    argument.sender = this.character.Name;
                    argument.itemname = item.info.name;
                    argument.id = Singleton.Database.RegisterNewMarketItem(this.character, argument);

                    spkt.Item = bitem;
                    spkt.AuctionID = argument.id;
                    spkt.Zeny = cpkt.Price;
                    spkt.ExpireDate = argument.expires;

                    this.character.ZENY -= requiredZeny;
                    CommonFunctions.UpdateZeny(this.character);

                    int newCount = item.count - cpkt.Count;
                    if (newCount > 0)
                    {
                        item.count = newCount;
                        SMSG_UPDATEITEM spkt2 = new SMSG_UPDATEITEM();
                        spkt2.Index = cpkt.Index;
                        spkt2.UpdateReason = (byte)ItemUpdateReason.AuctionRegister;
                        spkt2.UpdateType = 4;
                        spkt2.SessionId = this.character.id;
                        spkt2.Amount = (byte)item.count;
                        spkt2.Container = 2;
                        this.Send((byte[])spkt2);
                    }
                    else
                    {
                        this.character.container.RemoveAt(cpkt.Index);
                        SMSG_DELETEITEM spkt2 = new SMSG_DELETEITEM();
                        spkt2.Index = cpkt.Index;
                        spkt2.UpdateReason = (byte)ItemUpdateReason.AuctionRegister;
                        spkt2.UpdateReason = 4;
                        spkt2.SessionId = this.character.id;
                        spkt2.Container = 2;
                        this.Send((byte[])spkt2);
                    }
                }
            }
            //DATABASE ERROR
            catch (Exception e)
            {
                result = 1;
                Console.WriteLine(e);
            }
            finally
            {
                spkt.SessionId = this.character.id;
                spkt.Result = result;
                this.Send((byte[])spkt);
            }
        }
    }
}