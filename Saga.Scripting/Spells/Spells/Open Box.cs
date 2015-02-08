using Saga.PrimaryTypes;
using Saga.Scripting.Interfaces;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void GENERAL_OPENBOX(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;

                if (arguments.Target is OpenBox)
                {
                    OpenBox current = arguments.Target as OpenBox;
                    arguments.Result = Saga.SkillBaseEventArgs.ResultType.NoDamage;
                    arguments.Damage = 0;
                    arguments.Failed = false;
                    current.OnOpenBox(arguments.Sender as Character);
                }
            }
        }
    }
}