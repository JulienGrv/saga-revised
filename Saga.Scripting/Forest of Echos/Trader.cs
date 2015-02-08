using Saga.Structures;
using Saga.Templates;

namespace Saga.Scripting.ForestOfEcho
{
    internal class Trader : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            NpcFunction.Create<Saga.Npc.Functions.TraderConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~Trader()
        {
        }

        public Trader()
        {
        }

        #endregion Constructor/Deconstructor
    }
}