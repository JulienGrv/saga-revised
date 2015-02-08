using Saga.PrimaryTypes;
using Saga.Tasks;
using System;
using System.Collections.Generic;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <title>Saga.FindQuestItem</title>
        /// <code>
        /// Saga.FindQuestItem(QuestId, NPCId, ItemId, Ratio);
        /// </code>
        /// <description>
        /// Returns the number of item occurences of the specified item.
        /// </description>
        /// <example>
        /// local ItemCount =
        /// Saga.FindQuestItem(cid, QuestId, 101, 10026, 2630, 1000);
        /// Saga.FindQuestItem(cid, QuestId, 101, 10027, 2630, 1000);
        /// Saga.FindQuestItem(cid, QuestId, 101, 10028, 2630, 1000);
        /// Saga.FindQuestItem(cid, QuestId, 101, 10029, 2630, 1000);
        /// if ItemCount > 6
        ///     Saga.CompleteStep(cid, QuestID, 101);
        /// else
        ///     return -1;
        /// end
        /// return 0;
        /// </example>
        public static int FindQuestItem(uint cid, uint QID, uint stepid, uint npcid, uint itemid, uint rate, uint count, uint substepid)
        {
            try
            {
                substepid--;

                //HELPER VARIABLES
                Character value;

                Predicate<Saga.Quests.Objectives.ObjectiveList.Loot> callback = delegate(Saga.Quests.Objectives.ObjectiveList.Loot objective)
                {
                    return objective.ItemId == itemid &&
                           objective.Npc == npcid &&
                           objective.StepId == stepid;
                };

                Predicate<Saga.Quests.Objectives.ObjectiveList.Loot2> callback2 = delegate(Saga.Quests.Objectives.ObjectiveList.Loot2 objective)
                {
                    return objective.ItemId == itemid &&
                           objective.Quest == QID;
                };

                if (LifeCycle.TryGetById(cid, out value))
                {
                    List<Saga.Quests.Objectives.ObjectiveList.SubStep> Substeps = value.QuestObjectives.Substeps;
                    List<Saga.Quests.Objectives.ObjectiveList.Loot> LootObjectives = value.QuestObjectives.LootObjectives;
                    if (LootObjectives.FindIndex(callback) == -1)
                    {
                        LootObjectives.Add(
                            new Saga.Quests.Objectives.ObjectiveList.Loot(
                                itemid,     //item to get
                                rate,       //rate at which it drops
                                npcid,      //the npc which drops it
                                QID,        //Quest
                                stepid,     //Step of the quest
                                (int)substepid   //Substep of the quest (is needed for number of items)
                            )
                        );

                        if (value.QuestObjectives.IsSubstepAdded(QID, stepid, substepid) == false)
                        {
                            Console.WriteLine("Create substep: {0}", substepid);
                            Substeps.Add(
                                new Saga.Quests.Objectives.ObjectiveList.SubStep(
                                    (int)substepid,  //Substep
                                    false,      //Not completed
                                    0,          //Current count
                                    (int)count,      //Find n items;
                                    QID,        //QuestId
                                    stepid      //StepId
                                )
                            );
                        }
                    }

                    List<Saga.Quests.Objectives.ObjectiveList.Loot2> NonDiscardable = value.QuestObjectives.NonDiscardableItems;
                    if (NonDiscardable.FindIndex(callback2) == -1)
                    {
                        NonDiscardable.Add(
                            new Saga.Quests.Objectives.ObjectiveList.Loot2(
                                itemid,
                                QID,
                                stepid
                            )
                        );
                    }

                    return value.container.GetItemCount(itemid);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }
    }
}