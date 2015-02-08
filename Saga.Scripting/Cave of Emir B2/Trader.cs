using Saga.Structures;
using Saga.Templates;

namespace Saga.Scripting.CaveOfEmir
{
    internal class Lentz : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.BlackSmith>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~Lentz()
        {
        }

        public Lentz()
        {
        }

        #endregion Constructor/Deconstructor
    }
}