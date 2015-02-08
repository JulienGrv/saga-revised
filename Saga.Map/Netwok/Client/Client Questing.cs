using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Quests;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Saga.Map.Client
{
    partial class Client
    {
        private uint QuestBaseID = 0;
        internal Dictionary<uint, uint> AvailablePersonalRequests = new Dictionary<uint, uint>();

        private void OnWantQuestGroupList()
        {
            Console.WriteLine("OnWantQuestBaseGroupList");
        }

        /// <remarks>
        /// Cancels only the QuestBase of it can be canceled.
        /// This is not yet implamented in any QuestBase of kro2, but most likely
        /// in the future there are going to be certain QuestBases that supports that.
        ///
        /// For example a chain of QuestBase events.
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_QUESTCONFIRMCANCEL(CMSG_QUESTCONFIRMCANCEL cpkt)
        {
            if (QuestBaseID > 0)
            {
                try
                {
                    QuestBase myQuestBase = this.character.QuestObjectives[QuestBaseID];
                    if (myQuestBase != null)
                    {
                        //Removes the quest
                        this.character.QuestObjectives[QuestBaseID] = null;

                        //Invalidates all stepinfo
                        QuestBase.InvalidateQuest(myQuestBase, this.character);

                        //Send over new quest list
                        SMSG_QUESTINFO spkt3 = new SMSG_QUESTINFO();
                        spkt3.SessionId = this.character.id;
                        foreach (QuestBase Quest in this.character.QuestObjectives)
                        {
                            List<Saga.Quests.Objectives.ObjectiveList.StepInfo> Steps =
                                QuestBase.GetSteps(this.character, Quest.QuestId);

                            spkt3.AddQuest(Quest.QuestId, (byte)Steps.Count);
                            for (int i = 0; i < Steps.Count; i++)
                            {
                                Saga.Quests.Objectives.ObjectiveList.StepInfo currentStep =
                                    Steps[i];

                                uint nextstep = (i + 1 < Steps.Count) ? Steps[i + 1].StepId : 0;
                                spkt3.AddQuestStep(currentStep.StepId, currentStep.State, nextstep, Quest.isnew);
                            }
                        }
                        this.Send((byte[])spkt3);

                        //Remove all waypoints
                        SMSG_REMOVENAVIGATIONPOINT spkt = new SMSG_REMOVENAVIGATIONPOINT();
                        spkt.QuestID = QuestBaseID;
                        spkt.SessionId = this.character.id;
                        this.Send((byte[])spkt);

                        //Updates all new icons
                        CommonFunctions.RefreshPersonalRequests(this.character);
                        CommonFunctions.UpdateNpcIcons(this.character);
                    }
                }
                finally
                {
                    //Reset our Quest Base Id
                    QuestBaseID = 0;
                }
            }
            else
            {
                QuestBaseID = cpkt.QuestID;
            }
        }

        /// <summary>
        /// Confirms the current quests
        /// </summary>
        /// <remarks>
        /// Starts a new QuestBase, if the QuestBase isn't started before
        /// otherwise we'll just negiotate it.
        ///
        /// QuestBase_START from our lua file should be used for
        /// inventory checks if the QuestBase gives QuestBase items.
        /// So when the QuestBase fails to give the appropiate items
        /// it should have a fail mechanisme.
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_QUESTCONFIRM(CMSG_QUESTCONFIRMED cpkt)
        {
            if (QuestBaseID == 0)
            {
                QuestBaseID = cpkt.QuestID;
                SMSG_QUESTCONFIRM spkt = new SMSG_QUESTCONFIRM();
                spkt.QuestID = cpkt.QuestID;
                spkt.SessionId = this.character.id;
                this.Send((byte[])spkt);
            }
            else
            {
                try
                {
                    QuestBase Quest;
                    if (Singleton.Quests.TryFindQuests(cpkt.QuestID, out Quest) &&
                        Quest.OnStart(this.character.id) > -1)
                    {
                        Quest.CheckQuest(this.character);
                        if (character._target != null)
                            Singleton.Quests.OpenQuest(character, character._target);
                    }
                    else
                    {
                        QuestBase.InvalidateQuest(Quest, this.character);
                        //HAX: Add official quest failed error
                        CommonFunctions.Broadcast(this.character, this.character, "quest failed");
                    }
                }
                finally
                {
                    QuestBaseID = 0;
                }
            }
        }

        /// <summary>
        /// Quest Complete
        /// </summary>
        /// <remarks>
        /// Completes the current QuestBase. This should invoke our lua file, just as origional
        /// kRO2 does. This information is distracted from a leaked QuestBase file posted on some
        /// forum a while ago.
        /// </remarks>
        /// <param name="cpkt"></param>
        private void CM_QUESTCOMPLETE(CMSG_QUESTCOMPLETE cpkt)
        {
            QuestBase myQuestBase = this.character.QuestObjectives[cpkt.QuestID];
            if (myQuestBase != null)
            {
                SMSG_QUESTREMOVE spkt = new SMSG_QUESTREMOVE();
                spkt.QuestID = cpkt.QuestID;
                spkt.SessionId = this.character.id;
                this.character.Tag = spkt;

                if (myQuestBase.OnFinish(this.character.id) > -1)
                {
                    //Removes the quest
                    this.character.QuestObjectives[cpkt.QuestID] = null;

                    //Invalidates all stepinfo
                    QuestBase.InvalidateQuest(myQuestBase, this.character);

                    //Logs into the quest history
                    Singleton.Database.QuestComplete(this.character.ModelId, cpkt.QuestID);

                    //Refresh personal reqeusts table
                    CommonFunctions.RefreshPersonalRequests(this.character);

                    //Refresh all our npc icons
                    CommonFunctions.UpdateNpcIcons(this.character);

                    if (this.character.Tag is SMSG_QUESTREMOVE)
                    {
                        this.character.Tag = null;
                        this.Send((byte[])spkt);
                    }
                }
            }
        }

        private void CM_QUESTITEMSTART(CMSG_QUESTITEMSTART cpkt)
        {
            Rag2Item item = this.character.container[cpkt.Index];
            if (item == null) return;
            if (item.info.quest == 0) return;

            byte result = 1;
            if (Singleton.Database.IsQuestComplete(this.character, item.info.quest))
            {
                result = 1;
            }
            else if (this.character.QuestObjectives[item.info.quest] != null)
            {
                result = 2;
            }
            else
            {
                try
                {
                    QuestBase Quest;
                    if (Singleton.Quests.TryFindQuests(item.info.quest, out Quest) == false || Quest.OnStart(this.character.id) < 0)
                    {
                        result = 1;
                        QuestBase.InvalidateQuest(Quest, this.character);
                    }
                    else
                    {
                        result = 0;
                        int newLength = this.character.container[cpkt.Index].count - 1;
                        if (newLength > 0)
                        {
                            this.character.container[cpkt.Index].count = newLength;
                            SMSG_UPDATEITEM spkt2 = new SMSG_UPDATEITEM();
                            spkt2.Amount = (byte)newLength;
                            spkt2.UpdateReason = 8;
                            spkt2.UpdateType = 4;
                            spkt2.Container = 2;
                            spkt2.SessionId = this.character.id;
                            spkt2.Index = cpkt.Index;
                            this.Send((byte[])spkt2);
                        }
                        else
                        {
                            this.character.container.RemoveAt(cpkt.Index);
                            SMSG_DELETEITEM spkt3 = new SMSG_DELETEITEM();
                            spkt3.UpdateReason = 8;
                            spkt3.Container = 2;
                            spkt3.Index = cpkt.Index;
                            spkt3.SessionId = this.character.id;
                            this.Send((byte[])spkt3);
                        }

                        Quest.CheckQuest(this.character);
                    }
                }
                catch (Exception)
                {
                    Trace.TraceError("Error starting quest: {0}", item.info.quest);
                }
            }

            SMSG_USEQUESTITEM spkt = new SMSG_USEQUESTITEM();
            spkt.Index = cpkt.Index;
            spkt.Result = result;
            spkt.SessionId = this.character.id;
            this.Send((byte[])spkt);
        }

        private void OnQuestRewardChoice()
        {
            Console.WriteLine("OnQuestBaseRewardChoice");
        }
    }
}