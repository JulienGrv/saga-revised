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
        /// Adds a substep to the quests 
        /// </summary>
        public static void AddSubStep(uint CID, uint QID, uint StepID, byte SSID)
        {


            /*
            Character value;
            if (LifeCycle.TryGetById(CID, out value))
            {
                QuestBase myQuest;
                if (value.client.questlist.TryGetValue(QID, out myQuest))
                {
                    myQuest.listofsubsteps.Add(new SubStepInfo(StepID, SSID));
                }
                else if (value.client.pendingquest != null && value.client.pendingquest.QuestId == QID)
                {
                    value.client.pendingquest.listofsubsteps.Add(new SubStepInfo(StepID, SSID));
                }
            }
            */
        }
    }
}
