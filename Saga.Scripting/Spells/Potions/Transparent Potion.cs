using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void GENERAL_TRANSPARANTPOTION(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.ItemUsage)
            {
                ItemSkillUsageEventArgs arguments = (ItemSkillUsageEventArgs)bargument;
                arguments.Result = Saga.SkillBaseEventArgs.ResultType.Item;
                arguments.Damage = 0;
                arguments.Failed = false;
                Common.Skills.CreateAddition(arguments.Sender as Actor, arguments.Addition, 30000);
            }
        }
    }
}