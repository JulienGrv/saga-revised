using Saga.Enumarations;
using Saga.Structures;
using Saga.Templates;

namespace Saga.Actors
{
    internal class Wayendi : BaseNPC
    {
        protected override void Initialize()
        {
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<BlacksmithFunction>(this);
        }

        private class BlacksmithFunction : Saga.Npc.Functions.BlackSmith
        {
            protected override void OnRegister(Saga.Templates.BaseNPC npc)
            {
                RegisterDialog(npc, DialogType.Smith, new FunctionCallback(OnBlackSmithMenu));
                RegisterDialog(npc, DialogType.Smith, 50, new FunctionCallback(OnRepairEquipment));
                RegisterDialog(npc, DialogType.Smith, 53, new FunctionCallback(OnUpgradeWeapon));
                RegisterDialog(npc, DialogType.Smith, 55, new FunctionCallback(OnChangeWeaponSuffix));
            }

            protected override void OnBlackSmithMenu(BaseNPC npc, Saga.PrimaryTypes.Character target)
            {
                Common.Actions.OpenSubmenu(target, npc,
                  _BlackSmithMenu,        //Dialog script to show
                  DialogType.Smith,       //Button function
                  50,                     //Repair
                  53,                     //Upgrade
                  55                      //Change Suffix
                );
            }
        }
    }
}