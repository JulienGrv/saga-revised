using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;
using System.Collections.Generic;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <title>Saga.StepComplete</title>
        /// <code>
        /// Saga.StepComplete(cid, QuestID, StepId);
        /// </code>
        /// <description>
        /// Completes a quest
        /// </description>
        /// <example>
        /// function QUEST_STEP_2(cid)
        ///	    -- Talk to mischa
        ///     local NPCIndex = 1000;
        ///     local ret = Saga.GetNPCIndex(cid);
        ///
        ///     Saga.AddWaypoint(cid, QuestID, NPCIndex, -12092, -6490, -8284, 1);
        ///     if ret == NPCIndex then
        ///         Saga.StepComplete(cid, QuestID, 102);
        ///     else
        ///         return  -1;
        ///     end
        ///
        ///     Saga.ClearWaypoints(cid, QuestID);
        ///     return 0;
        /// end
        /// </example>
        public static void StepComplete(uint cid, uint QID, uint StepID)
        {
            Character characterTarget;
            List<Saga.Quests.Objectives.ObjectiveList.StepInfo> Steps;
            if (LifeCycle.TryGetById(cid, out characterTarget))
            {
                uint stepIndexies = 0;
                Steps = QuestBase.GetSteps(characterTarget, QID);
                IEnumerator<Saga.Quests.Objectives.ObjectiveList.StepInfo> i = Steps.GetEnumerator();
                while (i.MoveNext())
                    if (i.Current.StepId == StepID)
                    {
                        i.Current.State = 2;
                        if (i.MoveNext())
                            if (i.Current.State == 0)
                            {
                                i.Current.State = 1;
                                stepIndexies = i.Current.StepId;
                            }
                        break;
                    }

                QuestBase.InvalidateStep(QID, StepID, characterTarget);
                if (characterTarget.QuestObjectives[QID] != null)
                {
                    SMSG_QUESTSTEPUPDATE spkt = new SMSG_QUESTSTEPUPDATE();
                    spkt.QuestID = QID;
                    spkt.StepID = StepID;
                    spkt.NextStepID = stepIndexies;
                    spkt.Progress = 2;
                    spkt.SessionId = characterTarget.id;
                    characterTarget.client.Send((byte[])spkt);
                    CommonFunctions.UpdateNpcIcons(characterTarget);
                }
            }

            /*
             *
            QuestBase myQuest;
            Character characterTarget;

            if (LifeCycle.TryGetById(cid, out characterTarget))
            if (characterTarget.client.questlist.TryGetValue(QID, out myQuest))
            {
                    uint stepIndexies = 0;
                    IEnumerator<StepInfo> i = myQuest.listofsteps.GetEnumerator();
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

                    SMSG_QUESTSTEPUPDATE spkt = new SMSG_QUESTSTEPUPDATE();
                    spkt.QuestID = QID;
                    spkt.StepID = StepID;
                    spkt.NextStepID = stepIndexies;
                    spkt.Progress = 2;
                    spkt.SessionId = characterTarget.id;
                    characterTarget.client.Send((byte[])spkt);
                    CommonFunctions.UpdateNpcIcons(characterTarget);
             }
             else if (characterTarget.client.pendingquest != null && characterTarget.client.pendingquest.QuestId == QID)
             {
                    uint stepIndexies = 0;
                    IEnumerator<StepInfo> i = characterTarget.client.pendingquest.listofsteps.GetEnumerator();
                    while (i.MoveNext())
                    {
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
                    }
                }

            //// Deactive
            ///  All inactivated item mobs here

            List<LootObjective> d = new List<LootObjective>();
            List<LootObjective> e = QuestBase.GetLootObjectives(QID, characterTarget);
            foreach (LootObjective c in e)
            {
                if (c.StepID == StepID)
                {
                    d.Add(c);
                }
            }
            foreach (LootObjective c in d)
            {
                e.Remove(c);
            }

            foreach (MapObject c in characterTarget.ObjectsInSightrange)
            {
                if (c.id >= ActorManager.PlayerBorder &&
                       c.id < ActorManager.MapItemBorder)
                {
                    Predicate<LootObjective> ActionObjectDeactivate = delegate(LootObjective f)
                    {
                        return f.NPCType == c.ModelId;
                    };

                    if (d.Exists(ActionObjectDeactivate) == true)
                    {
                        MapItem item = c as MapItem;
                        SMSG_ITEMUPDATE spkt = new SMSG_ITEMUPDATE();
                        spkt.ActorID = c.id;
                        spkt.Active1 = 0;
                        spkt.Active = item.IsHighlighted(characterTarget);
                        spkt.Active2 = item.IsInteractable(characterTarget);
                        spkt.SessionId = characterTarget.id;
                        characterTarget.client.Send((byte[])spkt);
                    }
                }
            }

            */
        }
    }
}