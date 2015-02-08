using Saga.PrimaryTypes;

namespace Saga.Templates
{
    internal class KaftraMailbox : MapItem
    {
        public override void OnClick(Character target)
        {
            OnCheckQuest(target);
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