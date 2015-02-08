using Saga.Structures;

namespace Saga.Templates
{
    public class Warper : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize warper class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.WarperConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~Warper()
        {
        }

        public Warper()
        {
        }

        #endregion Constructor/Deconstructor
    }
}