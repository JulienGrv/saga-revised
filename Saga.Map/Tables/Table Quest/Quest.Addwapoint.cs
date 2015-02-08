using Saga.Enumarations;
using Saga.Map;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System;
using System.Collections.Generic;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <title>Saga.AddWaypoint</title>
        /// <code>
        /// Saga.AddWaypoint(cid, QuestID, StepId, State);
        /// </code>
        /// <description>
        /// Adds a waypoint to the quest list.
        /// (Note it will also activate the npc if it's not yet activated)
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
        public static void AddWayPoint(uint cid, uint QID, uint stepid, byte substepid, uint npcType)
        {
            Character Character;
            if (LifeCycle.TryGetById(cid, out Character))
            {
                //Substep - 1;
                substepid--;

                //Find the substep
                Predicate<Saga.Quests.Objectives.ObjectiveList.SubStep> FindSubstep =
                delegate(Saga.Quests.Objectives.ObjectiveList.SubStep objective)
                {
                    return objective.Quest == QID
                        && objective.StepId == stepid
                        && objective.SubStepId == substepid;
                };

                Predicate<Saga.Quests.Objectives.ObjectiveList.Waypoint> FindWaypoint =
                delegate(Saga.Quests.Objectives.ObjectiveList.Waypoint objective)
                {
                    return objective.Quest == QID
                        && objective.StepId == stepid
                        && objective.NpcId == npcType;
                };

                //Get substep list
                List<Saga.Quests.Objectives.ObjectiveList.SubStep> Substeps =
                     Character.QuestObjectives.Substeps;

                //Add waypoint
                List<Saga.Quests.Objectives.ObjectiveList.Waypoint> Waypoints =
                    Character.QuestObjectives.GuidancePoints;

                //Add substeps
                int index = Substeps.FindIndex(FindSubstep);
                if (index == -1)
                {
                    int index2 = Waypoints.FindIndex(FindWaypoint);
                    if (index2 == -1)
                    {
                        Substeps.Add(new Saga.Quests.Objectives.ObjectiveList.SubStep(substepid, false, 0, 1, QID, stepid));
                        Waypoints.Add(new Saga.Quests.Objectives.ObjectiveList.Waypoint(npcType, QID, stepid, substepid));
                        Regiontree tree = Character.currentzone.Regiontree;
                        foreach (MapObject a in tree.SearchActors(Character, SearchFlags.Npcs))
                        {
                            BaseMob mob = a as BaseMob;
                            if (a.ModelId == npcType)
                            {
                                if (mob != null)
                                    Common.Actions.UpdateIcon(Character, mob);
                            }
                        }
                    }
                }
            }
        }
    }
}