using Saga.Enumarations;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System.Collections.Generic;

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
        public static bool NpcTakeItem(uint CID, uint ItemId, int ItemCount)
        {
            Character Character;
            if (LifeCycle.TryGetById(CID, out Character))
            {
                foreach (KeyValuePair<byte, Rag2Item> pair in Character.container.GetAllItems())
                {
                    if (pair.Value.info.item != ItemId || pair.Value.count < ItemCount) continue;
                    int newCount = pair.Value.count -= ItemCount;
                    if (newCount > 0)
                    {
                        SMSG_ITEMADJUST spkt = new SMSG_ITEMADJUST();
                        spkt.Container = 2;
                        spkt.Function = 4;
                        spkt.Slot = pair.Key;
                        spkt.UpdateReason = (byte)ItemUpdateReason.GiveToNpc;
                        spkt.Value = (byte)pair.Value.count;
                        spkt.SessionId = Character.id;
                        Character.client.Send((byte[])spkt);
                    }
                    else
                    {
                        Character.container.RemoveAt(pair.Key);
                        SMSG_DELETEITEM spkt = new SMSG_DELETEITEM();
                        spkt.Container = 2;
                        spkt.Index = pair.Key;
                        spkt.UpdateReason = (byte)ItemUpdateReason.GiveToNpc;
                        spkt.SessionId = Character.id;
                        Character.client.Send((byte[])spkt);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}