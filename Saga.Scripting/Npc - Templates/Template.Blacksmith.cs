using Saga.Structures;

namespace Saga.Templates
{
    public class BlackSmith : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize BlackSmith class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.BlackSmith>(this);
            NpcFunction.Create<Saga.Npc.Functions.ShopConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.TraderConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~BlackSmith()
        {
        }

        public BlackSmith()
        {
        }

        #endregion Constructor/Deconstructor
    }
}