using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void SWORDMAN_COURAGEOUSASSAULT(SkillBaseEventArgs bargument)
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

                    switch (asource.Status.CurrentLp)
                    {
                        case 1:
                            arguments.Damage += (uint)(7 + arguments.SkillLevel);
                            break;

                        case 2:
                            arguments.Damage += (uint)(13 + arguments.SkillLevel * 2);
                            break;

                        case 3:
                            arguments.Damage += (uint)(21 + arguments.SkillLevel * 3);
                            break;

                        case 4:
                            arguments.Damage += (uint)(29 + arguments.SkillLevel * 4);
                            break;

                        case 5:
                            arguments.Damage += (uint)(37 + arguments.SkillLevel * 5);
                            break;
                    }

                    asource.Status.CurrentLp = 0;
                    asource.Status.Updates |= 1;
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