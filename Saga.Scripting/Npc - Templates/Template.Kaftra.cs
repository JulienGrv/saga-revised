using Saga.Structures;

namespace Saga.Templates
{
    public class Kaftra : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize Kaftra class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.KaftraConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~Kaftra()
        {
        }

        public Kaftra()
        {
        }

        #endregion Constructor/Deconstructor
    }
}