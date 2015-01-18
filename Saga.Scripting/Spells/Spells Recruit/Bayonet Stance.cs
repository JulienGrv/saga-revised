using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.Definitions;
using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {

        public static void RECRUIT_BAYONETSTANCE(SkillBaseEventArgs bargument)
        {
            Console.WriteLine("Bayonet stance");
            if (bargument.Context == Saga.Enumarations.SkillContext.SkillToggle)            
            {
                bargument.Failed = false;
                SkillToggleEventArgs arguments = (SkillToggleEventArgs)bargument;
                Actor starget = arguments.Target as Actor; 
                arguments.Toggle(starget, arguments.SpellInfo.addition);
            }
        }



    }
}
