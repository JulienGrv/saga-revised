using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void RECRUIT_AIMINGSHOT(SkillBaseEventArgs bargument)
        {
            int Lvldiff;
            SkillUsageEventArgs.SkillMatrix matrix;
            Actor asource = bargument.Sender as Actor;
            Actor atarget = bargument.Target as Actor;

            if (asource != null && atarget != null && bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                matrix = arguments.GetDefaultSkillMatrix(asource, atarget);
                Lvldiff = arguments.GetCappedLevelDifference(matrix);
                matrix[2, 3] = 1200 + (int)((double)matrix[2, 3] * (double)((double)0.5 - ((double)0.05 * (double)arguments.SkillLevel)));
                matrix[4, 3] += (Lvldiff * 120);

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