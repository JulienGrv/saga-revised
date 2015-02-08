using Saga.Structures;

namespace Saga.Templates
{
    public class SkillMaster : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize skillmaster class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.SkillMasterConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~SkillMaster()
        {
        }

        public SkillMaster()
        {
        }

        #endregion Constructor/Deconstructor
    }
}