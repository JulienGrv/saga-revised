using System;
using System.Collections.Generic;
using System.Text;
using Saga.Templates;
using Saga.PrimaryTypes;
using Saga.Npc.Functions;
using Saga.Structures;

namespace Saga.Scripting.ForestOfEcho
{
    class Trader : BaseNPC
    {

        #region Base Members

        protected override void Initialize()
        {
            NpcFunction.Create<Saga.Npc.Functions.TraderConversation>(this);
            NpcFunction.Create<Saga.Npc.Functions.QuestConversation>(this);
        }

        #endregion

        #region Constructor/Deconstructor

        ~Trader() { }
        public Trader() { }

        #endregion


    }


}
