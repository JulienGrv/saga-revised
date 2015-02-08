using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void RECRUIT_POLLEOSHOT(SkillBaseEventArgs bargument)
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
                matrix[4, 3] += (Lvldiff * 120);

                switch (asource.Status.CurrentLp)
                {
                    case 1:
                        matrix.matrix[1, 2] += (9 + 1 * arguments.SkillLevel);
                        break;

                    case 2:
                        matrix.matrix[1, 2] += (19 + 2 * arguments.SkillLevel);
                        break;

                    case 3:
                        matrix.matrix[1, 2] += (30 + 3 * arguments.SkillLevel);
                        break;

                    case 4:
                        matrix.matrix[1, 2] += (42 + 4 * arguments.SkillLevel);
                        break;

                    case 5:
                        matrix.matrix[1, 2] += (55 + 5 * arguments.SkillLevel);
                        break;
                }

                asource.Status.CurrentLp = 0;
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