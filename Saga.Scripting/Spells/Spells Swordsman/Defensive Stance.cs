using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void SWORDMAN_DEFENSIVESTANCE(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.SkillToggle)
            {
                SkillToggleEventArgs arguments = (SkillToggleEventArgs)bargument;
                Actor starget = arguments.Target as Actor;
                if (Common.Skills.HasAddition(starget, arguments.Addition))
                    Common.Skills.DeleteStaticAddition(starget, arguments.Addition);
                else
                    Common.Skills.CreateAddition(starget, arguments.Addition);
            }
        }
    }
}