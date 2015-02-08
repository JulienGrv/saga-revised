using Saga.Map;
using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void RECRUIT_FINALBLOW(SkillBaseEventArgs bargument)
        {
            int Lvldiff;
            SkillUsageEventArgs.SkillMatrix matrix;
            Actor asource = bargument.Sender as Actor;
            Actor atarget = bargument.Target as Actor;

            if (asource != null && atarget != null && bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                //Apply additions and deapply after
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                Singleton.Additions.ApplyAddition(arguments.Addition, asource);
                matrix = arguments.GetDefaultSkillMatrix(asource, atarget);
                Singleton.Additions.DeapplyAddition(arguments.Addition, asource);

                //Calculation
                Lvldiff = arguments.GetCappedLevelDifference(matrix);
                matrix[4, 3] += (Lvldiff * 120);

                //Remove all LP
                asource.Status.CurrentLp = 0;
                asource.Status.Updates |= 1;

                if (arguments.IsMissed(matrix) || arguments.IsBlocked(matrix))
                {
                    return;
                }
                else
                {
                    arguments.CanCheckEquipmentDurabillity = true;
                    arguments.CanCheckWeaponDurabillity = true;
                    arguments.Damage = arguments.GetDamage(matrix);
                    arguments.Damage = arguments.GetDefenseReduction(matrix, arguments.Damage);
                    if (arguments.Damage >= atarget.Status.CurrentHp)
                        asource.Status.CurrentLp = 7;
                    arguments.IsCritical(matrix);
                }
            }
            else
            {
                bargument.Failed = true;
            }
        }
    }
}