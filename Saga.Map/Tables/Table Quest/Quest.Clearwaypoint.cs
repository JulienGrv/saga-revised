using Saga.Packets;
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
        public static void ClearWayPoints(uint CID, uint QID)
        {
            QuestBase quest;
            Character value;
            if (LifeCycle.TryGetById(CID, out value))
            {
                quest = value.QuestObjectives[QID];
                Predicate<Saga.Quests.Objectives.ObjectiveList.Waypoint> FindGuidancePoints =
                    delegate(Saga.Quests.Objectives.ObjectiveList.Waypoint objective)
                    {
                        return objective.Quest == QID;
                    };

                if (quest != null) quest.IsWaypointsCleared = true;
                int count = value.QuestObjectives.GuidancePoints.RemoveAll(FindGuidancePoints);
                if (count > 0)
                {
                    SMSG_REMOVENAVIGATIONPOINT spkt = new SMSG_REMOVENAVIGATIONPOINT();
                    spkt.SessionId = value.id;
                    spkt.QuestID = QID;
                    value.client.Send((byte[])spkt);
                }
            }
        }
    }
}