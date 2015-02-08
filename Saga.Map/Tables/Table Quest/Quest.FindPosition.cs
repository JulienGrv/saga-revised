using Saga.PrimaryTypes;
using Saga.Tasks;
using System;
using System.Collections.Generic;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        public static void FindPosition(uint cid, uint questid, uint stepid, byte substepid, float x, float y, float z, byte map, uint range)
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
                    return objective.Quest == questid
                        && objective.StepId == stepid
                        && objective.SubStepId == substepid;
                };

                Predicate<Saga.Quests.Objectives.ObjectiveList.Position> FindPoint =
                delegate(Saga.Quests.Objectives.ObjectiveList.Position objective)
                {
                    return objective.Quest == questid
                        && objective.StepId == stepid
                        && objective.SubStepId == substepid;
                };

                //Get substep list
                List<Saga.Quests.Objectives.ObjectiveList.SubStep> Substeps =
                     Character.QuestObjectives.Substeps;

                //Add waypoint
                List<Saga.Quests.Objectives.ObjectiveList.Position> Positions =
                    Character.QuestObjectives.Points;

                //Add substeps
                int index = Substeps.FindIndex(FindSubstep);
                if (index == -1)
                {
                    int index2 = Positions.FindIndex(FindPoint);
                    if (index2 == -1)
                    {
                        Substeps.Add(new Saga.Quests.Objectives.ObjectiveList.SubStep(substepid, false, 0, 1, questid, stepid));
                        Positions.Add(new Saga.Quests.Objectives.ObjectiveList.Position(x, y, z, map, range, questid, stepid, substepid));
                    }
                }
            }
        }
    }
}