using Saga.Enumarations;
using Saga.Map;
using Saga.PrimaryTypes;
using Saga.Quests;
using Saga.Structures;
using Saga.Templates;

namespace Saga.Npc.Functions
{
    /// <summary>
    /// This is a quest conversation. Include this in a npc to show a
    /// quest dialog and make it sensitive for quests. Every npc should
    /// include this to embed seamlessly with the quests subsystem.
    /// </summary>
    /// <remarks>
    /// This class can't be inherited.
    /// </remarks>
    public sealed class QuestConversation : NpcFunction
    {
        #region Protected Methods

        /// <summary>
        /// Registers all the menu's and buttons on the npc.
        /// </summary>
        /// <param name="npc">Npc who to register with</param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.PersonalQuest, new FunctionCallback(OnPersonalQuest));
            RegisterDialog(npc, DialogType.OfficialQuest, new FunctionCallback(OnOfficialQuest));
            RegisterDialog(npc, DialogType.ScenarioQuest, new FunctionCallback(OnScenarioQuest));
            RegisterDialog(npc, DialogType.AcceptPersonalRequest, new FunctionCallback(OnAcceptPersonalRequest));
        }

        /// <summary>
        /// Checks if certain buttons should be visible.
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="dialog"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected internal override bool OnCheckDialogIsVisible(BaseNPC npc, DialogType dialog, Character target)
        {
            bool PersonalQuest = false;
            bool OfficialQuest = false;
            bool ScenarioQuest = false;

            /*
            System.Predicate<Saga.Quests.Objectives.ObjectiveList.Activation> pred = delegate(Saga.Quests.Objectives.ObjectiveList.Activation act)
            {
                //act.Quest
                if (act.NpcId != npc.ModelId) return false;
                QuestBase baseQ = target.QuestObjectives[act.Quest];
                if( baseQ != null )
                    switch (baseQ.questtype)
                    {
                        case 1: OfficialQuest = true; break;
                        case 2: PersonalQuest = true; break;
                        case 3: ScenarioQuest = true;break;
                    }
                return true;
            };*/

            System.Predicate<Saga.Quests.Objectives.ObjectiveList.Waypoint> FindActivatedQuests = delegate(Saga.Quests.Objectives.ObjectiveList.Waypoint objective)
            {
                if (objective.NpcId != npc.ModelId) return false;

                System.Predicate<Saga.Quests.Objectives.ObjectiveList.SubStep> FindSubstep = delegate(Saga.Quests.Objectives.ObjectiveList.SubStep substepobjective)
                {
                    return substepobjective.Quest == objective.Quest
                        && substepobjective.StepId == objective.StepId
                        && substepobjective.SubStepId == objective.SubStepId;
                };

                if (target.QuestObjectives.Substeps.Find(FindSubstep).Completed == false)
                {
                    QuestBase baseQ = target.QuestObjectives[objective.Quest];
                    if (baseQ != null)
                        switch (baseQ.questtype)
                        {
                            case 1: OfficialQuest = true; break;
                            case 2: PersonalQuest = true; break;
                            case 3: ScenarioQuest = true; break;
                        }
                    return true;
                }
                else
                {
                    return false;
                }
            };

            uint val = 0;
            //target.QuestObjectives.ActivatedNpc.Find(pred);
            target.QuestObjectives.GuidancePoints.Find(FindActivatedQuests);
            switch (dialog)
            {
                case DialogType.PersonalQuest:
                    return PersonalQuest;

                case DialogType.OfficialQuest:
                    return OfficialQuest;

                case DialogType.ScenarioQuest:
                    return ScenarioQuest;

                case DialogType.AcceptPersonalRequest:
                    return target.client.AvailablePersonalRequests.TryGetValue(npc.ModelId, out val);

                default:
                    return true;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Occurs on a personal quest.
        /// </summary>
        /// <param name="npc">Npc who calls the event</param>
        /// <param name="target">Character who requires interaction</param>
        private void OnPersonalQuest(BaseNPC npc, Character target)
        {
            QuestBase PersonalQuest = target.QuestObjectives.PersonalQuest;
            if (PersonalQuest != null)
            {
                QuestBase.UserTalktoTarget(npc.ModelId, target, PersonalQuest);
            }
        }

        /// <summary>
        /// Occurs on a scenario quest.
        /// </summary>
        /// <param name="npc">Npc who calls the event</param>
        /// <param name="target">Character who requires interaction</param>
        private void OnScenarioQuest(BaseNPC npc, Character target)
        {
            QuestBase ScenarioQuest = target.QuestObjectives.ScenarioQuest;
            if (ScenarioQuest != null)
            {
                QuestBase.UserTalktoTarget(npc.ModelId, target, ScenarioQuest);
            }
        }

        /// <summary>
        /// Occurs on a official quest.
        /// </summary>
        /// <param name="npc">Npc who calls the event</param>
        /// <param name="target">Character who requires interaction</param>
        private void OnOfficialQuest(BaseNPC npc, Character target)
        {
            QuestBase OfficialQuest = target.QuestObjectives.OfficialQuest;
            if (OfficialQuest != null)
            {
                QuestBase.UserTalktoTarget(npc.ModelId, target, OfficialQuest);
            }
        }

        /// <summary>
        /// Occurs on a accepting a personal quest.
        /// </summary>
        /// <param name="npc">Npc who calls the event</param>
        /// <param name="target">Character who requires interaction</param>
        private void OnAcceptPersonalRequest(BaseNPC npc, Character target)
        {
            uint quest;
            if (target.client.AvailablePersonalRequests.TryGetValue(npc.ModelId, out quest))
            {
                Singleton.Quests.TryFindQuests(quest, out target.client.pendingquest);
                target.client.pendingquest.OnVerify(target.id);
            }
        }

        #endregion Private Methods
    }
}