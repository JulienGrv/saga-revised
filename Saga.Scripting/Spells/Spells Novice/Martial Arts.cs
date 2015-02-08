using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void NOVICE_MARTIALARTS(SkillBaseEventArgs bargument)
        {
            SkillToggleEventArgs arguments = (SkillToggleEventArgs)bargument;
            Actor starget = arguments.Target as Actor;
            Common.Skills.CreateAddition(starget, arguments.Addition);
        }
    }
}