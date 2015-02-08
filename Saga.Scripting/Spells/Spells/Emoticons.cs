namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void GENERAL_EMOTICON(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                arguments.Result = SkillBaseEventArgs.ResultType.NoDamage;
                arguments.Damage = 0;
                arguments.CanCheckEquipmentDurabillity = false;
                arguments.CanCheckWeaponDurabillity = false;
                arguments.Failed = false;
            }
        }
    }
}