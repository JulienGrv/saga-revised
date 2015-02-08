using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        public static void UserUpdateActionObjectType(uint cid, uint QID, uint SID, uint AID, byte State)
        {
            //HELPER VARIABLES
            Character value;

            //GET THE ACTIVE QUEST
            if (LifeCycle.TryGetById(cid, out value))
            {
                Predicate<Saga.Quests.Objectives.ObjectiveList.Activation> FindActivatedObject = delegate(Saga.Quests.Objectives.ObjectiveList.Activation objective)
                {
                    return objective.NpcId == AID &&
                        objective.StepId == SID;
                };

                bool process = false;
                if (State == 0)
                {
                    if (value.QuestObjectives.ActivatedNpc.FindIndex(FindActivatedObject) == -1)
                    {
                        value.QuestObjectives.ActivatedNpc.Add(
                            new Saga.Quests.Objectives.ObjectiveList.Activation(
                                AID,
                                QID,
                                SID
                            )
                        );
                        process = true;
                    }
                }
                else
                {
                    value.QuestObjectives.ActivatedNpc.RemoveAll(FindActivatedObject);
                    process = true;
                }

                if (process == true) //For optimilisation
                {
                    Regiontree tree = value.currentzone.Regiontree;
                    foreach (MapItem item in tree.SearchActors(value, Saga.Enumarations.SearchFlags.MapItems))
                        if (item.ModelId == AID)
                        {
                            SMSG_ITEMUPDATE spkt = new SMSG_ITEMUPDATE();
                            spkt.ActorID = item.id;
                            spkt.Active1 = 0;
                            spkt.Active = item.IsHighlighted(value);
                            spkt.Active2 = item.IsInteractable(value);
                            spkt.SessionId = value.id;
                            value.client.Send((byte[])spkt);
                        }
                }
            }
        }
    }
}