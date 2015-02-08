using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void THIEF_DISGUISE(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                Actor starget = arguments.Sender as Actor;
                if (starget is Actor)
                {
                    Common.Skills.UpdateAddition(starget, arguments.Addition, 240000);
                }
                else
                {
                    arguments.Failed = true;
                }
            }
        }
    }
}