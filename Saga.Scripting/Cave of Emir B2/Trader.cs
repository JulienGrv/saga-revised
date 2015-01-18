using System;
using System.Collections.Generic;
using System.Text;
using Saga.Templates;
using Saga.PrimaryTypes;
using Saga.Npc.Functions;
using Saga.Structures;

namespace Saga.Scripting.CaveOfEmir
{
    class Lentz : BaseNPC
    {

        #region Base Members

        protected override void Initialize()
        {
            NpcFunction.Create<Saga.Npc.Functions.EverydayConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.BlackSmith>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion

        #region Constructor/Deconstructor

        ~Lentz() { }
        public Lentz() { }

        #endregion


    }


}
