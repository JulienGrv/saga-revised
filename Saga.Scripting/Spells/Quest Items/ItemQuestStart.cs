using System;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void GENERAL_ITEMQUESTSTART(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.ItemUsage)
            {
                ItemSkillUsageEventArgs arguments = (ItemSkillUsageEventArgs)bargument;
                arguments.Result = Saga.SkillBaseEventArgs.ResultType.Item;
                arguments.Damage = 0;
                Console.WriteLine("Start using quest: {0}", arguments.ItemInfo.quest);
            }
        }
    }
}