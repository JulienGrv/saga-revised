namespace Saga.Quests.Scenario
{
    static partial class QUEST_TABLE
    {
        /// <summary>
        ///
        /// </summary>
        public static void StepComplete(uint cid, uint QID, uint StepID)
        {
            /*
            QuestBase myQuest;
            Character value;

            Predicate<LootObjective> callback = delegate(LootObjective objective)
            {
                return objective.StepID == StepID;
            };

            if (LifeCycle.TryGetById(cid, out value))
            {
                if (value.client.scenarioquest != null)
                {
                    myQuest = value.client.scenarioquest;
                    myQuest.listoflootobjectives.RemoveAll(callback);

                    uint stepIndexies = 0;
                    IEnumerator<StepInfo> i = myQuest.Steps.GetEnumerator();
                    while (i.MoveNext())
                        if (i.Current.StepID == StepID)
                        {
                            i.Current.Status = 2;
                            if (i.MoveNext())
                                if (i.Current.Status == 0)
                                {
                                    i.Current.Status = 1;
                                    stepIndexies = i.Current.StepID;
                                }
                            break;
                        }

                    SMSG_SCENARIOSTEPCOMPLETE spkt = new SMSG_SCENARIOSTEPCOMPLETE();
                    spkt.Step = StepID;
                    spkt.NextStep = stepIndexies;
                    value.client.ScenarioQuestStep = stepIndexies;

                    CommonFunctions.UpdateNpcIcons(value);
                }
            }
            */
        }
    }
}