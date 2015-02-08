using Saga.PrimaryTypes;

namespace Saga.Templates
{
    internal class InteractiveItem : MapItem
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