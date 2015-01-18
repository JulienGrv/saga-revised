using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.Definitions;
using Saga.Packets;
using Saga.Managers;
using Saga.Tasks;
using Saga.Map;
using Saga.PrimaryTypes;

namespace Saga.Quests.Scenario
{
    static partial class QUEST_TABLE
    {
        /// <summary>
        /// Inserts a new quests into the questlist.
        /// </summary>
        public static void InsertQuest(uint cid, uint QID)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                #region Check Quest

                QuestBase myquest = null;
                bool result = false;

                if (Singleton.Quests.TryFindQuests(QID, out value.client.scenarioquest))
                {
                    if (value.client.scenarioquest.OnStart(value.id) > -1)
                   {                   
                       myquest = value.client.scenarioquest;
                       myquest.QuestId = QID;
                       result = true;
                   }
                }

                #endregion

                #region Add to Quest List

                if (result == false) return;

                SMSG_INITIALIZESCENARIO spkt = new SMSG_INITIALIZESCENARIO();
                spkt.StepStatus = 1;
                spkt.Scenario1 = myquest.QuestId;
                spkt.Scenario2 = GetCurrentStep(cid, QID);
                value.client.Send((byte[])spkt);

                #endregion

                #region Check Quest

                //value.client.scenarioquest.CheckQuest(cid);

                #endregion
            }
        }
    }
}
