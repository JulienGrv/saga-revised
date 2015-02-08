using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Quests;

namespace Saga.Map.Client
{
    partial class Client
    {
        /// <summary>
        /// Container for the scenario quest. There can only be 1 quest active
        /// at the same time.
        /// </summary>
        internal QuestBase scenarioquest = null;

        internal uint ScenarioQuestStep = 0;

        /// <summary>
        /// This is invoked after the client presses to be continued after
        /// the dialogs occured.
        /// </summary>
        /// <param name="buffer"></param>
        private void CM_SCENARIO_EVENTEND(byte[] buffer)
        {
            //CHECK THE QUEST
            this.character._Event = 0;

            //BROADCAST TO EVERYBODY EVENT HAS ENDED
            SMSG_SCENARIOEVENTEND spkt = new SMSG_SCENARIOEVENTEND();
            spkt.ActorId = this.character.id;
            foreach (MapObject myObject in this.character.currentzone.GetObjectsInRegionalRange(this.character))
            {
                if (this.character.currentzone.IsInSightRangeByRadius(this.character.Position, myObject.Position))
                    if (MapObject.IsPlayer(myObject))
                    {
                        Character characterTarget = myObject as Character;
                        spkt.SessionId = characterTarget.id;
                        characterTarget.client.Send((byte[])spkt);
                    }
            }
        }
    }
}