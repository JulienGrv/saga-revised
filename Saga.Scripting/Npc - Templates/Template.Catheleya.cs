using Saga.Structures;

namespace Saga.Templates
{
    public class Catheleya : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize Catheleya class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.CatheleyaConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~Catheleya()
        {
        }

        public Catheleya()
        {
        }

        #endregion Constructor/Deconstructor
    }
}