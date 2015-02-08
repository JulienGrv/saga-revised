namespace Saga.Quests.Scenario
{
    static partial class QUEST_TABLE
    {
        /// <summary>
        /// Get's the currentscenariostep
        /// </summary>
        public static uint GetCurrentStep(uint cid, uint QID)
        {
            /*
            Character value;

            if (LifeCycle.TryGetById(cid, out value))
            if( value.client.scenarioquest != null )
               foreach (StepInfo c in value.client.scenarioquest.listofsteps)
               {
                   if (c.Status == 1) { return c.StepID; }
               }
            */
            return 0;
        }
    }
}