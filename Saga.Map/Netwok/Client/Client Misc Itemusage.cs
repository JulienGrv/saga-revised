using Saga.Enumarations;
using Saga.Map.Utils.Definitions.Misc;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Quests;
using Saga.Shared.Definitions;
using Saga.Structures;
using Saga.Structures.Collections;
using Saga.Tasks;
using Saga.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Saga.Map.Client
{
    partial class Client
    {
        /// <summary>
        /// This function is used when a player discards item x from either their storage
        /// or from their inventory. Also here we safely check for any weird stuff.
        /// </summary>
        /// <remarks>
        /// This function should also check if you have the item you want to delete is a quest item
        /// because if the quest is active you shouln't be allowed to discard your item.
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_DISCARDITEM(CMSG_DELETEITEM cpkt)
        {
            Rag2Item item = null;
            if (cpkt.Container == 2)
            {
                item = this.character.container[cpkt.Index];
                if (item == null)
                {
                    Common.Errors.GeneralErrorMessage(
                        this.character,
                        (uint)Generalerror.InventoryItemNotFound
                    );
                    return;
                }
                else
                {
                    if (QuestBase.IsDiscardAble(item.info.item, this.character))
                    {
                        Common.Errors.GeneralErrorMessage(
                            this.character,
                            (uint)Generalerror.CannotDiscard
                        );
                        return;
                    }
                }
            }
            else if (cpkt.Container == 3)
            {
                item = this.character.STORAGE[cpkt.Index];
                if (item == null)
                {
                    Common.Errors.GeneralErrorMessage(
                        this.character,
                        (uint)Generalerror.StorageItemNotFound
                    );
                    return;
                }
                else
                {
                    if (QuestBase.IsDiscardAble(item.info.item, this.character))
                    {
                        Common.Errors.GeneralErrorMessage(
                            this.character,
                            (uint)Generalerror.CannotDiscard
                        );
                        return;
                    }
                }
            }
            else
            {
                Common.Errors.GeneralErrorMessage(
                    this.character,
                    (uint)Generalerror.WrongServerIndex
                );

                return;
            }

            //VALIDATE ITEM INFORMATION
            if (cpkt.Amount > item.count || cpkt.Amount > item.info.max_stack) return;

            //PROCESS DISCARDING
            int nCount = item.count - cpkt.Amount;
            if (nCount == 0)
            {
                this.character.container.RemoveAt(cpkt.Index);
                SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                spkt.Container = cpkt.Container;
                spkt.Index = cpkt.Index;
                spkt.UpdateReason = 1;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
            else
            {
                item.count = nCount;
                SMSG_UPDATEITEM spkt2 = new SMSG_UPDATEITEM();
                spkt2.Amount = (byte)nCount;
                spkt2.UpdateReason = 1;
                spkt2.UpdateType = 4;
                spkt2.Container = cpkt.Container;
                spkt2.SessionId = this.character.id;
                spkt2.Index = cpkt.Index;
                this.Send((byte[])spkt2);
            }
        }

        /// <summary>
        /// This function occurs when you want to sell a item. The sell price of the item is
        /// one-fourth of the actual market value.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_NPCINTERACTION_SHOPSELL(CMSG_NPCSHOPSELL cpkt)
        {
            BaseNPC npc = this.character.Target as BaseNPC;
            BaseShopCollection list = this.character.Tag as BaseShopCollection;
            if (npc != null && list != null)
            {
                #region OBTAIN ITEM FROM INVENTORY

                //OBTAIN THE REQUIRED ITEM FROM THE MERCHANT
                Rag2Item item = this.character.container[cpkt.Index];
                if (item == null || cpkt.Amount > item.count) return;

                #endregion OBTAIN ITEM FROM INVENTORY

                #region MERCHANDISE - CHECK MERCHANTS MONEY

                //CHECK IF THE MERCHANT HAS ENOUGH MONEY

                double durabillity_scalar = (item.info.max_durability > 0) ? item.durabillty / item.info.max_durability : 1;
                uint req_money = (uint)((double)((item.info.price / 4) * cpkt.Amount) * durabillity_scalar);
                if (npc.Zeny < req_money)
                {
                    Common.Errors.GeneralErrorMessage(this.character, 3);
                    return;
                }

                #endregion MERCHANDISE - CHECK MERCHANTS MONEY

                #region MERCHANDISE - CHECK IF ITEM CAN BE SOLD

                //CHECKS IF THE ITEM IS TRADEABLE
                if (item.info.trade == 0)
                {
                    Common.Errors.GeneralErrorMessage(this.character, 4);
                    return;
                }

                #endregion MERCHANDISE - CHECK IF ITEM CAN BE SOLD

                #region MERCHANDISE - UPDATE ZENY

                this.character.ZENY += req_money;
                npc.Zeny -= req_money;

                CommonFunctions.UpdateZeny(this.character);
                CommonFunctions.UpdateShopZeny(this.character);

                #endregion MERCHANDISE - UPDATE ZENY

                #region INVENTORY - ITEM

                int newCount = item.count - cpkt.Amount;
                if (newCount == 0)
                {
                    //SEND INVENTORY - ITEM DELETE REASON WITH REASON: Sold
                    this.character.container.RemoveAt(cpkt.Index);
                    SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                    spkt.Container = 2;
                    spkt.Index = cpkt.Index;
                    spkt.UpdateReason = 3;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
                else
                {
                    //SEND INVENTORY - ITEM UPDATE REASON WITH REASON: Sold
                    this.character.container[cpkt.Index].count = newCount;
                    SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                    spkt.Amount = (byte)newCount;
                    spkt.UpdateReason = 3;
                    spkt.UpdateType = 4;
                    spkt.Container = 2;
                    spkt.SessionId = this.character.id;
                    spkt.Index = cpkt.Index;
                    this.Send((byte[])spkt);
                }

                #endregion INVENTORY - ITEM

                #region MERCHANDISE - STACK ON REBUY LIST

                //STRUCTURIZE NEW STACKED RAG2 ITEM
                Rag2Item newItem = item.Clone(cpkt.Amount);
                this.character.REBUY.Add(newItem);

                //POPS THE FIRST ITEM OFF THE LIST
                if (this.character.REBUY.Count > 16) this.character.REBUY.RemoveAt(0);
                CommonFunctions.SendRebuylist(this.character);

                #endregion MERCHANDISE - STACK ON REBUY LIST

                #region PLAYER - OPTION

                //Type is used to calc type of item
                //(21 seems to be used for Applogy Item)
                if (newItem.info.type == 21)
                {
                    Common.Skills.DeleteAddition(this.character, newItem.info.option_id);
                }

                #endregion PLAYER - OPTION
            }
        }

        /// <summary>
        /// This function occurs when you want to buy a item.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_NPCINTERACTION_SHOPBUY(CMSG_NPCSHOPBUY cpkt)
        {
            BaseNPC npc = this.character.Target as BaseNPC;
            BaseShopCollection list = this.character.Tag as BaseShopCollection;
            if (npc != null && list != null)
            {
                //OBTAIN THE REQUIRED ITEMS FROM THE MERCHANT
                BaseShopCollection.ShopPair pair = list.list[cpkt.Index];
                if (pair == null) return;

                Rag2Item item = pair.item;
                if (item == null) return;

                //REQUIRED DEPENDECIES
                int req_slots = 1;
                uint req_money = item.info.price * cpkt.Amount;

                //CHECK IF PLAYER HAS ENOUGH MONEY
                if (this.character.ZENY < req_money)
                {
                    Common.Errors.GeneralErrorMessage(this.character, 2);
                    return;
                }
                //CHECK MERCHANTS MONEY
                else if (this.character.container.Count + req_slots > this.character.container.Capacity)
                {
                    Common.Errors.GeneralErrorMessage(this.character, 1);
                    return;
                }
                else
                {
                    //UPDATE ZENY
                    this.character.ZENY -= req_money;
                    npc.Zeny += req_money;

                    CommonFunctions.UpdateZeny(this.character);
                    CommonFunctions.UpdateShopZeny(this.character);

                    //AMOUNT USED IN DECREMENT CALCULATIONS
                    int amount = (int)cpkt.Amount;
                    int index = this.character.container.FindFirstFreeIndex();

                    if (index > -1)
                    {
                        Rag2Item invItem = item.Clone(amount);
                        this.character.container[index] = invItem;
                        SMSG_ADDITEM spkt = new SMSG_ADDITEM();
                        spkt.Container = 2;
                        spkt.UpdateReason = 2;
                        spkt.SetItem(invItem, index);
                        spkt.SessionId = this.character.id;
                        this.Send((byte[])spkt);

                        //Type is used to calc type of item
                        //(21 seems to be used for Applogy Item)
                        if (invItem.info.type == 21)
                        {
                            Common.Skills.UpdateAddition(this.character, invItem.info.option_id);
                        }
                    }

                    //If the item has a limited stock
                    if (pair.NoStock == false)
                    {
                        item.count -= (int)cpkt.Amount;
                        list.Open(this.character, npc);
                    }
                }
            }
        }

        /// <summary>
        /// This function occurs when you want to rebuy a item.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_NPCINTERACTION_SHOPREBUY(CMSG_NPCREBUY cpkt)
        {
            BaseNPC npc = this.character.Target as BaseNPC;
            BaseShopCollection list = this.character.Tag as BaseShopCollection;
            if (npc != null && list != null)
            {
                #region OBTAIN ITEM FROM REBUYLIST

                //OBTAIN THE REQUIRED ITEMS FROM THE MERCHANT
                int Index = cpkt.Index - 1;

                if (Index >= this.character.REBUY.Count) return;
                Rag2Item item = this.character.REBUY[Index];
                if (item == null) return;
                if (cpkt.Amount > item.count) return;

                #endregion OBTAIN ITEM FROM REBUYLIST

                #region MERCHANDISE - CHECK PLAYER'S MONEY

                double durabillity_scalar = (item.info.max_durability > 0) ? item.durabillty / item.info.max_durability : 1;
                uint req_zeny = (uint)((double)((item.info.price / 4) * cpkt.Amount) * durabillity_scalar);

                if (this.character.ZENY < req_zeny)
                {
                    Common.Errors.GeneralErrorMessage(this.character, 2);
                    return;
                }

                #endregion MERCHANDISE - CHECK PLAYER'S MONEY

                #region MERCHANDISE - CHECK INVENTORY

                //TEMP HELPER VARIABLES
                int nstacked = 0;
                List<int> update_queue = new List<int>();

                //WALKTHROUGH EVERY ITEM AND CHECK IF IT CAN BE STACKED
                foreach (int index in this.character.container.FindAllItems(item.info.item))
                {
                    Rag2Item invItem = this.character.container[index];
                    nstacked += (item.info.max_stack - invItem.count);
                    if (invItem.count < item.info.max_stack) update_queue.Add(index);
                }

                //CALCULATE THE AMOUNT OF NEW SLOTS REQUIRED
                int req_hslot = (int)cpkt.Amount % (int)this.character.container.Capacity;
                int div_rem = (int)((cpkt.Amount - nstacked) / item.info.max_stack);
                int div_rem2 = (req_hslot > 0) ? 1 : 0;
                int req_slots = div_rem + div_rem2;

                if (this.character.container.Count + req_slots > this.character.container.Capacity)
                {
                    Common.Errors.GeneralErrorMessage(this.character, 1);
                    return;
                }

                #endregion MERCHANDISE - CHECK INVENTORY

                #region MERCHANDISE - UPDATE ZENY

                this.character.ZENY -= req_zeny;
                npc.Zeny += req_zeny;

                CommonFunctions.UpdateZeny(this.character);
                CommonFunctions.UpdateShopZeny(this.character);

                #endregion MERCHANDISE - UPDATE ZENY

                #region PLAYER - INVENTORY ITEMS

                //AMOUNT USED IN DECREMENT CALCULATIONS
                int amount = (int)cpkt.Amount;

                //ITERATE THROUGH ALL AVAILABLE ITEM THAT CAN BE PROCESSED FOR UPDATES
                foreach (int invIndex in update_queue)
                {
                    Rag2Item invItem = this.character.container[invIndex];
                    int leftover = item.info.max_stack - invItem.count;
                    invItem.count += Math.Max(0, leftover);
                    amount -= leftover;

                    SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                    spkt.Index = (byte)invIndex;
                    spkt.UpdateReason = 2;
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
                    spkt.UpdateReason = 2;
                    spkt.SetItem(invItem, invIndex);
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);

                    //Type is used to calc type of item
                    //(21 seems to be used for Applogy Item)
                    if (invItem.info.type == 21)
                    {
                        Common.Skills.UpdateAddition(this.character, invItem.info.option_id);
                    }
                }

                #endregion PLAYER - INVENTORY ITEMS

                #region MERCHANDISE - REBUYLIST

                if (amount == 0)
                {
                    this.character.REBUY.RemoveAt(Index);
                    CommonFunctions.SendRebuylist(this.character);
                }
                else
                {
                    item.count = amount;
                    CommonFunctions.SendRebuylist(this.character);
                }

                #endregion MERCHANDISE - REBUYLIST
            }
        }

        /// <summary>
        /// Warps a player to the given warp id.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_NPCINTERACTION_WARPSELECT(CMSG_WARP cpkt)
        {
            Saga.Factory.Warps.Info info;
            if (Singleton.Warps.TryFind(cpkt.WarpId, out info) && info.map > 0)
            {
                //WARP
                if (info.price > this.character.ZENY)
                {
                    Common.Errors.GeneralErrorMessage(this.character,
                        (uint)Generalerror.NotEnoughMoneyWithDialog);
                }
                //NOT ENOUGH MONEY
                else
                {
                    //FORCE TO LEAVE CONVERSATION
                    SMSG_LEAVENPC spkt = new SMSG_LEAVENPC();
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);

                    //UPDATE ZENY
                    this.character.ZENY -= info.price;
                    CommonFunctions.UpdateZeny(this.character);

                    //WARP
                    CommonFunctions.Warp(this.character, info.map,
                            new Point(info.x, info.y, info.z));
                }
            }
            else
            {
                Trace.TraceWarning("Warp id {0} is not found", cpkt.WarpId);
            }
        }

        /// <summary>
        /// Requests for the item drop list.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_REQUESTITEMDROPLIST(CMSG_DROPLIST cpkt)
        {
            ILootable target;
            LootCollection collection;

            //Corse has disappeared
            if (!Regiontree.TryFind<ILootable>(cpkt.ActorId, this.character, out target))
            {
                SMSG_NPCDROPLISTRESULT spkt = new SMSG_NPCDROPLISTRESULT();
                spkt.Result = 0;
                spkt.ActorID = cpkt.ActorId;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);

                #region Junk

                /*
                ILootable target = character.Target as ILootable;
                bool LootLeader = false;

                //Allow droplist if i'm the looter
                if (target.LootLeader == this.character.id)
                {
                    CommonFunctions.SendInventoryList
                    (
                        this.character,
                        this.character.Target
                    );
                }
                //Allow the party settings is not set to a lootleader
                else if (LootLeader == false && this.character.sessionParty != null &&
                    this.character.sessionParty.IsMemberOfParty(target.LootLeader))
                {
                    CommonFunctions.SendInventoryList
                    (
                        this.character,
                        this.character.Target
                    );
                }
                //Don't allow
                else
                {
                    SMSG_NPCDROPLISTRESULT spkt = new SMSG_NPCDROPLISTRESULT();
                    spkt.Result = 1;
                    spkt.ActorID = this.character.Target.id;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }*/
                //target.

                #endregion Junk
            }
            //No right to loot the corpse
            else if (!target.GetLootCollection(this.character, out collection))
            {
                SMSG_NPCDROPLISTRESULT spkt = new SMSG_NPCDROPLISTRESULT();
                spkt.Result = 1;
                spkt.ActorID = cpkt.ActorId;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
            //Everything okay
            else
            {
                collection.RequestLootLock();
                collection.Open(this.character, target as MapObject);
            }
        }

        /// <summary>
        /// Uses a map item to unhide a hidden region.
        /// </summary>
        /// <remarks>
        /// As a programmer would know a byte had eight bits.
        /// Each bit represents a region that can be unhidden.
        /// These bit's are invered however, so 128 means first
        /// region is unhidden. While 255 would mean all eight
        /// regions are unhiden.
        ///
        ///  128 = 7 = Region 1
        ///   64 = 6 = Region 2
        ///   32 = 5 = Region 3
        ///   16 = 4 = Region 4
        ///    8 = 3 = Region 5
        ///    4 = 2 = Region 6
        ///    2 = 1 = Region 7
        ///    1 = 0 = Region 8
        ///
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_USEMAPITEM(CMSG_USEMAP cpkt)
        {
            Rag2Item item = character.container[cpkt.Index];
            byte result = 0;
            byte map = 0;
            byte zone = 0;
            int value = 0;

            try
            {
                if (item != null)
                {
                    map = (byte)(item.info.skill / 10);
                    zone = (byte)(item.info.skill % 10);
                    value = (int)Math.Pow(2, (8 - zone));

                    //ALREADY LEARNED
                    if ((this.character.ZoneInformation[map] & value) == value)
                    {
                        result = 1;
                    }
                    //EVERYTHING IS OKAY
                    else
                    {
                        //UPDATE ITEM COUNT
                        int newLength = item.count - 1;
                        if (newLength > 0)
                        {
                            item.count = newLength;
                            SMSG_UPDATEITEM spkt2 = new SMSG_UPDATEITEM();
                            spkt2.Amount = (byte)newLength;
                            spkt2.UpdateReason = 0;
                            spkt2.UpdateType = 4;
                            spkt2.Container = 2;
                            spkt2.SessionId = this.character.id;
                            spkt2.Index = cpkt.Index;
                            this.Send((byte[])spkt2);
                        }
                        else
                        {
                            this.character.container.RemoveAt(cpkt.Index);
                            SMSG_DELETEITEM spkt3 = new SMSG_DELETEITEM();
                            spkt3.UpdateReason = 0;
                            spkt3.Container = 2;
                            spkt3.Index = cpkt.Index;
                            spkt3.SessionId = this.character.id;
                            this.Send((byte[])spkt3);
                        }

                        //UPDATE MAP STATE
                        this.character.ZoneInformation[map] |= (byte)value;
                    }
                }
            }
            catch (Exception e)
            {
                result = 1;
                Console.WriteLine(e);
            }
            finally
            {
                SMSG_SHOWMAP spkt = new SMSG_SHOWMAP();
                spkt.Reason = result;
                spkt.Map = map;
                spkt.Zone = zone;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// This function is sent when you use an dying item
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_USEDYEITEM(CMSG_USEDYEITEM cpkt)
        {
            Rag2Item dyeitem = null;
            Rag2Item item = null;
            byte result = 0;

            try
            {
                //Get the dyeitem
                dyeitem = this.character.container[cpkt.Index];

                //Get the equipment item
                switch (cpkt.Container)
                {
                    case 1: item = this.character.Equipment[cpkt.Slot]; break;
                    case 2: item = this.character.container[cpkt.Slot]; break;
                    case 3: item = this.character.STORAGE[cpkt.Slot]; break;
                }

                //Check if equipment is found
                if (item == null)
                {
                    result = (byte)Generalerror.InventoryItemNotFound;
                }
                //Check if the dye is found
                else if (dyeitem == null)
                {
                    result = (byte)Generalerror.InventoryItemNotFound;
                }
                //Everything is okay
                else
                {
                    //Set the dye color
                    switch (dyeitem.info.skill)
                    {
                        case 0: item.dyecolor = 0; break; //Nothing
                        case 1: item.dyecolor = (byte)Saga.Utils.Generator.Random(2, 2); break;   //red
                        case 2: item.dyecolor = (byte)Saga.Utils.Generator.Random(10, 10); break;   //Yellow
                        case 3: item.dyecolor = (byte)Saga.Utils.Generator.Random(17, 17); break;   //Green
                        case 4: item.dyecolor = (byte)Saga.Utils.Generator.Random(21, 21); break;   //Blue
                        case 5: item.dyecolor = (byte)Saga.Utils.Generator.Random(34, 34); break;  //Purple
                    }

                    //The appearance has changed
                    bool IsEquipment = cpkt.Container == 1;

                    Regiontree tree = this.character.currentzone.Regiontree;
                    foreach (Character target in tree.SearchActors(this.character, SearchFlags.Characters))
                    {
                        if (!Point.IsInSightRangeByRadius(this.character.Position, target.Position)) continue;
                        if (target.id == this.character.id)
                        {
                            //Adjust the item
                            SMSG_ITEMADJUST spkt2 = new SMSG_ITEMADJUST();
                            spkt2.Container = cpkt.Container;
                            spkt2.Slot = cpkt.Slot;
                            spkt2.Value = item.dyecolor;
                            spkt2.Function = 6;
                            spkt2.SessionId = this.character.id;
                            this.Send((byte[])spkt2);
                        }
                        else if (IsEquipment)
                        {
                            SMSG_CHANGEEQUIPMENT spkt = new SMSG_CHANGEEQUIPMENT();
                            spkt.ActorID = this.character.id;
                            spkt.ItemID = (item != null) ? item.info.item : 0;
                            spkt.Dye = (item != null) ? (byte)item.dyecolor : (byte)0;
                            spkt.Slot = cpkt.Slot;
                            spkt.SessionId = target.id;
                            target.client.Send((byte[])spkt);
                        }
                    }

                    //Remove item
                    if (dyeitem.count - 1 > 0)
                    {
                        dyeitem.count -= 1;
                        SMSG_ITEMADJUST spkt3 = new SMSG_ITEMADJUST();
                        spkt3.Container = 2;
                        spkt3.Slot = cpkt.Index;
                        spkt3.Value = (byte)dyeitem.count;
                        spkt3.Function = 4;
                        spkt3.SessionId = this.character.id;
                        this.Send((byte[])spkt3);
                    }
                    else
                    {
                        this.character.container.RemoveAt(cpkt.Index);
                        SMSG_DELETEITEM spkt3 = new SMSG_DELETEITEM();
                        spkt3.UpdateReason = 0;
                        spkt3.Index = cpkt.Index;
                        spkt3.SessionId = this.character.id;
                        spkt3.Container = 2;
                        this.Send((byte[])spkt3);
                    }
                }
            }
            finally
            {
                SMSG_USEDYEITEM spkt = new SMSG_USEDYEITEM();
                if (dyeitem != null)
                    spkt.ItemId = dyeitem.info.item;
                spkt.Equipment = cpkt.Slot;
                spkt.Container = cpkt.Container;
                spkt.Result = result;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Function is called when a user tries to unlock a new
        /// weapon slot.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_USEADMISSIONWEAPON(CMSG_USEWEAPONADMISSION cpkt)
        {
            byte error = 0;
            try
            {
                Rag2Item item = this.character.container[cpkt.Index];
                if (item == null)
                {
                    error = (byte)Generalerror.InventoryItemNotFound;
                }
                else if (this.character.weapons.UnlockedWeaponSlots >= 5)
                {
                    error = (byte)Generalerror.AllWeaponSlotsOpen;
                }
                else
                {
                    this.character.weapons.UnlockedWeaponSlots++;
                }

                if (item.count - 1 > 0)
                {
                    item.count -= 1;
                    SMSG_ITEMADJUST spkt3 = new SMSG_ITEMADJUST();
                    spkt3.Container = 2;
                    spkt3.Slot = cpkt.Index;
                    spkt3.Value = (byte)item.count;
                    spkt3.Function = 4;
                    spkt3.SessionId = this.character.id;
                    this.Send((byte[])spkt3);
                }
                else
                {
                    this.character.container.RemoveAt(cpkt.Index);
                    SMSG_DELETEITEM spkt3 = new SMSG_DELETEITEM();
                    spkt3.UpdateReason = 0;
                    spkt3.Index = cpkt.Index;
                    spkt3.SessionId = this.character.id;
                    spkt3.Container = 2;
                    this.Send((byte[])spkt3);
                }
            }
            finally
            {
                SMSG_WEAPONMAX spkt = new SMSG_WEAPONMAX();
                spkt.Result = error;
                spkt.Count = this.character.weapons.UnlockedWeaponSlots;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when a player tries to upgrade her weapon
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_WEAPONARY_UPGRADE(CMSG_WEAPONUPGRADE cpkt)
        {
            //Helper variables
            Character target = this.character;
            Saga.Factory.Weaponary.Info info;
            int slot = cpkt.SlotId - 1;
            byte result = 0;

            try
            {
                //CHECK FOR VALID SLOT
                if (slot < 5 && target.weapons[slot] != null)
                {
                    Weapon weapon = target.weapons[slot];
                    byte newLevel = (byte)(weapon._weaponlevel + 1);
                    uint ReqZeny = Singleton.experience.FindUpgradeCosts(weapon._weaponlevel);

                    //CHECK IF WE HAVEN'T REACHED OUR LEVEL LIMIT
                    if (weapon._weaponlevel == Singleton.experience.MaxWLVL)
                    {
                        result = (byte)Generalerror.LevelLimitReached;
                    }
                    //NOT ENOUGH MONEY TO UPGRADE
                    else if (ReqZeny > target.ZENY)
                    {
                        result = (byte)Generalerror.NotEnoughMoneyWithDialog;
                    }
                    //ONLY PROCESS IF WEAPON WAS FOUND
                    else if (Singleton.Weapons.TryGetWeaponInfo(weapon._weapontype, newLevel, out info))
                    {
                        BattleStatus status = this.character._status;
                        status.MaxWMAttack -= (ushort)weapon.Info.max_magic_attack;
                        status.MinWMAttack -= (ushort)weapon.Info.min_magic_attack;
                        status.MaxWPAttack -= (ushort)weapon.Info.max_short_attack;
                        status.MinWPAttack -= (ushort)weapon.Info.min_short_attack;
                        status.MaxWRAttack -= (ushort)weapon.Info.max_range_attack;
                        status.MinWRAttack -= (ushort)weapon.Info.min_range_attack;
                        status.MaxWMAttack += (ushort)info.max_magic_attack;
                        status.MinWMAttack += (ushort)info.min_magic_attack;
                        status.MaxWPAttack += (ushort)info.max_short_attack;
                        status.MinWPAttack += (ushort)info.min_short_attack;
                        status.MaxWRAttack += (ushort)info.max_range_attack;
                        status.MinWRAttack += (ushort)info.min_range_attack;
                        status.Updates |= 2;

                        weapon._weaponlevel += 1;
                        weapon.Info = info;

                        SMSG_WEAPONADJUST spkt2 = new SMSG_WEAPONADJUST();
                        spkt2.Function = 1;
                        spkt2.Value = newLevel;
                        spkt2.Slot = (byte)slot;
                        spkt2.SessionId = target.id;
                        this.Send((byte[])spkt2);

                        Tasks.LifeCycle.Update(this.character);

                        target.ZENY -= ReqZeny;
                        CommonFunctions.UpdateZeny(target);
                    }
                }
                //NOT VALID SLOT
                else
                {
                    result = (byte)Generalerror.WeaponNotExists;
                }

                //ADJUST WEAPONARY
                SMSG_WEAPONUPGRADE spkt = new SMSG_WEAPONUPGRADE();
                spkt.Result = result;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
            //ON ERROR SENT FAILURE MESSAGE
            catch (Exception)
            {
                SMSG_WEAPONUPGRADE spkt = new SMSG_WEAPONUPGRADE();
                spkt.Result = result;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when a player tries to change here suffix name.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_WEAPONARY_CHANGESUFFIX(CMSG_WEAPONCHANGESUFFIX cpkt)
        {
            //Helper variables
            Character target = this.character;
            byte slot = cpkt.SlotId;
            byte result = 0;
            int ReqZeny = 16;

            try
            {
                //NOT VALID SLOT
                if (slot >= 5 || target.weapons[slot] == null)
                {
                    result = (byte)Generalerror.WeaponNotExists;
                }
                //NOT ENOUGH MONEY TO RENAME WEAPON
                else if (ReqZeny > target.ZENY)
                {
                    result = (byte)Generalerror.NotEnoughMoneyToRenameWeapon;
                }
                //EVERYTHING IS OKAY
                else
                {
                    Weapon weapon = target.weapons[slot];
                    weapon._suffix = cpkt.Suffix;
                }

                //OUTPUT PACKET
                SMSG_WEAPONCHANGESUFFIX spkt = new SMSG_WEAPONCHANGESUFFIX();
                spkt.Result = result;
                spkt.SessionId = this.character.id;
                spkt.Suffix = cpkt.Suffix;
                this.Send((byte[])spkt);
            }
            //ON ERROR SENT FAILURE MESSAGE
            catch (Exception)
            {
                SMSG_WEAPONCHANGESUFFIX spkt = new SMSG_WEAPONCHANGESUFFIX();
                spkt.Result = (byte)Generalerror.WeaponNotExists;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when a player tries to change here suffix name.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_WEAPONARY_NEWCHANGESUFFIX(CMSG_WEAPONCHANGESUFFIX2 cpkt)
        {
            SMSG_WEAPONNEWCHANGESUFFIX spkt = new SMSG_WEAPONNEWCHANGESUFFIX();
            spkt.Result = 1;
            spkt.SessionId = this.character.id;

            //Helper variables
            Character target = this.character;
            byte slot = cpkt.SlotId;
            byte result = 0;

            try
            {
                //NOT VALID SLOT
                if (slot >= 5 || target.weapons[slot] == null)
                {
                    result = (byte)Generalerror.WeaponNotExists;
                }
                else if (target.weapons[slot]._weaponname.Length > 0)
                {
                    result = (byte)Generalerror.WeaponNotNameless;
                }
                //EVERYTHING IS OKAY
                else
                {
                    Weapon weapon = target.weapons[slot];
                    weapon._suffix = cpkt.Suffix;
                    result = 0;
                }
            }
            //ON ERROR SENT FAILURE MESSAGE
            finally
            {
                spkt.Result = result;
                spkt.SlotId = slot;
                spkt.Suffix = cpkt.Suffix;
                spkt.WeaponName = cpkt.WeaponName;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Occurs when sorting the inventory
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_IVENTORY_SORT(CMSG_SORTINVENTORYLIST cpkt)
        {
            if (cpkt.SortType > 1) return;
            for (int i = 0; i < this.character.container.Capacity; i++)
            {
                Rag2Item a = character.container[i];
                if (a == null) continue;

                Predicate<KeyValuePair<byte, Rag2Item>> FindRag2Items = delegate(KeyValuePair<byte, Rag2Item> b)
                {
                    return b.Value.info == a.info &&
                           b.Value.clvl == a.clvl;
                };

                bool result = false;
                int count = a.info.max_stack - a.count;

                List<KeyValuePair<byte, Rag2Item>> items = new List<KeyValuePair<byte, Rag2Item>>();
                foreach (KeyValuePair<byte, Rag2Item> b in this.character.container.GetAllItems(FindRag2Items))
                {
                    if (b.Value.count < b.Value.info.max_stack && b.Key != i)
                        items.Add(b);
                }

                foreach (KeyValuePair<byte, Rag2Item> b in items)
                {
                    if (b.Value.count > count)
                    {
                        a.count = a.info.max_stack;
                        b.Value.count -= count;
                        count = 0;

                        SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                        spkt.Container = 2;
                        spkt.UpdateType = 4;
                        spkt.SessionId = this.character.id;
                        spkt.Index = b.Key;
                        spkt.Amount = (byte)b.Value.count;
                        spkt.UpdateReason = 0;
                        this.Send((byte[])spkt);
                        result = true;
                        break;
                    }
                    else
                    {
                        a.count += b.Value.count;
                        count -= b.Value.count;
                        this.character.container.RemoveAt(b.Key);

                        SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                        spkt.Index = b.Key;
                        spkt.UpdateReason = 0;
                        spkt.SessionId = this.character.id;
                        spkt.Container = 2;
                        result = true;
                        this.Send((byte[])spkt);
                    }
                }

                if (result == true)
                {
                    SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                    spkt.Container = 2;
                    spkt.UpdateType = 4;
                    spkt.SessionId = this.character.id;
                    spkt.Index = (byte)i;
                    spkt.Amount = (byte)a.count;
                    spkt.UpdateReason = 0;
                    this.Send((byte[])spkt);
                }
            }
        }

        /// <summary>
        /// Occurs when sorting the inventory
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_STORAGE_SORT(CMSG_SORTSTORAGELIST cpkt)
        {
            for (int i = 0; i < this.character.container.Capacity; i++)
            {
                Rag2Item a = character.STORAGE[i];
                if (a == null) continue;

                Predicate<KeyValuePair<byte, Rag2Item>> FindRag2Items = delegate(KeyValuePair<byte, Rag2Item> b)
                {
                    return b.Value.info == a.info &&
                           b.Value.tradeable == a.tradeable &&
                           b.Value.clvl == a.clvl;
                };

                bool result = false;
                int count = a.info.max_stack - a.count;

                List<KeyValuePair<byte, Rag2Item>> items = new List<KeyValuePair<byte, Rag2Item>>();
                foreach (KeyValuePair<byte, Rag2Item> b in this.character.STORAGE.GetAllItems(FindRag2Items))
                {
                    if (b.Value.count < b.Value.info.max_stack && b.Key != i)
                        items.Add(b);
                }

                foreach (KeyValuePair<byte, Rag2Item> b in items)
                {
                    if (b.Value.count > count)
                    {
                        a.count = a.info.max_stack;
                        b.Value.count -= count;
                        count = 0;

                        SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                        spkt.Container = 3;
                        spkt.UpdateType = 4;
                        spkt.SessionId = this.character.id;
                        spkt.Index = b.Key;
                        spkt.Amount = (byte)b.Value.count;
                        spkt.UpdateReason = 0;
                        this.Send((byte[])spkt);
                        result = true;
                        break;
                    }
                    else
                    {
                        a.count += b.Value.count;
                        count -= b.Value.count;
                        this.character.STORAGE.RemoveAt(b.Key);

                        SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                        spkt.Index = b.Key;
                        spkt.UpdateReason = 0;
                        spkt.SessionId = this.character.id;
                        spkt.Container = 3;
                        result = true;
                        this.Send((byte[])spkt);
                    }
                }

                if (result == true)
                {
                    SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                    spkt.Container = 3;
                    spkt.UpdateType = 4;
                    spkt.SessionId = this.character.id;
                    spkt.Index = (byte)i;
                    spkt.Amount = (byte)a.count;
                    spkt.UpdateReason = 0;
                    this.Send((byte[])spkt);
                }
            }
        }

        /// <summary>
        /// This occurs after adding a auge skill to the weaponary
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_WEAPONAUGE(CMSG_WEAPONAUGE cpkt)
        {
            Rag2Item item = this.character.container[cpkt.Index];
            Weapon wep = this.character.weapons[cpkt.WeaponSlot];
            if (item == null) return;

            wep._augeskill = item.info.skill;
            Point oldPos = this.character.Position;
            Regiontree tree = this.character.currentzone.Regiontree;
            bool IsActiveItem = cpkt.WeaponSlot == (byte)((character.weapons.ActiveWeaponIndex == 0) ? character.weapons.PrimaryWeaponIndex : character.weapons.SeconairyWeaponIndex);

            //Update item count
            int newLength = item.count - 1;
            if (newLength > 0)
            {
                item.count = newLength;
                SMSG_UPDATEITEM spkt2 = new SMSG_UPDATEITEM();
                spkt2.Amount = (byte)newLength;
                spkt2.UpdateReason = 0;
                spkt2.UpdateType = 4;
                spkt2.Container = 2;
                spkt2.SessionId = this.character.id;
                spkt2.Index = cpkt.Index;
                this.Send((byte[])spkt2);
            }
            else
            {
                this.character.container.RemoveAt(cpkt.Index);
                SMSG_DELETEITEM spkt3 = new SMSG_DELETEITEM();
                spkt3.UpdateReason = 0;
                spkt3.Container = 2;
                spkt3.Index = cpkt.Index;
                spkt3.SessionId = this.character.id;
                this.Send((byte[])spkt3);
            }

            //Update all objects
            foreach (Character regionObject in tree.SearchActors(SearchFlags.Characters))
            {
                if (regionObject.id == this.character.id)
                {
                    SMSG_ENCHANTMENT spkt = new SMSG_ENCHANTMENT();
                    spkt.Unknown1 = 1;
                    spkt.Unknown2 = cpkt.WeaponSlot;
                    spkt.Weaponslot = cpkt.Slot;
                    spkt.SkillId = wep._augeskill;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
                else if (IsActiveItem && Point.IsInSightRangeByRadius(regionObject.Position, oldPos))
                {
                    SMSG_SHOWWEAPON spkt = new SMSG_SHOWWEAPON();
                    spkt.ActorID = this.character.id;
                    spkt.AugeID = this.character.ComputeAugeSkill();
                    spkt.SessionId = regionObject.id;
                    regionObject.client.Send((byte[])spkt);
                }
            }
        }

        /// <summary>
        /// Occurs when repairing equipment
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_REPAIREQUIPMENT(CMSG_REPAIRITEM cpkt)
        {
            bool HasBadIndex = false;
            double price = 0;
            int amount = cpkt.Amount;

            List<KeyValuePair<byte, byte>> list = new List<KeyValuePair<byte, byte>>();
            for (int i = 0; i < amount; i++)
            {
                //Read the equipment information
                byte index;
                byte container;
                cpkt.ReadEquipmentInfo(out index, out container);
                list.Add(new KeyValuePair<byte, byte>(container, index));

                // Item from weaponary
                if (container == 8)
                {
                    //CHECK IF WEAPON EXISTS
                    Weapon current = this.character.weapons[index];
                    if (current == null) { HasBadIndex = true; continue; }

                    //Compute the repair costs
                    float scalar = (float)(current.Info.max_durabillity - current._durabillity) / (float)current.Info.max_durabillity;
                    double costs = (double)(Singleton.experience.FindRepairCosts(current._weaponlevel) * scalar);

                    //Increment the repair costs
                    price += costs;
                }
                //Increment the repair costs
                else if (container == 1)
                {
                    //Check if equipment exists
                    Rag2Item current = this.character.Equipment[index];
                    if (current == null) { HasBadIndex = true; continue; }

                    //Compute the repair costs
                    double scalar = (double)(current.info.max_durability - current.durabillty) / (double)current.info.max_durability;
                    double costs = (double)
                            Rag2Item.CalculateRepairCosts(
                                 (double)current.info.price,
                                 (double)current.clvl,
                                 (double)current.info.max_durability
                            ) * scalar;

                    //Increment the repair costs
                    price += costs;
                }
                //Item from inventory
                else if (container == 2)
                {
                    //Check if equipment exists
                    Rag2Item current = this.character.container[index];
                    if (current == null) { HasBadIndex = true; continue; }

                    //Compute the repair costs
                    double scalar = (double)(current.info.max_durability - current.durabillty) / (double)current.info.max_durability;
                    double costs = (double)
                           Rag2Item.CalculateRepairCosts(
                                (double)current.info.price,
                                (double)current.clvl,
                                (double)current.info.max_durability
                           ) * scalar;

                    //Increment the repair costs
                    price += costs;
                }
            }

            price = Math.Ceiling(price);

            //Check if a bad index occured
            if (HasBadIndex)
            {
                Common.Errors.GeneralErrorMessage(
                    this.character,
                    (uint)Generalerror.WrongServerIndex
                );
            }
            //Check if our price is right
            uint aprice = (uint)price;
            if (this.character.ZENY >= aprice)
            {
                this.character.ZENY -= aprice;
                foreach (KeyValuePair<byte, byte> pair in list)
                {
                    // Item from weaponary
                    if (pair.Key == 8)
                    {
                        Weapon current = this.character.weapons[pair.Value];
                        if (current._durabillity == 0 &&
                            Common.Skills.HasRootSkillPresent(this.character, current.Info.weapon_skill))
                        {
                            current._active = 1;
                            SMSG_WEAPONADJUST spkt2 = new SMSG_WEAPONADJUST();
                            spkt2.SessionId = this.character.id;
                            spkt2.Function = 6;
                            spkt2.Value = 1;
                            spkt2.Slot = pair.Value;
                            this.Send((byte[])spkt2);
                        }

                        current._durabillity = (ushort)current.Info.max_durabillity;
                        SMSG_WEAPONADJUST spkt = new SMSG_WEAPONADJUST();
                        spkt.SessionId = this.character.id;
                        spkt.Function = 3;
                        spkt.Value = current.Info.max_durabillity;
                        spkt.Slot = pair.Value;
                        this.Send((byte[])spkt);
                    }
                    //Equipment
                    else if (pair.Key == 1)
                    {
                        Rag2Item current = this.character.Equipment[pair.Value];
                        if (current.durabillty == 0 && this.character.jlvl >= current.info.JobRequirement[this.character.job - 1])
                        {
                            current.active = 1;
                            SMSG_ITEMADJUST spkt2 = new SMSG_ITEMADJUST();
                            spkt2.Function = 5;
                            spkt2.Container = pair.Key;
                            spkt2.Slot = pair.Value;
                            spkt2.Value = 1;
                            spkt2.SessionId = this.character.id;
                            this.Send((byte[])spkt2);

#warning "Equipment applied"
                            current.Activate(AdditionContext.Applied, this.character);
                        }

                        current.durabillty = (int)current.info.max_durability;
                        SMSG_ITEMADJUST spkt = new SMSG_ITEMADJUST();
                        spkt.Function = 3;
                        spkt.Container = pair.Key;
                        spkt.Slot = pair.Value;
                        spkt.Value = (uint)current.durabillty;
                        spkt.SessionId = this.character.id;
                        this.Send((byte[])spkt);
                    }
                    //Inventory
                    else if (pair.Key == 2)
                    {
                        Rag2Item current = this.character.container[pair.Value];
                        current.durabillty = (int)current.info.max_durability;

                        SMSG_ITEMADJUST spkt = new SMSG_ITEMADJUST();
                        spkt.Function = 3;
                        spkt.Container = pair.Key;
                        spkt.Slot = pair.Value;
                        spkt.Value = (uint)current.durabillty;
                        spkt.SessionId = this.character.id;
                        this.Send((byte[])spkt);
                    }
                }

                //Flush all pending updates
                LifeCycle.Update(this.character);

                //Update zeny
                CommonFunctions.UpdateZeny(
                    this.character
                );
            }
            else
            {
                Common.Errors.GeneralErrorMessage(
                    this.character,
                    (uint)Generalerror.NotEnoughMoneyToRepair
                );
            }
        }

        /// <summary>
        /// This function process all inventory interaction. For example to equip a
        /// item or a to switch item from your inventory to the storage. Because this
        /// is populair place to exploit we do some heavy loaded item checking.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_MOVEITEM(CMSG_MOVEITEM cpkt)
        {
            if (cpkt.MovementType == 1)
            {
                #region EQUIPMENT TO INVENTORY SWAP

                int result = 0;
                Rag2Item[] Equips = this.character.Equipment;
                Rag2Collection Inventory = this.character.container;

                //PROCESS EQUIPMENT SWAPPING
                int dest = 255;
                if (cpkt.DestinationIndex == 255)
                {
                    Rag2Item temp = Equips[cpkt.SourceIndex];
                    dest = this.character.container.Add(temp);
                    if (dest == -1) { result = 14; goto Notifycation; }
                    Equips[cpkt.SourceIndex] = null;

#warning "Equipment deapplied"
                    temp.Activate(AdditionContext.Deapplied, this.character);
                    Tasks.LifeCycle.Update(this.character);
                }
                else
                {
                    Rag2Item temp2 = this.character.container[cpkt.DestinationIndex];
                    if (temp2 == null) { result = 16; goto Notifycation; }

                    Rag2Item temp = Equips[cpkt.SourceIndex];
                    Equips[cpkt.SourceIndex] = temp2;
                    this.character.container[cpkt.SourceIndex] = temp;

#warning "Equipment applied/deapplied"
                    temp.Activate(AdditionContext.Deapplied, this.character);
                    temp2.Activate(AdditionContext.Reapplied, this.character);
                    Tasks.LifeCycle.Update(this.character);
                }

                //MOVE THE ITEM
                SMSG_MOVEITEM spkt = new SMSG_MOVEITEM();
                spkt.DestinationIndex = (byte)dest;
                spkt.SourceIndex = cpkt.SourceIndex;
                spkt.MovementType = cpkt.MovementType;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);

                Regiontree tree = this.character.currentzone.Regiontree;
                int SourceIndex = cpkt.SourceIndex;
                int ShieldIndex = (this.character.weapons.ActiveWeaponIndex == 1) ? 15 : 14;
                if (SourceIndex < 6 || SourceIndex == 8 || SourceIndex == ShieldIndex)
                    foreach (Character regionObject in tree.SearchActors(SearchFlags.Characters))
                    {
                        //FORWARD CHANGE TO ALL ACTORS
                        Rag2Item equip = Equips[cpkt.SourceIndex];
                        SMSG_CHANGEEQUIPMENT spkt2 = new SMSG_CHANGEEQUIPMENT();
                        spkt2.Slot = cpkt.SourceIndex;
                        spkt2.ActorID = this.character.id;
                        spkt2.ItemID = (equip != null) ? equip.info.item : 0;
                        spkt2.Dye = (byte)((equip != null) ? equip.dyecolor : 0);
                        spkt2.SessionId = regionObject.id;
                        regionObject.client.Send((byte[])spkt2);
                    }

            Notifycation:
                //NOTIFY THE USER OF AN ERROR
                SMSG_MOVEREPLY spkt3 = new SMSG_MOVEREPLY();
                spkt3.MovementType = cpkt.MovementType;
                spkt3.Message = (byte)result;
                spkt3.SessionId = this.character.id;
                this.Send((byte[])spkt3);

                #endregion EQUIPMENT TO INVENTORY SWAP
            }
            else if (cpkt.MovementType == 2)
            {
                #region INVENTORY TO EQUIPMENT SWAP

                //INVENTORY TO EQUIPMENT
                byte result = 0;
                Rag2Item[] Equips = this.character.Equipment;

                //CHECK INVENTORY ITEM
                Rag2Item InventoryItem = this.character.container[cpkt.SourceIndex];
                if (InventoryItem == null)
                {
                    result = 16;
                    goto Notifycation;
                }

                //CHECK LEVEL
                if (this.character._level < InventoryItem.info.req_clvl)
                {
                    result = 1;
                    goto Notifycation;
                }

                //CHECK GENDER
                if ((InventoryItem.info.req_male + InventoryItem.info.req_female < 2) &&
                ((InventoryItem.info.req_male == 1 && this.character.gender == 2) ||
                    (InventoryItem.info.req_female == 1 && this.character.gender == 1)))
                {
                    result = 2;
                    goto Notifycation;
                }

                //CHECK RACE
                if ((this.character.race == 1 && InventoryItem.info.req_norman == 1) ||
                    (this.character.race == 2 && InventoryItem.info.req_ellr == 1) ||
                    (this.character.race == 3 && InventoryItem.info.req_dimago == 1))
                {
                    result = 3;
                    goto Notifycation;
                }

                //CHECK STRENGTH
                if (this.character.stats.CHARACTER.strength < InventoryItem.info.req_str)
                {
                    result = 4;
                    goto Notifycation;
                }

                //CHECK DEXTERITY
                if (this.character.stats.CHARACTER.dexterity < InventoryItem.info.req_dex)
                {
                    result = 5;
                    goto Notifycation;
                }

                //CHECK CONCENCENTRATION
                if (this.character.stats.CHARACTER.concentration < InventoryItem.info.req_con)
                {
                    result = 6;
                    goto Notifycation;
                }

                //CHECK LUCK
                if (this.character.stats.CHARACTER.luck < InventoryItem.info.req_luc)
                {
                    result = 7;
                    goto Notifycation;
                }

                //CHECK JOB

                //UNSEAL THE ITEM
                if (InventoryItem.tradeable == true)
                {
                    InventoryItem.tradeable = false;
                    SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                    spkt.Container = 2;
                    spkt.Index = cpkt.SourceIndex;
                    spkt.UpdateReason = (byte)ItemUpdateReason.NoReason;
                    spkt.UpdateType = 7;
                    spkt.Amount = 1;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }

                //EVERYTHING IS OKAY PROCESS SWAPPING
                {
                    InventoryItem.active = 1;
                    SMSG_MOVEITEM spkt = new SMSG_MOVEITEM();
                    spkt.DestinationIndex = cpkt.DestinationIndex;
                    spkt.SourceIndex = cpkt.SourceIndex;
                    spkt.MovementType = cpkt.MovementType;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }

                //PROCESS EQUIPMENT SWAPPING
                Rag2Item temp = Equips[cpkt.DestinationIndex];
                Equips[cpkt.DestinationIndex] = this.character.container[cpkt.SourceIndex];
                if (temp != null)
                {
                    this.character.container[cpkt.SourceIndex] = temp;
                    temp.Activate(AdditionContext.Deapplied, this.character);
                    InventoryItem.Activate(AdditionContext.Applied, this.character);
                    Tasks.LifeCycle.Update(this.character);
                }
                else
                {
                    this.character.container.RemoveAt(cpkt.SourceIndex);
                    InventoryItem.Activate(AdditionContext.Applied, this.character);
                    Tasks.LifeCycle.Update(this.character);
                }

                Regiontree tree = this.character.currentzone.Regiontree;
                int DestIndex = cpkt.DestinationIndex;
                int ShieldIndex = (this.character.weapons.ActiveWeaponIndex == 1) ? 15 : 14;
                if (DestIndex < 6 || DestIndex == 8 || DestIndex == ShieldIndex)
                    foreach (Character regionObject in tree.SearchActors(SearchFlags.Characters))
                    {
                        //FORWARD CHANGE TO ALL ACTORS
                        Rag2Item equip = Equips[cpkt.DestinationIndex];
                        SMSG_CHANGEEQUIPMENT spkt2 = new SMSG_CHANGEEQUIPMENT();
                        spkt2.Slot = cpkt.SourceIndex;
                        spkt2.ActorID = this.character.id;
                        spkt2.ItemID = (equip != null) ? equip.info.item : 0;
                        spkt2.Dye = (byte)((equip != null) ? equip.dyecolor : 0);
                        regionObject.client.Send((byte[])spkt2);
                    }

            Notifycation:
                //NOTIFY THE USER OF AN ERROR
                SMSG_MOVEREPLY spkt3 = new SMSG_MOVEREPLY();
                spkt3.MovementType = cpkt.MovementType;
                spkt3.Message = result;
                spkt3.SessionId = this.character.id;
                this.Send((byte[])spkt3);

                #endregion INVENTORY TO EQUIPMENT SWAP
            }
            else if (cpkt.MovementType == 3)
            {
                #region INVENTORY TO STORAGE SWAP

                //STORAGE TO INVENTORY
                byte result = 0;

                //CHECK STORAGE ITEM
                Rag2Item storageItem = null;
                Rag2Item invenItem = this.character.container[cpkt.SourceIndex];
                if (invenItem == null) { result = 16; goto Notifycation; }

                //CHECK PROVIDED AMOUNT
                if (cpkt.Amount > invenItem.count) { result = 25; goto Notifycation; }

                //CHECK MAX STACK LIMITS
                if (cpkt.Amount > invenItem.info.max_stack) { result = 24; goto Notifycation; }

                //CHECK DESTINATION
                if (cpkt.DestinationIndex == 255)
                {
                    if (this.character.STORAGE.Count == this.character.STORAGE.Capacity) { result = 17; goto Notifycation; }
                }
                else
                {
                    storageItem = this.character.STORAGE[cpkt.DestinationIndex];
                    if (storageItem == null) { result = 19; goto Notifycation; }
                    if (storageItem.Equals(invenItem)) { result = 23; goto Notifycation; }
                    if (storageItem.count + cpkt.Amount > invenItem.info.max_stack) { result = 24; goto Notifycation; }
                }

                //PROCESS MOVEMENT - PART 1
                if (cpkt.DestinationIndex == 255)
                {
                    storageItem = invenItem.Clone(cpkt.Amount);
                    int index = this.character.STORAGE.Add(storageItem);

                    SMSG_ADDITEM spkt4 = new SMSG_ADDITEM();
                    spkt4.Container = 3;
                    spkt4.SessionId = this.character.id;
                    spkt4.UpdateReason = 0;
                    spkt4.SetItem(invenItem, index);
                    this.Send((byte[])spkt4);
                }
                else
                {
                    storageItem.count += cpkt.Amount;
                    SMSG_UPDATEITEM spkt4 = new SMSG_UPDATEITEM();
                    spkt4.Amount = (byte)storageItem.count;
                    spkt4.Container = 3;
                    spkt4.Index = cpkt.DestinationIndex;
                    spkt4.UpdateReason = 0;
                    spkt4.UpdateType = 2;
                    this.Send((byte[])spkt4);
                }

                //PROCESS MOVEMENT - PART 2
                int nCount = invenItem.count - cpkt.Amount;
                if (nCount > 0)
                {
                    invenItem.count = nCount;
                    SMSG_UPDATEITEM spkt4 = new SMSG_UPDATEITEM();
                    spkt4.Amount = (byte)nCount;
                    spkt4.Container = 2;
                    spkt4.Index = cpkt.SourceIndex;
                    spkt4.UpdateReason = (byte)ItemUpdateReason.StorageSent;
                    spkt4.UpdateType = 2;
                    this.Send((byte[])spkt4);
                }
                else
                {
                    this.character.container.RemoveAt(cpkt.SourceIndex);
                    SMSG_DELETEITEM spkt2 = new SMSG_DELETEITEM();
                    spkt2.Container = 2;
                    spkt2.Index = cpkt.SourceIndex;
                    spkt2.UpdateReason = (byte)ItemUpdateReason.StorageSent;
                    spkt2.SessionId = this.character.id;
                    this.Send((byte[])spkt2);
                }

            Notifycation:
                SMSG_MOVEREPLY spkt3 = new SMSG_MOVEREPLY();
                spkt3.Message = (byte)result;
                spkt3.MovementType = cpkt.MovementType;
                spkt3.SessionId = this.character.id;
                this.Send((byte[])spkt3);

                //Type is used to calc type of item
                //(21 seems to be used for Applogy Item)
                if (result == 0 && invenItem.info.type == 21)
                {
                    Common.Skills.UpdateAddition(this.character, 101);
                }

                #endregion INVENTORY TO STORAGE SWAP
            }
            else if (cpkt.MovementType == 4)
            {
                #region STORAGE TO INVENTORY SWAP

                //CHECK STORAGE ITEM
                int result = 0;
                Rag2Item invenItem = null;
                Rag2Item storageItem = this.character.STORAGE[cpkt.SourceIndex];
                if (storageItem == null) { result = 19; goto Notifycation; }

                //CHECK PROVIDED AMOUNT
                if (cpkt.Amount > storageItem.count) { result = 25; goto Notifycation; }

                //CHECK MAX STACK LIMITS
                if (cpkt.Amount > storageItem.info.max_stack) { result = 24; goto Notifycation; }

                //CHECK DESTINATION
                if (cpkt.DestinationIndex == 255)
                {
                    if (this.character.container.Count == this.character.container.Capacity) { result = 14; goto Notifycation; }
                }
                else
                {
                    invenItem = this.character.container[cpkt.DestinationIndex];
                    if (invenItem == null) { result = 16; goto Notifycation; }
                    if (invenItem.Equals(storageItem)) { result = 23; goto Notifycation; }
                    if (invenItem.count + cpkt.Amount > storageItem.info.max_stack) { result = 24; goto Notifycation; }
                }

                //PROCESS MOVEMENT - PART 1
                if (cpkt.DestinationIndex == 255)
                {
                    invenItem = storageItem.Clone(cpkt.Amount);
                    int index = this.character.container.Add(invenItem);

                    SMSG_ADDITEM spkt4 = new SMSG_ADDITEM();
                    spkt4.Container = 2;
                    spkt4.SessionId = this.character.id;
                    spkt4.UpdateReason = (byte)ItemUpdateReason.StorageReceived;
                    spkt4.SetItem(invenItem, index);
                    this.Send((byte[])spkt4);
                }
                else
                {
                    invenItem.count += cpkt.Amount;
                    SMSG_UPDATEITEM spkt4 = new SMSG_UPDATEITEM();
                    spkt4.Amount = (byte)invenItem.count;
                    spkt4.Container = 2;
                    spkt4.Index = cpkt.DestinationIndex;
                    spkt4.UpdateReason = (byte)ItemUpdateReason.StorageReceived;
                    spkt4.UpdateType = 2;
                    this.Send((byte[])spkt4);
                }

                //PROCESS MOVEMENT - PART 2
                int nCount = storageItem.count - cpkt.Amount;
                if (nCount > 0)
                {
                    storageItem.count = nCount;
                    SMSG_UPDATEITEM spkt4 = new SMSG_UPDATEITEM();
                    spkt4.Amount = (byte)nCount;
                    spkt4.Container = 3;
                    spkt4.Index = cpkt.SourceIndex;
                    spkt4.UpdateReason = 0;
                    spkt4.UpdateType = 2;
                    this.Send((byte[])spkt4);
                }
                else
                {
                    this.character.STORAGE.RemoveAt(cpkt.SourceIndex);
                    SMSG_DELETEITEM spkt2 = new SMSG_DELETEITEM();
                    spkt2.Container = 3;
                    spkt2.Index = cpkt.SourceIndex;
                    spkt2.UpdateReason = 0;
                    spkt2.SessionId = this.character.id;
                    this.Send((byte[])spkt2);
                }

            Notifycation:
                SMSG_MOVEREPLY spkt3 = new SMSG_MOVEREPLY();
                spkt3.MovementType = cpkt.MovementType;
                spkt3.Message = (byte)result;
                spkt3.SessionId = this.character.id;
                this.Send((byte[])spkt3);

                //Type is used to calc type of item
                //(21 seems to be used for Applogy Item)
                if (result == 0 && invenItem.info.type == 21)
                {
                    Common.Skills.UpdateAddition(this.character, 101);
                }

                #endregion STORAGE TO INVENTORY SWAP
            }
        }

        /// <summary>
        /// Uses a supplement stone.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_USESUPLEMENTSTONE(CMSG_USESUPPLEMENTSTONE cpkt)
        {
            //TODO: ADD HANDLER SO SUPPLY STONES TAKE EFFECT

            byte result = 0;
            uint skill = 0;

            try
            {
                Rag2Item Supplement = this.character.container[cpkt.IventoryId];
                Rag2Item Equipment = (cpkt.Container == 1) ? this.character.container[cpkt.ContainerSlot] : this.character.Equipment[cpkt.ContainerSlot];

                bool IsEquipmentItem = cpkt.Container != 1;

                if (Supplement == null || Equipment == null)
                {
                    result = (byte)Generalerror.ItemEnchantmentFailed;
                }
                else
                {
                    skill = Supplement.info.skill;

                    //If it is a equipment item and activated
                    //(active is set to 0 if durabillity is 0 or doesn't meet job criteria)
                    if (IsEquipmentItem && Equipment.active == 1)
                    {
                        uint oldskill = Equipment.Enchantments[cpkt.EnchantmentSlot];
                        if (oldskill > 0)
                        {
#warning "Skill Deapplied"

                            Factory.Spells.Info info1;
                            Factory.Additions.Info info2;
                            if (skill > 0)
                            {
                                if (Singleton.SpellManager.TryGetSpell(oldskill, out info1)
                                && Singleton.Additions.TryGetAddition(info1.addition, out info2))
                                {
                                    info2.Do(character, character, AdditionContext.Deapplied);
                                }
                            }
                        }

                        if (skill > 0)
                        {
#warning "Skill Applied"

                            Factory.Spells.Info info1;
                            Factory.Additions.Info info2;
                            if (skill > 0)
                            {
                                if (Singleton.SpellManager.TryGetSpell(skill, out info1)
                                && Singleton.Additions.TryGetAddition(info1.addition, out info2))
                                {
                                    info2.Do(character, character, AdditionContext.Applied);
                                }
                            }
                        }
                    }

                    Equipment.Enchantments[cpkt.EnchantmentSlot] = skill;

                    int newLength = Supplement.count - 1;
                    if (newLength > 0)
                    {
                        Supplement.count = newLength;
                        SMSG_UPDATEITEM spkt2 = new SMSG_UPDATEITEM();
                        spkt2.Amount = (byte)newLength;
                        spkt2.UpdateReason = 0;
                        spkt2.UpdateType = 4;
                        spkt2.Container = 2;
                        spkt2.SessionId = this.character.id;
                        spkt2.Index = cpkt.IventoryId;
                        this.Send((byte[])spkt2);
                    }
                    else
                    {
                        this.character.container.RemoveAt(cpkt.IventoryId);
                        SMSG_DELETEITEM spkt3 = new SMSG_DELETEITEM();
                        spkt3.UpdateReason = 0;
                        spkt3.Container = 2;
                        spkt3.Index = cpkt.IventoryId;
                        spkt3.SessionId = this.character.id;
                        this.Send((byte[])spkt3);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = (byte)Generalerror.ItemEnchantmentFailed;
            }
            finally
            {
                SMSG_USESUPPLEMENTSTONE spkt = new SMSG_USESUPPLEMENTSTONE();
                spkt.Result = result;
                spkt.InventoryId = cpkt.IventoryId;
                spkt.Container = cpkt.Container;
                spkt.ContainerSlot = cpkt.ContainerSlot;
                spkt.EnchantmentSlot = cpkt.EnchantmentSlot;
                spkt.Skill = skill;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
        }

        /// <summary>
        /// Resets your characters stats.
        /// </summary>
        /// <param name="cpkt"></param>
        private void CM_USESTATRESETITEM(CMSG_STATRESETPOTION cpkt)
        {
            Rag2Item InventoryItem = this.character.container[cpkt.SlotId];
            if (InventoryItem != null)
            {
                //Update stats
                lock (this.character.stats)
                {
                    Saga.PrimaryTypes.CharacterStats.Stats stats = this.character.stats.CHARACTER;
                    int remaining = stats.concentration
                                  + stats.dexterity
                                  + stats.intelligence
                                  + stats.strength
                                  + this.character.stats.REMAINING;

                    lock (character._status)
                    {
                        //Update strength
                        character._status.MaxPAttack -= (ushort)(2 * stats.strength);
                        character._status.MinPAttack -= (ushort)(1 * stats.strength);
                        character._status.MaxHP -= (ushort)(10 * stats.strength);
                        stats.strength = 0;

                        //Update Dextericty
                        character._status.BaseMHitrate -= (ushort)(1 * stats.dexterity);
                        stats.dexterity = 0;

                        //Update Intellect
                        character._status.MaxMAttack -= (ushort)(6 * stats.intelligence);
                        character._status.MinMAttack -= (ushort)(3 * stats.intelligence);
                        character._status.BaseRHitrate -= (ushort)(1 * stats.intelligence);
                        stats.intelligence = 0;

                        //Update Concentration
                        character._status.MaxRAttack -= (ushort)(4 * stats.concentration);
                        character._status.MinRAttack -= (ushort)(2 * stats.concentration);
                        character._status.BasePHitrate -= (ushort)(2 * stats.concentration);
                        stats.concentration = 0;

                        this.character.stats.REMAINING = (ushort)remaining;
                    }
                }

                //Reset stat points
                SMSG_EXTSTATS spkt = new SMSG_EXTSTATS();
                spkt.base_stats_1 = character.stats.BASE;
                spkt.base_stats_2 = character.stats.CHARACTER;
                spkt.base_stats_jobs = character.stats.EQUIPMENT;
                spkt.base_stats_bonus = character.stats.ENCHANTMENT;
                spkt.statpoints = character.stats.REMAINING;
                spkt.SessionId = character.id;
                this.Send((byte[])spkt);

                //Use the item
                int newLength = InventoryItem.count - 1;
                if (newLength > 0)
                {
                    InventoryItem.count = newLength;
                    SMSG_UPDATEITEM spkt2 = new SMSG_UPDATEITEM();
                    spkt2.Amount = (byte)newLength;
                    spkt2.UpdateReason = 0;
                    spkt2.UpdateType = 4;
                    spkt2.Container = 2;
                    spkt2.SessionId = this.character.id;
                    spkt2.Index = cpkt.SlotId;
                    this.Send((byte[])spkt2);
                }
                else
                {
                    this.character.container.RemoveAt(cpkt.SlotId);
                    SMSG_DELETEITEM spkt3 = new SMSG_DELETEITEM();
                    spkt3.UpdateReason = 0;
                    spkt3.Container = 2;
                    spkt3.Index = cpkt.SlotId;
                    spkt3.SessionId = this.character.id;
                    this.Send((byte[])spkt3);
                }
            }
        }

        /// <summary>
        /// Occurs when you switch between weapons (primary -> secondary)
        /// </summary>
        private void CM_WEAPONSWITCH(CMSG_WEAPONSWITCH cpkt)
        {
            //Helper variables
            byte prev_weapontype = 0;
            byte next_weapontype = 0;

            //Deapplies the currrent stats
            lock (this.character._status)
            {
                int WeaponIndex = (this.character.weapons.ActiveWeaponIndex == 1) ? this.character.weapons.SeconairyWeaponIndex : this.character.weapons.PrimaryWeaponIndex;
                if (WeaponIndex < this.character.weapons.UnlockedWeaponSlots)
                {
                    Weapon CurrentWeapon = this.character.weapons[WeaponIndex];
                    prev_weapontype = CurrentWeapon._weapontype;

                    if (CurrentWeapon != null && CurrentWeapon._active == 1)
                    {
                        //Deapplies the weapon stats
                        this.character._status.MaxWMAttack -= (ushort)CurrentWeapon.Info.max_magic_attack;
                        this.character._status.MinWMAttack -= (ushort)CurrentWeapon.Info.min_magic_attack;
                        this.character._status.MaxWPAttack -= (ushort)CurrentWeapon.Info.max_short_attack;
                        this.character._status.MinWPAttack -= (ushort)CurrentWeapon.Info.min_short_attack;
                        this.character._status.MaxWRAttack -= (ushort)CurrentWeapon.Info.max_range_attack;
                        this.character._status.MinWRAttack -= (ushort)CurrentWeapon.Info.min_range_attack;
                        this.character._status.Updates |= 2;

                        //Deapplies the fusion stone
                        if (CurrentWeapon._fusion > 0)
                            Common.Skills.DeleteStaticAddition(this.character, CurrentWeapon._fusion);

                        //Deapplies alterstone additions
                        for (int i = 0; i < 8; i++)
                        {
                            uint addition = CurrentWeapon.Slots[i];
                            if (addition > 0)
                            {
                                Singleton.Additions.DeapplyAddition(addition, character);
                            }
                        }
                    }
                }
            }

            //Toggles the weapons index
            this.character.weapons.ActiveWeaponIndex ^= 1;
            int ShieldIndex = (this.character.weapons.ActiveWeaponIndex == 1) ? 15 : 14;
            Rag2Item shield = this.character.Equipment[ShieldIndex];

            lock (this.character._status)
            {
                int NewWeaponIndex = (this.character.weapons.ActiveWeaponIndex == 1) ? this.character.weapons.SeconairyWeaponIndex : this.character.weapons.PrimaryWeaponIndex;
                if (NewWeaponIndex < this.character.weapons.UnlockedWeaponSlots)
                {
                    //Apples the battle stats
                    Weapon CurrentWeapon = this.character.weapons[NewWeaponIndex];
                    next_weapontype = CurrentWeapon._weapontype;

                    if (CurrentWeapon != null && CurrentWeapon._active == 1)
                    {
                        this.character._status.MaxWMAttack += (ushort)CurrentWeapon.Info.max_magic_attack;
                        this.character._status.MinWMAttack += (ushort)CurrentWeapon.Info.min_magic_attack;
                        this.character._status.MaxWPAttack += (ushort)CurrentWeapon.Info.max_short_attack;
                        this.character._status.MinWPAttack += (ushort)CurrentWeapon.Info.min_short_attack;
                        this.character._status.MaxWRAttack += (ushort)CurrentWeapon.Info.max_range_attack;
                        this.character._status.MinWRAttack += (ushort)CurrentWeapon.Info.min_range_attack;
                        this.character._status.Updates |= 2;
                    }

                    //Reapplies the fusion stone
                    if (CurrentWeapon._fusion > 0)
                        Common.Skills.CreateAddition(this.character, CurrentWeapon._fusion);

                    //Reapplies alterstone additions
                    for (int i = 0; i < 8; i++)
                    {
                        uint addition = CurrentWeapon.Slots[i];
                        if (addition > 0)
                        {
                            Singleton.Additions.DeapplyAddition(addition, character);
                        }
                    }
                }

                if (prev_weapontype != next_weapontype)
                {
                    foreach (Skill skill in this.character.learnedskills)
                    {
                        //If it's a pasive skill
                        if (skill.info.skilltype == 2)
                        {
                            bool PreviousAble = skill.info.requiredWeapons[prev_weapontype] == 1;
                            bool NextAble = skill.info.requiredWeapons[next_weapontype] == 1;

                            if (PreviousAble == false && NextAble == true)
                            {
                                Singleton.Additions.ApplyAddition(skill.info.addition, this.character);
                                this.character._status.Updates |= 2;
                            }
                            else if (NextAble == false && PreviousAble == true)
                            {
                                Singleton.Additions.DeapplyAddition(skill.info.addition, this.character);
                                this.character._status.Updates |= 2;
                            }
                        }
                    }
                }
            }

            //Switch the weapons
            Point oldPos = this.character.Position;
            Regiontree tree = this.character.currentzone.Regiontree;
            foreach (Character regionObject in tree.SearchActors(this.character, SearchFlags.Characters))
            {
                if (regionObject.id == this.character.id)
                {
                    SMSG_WEAPONSWITCH spkt = new SMSG_WEAPONSWITCH();
                    spkt.slotid = cpkt.slotid;
                    spkt.SessionId = this.character.id;
                    this.Send((byte[])spkt);
                }
                else if (Point.IsInSightRangeByRadius(regionObject.Position, oldPos))
                {
                    SMSG_SHOWWEAPON spkt = new SMSG_SHOWWEAPON();
                    spkt.ActorID = this.character.id;
                    spkt.AugeID = this.character.ComputeAugeSkill();
                    spkt.SessionId = regionObject.id;
                    regionObject.client.Send((byte[])spkt);

                    SMSG_CHANGEEQUIPMENT spkt2 = new SMSG_CHANGEEQUIPMENT();
                    spkt2.ActorID = this.character.id;
                    spkt2.Slot = (byte)ShieldIndex;
                    spkt2.ItemID = (shield != null) ? shield.info.item : 0;
                    spkt2.Dye = shield.dyecolor;
                    spkt2.SessionId = regionObject.id;
                    regionObject.client.Send((byte[])spkt2);
                }
            }

            //Does the switch
            Tasks.LifeCycle.Update(this.character);
        }

        /// <summary>
        /// Occurs when changing the weapon
        /// </summary>
        private void CM_WEAPONMOVE(CMSG_WEAPONMOVE cpkt)
        {
            bool isselectedweapon = cpkt.Slot == this.character.weapons.ActiveWeaponIndex;
            byte prev_slot = (cpkt.Slot == 1) ? this.character.weapons.SeconairyWeaponIndex : this.character.weapons.PrimaryWeaponIndex;
            byte next_slot = cpkt.Slot;
            byte prev_weapontype = 0;
            byte next_weapontype = 0;

            if (isselectedweapon)
            {
                lock (this.character._status)
                {
                    if (prev_slot < this.character.weapons.UnlockedWeaponSlots)
                    {
                        Weapon CurrentWeapon = this.character.weapons[prev_slot];
                        prev_weapontype = CurrentWeapon._weapontype;

                        if (CurrentWeapon != null && CurrentWeapon._active == 1)
                        {
                            //Deapplies the weapon stats
                            this.character._status.MaxWMAttack -= (ushort)CurrentWeapon.Info.max_magic_attack;
                            this.character._status.MinWMAttack -= (ushort)CurrentWeapon.Info.min_magic_attack;
                            this.character._status.MaxWPAttack -= (ushort)CurrentWeapon.Info.max_short_attack;
                            this.character._status.MinWPAttack -= (ushort)CurrentWeapon.Info.min_short_attack;
                            this.character._status.MaxWRAttack -= (ushort)CurrentWeapon.Info.max_range_attack;
                            this.character._status.MinWRAttack -= (ushort)CurrentWeapon.Info.min_range_attack;
                            this.character._status.Updates |= 2;

                            //Deapplies the fusion stone
                            if (CurrentWeapon._fusion > 0)
                                Common.Skills.DeleteStaticAddition(this.character, CurrentWeapon._fusion);

                            //Deapplies alterstone additions
                            for (int i = 0; i < 8; i++)
                            {
                                uint addition = CurrentWeapon.Slots[i];
                                if (addition > 0)
                                {
                                    Singleton.Additions.DeapplyAddition(addition, character);
                                }
                            }
                        }
                    }

                    if (next_slot < this.character.weapons.UnlockedWeaponSlots)
                    {
                        //Apples the battle stats
                        Weapon CurrentWeapon = this.character.weapons[next_slot];
                        next_weapontype = CurrentWeapon._weapontype;

                        if (CurrentWeapon != null && CurrentWeapon._active == 1)
                        {
                            this.character._status.MaxWMAttack += (ushort)CurrentWeapon.Info.max_magic_attack;
                            this.character._status.MinWMAttack += (ushort)CurrentWeapon.Info.min_magic_attack;
                            this.character._status.MaxWPAttack += (ushort)CurrentWeapon.Info.max_short_attack;
                            this.character._status.MinWPAttack += (ushort)CurrentWeapon.Info.min_short_attack;
                            this.character._status.MaxWRAttack += (ushort)CurrentWeapon.Info.max_range_attack;
                            this.character._status.MinWRAttack += (ushort)CurrentWeapon.Info.min_range_attack;
                            this.character._status.Updates |= 2;
                        }

                        //Reapplies the fusion stone
                        if (CurrentWeapon._fusion > 0)
                            Common.Skills.CreateAddition(this.character, CurrentWeapon._fusion);

                        //Reapplies alterstone additions
                        for (int i = 0; i < 8; i++)
                        {
                            uint addition = CurrentWeapon.Slots[i];
                            if (addition > 0)
                            {
                                Singleton.Additions.DeapplyAddition(addition, character);
                            }
                        }
                    }

                    if (prev_weapontype != next_weapontype)
                    {
                        foreach (Skill skill in this.character.learnedskills)
                        {
                            //If it's a pasive skill
                            if (skill.info.skilltype == 2)
                            {
                                bool PreviousAble = skill.info.requiredWeapons[prev_weapontype] == 1;
                                bool NextAble = skill.info.requiredWeapons[next_weapontype] == 1;

                                if (PreviousAble == false && NextAble == true)
                                    Singleton.Additions.ApplyAddition(skill.info.addition, this.character);
                                else if (NextAble == false && PreviousAble == true)
                                    Singleton.Additions.DeapplyAddition(skill.info.addition, this.character);
                            }
                        }
                    }
                }
            }

            //Switch the weapons
            Point oldPos = this.character.Position;
            Regiontree tree = this.character.currentzone.Regiontree;
            foreach (Character regionObject in tree.SearchActors(SearchFlags.Characters))
            {
                if (regionObject.id == this.character.id)
                {
                    SMSG_WEAPONMOVE spkt = new SMSG_WEAPONMOVE();
                    spkt.Unknown1 = cpkt.Index;
                    spkt.Unknown2 = cpkt.WeaponSlot;
                    spkt.Weaponslot = cpkt.Slot;
                    spkt.SessionId = this.character.id;
                    regionObject.client.Send((byte[])spkt);
                }
                else if (isselectedweapon && Point.IsInSightRangeByRadius(regionObject.Position, oldPos))
                {
                    SMSG_SHOWWEAPON spkt = new SMSG_SHOWWEAPON();
                    spkt.ActorID = this.character.id;
                    spkt.AugeID = this.character.ComputeAugeSkill();
                    spkt.SessionId = regionObject.id;
                    regionObject.client.Send((byte[])spkt);
                }
            }

            Tasks.LifeCycle.Update(this.character);
        }
    }
}