using Saga.PrimaryTypes;
using Saga.Tasks;
using System.Collections.Generic;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <summary>
        ///
        /// </summary>
        public static uint GetCurrentStep(uint cid, uint QID)
        {
            Character character;
            if (LifeCycle.TryGetById(cid, out character))
            {
                if (character.QuestObjectives[QID] != null)
                {
                    List<Saga.Quests.Objectives.ObjectiveList.StepInfo> steps =
                        QuestBase.GetSteps(character, QID);
                    foreach (Saga.Quests.Objectives.ObjectiveList.StepInfo c in steps)
                    {
                        if (c.State == 1) { return c.StepId; }
                    }
                }
            }

            return 0;
        }
    }
}