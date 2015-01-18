using System;
using System.Collections.Generic;
using System.Text;
using Saga.Quests;
using Saga.PrimaryTypes;
using Saga.Scripting.Interfaces;

namespace Saga.Templates
{

    class InteractiveItem : MapItem
    {

        public override void OnClick(Character target)
        {
            //OnCheckMail(target);
        }

        public override byte IsHighlighted(Character target)
        {
            return 1;
        }

        public override byte IsInteractable(Character target)
        {
            return 1;
        }
    }
}
