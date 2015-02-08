using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <title>Saga.QuestComplete</title>
        /// <code>
        /// Saga.QuestComplete(cid, QuestID);
        /// </code>
        /// <description>
        /// Completes a quest
        /// </description>
        /// <example>
        /// function QUEST_STEP_3(cid)
        ///     -- Complete quest
        ///     Saga.QuestComplete(cid, QuestID);
        ///     return 0;
        /// end
        /// </example>
        public static void QuestComplete(uint CID, uint QID)
        {
            Character value;
            if (LifeCycle.TryGetById(CID, out value))
            {
                if (value.QuestObjectives[QID] != null)
                {
                    SMSG_QUESTCOMPLETE spkt = new SMSG_QUESTCOMPLETE();
                    spkt.QuestID = QID;
                    spkt.SessionId = value.id;
                    value.client.Send((byte[])spkt);
                }
            }
        }
    }
}