using Saga.Structures;

namespace Saga.Templates
{
    public class Auctioneer : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize Auctioneer class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.AuctionConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~Auctioneer()
        {
        }

        public Auctioneer()
        {
        }

        #endregion Constructor/Deconstructor
    }
}