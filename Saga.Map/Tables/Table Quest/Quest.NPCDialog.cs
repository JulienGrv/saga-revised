using Saga.Enumarations;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Tasks;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <title>Saga.GeneralDialog</title>
        /// <code>
        /// Saga.GeneralDialog(cid, dialogid);
        /// </code>
        /// <description>
        /// Outputs a npc's dialog.
        /// </description>
        public static void GeneralDialog(uint cid, uint dialog)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value) && value.Target != null)
            {
                SMSG_QUESTNPCNOTE spkt = new SMSG_QUESTNPCNOTE();
                spkt.SessionId = value.id;
                spkt.QuestID = 396;
                spkt.StepId = 1;
                value.client.Send((byte[])spkt);

                Common.Actions.OpenMenu(
                    value, value.Target,
                    dialog,
                    DialogType.AcceptPersonalRequest,
                    new byte[] { }
                 );
            }
        }
    }
}