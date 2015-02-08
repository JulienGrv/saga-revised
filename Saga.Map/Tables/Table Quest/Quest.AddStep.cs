using Saga.PrimaryTypes;
using Saga.Tasks;
using System.Collections.Generic;

namespace Saga.Quests
{
    public static partial class QUEST_TABLE
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
        public static void AddStep(uint CID, uint QID, uint StepID)
        {
            Character Character;
            if (LifeCycle.TryGetById(CID, out Character))
            {
                List<Saga.Quests.Objectives.ObjectiveList.StepInfo> Steps = QuestBase.GetSteps(Character, QID);
                Steps.Add(new Saga.Quests.Objectives.ObjectiveList.StepInfo(0, QID, StepID));
            }
        }

        public static void SendEmptyInventory(uint cid)
        {
            Character character;
            if (LifeCycle.TryGetById(cid, out character))
            {
                //Show low inventory space
                Common.Errors.GeneralErrorMessage(character,
                    (uint)Enumarations.Generalerror.LowIventorySpace);
            }
        }

        public static void SendInventoryNotFound(uint cid)
        {
            Character character;
            if (LifeCycle.TryGetById(cid, out character))
            {
                //Show inventory item not found
                Common.Errors.GeneralErrorMessage(character,
                    (uint)Enumarations.Generalerror.InventoryItemNotFound);
            }
        }
    }
}