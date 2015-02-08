using Saga.Structures;

namespace Saga.Templates
{
    public class ShopableTrader : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize ShopableTrader class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.ShopConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.TraderConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~ShopableTrader()
        {
        }

        public ShopableTrader()
        {
        }

        #endregion Constructor/Deconstructor
    }
}