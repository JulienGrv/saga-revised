using Saga.Structures;

namespace Saga.Templates
{
    public class Guide : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize Guide class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.LocationConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~Guide()
        {
        }

        public Guide()
        {
        }

        #endregion Constructor/Deconstructor
    }
}