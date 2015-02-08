using Saga.Map;
using Saga.PrimaryTypes;

namespace Saga.Templates
{
    internal class Questboard : MapItem
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