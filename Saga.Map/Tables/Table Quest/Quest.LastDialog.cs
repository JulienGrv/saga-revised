using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.Definitions;
using Saga.Tasks;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {

        /// <title>Saga.GetLastDialogId</title>
        /// <code>
        /// Saga.GetLastDialogId(cid, Zeny);
        /// </code>
        /// <description>
        /// Returns the dialog id of the last spoken dialog.
        /// </description>
        public static uint GetNPCLastConversation(uint CID)
        {
            /*
            Character value;
            if (LifeCycle.TryGetById(CID, out value))
            {
                return value.ActiveDialog;
            }
            return 0;
            */
        }
    }
}
