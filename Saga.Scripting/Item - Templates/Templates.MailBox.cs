using System;
using System.Collections.Generic;
using System.Text;
using Saga.PrimaryTypes;

namespace Saga.Templates
{
    class Mailbox : MapItem
    {

        public override void OnClick(Character target)
        {
            OnCheckMail(target);
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
