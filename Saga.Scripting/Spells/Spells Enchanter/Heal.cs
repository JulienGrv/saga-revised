using Saga.Map;
using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void ENCHANTER_HEAL(SkillBaseEventArgs bargument)
        {
            Actor asource = bargument.Sender as Actor;
            Actor atarget = bargument.Target as Actor;
            if (bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                Singleton.Additions.ApplyAddition(arguments.Addition, atarget);
                arguments.Result = Saga.SkillBaseEventArgs.ResultType.Heal;
                arguments.Damage = 0;
                arguments.Failed = false;
                Singleton.Additions.DeapplyAddition(arguments.Addition, atarget);
            }
        }
    }
}