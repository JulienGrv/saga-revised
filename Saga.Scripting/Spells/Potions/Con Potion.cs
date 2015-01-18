﻿using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.Definitions;
using Saga.PrimaryTypes;
using Saga.Scripting.Interfaces;
using Saga.Factory;

namespace Saga.Skills
{
    static partial class Spelltable
    {

        public static void GENERAL_STATPOTION(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.ItemUsage)
            {
                ItemSkillUsageEventArgs arguments = (ItemSkillUsageEventArgs)bargument;
                arguments.Result = Saga.SkillBaseEventArgs.ResultType.Item;
                arguments.Damage = 0;
                arguments.Failed = false;
                Common.Skills.CreateAddition(arguments.Sender as Actor, arguments.Addition, 900000);
            }
        }

        public static void GENERAL_STATPOTION2(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.ItemUsage)
            {
                ItemSkillUsageEventArgs arguments = (ItemSkillUsageEventArgs)bargument;
                arguments.Result = Saga.SkillBaseEventArgs.ResultType.Item;
                arguments.Damage = 0;
                arguments.Failed = false;
                Common.Skills.CreateAddition(arguments.Sender as Actor, arguments.Addition, 1800000);
            }
        }

    }
}
