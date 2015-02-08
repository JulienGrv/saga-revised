using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <summary>
        ///
        /// </summary>
        public static void SubStepComplete(uint cid, uint QID, uint stepid, int substepid)
        {
            //HELPER VARIABLES
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                substepid--;

                Predicate<Saga.Quests.Objectives.ObjectiveList.SubStep> FindSubstep =
                    delegate(Saga.Quests.Objectives.ObjectiveList.SubStep objective)
                    {
                        return objective.Quest == QID
                            && objective.StepId == stepid
                            && objective.SubStepId == substepid;
                    };

                Saga.Quests.Objectives.ObjectiveList.SubStep substep =
                    value.QuestObjectives.Substeps.Find(FindSubstep);

                if (substep != null)
                {
                    //Update the quest
                    substep.Completed = true;
                    substep.current = substep.max;

                    SMSG_QUESTSUBSTEPUPDATE spkt = new SMSG_QUESTSUBSTEPUPDATE();
                    spkt.Amount = (byte)substep.current;
                    spkt.QuestID = substep.Quest;
                    spkt.StepID = substep.StepId;
                    spkt.SubStep = (byte)substep.SubStepId;
                    spkt.SessionId = value.id;
                    spkt.Unknown = 1;
                    value.client.Send((byte[])spkt);
                }
            }
        }
    }
}