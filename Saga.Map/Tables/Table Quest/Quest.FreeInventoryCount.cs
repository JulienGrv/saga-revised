using Saga.PrimaryTypes;
using Saga.Tasks;

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
        public static int FreeInventoryCount(uint CID, uint ItemId)
        {
            Character Character;
            if (LifeCycle.TryGetById(CID, out Character))
            {
                return Character.container.Capacity - Character.container.Count;
            }
            return 0;
        }
    }
}