using Saga.Map;
using Saga.PrimaryTypes;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        public static void NOVICE_IMPROVEDCOMBO(SkillBaseEventArgs bargument)
        {
            int Lvldiff;
            SkillUsageEventArgs.SkillMatrix matrix;
            Actor asource = bargument.Sender as Actor;
            Actor atarget = bargument.Target as Actor;

            if (asource != null && atarget != null && bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                Singleton.Additions.ApplyAddition(arguments.Addition, asource);
                matrix = arguments.GetDefaultSkillMatrix(asource, atarget);
                Lvldiff = arguments.GetCappedLevelDifference(matrix);
                matrix[1, 2] = (int)((double)(matrix[1, 2]) + (double)(20 + 10 * arguments.SkillLevel));
                matrix[4, 3] += (Lvldiff * 120);
                Singleton.Additions.DeapplyAddition(arguments.Addition, asource);

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