using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.Definitions;
using Saga.Managers;
using Saga.Tasks;
using Saga.PrimaryTypes;
using QuestObjectives = Saga.Quests.Objectives.ObjectiveList;

namespace Saga.Quests
{
    
    static partial class QUEST_TABLE
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool IsSubStepCompleted(uint CID, uint QID, uint SID, byte SSID)
        {
            Character value;
            if (LifeCycle.TryGetById(CID, out value))
            {
                SSID--;

                Predicate<QuestObjectives.SubStep> FindSubstep = delegate(QuestObjectives.SubStep objective)
                {
                    return objective.SubStepId == SSID
                        && objective.StepId == SID
                        && objective.Quest == QID;
                };

               QuestObjectives.SubStep fsubstep = value.QuestObjectives.Substeps.Find(FindSubstep);
               if (fsubstep != null)
               {
                   return fsubstep.Completed;
               }
            }

            return false;
        }
    }
}
