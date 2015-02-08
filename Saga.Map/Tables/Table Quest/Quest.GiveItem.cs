using Saga.Enumarations;
using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <title>Saga.GiveItem</title>
        /// <code>
        /// Saga.GiveItem(cid, RewItem, RewItemCount);
        /// </code>
        /// <description>
        /// Gives a certain item and with the specified
        /// itemcount as part of a quest reward.
        /// </description>
        /// <example>
        ///function QUEST_FINISH(cid)
        ///	    -- Gives all rewards
        ///
        ///	    Saga.GiveItem(cid, RewItem1, RewItemCount1 );
        ///	    Saga.GiveZeny(cid, RewZeny);
        ///	    Saga.GiveExp(cid, RewCxp, RewJxp, RewWxp);
        ///	    return 0;
        /// end
        /// </example>
        public static bool GiveItem(uint cid, uint itemid, byte count)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                Rag2Item item;
                if (Singleton.Item.TryGetItemWithCount(itemid, count, out item))
                {
                    int index = value.container.Add(item);
                    if (index > -1)
                    {
                        SMSG_ADDITEM spkt = new SMSG_ADDITEM();
                        spkt.Container = 2;
                        spkt.SessionId = value.id;
                        spkt.UpdateReason = (byte)ItemUpdateReason.ReceivedAsQuestReward;
                        spkt.SetItem(item, index);
                        value.client.Send((byte[])spkt);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}