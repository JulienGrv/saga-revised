using Saga.Enumarations;
using Saga.Map;
using Saga.Map.Librairies;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Saga.Quests.Scenario
{
    public static partial class QUEST_TABLE
    {
        /// <summary>
        /// Adds a quest step to the quests.
        /// </summary>
        public static void AddStep(uint CID, uint QID, uint StepID)
        {
            Character value;
            if (LifeCycle.TryGetById(CID, out value))
            {
                List<Saga.Quests.Objectives.ObjectiveList.StepInfo> info;
                if (value.QuestObjectives.ScenarioSteps.TryGetValue(QID, out info))
                {
                    Predicate<Saga.Quests.Objectives.ObjectiveList.StepInfo> FindItem = delegate(Saga.Quests.Objectives.ObjectiveList.StepInfo myInfo)
                    {
                        return myInfo.Quest == QID && myInfo.StepId == StepID;
                    };

                    Saga.Quests.Objectives.ObjectiveList.StepInfo stepInfo = info.Find(FindItem);
                    if (stepInfo == null)
                    {
                        info.Add(new Saga.Quests.Objectives.ObjectiveList.StepInfo(1, QID, StepID));
                    }
                }
                else
                {
                    info = new List<Saga.Quests.Objectives.ObjectiveList.StepInfo>();
                    info.Add(new Saga.Quests.Objectives.ObjectiveList.StepInfo(1, QID, StepID));
                    value.QuestObjectives.ScenarioSteps.Add(QID, info);
                }
            }
        }

        public static uint GetCurrentStep2(uint CID, uint QID)
        {
            Character value;
            if (LifeCycle.TryGetById(CID, out value))
            {
                List<Saga.Quests.Objectives.ObjectiveList.StepInfo> info;
                if (value.QuestObjectives.ScenarioSteps.TryGetValue(QID, out info))
                {
                    if (info.Count > 0)
                        return info[0].StepId;
                    else
                        return 0;
                }
                else
                {
                    return 0;
                }
            }

            return 0;
        }

        /// <summary>
        /// Get's the currentscenariostep
        /// </summary>
        public static uint StartEvent(uint cid, uint EventId)
        {
            Console.WriteLine("Start event: {0}", EventId);

            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                SMSG_SCENARIOEVENTBEGIN spkt = new SMSG_SCENARIOEVENTBEGIN();
                spkt.ActorId = value.id;
                spkt.Event = EventId;
                value._Event = EventId;

                Regiontree tree = value.currentzone.Regiontree;
                foreach (Character regionObject in tree.SearchActors(value, SearchFlags.Characters))
                {
                    if (value.currentzone.IsInSightRangeByRadius(value.Position, regionObject.Position))
                    {
                        spkt.SessionId = regionObject.id;
                        regionObject.client.Send((byte[])spkt);
                    }
                }

                while (value._Event > 0)
                {
                    Thread.Sleep(0);
                }
            }
            return 0;
        }

        public static void CompleteQuest(uint cid, uint questid)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                QuestBase scenarioQuest = value.QuestObjectives.Quests[3];
                if (scenarioQuest != null &&
                    scenarioQuest.QuestId == questid)
                {
                    Predicate<Saga.Quests.Objectives.ObjectiveList.Position> FindPosition = delegate(Saga.Quests.Objectives.ObjectiveList.Position point)
                    {
                        return point.Quest == questid;
                    };

                    lock (value.QuestObjectives.ScenarioPosition)
                    {
                        value.QuestObjectives.ScenarioPosition.RemoveAll(FindPosition);
                    }
                    value.QuestObjectives.ScenarioSteps.Remove(questid);
                    value.QuestObjectives.Quests[3] = null;

                    SMSG_SCENARIOCOMPLETE spkt = new SMSG_SCENARIOCOMPLETE();
                    spkt.Scenario = questid;
                    spkt.SessionId = value.id;
                    value.client.Send((byte[])spkt);
                    Thread.Sleep(3);
                    scenarioQuest.OnFinish(value.id);
                }
            }
        }

        public static void StartQuest(uint cid, uint questid)
        {
            Character value;
            uint currentScenarioQuest = 0;

            if (LifeCycle.TryGetById(cid, out value))
            {
                QuestBase scenarioQuest = value.QuestObjectives.Quests[3];
                currentScenarioQuest = (scenarioQuest != null) ? scenarioQuest.QuestId : 0;
                if (currentScenarioQuest != questid)
                {
                    QuestBase newQuest;
                    if (Singleton.Quests.TryFindScenarioQuest(questid, out newQuest))
                    {
                        value.QuestObjectives.Quests[3] = newQuest;
                        newQuest.isnew = true;
                        newQuest.OnStart(value.id);
                        uint quest = (value.QuestObjectives.Quests[3] != null) ? value.QuestObjectives.Quests[3].QuestId : 0;
                        bool isnew = (value.QuestObjectives.Quests[3] != null) ? value.QuestObjectives.Quests[3].isnew : false;
                        if (quest > 0 && isnew)
                        {
                            SMSG_INITIALIZESCENARIO spkt = new SMSG_INITIALIZESCENARIO();
                            spkt.Scenario1 = quest;
                            List<Saga.Quests.Objectives.ObjectiveList.StepInfo> list;
                            if (value.QuestObjectives.ScenarioSteps.TryGetValue(quest, out list))
                            {
                                if (list.Count > 0)
                                {
                                    spkt.Scenario2 = list[0].StepId;
                                    spkt.StepStatus = 1;
                                    spkt.Enabled = 0;
                                }
                            }
                            spkt.SessionId = value.id;
                            value.client.Send((byte[])spkt);
                            value.QuestObjectives.Quests[3].isnew = true;
                        }

                        newQuest.OnCheckQuest(value.id);
                    }
                }
            }
        }

        public static void StepComplete2(uint cid, uint questid, uint stepid)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                SMSG_SCENARIOSTEPCOMPLETE spkt = new SMSG_SCENARIOSTEPCOMPLETE();
                List<Saga.Quests.Objectives.ObjectiveList.StepInfo> list;
                if (value.QuestObjectives.ScenarioSteps.TryGetValue(questid, out list))
                {
                    Predicate<Saga.Quests.Objectives.ObjectiveList.StepInfo> FindItem = delegate(Saga.Quests.Objectives.ObjectiveList.StepInfo myInfo)
                    {
                        return myInfo.Quest == questid && myInfo.StepId == stepid;
                    };

                    Predicate<Saga.Quests.Objectives.ObjectiveList.Position> FindPosition = delegate(Saga.Quests.Objectives.ObjectiveList.Position point)
                    {
                        return point.Quest == questid && point.StepId == stepid;
                    };

                    if (list.Count > 0)
                    {
                        spkt.Step = list[0].StepId;
                        list.RemoveAll(FindItem);
                    }
                    if (list.Count > 0)
                    {
                        spkt.NextStep = list[0].StepId;
                    }

                    //Removes all position objectives
                    value.QuestObjectives.ScenarioPosition.RemoveAll(FindPosition);
                }

                spkt.SessionId = value.id;
                value.client.Send((byte[])spkt);
            }
        }

        public static float FindPosition(uint cid, uint questid, float x, float y, float z, byte map)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                Predicate<Saga.Quests.Objectives.ObjectiveList.Position> callback = delegate(Saga.Quests.Objectives.ObjectiveList.Position objective)
                {
                    return objective.point == new Point(x, y, z) &&
                           objective.mapid == map &&
                           objective.Quest == questid;
                };

                if (value.QuestObjectives.ScenarioPosition.FindIndex(callback) == -1)
                {
                    lock (value.QuestObjectives.ScenarioPosition)
                    {
                        value.QuestObjectives.ScenarioPosition.Add(
                            new Saga.Quests.Objectives.ObjectiveList.Position(
                            x, y, z, map, 1000, questid, 0, 0
                        ));
                    }
                }
                if (map == value.map)
                    return (float)Vector.GetDistance3D(value.Position, new Point(x, y, z));
                else
                    return float.MaxValue;
            }

            return float.MaxValue;
        }
    }
}