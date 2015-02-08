using Saga.Enumarations;
using Saga.Map.Definitions.Misc;
using Saga.Packets;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;

namespace Saga.Map.Client
{
    [CLSCompliant(false)]
    partial class Client
    {
        private void CM_SELECTCHANNEL(CMSG_SELECTCHANNEL cpkt)
        {
            Console.WriteLine("Player selects channel {0}", cpkt.Channel);
        }

        private void CM_TAKESCREENSHOT(CMSG_TAKESCREENSHOT cpkt)
        {
            SMSG_SCREENSHOTALLOWED spkt = new SMSG_SCREENSHOTALLOWED();
            spkt.SessionId = this.character.id;
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// This function will initialize the trading.
        /// First it will check if the desired target is a existing target and fill it
        /// to your characer.target, by using is Character we check if the target is a valid
        /// Character or a other type of actor.
        ///
        /// If the selected character is a non-character or the target is yourself
        /// we'll send a reason of NO_TARGET. If the selected target already has a tradesession
        /// active we send a TRADE_ACTIVE. IF everything goes according to plans we'll not set
        /// a reason which equals sucess.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_TRADEINVITATION(CMSG_REQUESTTRADE cpkt)
        {
            if (Regiontree.TryFind(cpkt.TargetActor, this.character, out this.character._target))
                if (character.Target is Character)
                {
                    Character Target = this.character.Target as Character;
                    if (Target.TradeSession != null)
                    {
                        //CHARACTER CAN'T TRADE, HE IS ALREADY IN A TRADE
                        SMSG_TRADERESULT spkt = new SMSG_TRADERESULT();
                        spkt.ActorId = cpkt.TargetActor;
                        spkt.SessionId = this.character.id;
                        spkt.Reason = (byte)TradeResult.TargetAlreadyInTrade;
                        this.Send((byte[])spkt);
                    }
                    else
                    {
                        //TRADE SESSION ESTABLISHED
                        TradeSession tradesession = new TradeSession(this.character, Target);
                        Target.TradeSession = tradesession;
                        this.character.TradeSession = tradesession;

                        //TRADE INVITATION WAIT FOR CONFIRMATION
                        SMSG_TRADEINVITATION spkt = new SMSG_TRADEINVITATION();
                        spkt.SessionId = Target.id;
                        spkt.ActorId = this.character.id;
                        Target.client.Send((byte[])spkt);
                    }
                }
                else
                {
                    //SELECTED TARGET WAS NOT FOUND
                    SMSG_TRADERESULT spkt = new SMSG_TRADERESULT();
                    spkt.ActorId = cpkt.TargetActor;
                    spkt.SessionId = this.character.id;
                    spkt.Reason = (byte)TradeResult.TargetNotFound;
                    this.Send((byte[])spkt);
                }
        }

        /// <summary>
        /// This function will notify the people on the tradesession the you
        /// have set the amount of money to trade.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_TRADEMONEY(CMSG_TRADEZENY cpkt)
        {
            if (this.character.TradeSession != null)
            {
                //OBTAIN THE ORGIN TARGET
                Character target = (this.character.TradeSession.Source == this.character) ?
                    this.character.TradeSession.Target :
                    this.character.TradeSession.Source;

                if (cpkt.Zeny > this.character.ZENY)
                {
                    //NOT ENOUGH MONEY
                    SMSG_TRADERESULT2 spkt = new SMSG_TRADERESULT2();
                    spkt.Reason = (byte)TradeResult.NotEnoughMoney;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
                else
                {
                    //THIS IS USED BY YOURSELF YOU HAVE SET YOUR AMOUNT OF MONEY
                    SMSG_TRADEZENY spkt = new SMSG_TRADEZENY();
                    spkt.Zeny = cpkt.Zeny;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);

                    //THIS IS USED TO NOTIFY THE OTHER PERSON YOU HAVE SET THE AMOUNT OF MONEY
                    SMSG_TRADEZENYOTHER spkt2 = new SMSG_TRADEZENYOTHER();
                    spkt2.SessionId = target.id;
                    spkt2.Zeny = cpkt.Zeny;
                    target.client.Send((byte[])spkt2);

                    //SET THE MONEY
                    if (this.character == this.character.TradeSession.Source)
                        this.character.TradeSession.ZenySource = cpkt.Zeny;
                    else
                        this.character.TradeSession.ZenyTarget = cpkt.Zeny;
                }
            }
        }

        /// <summary>
        /// This function will notify the people in the tradession they have accepted
        /// or refused your request to trade. We invoked the methods on our
        /// tradesession in a event-based architechture.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_TRADEINVITATIONREPLY(CMSG_TRADEINVITATIONRESULT cpkt)
        {
            if (this.character.TradeSession != null)
            {
                //OBTAIN THE ORGIN TARGET
                Character target = (this.character.TradeSession.Source == this.character) ?
                    this.character.TradeSession.Target :
                    this.character.TradeSession.Source;

                if (this.character.TradeSession != null)
                    switch (cpkt.Result)
                    {
                        case 0:
                            SMSG_TRADERESULT spkt = new SMSG_TRADERESULT();
                            spkt.ActorId = this.character.id;
                            spkt.SessionId = target.id;
                            spkt.Reason = (byte)TradeResult.Success;
                            target.client.Send((byte[])spkt);
                            break;

                        case 1:
                            SMSG_TRADERESULT spkt2 = new SMSG_TRADERESULT();
                            spkt2.ActorId = target.id;
                            spkt2.SessionId = target.id;
                            spkt2.Reason = (byte)TradeResult.TargetCanceled;
                            target.client.Send((byte[])spkt2);
                            break;
                    }
            }
        }

        /// <summary>
        /// Occurs when setting an tradeitem
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_TRADEITEM(CMSG_TRADEITEM cpkt)
        {
            byte status = 1;
            try
            {
                //OBTAIN THE ORGIN TARGET
                Character target;
                TradeSession.TradeItem[] Items;
                if (this.character.TradeSession.Source == this.character)
                {
                    target = this.character.TradeSession.Target;
                    Items = this.character.TradeSession.SourceItem;
                }
                else
                {
                    target = this.character.TradeSession.Source;
                    Items = this.character.TradeSession.TargetItem;
                }

                Rag2Item item = this.character.container[cpkt.Item];
                if (item == null)
                {
                    SMSG_TRADERESULT2 spkt = new SMSG_TRADERESULT2();
                    spkt.Reason = (byte)TradeResult.Error;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
                if (item.tradeable == false)
                {
                    SMSG_TRADERESULT2 spkt = new SMSG_TRADERESULT2();
                    spkt.Reason = (byte)TradeResult.ItemNotTradeable;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
                else if (cpkt.Count > item.count)
                {
                    SMSG_TRADERESULT2 spkt = new SMSG_TRADERESULT2();
                    spkt.Reason = (byte)TradeResult.NotEnoughItems;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
                else
                {
                    TradeSession.TradeItem item2 = new TradeSession.TradeItem();
                    item2.Count = cpkt.Count;
                    item2.Slot = cpkt.Item;
                    Items[cpkt.Slot] = item2;

                    SMSG_TRADEITEMOTHER spkt = new SMSG_TRADEITEMOTHER();
                    spkt.Tradeslot = cpkt.Slot;
                    spkt.Item = item;
                    spkt.SessionId = target.id;
                    target.client.Send((byte[])spkt);

                    status = 0;
                }
            }
            finally
            {
                SMSG_TRADEITEM spkt = new SMSG_TRADEITEM();
                spkt.ItemSlot = cpkt.Item;
                spkt.Tradeslot = cpkt.Slot;
                spkt.Count = cpkt.Count;
                spkt.Status = status;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// This function will notify the people in the tradesession you have accepted
        /// the contents of the trade. In this context it means: items and/or the amount
        /// of money.
        /// </summary>
        private void CM_TRADECONTENTAGREE(CMSG_TRADECONTENTCONFIRM cpkt)
        {
            if (this.character.TradeSession != null)
            {
                //OBTAIN THE ORGIN TARGET
                Character target;
                if (this.character.TradeSession.Source == this.character)
                {
                    target = this.character.TradeSession.Target;
                }
                else
                {
                    target = this.character.TradeSession.Source;
                }

                //CONFIRM THE TRADELIST
                SMSG_TRADECONFIRM spkt = new SMSG_TRADECONFIRM();
                spkt.SessionId = this.character.id;
                target.client.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when your aggreeing with the trade.
        /// After the trade content has been agreed.
        /// </summary>
        private void CM_TRADECONFIRM(CMSG_TRADECONFIRM cpkt)
        {
            TradeSession session = this.character.TradeSession;

            if (session != null)
            {
                //OBTAIN THE ORGIN TARGET
                Character target;
                TradeSession.TradeItem[] Items;
                if (this.character.TradeSession.Source == this.character)
                {
                    target = session.Target;
                    Items = session.TargetItem;
                }
                else
                {
                    target = session.Source;
                    Items = session.SourceItem;
                }

                //Calculate required slots
                int slots = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (Items[i] == null) continue;
                    slots++;
                }

                if (slots > this.character.container.Capacity - this.character.container.Count)
                {
                    //Not enough space oponent
                    SMSG_TRADERESULT2 spkt = new SMSG_TRADERESULT2();
                    spkt.Reason = (byte)TradeResult.TargetNotEnoughInventorySpace;
                    spkt.SessionId = target.id;
                    target.client.Send((byte[])spkt);

                    //Not enough space myself
                    SMSG_TRADERESULT2 spkt2 = new SMSG_TRADERESULT2();
                    spkt2.Reason = (byte)TradeResult.NotEnoughIventorySpace;
                    spkt2.SessionId = this.character.id;
                    this.Send((byte[])spkt2);

                    //Set tradesession to null;
                    this.character.TradeSession = null;
                    target.TradeSession = null;
                    return;
                }
                else
                {
                    if (session.Source == this.character)
                    {
                        session.SourceHasAgreed = true;
                    }
                    else
                    {
                        session.TargetHasAgreed = true;
                    }
                }

                if (session.TargetHasAgreed && session.SourceHasAgreed)
                {
                    target.ZENY += session.ZenySource;
                    this.character.ZENY -= session.ZenySource;

                    target.ZENY -= session.ZenyTarget;
                    this.character.ZENY += session.ZenyTarget;

                    List<Rag2Item> SourceList = new List<Rag2Item>();
                    List<Rag2Item> TargetList = new List<Rag2Item>();
                    for (int i = 0; i < 16; i++)
                    {
                        TradeSession.TradeItem item = session.SourceItem[i];
                        if (item == null) continue;

                        Rag2Item ragitem = this.character.container[item.Slot];
                        if (ragitem.count - item.Count == 0)
                        {
                            SourceList.Add(ragitem);
                            this.character.container.RemoveAt(item.Slot);
                            SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                            spkt.Container = 2;
                            spkt.Index = item.Slot;
                            spkt.SessionId = this.character.id;
                            spkt.UpdateReason = (byte)ItemUpdateReason.SendToTrader;
                            this.Send((byte[])spkt);
                        }
                        else
                        {
                            SourceList.Add(ragitem.Clone(item.Count));
                            ragitem.count -= item.Count;
                            SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                            spkt.Container = 2;
                            spkt.Index = item.Slot;
                            spkt.SessionId = this.character.id;
                            spkt.UpdateReason = (byte)ItemUpdateReason.SendToTrader;
                            spkt.UpdateType = 4;
                            this.Send((byte[])spkt);
                        }
                    }

                    for (int i = 0; i < 16; i++)
                    {
                        TradeSession.TradeItem item = session.TargetItem[i];
                        if (item == null) continue;

                        Rag2Item ragitem = target.container[item.Slot];
                        if (ragitem.count - item.Count == 0)
                        {
                            TargetList.Add(ragitem);
                            this.character.container.RemoveAt(item.Slot);
                            SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                            spkt.Container = 2;
                            spkt.Index = item.Slot;
                            spkt.SessionId = target.id;
                            spkt.UpdateReason = (byte)ItemUpdateReason.SendToTrader;
                            target.client.Send((byte[])spkt);
                        }
                        else
                        {
                            TargetList.Add(ragitem.Clone(item.Count));
                            ragitem.count -= item.Count;
                            SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                            spkt.Container = 2;
                            spkt.Index = item.Slot;
                            spkt.SessionId = target.id;
                            spkt.UpdateReason = (byte)ItemUpdateReason.SendToTrader;
                            spkt.UpdateType = 4;
                            target.client.Send((byte[])spkt);
                        }
                    }

                    for (int i = 0; i < SourceList.Count; i++)
                    {
                        Rag2Item ragitem = SourceList[i];
                        int index = target.container.Add(ragitem);
                        SMSG_ADDITEM spkt = new SMSG_ADDITEM();
                        spkt.Container = 2;
                        spkt.UpdateReason = (byte)ItemUpdateReason.ReceiveFromTrade;
                        spkt.SessionId = target.id;
                        spkt.SetItem(ragitem, index);
                        target.client.Send((byte[])spkt);
                    }

                    for (int i = 0; i < TargetList.Count; i++)
                    {
                        Rag2Item ragitem = TargetList[i];
                        int index = this.character.container.Add(ragitem);
                        SMSG_ADDITEM spkt = new SMSG_ADDITEM();
                        spkt.Container = 2;
                        spkt.UpdateReason = (byte)ItemUpdateReason.ReceiveFromTrade;
                        spkt.SessionId = this.character.id;
                        spkt.SetItem(ragitem, index);
                        this.Send((byte[])spkt);
                    }

                    //Update zeny yourself
                    SMSG_SENDZENY spkt4 = new SMSG_SENDZENY();
                    spkt4.SessionId = this.character.id;
                    spkt4.Zeny = this.character.ZENY;
                    this.Send((byte[])spkt4);

                    //Update zeny opponent
                    SMSG_SENDZENY spkt5 = new SMSG_SENDZENY();
                    spkt5.SessionId = target.id;
                    spkt5.Zeny = target.ZENY;
                    target.client.Send((byte[])spkt5);

                    //Set traderesult to succesfull
                    SMSG_TRADERESULT2 spkt3 = new SMSG_TRADERESULT2();
                    spkt3.SessionId = this.character.id;
                    this.Send((byte[])spkt3);

                    //Set traderesult successfull oponent
                    SMSG_TRADERESULT2 spkt2 = new SMSG_TRADERESULT2();
                    spkt2.SessionId = target.id;
                    target.client.Send((byte[])spkt2);

                    //Set the tradesession to null
                    this.character.TradeSession = null;
                    target.TradeSession = null;
                }
            }
        }

        /// <summary>
        /// Occurs when canceling the trade
        /// </summary>
        private void CM_TRADECANCEL(CMSG_TRADECANCEL cpkt)
        {
            if (this.character.TradeSession != null)
            {
                //OBTAIN THE ORGIN TARGET
                Character target = (this.character.TradeSession.Source == this.character) ?
                    this.character.TradeSession.Target :
                    this.character.TradeSession.Source;

                SMSG_TRADERESULT2 spkt = new SMSG_TRADERESULT2();
                spkt.SessionId = target.id;
                spkt.Reason = (byte)TradeResult.TargetCanceled;
                target.client.Send((byte[])spkt);

                this.character.TradeSession.Target.TradeSession = null;
                this.character.TradeSession = null;
            }
        }
    }
}