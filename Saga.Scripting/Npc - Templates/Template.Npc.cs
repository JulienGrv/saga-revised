using Saga.Structures;

namespace Saga.Templates
{
    public class Npc : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize Npc class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~Npc()
        {
        }

        public Npc()
        {
        }

        #endregion Constructor/Deconstructor
    }
}