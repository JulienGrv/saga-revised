using Saga.Structures;

namespace Saga.Templates
{
    public class EquipmentTrader : BaseNPC
    {
        #region Base Members

        protected override void Initialize()
        {
            log.WriteLine("Scipting", "Initialize EquipmentTrader class npc");
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.TraderConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion Base Members

        #region Constructor/Deconstructor

        ~EquipmentTrader()
        {
        }

        public EquipmentTrader()
        {
        }

        #endregion Constructor/Deconstructor
    }
}