﻿using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.Definitions;
using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {

        public static void ENCHANTER_MENTALPOWER(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                arguments.Result = Saga.SkillBaseEventArgs.ResultType.NoDamage;
                arguments.Damage = 0;
                arguments.Failed = false;
                Common.Skills.UpdateAddition(arguments.Target as Actor, arguments.Addition, 600000);
            }
        }

    }
}
