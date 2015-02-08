using Saga.Structures;

namespace Saga.Templates
{
    public class Shop : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize shop class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.ShopConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~Shop()
        {
        }

        public Shop()
        {
        }

        #endregion Constructor/Deconstructor
    }
}