using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <summary>
        /// Inserts a new quests into the questlist.
        /// </summary>
        public static void InsertQuest(uint cid, uint QID, byte slot)
        {
            QuestBase oldQuest;
            QuestBase newQuest;
            Character Character;

            if (LifeCycle.TryGetById(cid, out Character))
            {
                newQuest = Character.QuestObjectives[QID];
                if (newQuest == null)

                    if (Singleton.Quests.TryFindQuests(QID, out newQuest))
                    {
                        if (slot == 1)
                        {
                            oldQuest = Character.QuestObjectives.OfficialQuest;
                            Character.QuestObjectives.OfficialQuest = newQuest;
                            newQuest.questtype = slot;
                        }
                        else if (slot == 2)
                        {
                            oldQuest = Character.QuestObjectives.PersonalQuest;
                            Character.QuestObjectives.PersonalQuest = newQuest;
                            newQuest.questtype = slot;
                        }

                        List<Saga.Quests.Objectives.ObjectiveList.StepInfo> Steps2 =
                            QuestBase.GetSteps(Character, QID);

                        if (Steps2.Count == 0)
                        {
                            newQuest.OnStart(Character.id);
                        }
                        if (Steps2.Count > 0)
                        {
                            newQuest.isnew = true;
                            Steps2[0].State = 1;
                        }

                        SMSG_QUESTREMOVE spkt4 = Character.Tag as SMSG_QUESTREMOVE;
                        if (spkt4 != null)
                        {
                            Character.client.Send((byte[])spkt4);
                            Character.Tag = null;
                        }

                        // Send new Quest list
                        SMSG_QUESTINFO spkt3 = new SMSG_QUESTINFO();
                        spkt3.SessionId = Character.id;
                        foreach (QuestBase Quest in Character.QuestObjectives)
                        {
                            List<Saga.Quests.Objectives.ObjectiveList.StepInfo> Steps =
                                QuestBase.GetSteps(Character, Quest.QuestId);
                            spkt3.AddQuest(Quest.QuestId, (byte)Steps.Count);
                            for (int i = 0; i < Steps.Count; i++)
                            {
                                Saga.Quests.Objectives.ObjectiveList.StepInfo currentStep
                                    = Steps[i];
                                uint nextstep = (i + 1 < Steps.Count) ? Steps[i + 1].StepId : 0;
                                spkt3.AddQuestStep(currentStep.StepId, currentStep.State, nextstep, Quest.isnew);
                                if (Quest.isnew == true) Quest.isnew = false;
                            }
                        }
                        Character.client.Send((byte[])spkt3);

                        newQuest.CheckQuest(Character);

                        /*
                        // Send waypoint contruction
                        SMSG_SENDNAVIGATIONPOINT spkt2 = new SMSG_SENDNAVIGATIONPOINT();
                        spkt2.SessionId = Character.id;
                        spkt2.QuestID = QID;
                        foreach (Saga.Quests.Objectives.ObjectiveList.Waypoint waypoint in QuestBase.UserGetWaypoints(Character, QID))
                        {
                            //Predicate to search the npc
                            Predicate<MapObject> IsNpc = delegate(MapObject match)
                            {
                                return match.ModelId == waypoint.NpcId;
                            };

                            MapObject myObject = Character.currentzone.Regiontree.SearchActor(IsNpc, SearchFlags.Npcs);
                            if ( myObject != null )
                            {
                                spkt2.AddPosition(waypoint.NpcId, myObject.Position.x, myObject.Position.y, myObject.Position.z);
                                if ( Point.IsInSightRangeByRadius(Character.Position, myObject.Position))
                                {
                                    BaseMob temp = (BaseMob)myObject;
                                    Common.Actions.UpdateIcon(Character, temp);
                                }
                            }
                        }
                        Character.client.Send((byte[])spkt2);*/
                    }
                    else
                    {
                        Console.WriteLine("Quest not found");
                    }
            }
            else
            {
                Trace.TraceError("Character not found");
            }
        }
    }
}