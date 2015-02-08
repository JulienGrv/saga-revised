using Saga.PrimaryTypes;
using Saga.Tasks;
using System;
using System.Collections.Generic;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <summary>
        ///
        /// </summary>
        public static int ObjectiveElemenation(uint cid, uint QID, uint stepid, uint ModelId, byte Count, uint substepid)
        {
            try
            {
                substepid--;

                //HELPER VARIABLES
                Character value;

                Predicate<Saga.Quests.Objectives.ObjectiveList.Elimination> callback =
                    delegate(Saga.Quests.Objectives.ObjectiveList.Elimination objective)
                    {
                        return objective.NpcId == ModelId &&
                               objective.StepId == stepid &&
                               objective.SubStepId == substepid;
                    };

                Predicate<Saga.Quests.Objectives.ObjectiveList.SubStep> FindSubstep =
                    delegate(Saga.Quests.Objectives.ObjectiveList.SubStep objective)
                    {
                        return objective.Quest == QID
                            && objective.StepId == stepid
                            && objective.SubStepId == substepid;
                    };

                if (LifeCycle.TryGetById(cid, out value))
                {
                    List<Saga.Quests.Objectives.ObjectiveList.Elimination> Elimination =
                        value.QuestObjectives.Elimintations;
                    List<Saga.Quests.Objectives.ObjectiveList.SubStep> Substeps =
                        value.QuestObjectives.Substeps;
                    int index = Elimination.FindIndex(callback);

                    if (index == -1)
                    {
                        Elimination.Add(
                            new Saga.Quests.Objectives.ObjectiveList.Elimination(
                                ModelId,    //Model to eliminate
                                QID,        //Quest
                                stepid,     //Step of the quest
                                (int)substepid   //Substep of the quest (is needed for number of items)
                            )
                        );

                        int index2 = Substeps.FindIndex(FindSubstep);
                        if (index2 == -1)
                        {
                            Substeps.Add(
                                new Saga.Quests.Objectives.ObjectiveList.SubStep(
                                    (int)substepid,  //Substep
                                    false,      //Not completed
                                    0,          //Current count
                                    (int)Count, //Find n items;
                                    QID,        //QuestId
                                    stepid      //StepId
                                )
                            );

                            return 0;
                        }
                        else
                        {
                            return Substeps[index2].current;
                        }
                    }
                    else
                    {
                        int index2 = Substeps.FindIndex(FindSubstep);
                        if (index2 > -1)
                            return Substeps[index2].current;
                        else
                            return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }
    }
}