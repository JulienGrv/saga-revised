using Saga.PrimaryTypes;
using Saga.Tasks;
using System;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <title>Saga.ClearWaypoints</title>
        /// <code>
        /// Saga.ClearWaypoints(cid, QuestID, StepId, State);
        /// </code>
        /// <description>
        /// Removes all waypoints for a the specified quest.
        /// </description>
        /// <example>
        /// function QUEST_STEP_2(cid)
        ///	    -- Talk to mischa
        ///     local NPCIndex = 1000;
        ///     local ret = Saga.GetNPCIndex(cid);
        ///
        ///     Saga.AddWaypoint(cid, QuestID, NPCIndex, -12092, -6490, -8284, 1);
        ///     if ret == NPCIndex then
        ///         Saga.StepComplete(cid, QuestID, 102);
        ///     else
        ///         return  -1;
        ///     end
        ///
        ///     Saga.ClearWaypoints(cid, QuestID);
        ///     return 0;
        /// end
        /// </example>
        public static void RestoreDiscardableItems(uint CID, uint QID)
        {
            Character value;
            if (LifeCycle.TryGetById(CID, out value))
            {
                Predicate<Saga.Quests.Objectives.ObjectiveList.Loot2> FindGuidancePoints =
                    delegate(Saga.Quests.Objectives.ObjectiveList.Loot2 objective)
                    {
                        return objective.Quest == QID;
                    };

                value.QuestObjectives.NonDiscardableItems.RemoveAll(FindGuidancePoints);
            }
        }
    }
}