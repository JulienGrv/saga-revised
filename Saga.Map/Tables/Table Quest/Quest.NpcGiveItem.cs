using Saga.Enumarations;
using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <title>Saga.AddStep</title>
        /// <code>
        /// Saga.AddStep(cid, QuestID, StepId, State);
        /// </code>
        /// <description>
        /// Adds a step to the quest. This can only be done during initialisation.
        /// 0 = Hidden, 1 = Visible, 2 = Completed
        /// </description>
        /// <example>
        ///function QUEST_START(cid)
        ///     -- Initialize all quest steps
        ///     -- Initialize all starting navigation points
        ///
        ///     Saga.AddStep(cid, QuestID, 101, 1);
        ///     Saga.AddStep(cid, QuestID, 102, 0);
        ///     Saga.AddStep(cid, QuestID, 103, 0);
        ///	    Saga.InsertQuest(cid, QuestID, 1);
        ///
        ///     return 0;
        ///end
        /// </example>
        public static bool NpcGiveItem(uint CID, uint ItemId, byte ItemCount)
        {
            Character value;
            if (LifeCycle.TryGetById(CID, out value))
            {
                Rag2Item item;
                if (Singleton.Item.TryGetItemWithCount(ItemId, ItemCount, out item))
                {
                    int index = value.container.Add(item);
                    if (index > -1)
                    {
                        SMSG_ADDITEM spkt = new SMSG_ADDITEM();
                        spkt.Container = 2;
                        spkt.SessionId = value.id;
                        spkt.UpdateReason = (byte)ItemUpdateReason.ReceiveFromNpc;
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