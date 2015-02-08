using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void THIEF_SHADOWSTEP(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                arguments.Result = Saga.SkillBaseEventArgs.ResultType.NoDamage;
                arguments.Damage = 0;
                arguments.Failed = false;
                Common.Skills.UpdateAddition(bargument.Sender as Actor, arguments.Addition, 300000);
                //Do lp effect
                Common.Skills.DoAddition(bargument.Sender as Actor, bargument.Target as Actor, 1003301);
            }
        }
    }
}