using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.Definitions;
using Saga.Map.Definitions.Misc;
using Saga.Packets;
using Saga.Tasks;
using Saga.Map;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {

        /// <title>Saga.NpcTakeItem</title>
        /// <code>
        /// Saga.NpcTakeItem(cid, QuestID, ItemId, Count);
        /// </code>
        /// <description>
        /// Takes a npc item as with a message that the npc took it.
        /// </description>
        /// <example>
        public static int TakeItem(uint cid, uint itemid, byte count)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {

                //HELPER VARIABLES
                DEFAULT_FACTORY_ITEMS.ItemInfo info;
                Singleton.Item.TryGetItem(itemid, out info);
                //int stackcount = 0;

                Predicate<Rag2Item> callback = delegate(Rag2Item item)
                {
                    bool result = item.info.item == itemid;
                    //if (result) stackcount += item.info.max_stack - item.count;
                    return result;
                };

                //CHECK IF RIGHT AMOUNT OF ITEMS WAS FOUND
                List<int> FoundItems = value.ITEMS.FindAll(callback);
                //if (count > stackcount) return -1;

                //DO THE ACTUAL TAKING
                int acount = count;
                foreach (int currentInxdex in FoundItems)
                {
                    Stackable<Rag2Item> item = value.ITEMS[currentInxdex];
                    int MinCount = Math.Min(acount, Math.Min(item.Count, info.max_stack));
                    acount -= MinCount;

                    if ((item.Count -= MinCount) == 0)
                    {
                        value.ITEMS.RemoveAt(currentInxdex);
                        SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                        spkt.Container = 2;
                        spkt.Index = (byte)currentInxdex;
                        spkt.UpdateReason = (byte)ITEMUPDATEREASON.GIVE_TO_NPC;
                        spkt.SessionId = value.id;
                        value.client.Send((byte[])spkt);
                    }
                    else
                    {
                        SMSG_UPDATEITEM spkt = new SMSG_UPDATEITEM();
                        spkt.Amount = (byte)item.Count;
                        spkt.UpdateReason = (byte)ITEMUPDATEREASON.GIVE_TO_NPC;
                        spkt.UpdateType = 4;
                        spkt.Container = 2;
                        spkt.SessionId = value.id;
                        spkt.Index = (byte)currentInxdex;
                        value.client.Send((byte[])spkt);
                        break;
                    }
                }
                return count;
            }
            else
            {
                return -1;
            }           
        }
    }
}
