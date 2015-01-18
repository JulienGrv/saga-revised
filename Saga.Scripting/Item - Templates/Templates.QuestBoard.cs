using System;
using System.Collections.Generic;
using System.Text;
using Saga.PrimaryTypes;
using Saga.Map;

namespace Saga.Templates
{
    class Questboard : MapItem
    {

        public override void OnClick(Character target)
        {
            Singleton.Quests.OpenQuest(target, this);
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
