using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.Definitions;
using Saga.Managers;
using Saga.Tasks;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <summary>
        /// Adds's a conversation Objective
        /// </summary>
        public static void Objective_Conversation(uint cid, uint QID, uint modelid)
        {

            /*
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                QuestBase myQuest;
                if (value.client.questlist.TryGetValue(QID, out myQuest))
                {
                    myQuest.listofactorsforofficialquest.Add(modelid);
                }
                else if (value.client.pendingquest != null && value.client.pendingquest.QuestId == QID)
                {
                    myQuest.listofactorsforofficialquest.Add(modelid);
                }
            }
            */
        }
    }
}
