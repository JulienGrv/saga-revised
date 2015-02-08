using Saga.Enumarations;
using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;

namespace Saga.Quests.Scenario
{
    static partial class QUEST_TABLE
    {
        /// <summary>
        /// Get's the currentscenariostep
        /// </summary>
        public static uint GetCurrentStep(uint cid, uint QID, uint EventId)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                SMSG_SCENARIOEVENTBEGIN spkt = new SMSG_SCENARIOEVENTBEGIN();
                spkt.ActorId = value.id;
                spkt.Event = EventId;

                Regiontree tree = value.currentzone.Regiontree;
                foreach (Character regionObject in tree.SearchActors(value, SearchFlags.Characters))
                {
                    if (value.currentzone.IsInSightRangeByRadius(value.Position, regionObject.Position))
                    {
                        spkt.SessionId = regionObject.id;
                        regionObject.client.Send((byte[])spkt);
                    }
                }
            }
            return 0;
        }
    }
}