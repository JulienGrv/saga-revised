using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void RECRUIT_PREDATORFOCUS(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                arguments.Result = SkillBaseEventArgs.ResultType.NoDamage;
                arguments.Damage = 0;
                Common.Skills.UpdateAddition(arguments.Target as Actor, arguments.Addition, 900000);
            }
        }
    }
}