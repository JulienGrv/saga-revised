using Saga.Enumarations;
using Saga.Map.Utils.Structures;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Quests;
using Saga.Structures;
using Saga.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Saga.Map.Client
{
    partial class Client
    {
        /// <summary>
        /// Occurs after a client displays a personal quest confirmation.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_PERSONALREQUESTCONFIRMATION(CMSG_PERSONALREQUEST cpkt)
        {
            BaseNPC npc = this.character.Target as BaseNPC;
            if (npc != null)
            {
                if (cpkt.Result == 2 && pendingquest != null)
                {
                    pendingquest.OnStart(this.character.id);
                    pendingquest.CheckQuest(this.character);
                    QuestBase.UserTalktoTarget(this.character.Target.ModelId, this.character, pendingquest);
                    CommonFunctions.RefreshPersonalRequests(this.character);
                    CommonFunctions.UpdateNpcIcons(this.character);
                    pendingquest = null;

                    Common.Actions.OpenMenu(
                        this.character, npc,
                        cpkt.Unknown,
                        DialogType.AcceptPersonalRequest,
                        new byte[] { }
                    );
                }
                else
                {
                    pendingquest = null;

                    Common.Actions.OpenMenu(
                        this.character, npc,
                        cpkt.Unknown,
                        DialogType.AcceptPersonalRequest,
                        npc.GetDialogButtons(this.character)
                    );
                }
            }
        }

        /// <summary>
        /// Occurs when a character selects a supply menu
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_SELECTSUPPPLYMENU(CMSG_SUPPLYSELECT cpkt)
        {
            try
            {
                //Try get dependecies
                MapObject regionoject = null;
                Regiontree tree = this.character.currentzone.Regiontree;
                Regiontree.TryFind(cpkt.ActorId, this.character, out regionoject);

                //Implicit casts
                BaseNPC npc = this.character.Target as BaseNPC;
                TradelistContainer container = this.character.Tag as TradelistContainer;

                if (npc != null && container != null)
                {
                    //Regossip the npc
                    npc.OnGossip(this.character);

                    //Find the tradelist
                    BaseTradelist utradelist;
                    if (container.dict.TryGetValue(cpkt.ButtonId, out utradelist))
                        utradelist.Open(this.character, npc, cpkt.ButtonId);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Tries to get the hate of the mob.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_REQUESTSHOWNPCLOCATION(CMSG_NPCLOCATIONSELECT cpkt)
        {
            List<GuidePoint> guidepoints = this.character.Tag as List<GuidePoint>;
            if (guidepoints != null && cpkt.LocationId < guidepoints.Count)
            {
                GuidePoint point = guidepoints[cpkt.LocationId];
                GuideNpc npoint = point as GuideNpc;
                GuidePosition ppoint = point as GuidePosition;

                if (npoint != null)
                {
                    //Predicated used searching
                    Predicate<MapObject> FindActor = delegate(MapObject actor)
                    {
                        return actor.ModelId == npoint.Map;
                    };

                    List<MapObject> matchingactors = new List<MapObject>();
                    Regiontree tree = this.character.currentzone.Regiontree;
                    matchingactors.AddRange(tree.SearchActors(FindActor, SearchFlags.Npcs));
                    if (matchingactors.Count > 0)
                    {
                        MapObject matchingactor = matchingactors[0];
                        SMSG_NPCASKLOCATIONSRC spkt = new SMSG_NPCASKLOCATIONSRC();
                        spkt.SessionId = this.character.id;
                        spkt.Result = 0;
                        spkt.DialogScript = 0;
                        spkt.NpcId = npoint.Map;
                        spkt.X = matchingactor.Position.x;
                        spkt.Y = matchingactor.Position.y;
                        spkt.Z = matchingactor.Position.z;
                        this.Send((byte[])spkt);
                    }
                }
                else if (ppoint != null)
                {
                    SMSG_NPCASKLOCATIONSRC spkt = new SMSG_NPCASKLOCATIONSRC();
                    spkt.SessionId = this.character.id;
                    spkt.Result = 2;
                    spkt.DialogScript = 0;
                    spkt.NpcId = ppoint.Npc;
                    spkt.X = ppoint.x;
                    spkt.Y = ppoint.y;
                    spkt.Z = ppoint.z;
                    spkt.Result2 = 6;
                    this.Send((byte[])spkt);
                }
            }
        }

        /// <summary>
        /// Tries to get the hate of the mob.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_GETHATEINFO(CMSG_HATEINFO cpkt)
        {
            //HELPER VARIABLES
            short Hate = 0;
            MapObject HatedObject;

            //TRY TO GET THE HATE
            if (Regiontree.TryFind(cpkt.Actor, this.character, out HatedObject))
            {
                IHateable HateableObject = HatedObject as IHateable;
                if (HateableObject != null)
                {
                    Hate = HateableObject.Hatetable[this.character];
                }
            }

            //SEND HATE
            SMSG_HATEINFO spkt = new SMSG_HATEINFO();
            spkt.ActorID = cpkt.Actor;
            spkt.Hate = (ushort)Hate;
            spkt.SessionId = this.character.id;
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// Occurs when exchanging goods
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_EXCHANGEGOODS(CMSG_SUPPLYEXCHANGE cpkt)
        {
            byte useresult = 1;
            byte result = 5;
            try
            {
                SingleTradelist stradelist = this.character.Tag as SingleTradelist;
                GroupedTradelist gtradelist = this.character.Tag as GroupedTradelist;
                List<Rag2Item> Production = null;
                List<Rag2Item> Supplyment = null;
                uint supplymoney = 0;
                uint productionmoney = 0;

                //Obtain type of tradelist
                if (stradelist != null)
                {
                    useresult = 1;
                    Production = stradelist.list.GetProductionlist;
                    Supplyment = stradelist.list.GetSupplementlist;
                    supplymoney = stradelist.supplementzeny;
                    productionmoney = stradelist.productionzeny;
                }
                else if (gtradelist != null)
                {
                    useresult = 2;
                    Production = gtradelist.list[cpkt.ButtonId].GetProductionlist;
                    Supplyment = gtradelist.list[cpkt.ButtonId].GetSupplementlist;
                }
                else
                {
                    return;
                }

                uint reqzeny = productionmoney - supplymoney;
                if (reqzeny < 0 && this.character.ZENY - reqzeny <= 0)
                {
                    result = 3;
                    return;
                }

                List<byte> ListOfIndexes = new List<byte>();
                int numdelitems = 0;
                for (int i = 0; i < Supplyment.Count; i++)
                {
                    bool IsFound = false;
                    Rag2Item item2 = Supplyment[i];

                    foreach (KeyValuePair<byte, Rag2Item> pair in this.character.container.GetAllItems())
                    {
                        Rag2Item item1 = pair.Value;

                        bool IsSame = item1.clvl == item2.clvl
                                   && item1.info == item2.info
                                   && item1.dyecolor == item2.dyecolor
                                   && item1.count >= item2.count
                                   && item1.clvl == item2.clvl;

                        //Set the to true, keep true if already true;
                        if (IsSame == true)
                        {
                            ListOfIndexes.Add(pair.Key);
                            IsFound |= IsSame;
                            int count = item1.count - item2.count;
                            if (count == 0) { numdelitems++; }
                            break;
                        }
                    }

                    if (IsFound == false)
                        break;
                }

                if (ListOfIndexes.Count != Supplyment.Count)
                {
                    result = 4;
                    return;
                }

                int reqslots = Production.Count - numdelitems;
                if (reqslots > 0 && this.character.container.Count + reqslots > this.character.container.Capacity)
                {
                    result = 2;
                    return;
                }

                for (int i = 0; i < ListOfIndexes.Count; i++)
                {
                    int index = ListOfIndexes[i];
                    int newcount = this.character.container[index].count - Supplyment[i].count;
                    if (newcount == 0)
                    {
                        this.character.container.RemoveAt(index);
                        SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                        spkt.Index = (byte)index;
                        spkt.Container = 2;
                        spkt.UpdateReason = (byte)ItemUpdateReason.SendToTrader;
                        spkt.SessionId = this.character.id;
                        this.Send((byte[])spkt);
                    }
                    else
                    {
                        this.character.container[index].count = newcount;
                        SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                        spkt.Amount = (byte)newcount;
                        spkt.UpdateReason = (byte)ItemUpdateReason.SendToTrader;
                        spkt.UpdateType = 4;
                        spkt.Container = 2;
                        spkt.SessionId = this.character.id;
                        spkt.Index = (byte)index;
                        this.Send((byte[])spkt);
                    }
                }

                foreach (Rag2Item item in Production)
                {
                    int index = this.character.container.Add(item);
                    SMSG_ADDITEM spkt = new SMSG_ADDITEM();
                    spkt.Container = 2;
                    spkt.UpdateReason = (byte)ItemUpdateReason.ReceiveFromTrade;
                    spkt.SetItem(item, index);
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }

                result = 0;
            }
            finally
            {
                SMSG_SUPPLYLISTRESULT spkt = new SMSG_SUPPLYLISTRESULT();
                if (useresult == 2)
                {
                    spkt.Reason1 = 1;
                    spkt.Reason2 = result;
                }
                else if (useresult == 1)
                {
                    spkt.Reason2 = result;
                }
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when a player selects a drop from the droplist.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_DROPSELECT(CMSG_DROPSELECT cpkt)
        {
            try
            {
                MapObject target = null;
                Rag2Collection list = null;
                LootCollection loot = this.character.Tag as LootCollection;

                //Corpse disappeared already
                if (!Regiontree.TryFind(cpkt.ActorID, this.character, out target))
                {
                    SMSG_NPCDROPLISTRESULT spkt = new SMSG_NPCDROPLISTRESULT();
                    spkt.ActorID = cpkt.ActorID;
                    spkt.Result = 0;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                    return;
                }
                //Uknown looting error (not implamented itseems)
                else if (loot == null)
                {
                    SMSG_NPCDROPLISTRESULT spkt = new SMSG_NPCDROPLISTRESULT();
                    spkt.ActorID = cpkt.ActorID;
                    spkt.Result = 4;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                    return;
                }
                else
                {
                    list = loot.Lootlist;
                }

                //OBTAIN THE REQUIRED ITEMS FROM THE MERCHANT
                Rag2Item item = list[cpkt.Index];
                if (item == null) return;

                //TEMP HELPER VARIABLES
                int nstacked = 0;
                List<int> update_queue = new List<int>();

                //WALKTHROUGH EVERY ITEM AND CHECK IF IT CAN BE STACKED
                foreach (int index in this.character.container.FindAllItems(item.info.item))
                {
                    Rag2Item invItem = this.character.container[index];
                    nstacked += Math.Min(0, (item.info.max_stack - invItem.count));
                    if (invItem.count < item.info.max_stack) update_queue.Add(index);
                }

                //CALCULATE THE AMOUNT OF NEW SLOTS REQUIRED
                int req_hslot = (int)item.count % (int)this.character.container.Capacity;
                int req_slots = item.count;
                //int max_stack = (item.info.max_stack == 0) ? 1 : item.info.max_stack;
                if (item.info.max_stack > 0)
                {
                    int div_rem = (int)((item.count - nstacked) / item.info.max_stack);
                    int div_rem2 = (req_hslot > 0) ? 1 : 0;
                    req_slots = div_rem + div_rem2;
                }

                if (this.character.container.Count + req_slots > this.character.container.Capacity)
                {
                    SMSG_NPCDROPLISTRESULT spkt = new SMSG_NPCDROPLISTRESULT();
                    spkt.ActorID = cpkt.ActorID;
                    spkt.Result = 3;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                    return;
                }

                //AMOUNT USED IN DECREMENT CALCULATIONS
                int amount = (int)item.count;

                //ITERATE THROUGH ALL AVAILABLE ITEM THAT CAN BE PROCESSED FOR UPDATES
                foreach (int invIndex in update_queue)
                {
                    Rag2Item invItem = this.character.container[invIndex];
                    int leftover = item.info.max_stack - invItem.count;
                    leftover = Math.Max(0, Math.Min(amount, leftover));
                    invItem.count += leftover;
                    amount -= leftover;

                    SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                    spkt.Index = (byte)invIndex;
                    spkt.UpdateReason = (byte)ItemUpdateReason.Obtained;
                    spkt.UpdateType = 4;
                    spkt.Container = 2;
                    spkt.Amount = (byte)invItem.count;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }

                //ITERATE THROUGH EVERY FREE INDEX AND PROCESS IT
                foreach (int invIndex in this.character.container.FindFreeIndexes())
                {
                    if (amount == 0) break;
                    int leftover = Math.Min(amount, item.info.max_stack);
                    Rag2Item invItem = item.Clone(leftover);
                    this.character.container[invIndex] = invItem;
                    amount -= leftover;

                    SMSG_ADDITEM spkt = new SMSG_ADDITEM();
                    spkt.Container = 2;
                    spkt.UpdateReason = (byte)ItemUpdateReason.Obtained;
                    spkt.SetItem(invItem, invIndex);
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }

                //CHECK THE LIST
                if (amount == 0)
                    list.RemoveAt(cpkt.Index);
                else
                    item.count = amount;

                //REFRESH THE INVENTORY
                loot.Open(this.character, target);
                QuestBase.UserObtainedItem(item.info.item, this.character);

                if (this.character.sessionParty != null)
                    for (int i = 0; i < this.character.sessionParty._Characters.Count; i++)
                    {
                        Character partyTarget = this.character.sessionParty._Characters[i];
                        if (partyTarget.id == this.character.id) continue;
                        SMSG_PARTYMEMBERLOOT spkt2 = new SMSG_PARTYMEMBERLOOT();
                        spkt2.SessionId = partyTarget.id;
                        spkt2.Index = (byte)(i + 1);
                        spkt2.ActorId = this.character.id;
                        spkt2.ItemId = item.info.item;
                        partyTarget.client.Send((byte[])spkt2);
                    }
            }
            //CATCH ALL UNKNOWN ERRORS
            catch (Exception e)
            {
                SMSG_NPCDROPLISTRESULT spkt = new SMSG_NPCDROPLISTRESULT();
                spkt.ActorID = cpkt.ActorID;
                spkt.Result = 5;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Occurs when closing the loot window.
        /// </summary>
        private void CM_CLEARCORPSE()
        {
            try
            {
                //Release resource
                LootCollection collection = this.character.Tag as LootCollection;
                if (collection != null)
                {
                    collection.ReleaseLootLock();
                    this.character.Tag = null;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Occurs when a player selects a event list from the event info
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_EVENTPARTICIPATE(CMSG_SELECTEVENTINFO cpkt)
        {
            try
            {
                Saga.Factory.EventManager.BaseEventInfo info = Singleton.EventManager.FindEventInformation(cpkt.EventId);
                if (info != null)
                {
                    info.OnEventParticipate(this.character.id);
                }
            }
            catch (Exception e)
            {
                //Write to trace log
                Trace.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Occurs when selecting a event reward
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_EVENTRECIEVEITEM(CMSG_RECEIVEEVENTITEM cpkt)
        {
            SMSG_EVENTINFO2 spkt = new SMSG_EVENTINFO2();
            spkt.Result = 1;

            try
            {
                Rag2Item item;
                EventItem b = Singleton.Database.FindEventItemById(this.character, cpkt.RewardId);
                if (b.ItemId == 0)
                {
                    //Event reward not found
                    spkt.Result = 1;
                }
                else if (b.ItemCount == 0)
                {
                    //Event reward not found
                    spkt.Result = 1;
                }
                else if (this.character.container.Count == this.character.container.Capacity)
                {
                    //Full storage
                    spkt.Result = 1;
                }
                else if (!Singleton.Item.TryGetItemWithCount(b.ItemId, b.ItemCount, out item))
                {
                    //Item id not found
                    spkt.Result = 1;
                }
                else
                {
                    Singleton.Database.DeleteEventItemId(cpkt.RewardId);

                    //AMOUNT USED IN DECREMENT CALCULATIONS
                    int index = this.character.container.FindFirstFreeIndex();

                    if (index > -1)
                    {
                        this.character.container[index] = item;
                        SMSG_ADDITEM spkt2 = new SMSG_ADDITEM();
                        spkt2.Container = 2;
                        spkt2.UpdateReason = (byte)ItemUpdateReason.EventReceived;
                        spkt2.SetItem(item, index);
                        spkt2.SessionId = this.character.id;
                        this.Send((byte[])spkt2);

                        //Type is used to calc type of item
                        //(21 seems to be used for Applogy Item)
                        if (item.info.type == 21)
                        {
                            Common.Skills.UpdateAddition(this.character, item.info.option_id);
                        }
                    }

                    spkt.Result = 0;
                }

                foreach (EventItem rewardItem in Singleton.Database.FindEventItemList(character))
                    spkt.AddItem(rewardItem.EventId, rewardItem.ItemId, rewardItem.ItemCount);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                spkt.SessionId = character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Is invoked when you greet the npc.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_NPCCHAT(CMSG_NPCCHAT cpkt)
        {
            if (Regiontree.TryFind(cpkt.ActorID, this.character, out this.character._target))
                if (character.Target is BaseNPC)
                {
                    BaseNPC current = character.Target as BaseNPC;
                    current.OnGossip(this.character);
                }
        }

        /// <summary>
        /// Occurs when you click on a object.
        /// Mapobject, Character doesn't matter.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_GETATTRIBUTE(CMSG_ATTRIBUTE cpkt)
        {
            MapObject Target;
            if (Regiontree.TryFind(cpkt.ActorID, this.character, out Target))
            {
                Target.OnClick(this.character);
            }

            //Reset the stance if we're not moving
            if (this.character.stance != 4 && this.character.stance != 5)
            {
                Regiontree tree = this.character.currentzone.Regiontree;
                foreach (Character current in tree.SearchActors(this.character, SearchFlags.Characters))
                {
                    if (current.id == this.character.id || current.client.isloaded == false || !Point.IsInSightRangeByRadius(current.Position, this.character.Position)) continue;
                    SMSG_ACTORCHANGESTATE spkt2 = new SMSG_ACTORCHANGESTATE();
                    spkt2.ActorID = character.id;
                    spkt2.State = (byte)((character.ISONBATTLE) ? 1 : 0);
                    spkt2.Stance = character.stance;
                    spkt2.TargetActor = character._targetid;
                    spkt2.SessionId = current.id;
                    current.client.Send((byte[])spkt2);
                }
            }
        }

        /// <summary>
        /// Occurs when releasing the attributes e.d. selection window.
        /// We reset the _targetid to 0 and let everybody see that don't
        /// look at this character anymore.
        /// </summary>
        private void CM_RELEASEATTRIBUTE()
        {
            this.character._targetid = 0;

            //Reset the stance
            foreach (Character current in this.character.currentzone.GetCharactersInSightRange(this.character))
            {
                if (current.id == this.character.id || current.client.isloaded == false) continue;
                SMSG_ACTORCHANGESTATE spkt2 = new SMSG_ACTORCHANGESTATE();
                spkt2.ActorID = character.id;
                spkt2.State = (byte)((character.ISONBATTLE) ? 1 : 0);
                spkt2.Stance = character.stance;
                spkt2.TargetActor = character._targetid;
                spkt2.SessionId = current.id;
                current.client.Send((byte[])spkt2);
            }
        }

        /// <summary>
        /// Occurs when leaving the npc. Use this target to release
        /// pointers we use internally.
        /// </summary>
        private void CM_LEAVENPC()
        {
            character._target = null;
        }

        /// <summary>
        /// Is used when the client invokes a submenu item.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_ONSELECTMENUSUBITEM(CMSG_NPCMENU cpkt)
        {
            try
            {
                if (character.Target is BaseNPC)
                {
                    BaseNPC current = character.Target as BaseNPC;
                    current.state.Open(cpkt.ButtonID, cpkt.MenuID, current, character);

                    SMSG_NPCMENU spkt = new SMSG_NPCMENU();
                    spkt.MenuID = cpkt.MenuID;
                    spkt.ButtonID = cpkt.ButtonID;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Is invoked when the players selects a button from the
        /// list choises.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_ONSELECTBUTTON(CMSG_SELECTBUTTON cpkt)
        {
            try
            {
                if (character.Target is BaseNPC)
                {
                    BaseNPC current = character.Target as BaseNPC;
                    current.state.Open(cpkt.Button, current, character);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }
    }
}