using Saga.Enumarations;
using Saga.Map;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;
using System.Collections.Generic;

namespace Saga.Spells
{
    public static partial class Additions
    {
        //539        | P. Short Critical Rate         		    | True                 | True
        //540        | P. Ranged Critical Rate       		    | True                 | True
        //541        | Magical Critical Rate                    | True                 | True
        //542        | P. Short Critical Rate                   | True                 | False
        //543        | P. Ranged Critical Rate                  | True                 | False

        //500        | Min. P. Short Attack        			    | True                 | True
        //501        | Min. P. Ranged Attack                    | True                 | True
        //502        | Min. Magic Attack                        | True                 | True
        //509        | Max. P. Short Attack                     | True                 | True
        //510        | Max. P. Ranged Attack                    | True                 | True
        //511        | Max. Magic Attack	                    | True                 | True
        public static void MINPATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 500 - Min. P. Short Attack;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MinPAttack += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MinPAttack -= value;
        }

        public static void MINRATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 501 - Min. P. R. Attack;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MinRAttack += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MinRAttack -= value;
        }

        public static void MINMATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 501 - Min. P. R. Attack;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MinMAttack += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MinMAttack -= value;
        }

        public static void MaxPATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 509 - Max. P. Short Attack;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MaxPAttack += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MaxPAttack -= value;
        }

        public static void MaxRATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 510 - Max. P. R. Attack;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MaxRAttack += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MaxRAttack -= value;
        }

        public static void MaxMATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 511 - Max. P. R. Attack;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MaxMAttack += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MaxMAttack -= value;
        }

        //518        | Min. Short Weapon Damage                 | True                 | False
        //519        | Min. Ranged Weapon Damage     		    | True                 | False
        //520        | Min. Magic Weapon Damage          		| True                 | False
        //521        | Max. Short Weapon Damage                 | True                 | False
        //522        | Max. Ranged Weapon Damage                | True                 | False
        //523        | Max. Magic Weapon Damage                 | True                 | False
        public static void MINWPATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 518 - Min. Short Weapon Damage  ;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MinWPAttack += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MinWPAttack -= (ushort)value;
        }

        public static void MINWRATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 519 - Min. Ranged Weapon Damage;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MinWRAttack += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MinWRAttack -= (ushort)value;
        }

        public static void MINWMATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 520 - Min. Ranged Weapon Damage;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MinWMAttack += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MinWMAttack -= (ushort)value;
        }

        public static void MaxWPATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 521 - Max. Short Weapon Damage;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MaxWPAttack += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MaxWPAttack -= (ushort)value;
        }

        public static void MaxWRATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 522 - Max. Ranged Weapon Damage ;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MaxWRAttack += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MaxWRAttack -= (ushort)value;
        }

        public static void MaxWMATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 523 - Max. Magic Weapon Damage;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MaxWMAttack += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MaxWMAttack -= (ushort)value;
        }

        //524        | P. Short Attack           			    | True                 | True
        //525        | P. Ranged Attack                		    | True                 | True
        //526        | Magic Attack           			        | True                 | True
        public static void PATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 500 - Min. P. Short Attack;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
            {
                status.MinPAttack += value;
                status.MaxPAttack += value;
            }
            else if (aval.context == AdditionContext.Deapplied)
            {
                status.MinPAttack -= value;
                status.MaxPAttack -= value;
            }
        }

        public static void RATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 501 - Min. P. R. Attack;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
            {
                status.MinRAttack += value;
                status.MaxRAttack += value;
            }
            else if (aval.context == AdditionContext.Deapplied)
            {
                status.MinRAttack -= value;
                status.MaxRAttack -= value;
            }
        }

        public static void MATTACK(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 501 - Min. P. R. Attack;

            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
            {
                status.MaxMAttack += value;
                status.MaxMAttack += value;
            }
            else if (aval.context == AdditionContext.Deapplied)
            {
                status.MinMAttack -= value;
                status.MaxMAttack -= value;
            }
        }

        //533        | P. Short Hitrate        			        | True                 | True
        //534        | P. Ranged Hitrate          			    | True                 | True
        //535        | Magic Hitrate          			        | True                 | True
        public static void PHITRATE(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 533 -  P. Short Hitrate ;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.BasePHitrate += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.BasePHitrate -= value;
        }

        public static void RHITRATE(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 534 -  P. Ranged Hitrate ;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.BaseRHitrate += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.BaseRHitrate -= value;
        }

        public static void MHITRATE(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 535 -  Magic Hitrate  ;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.BaseMHitrate += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.BaseMHitrate -= value;
        }

        //536        | P. Short Evasion Rate              | True                 | True
        //537        | P. Ranged Evasion Rate             | True                 | True
        //538        | Magical Evasion Rate       		  | True                 | True
        public static void PEVASIONRATE(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 533 -  P. Short Evasionrate ;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.BasePEvasionrate += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.BasePEvasionrate -= value;
        }

        public static void REVASIONRATE(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 534 -  P. Ranged Evasionrate ;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.BaseREvasionrate += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.BaseREvasionrate -= value;
        }

        public static void MEVASIONRATE(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 535 -  Magic Evasionrate  ;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.BaseMEvasionrate += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.BaseMEvasionrate -= value;
        }

        //541        | Magical Critical Rate             | True                 | True
        //542        | P. Short Critical Rate            | True                 | False
        //543        | P. Ranged Critical Rate           | True                 | False
        public static void PCRITICALRATE(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 533 -  P. Short Critrate ;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.BasePCritrate += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.BasePCritrate -= value;
        }

        public static void RCRITICALRATE(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 534 -  P. Ranged Critrate ;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.BaseRCritrate += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.BaseRCritrate -= value;
        }

        public static void MCRITICALRATE(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 535 -  Magic Critrate  ;
            Actor character = aval.sender as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.BaseMCritrate += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.BaseMCritrate -= value;
        }

        //545        | Strength                           | True                 | False
        //546        | Dexterity                          | True                 | False
        //547        | Intellect                          | True                 | False
        //548        | Concentration                      | True                 | False
        //549        | Luck                               | True                 | False
        public static void ADDITION_STR(ref AdditionValue aval, int value)
        {
            Character character = (Character)aval.sender;

            if (aval.context == AdditionContext.Applied)
            {
                character._status.MaxPAttack += (ushort)(2 * value);
                character._status.MinPAttack += (ushort)(1 * value);
                character._status.MaxHP += (ushort)(10 * value);
                character.stats.EQUIPMENT.strength += (byte)value;
                character._status.Updates |= 1;
            }
            else if (aval.context == AdditionContext.Deapplied)
            {
                character._status.MaxPAttack -= (ushort)(2 * value);
                character._status.MinPAttack -= (ushort)(1 * value);
                character._status.MaxHP -= (ushort)(10 * value);
                character.stats.EQUIPMENT.strength -= (byte)value;
                character._status.Updates |= 1;
            }
        }

        public static void ADDITION_DEX(ref AdditionValue aval, int value)
        {
            Character character = (Character)aval.sender;

            if (aval.context == AdditionContext.Applied)
            {
                character._status.BasePHitrate += (ushort)(1 * value);
                character.stats.EQUIPMENT.dexterity += (byte)value;
            }
            else if (aval.context == AdditionContext.Deapplied)
            {
                character._status.BasePHitrate -= (ushort)(1 * value);
                character.stats.EQUIPMENT.dexterity -= (byte)value;
            }
        }

        public static void ADDITION_INT(ref AdditionValue aval, int value)
        {
            Character character = (Character)aval.sender;

            if (aval.context == AdditionContext.Applied)
            {
                character._status.MaxMAttack += (ushort)(6 * value);
                character._status.MinMAttack += (ushort)(3 * value);
                character._status.BasePHitrate += (ushort)(1 * value);
                character.stats.EQUIPMENT.intelligence += (byte)value;
            }
            else if (aval.context == AdditionContext.Deapplied)
            {
                character._status.MaxMAttack -= (ushort)(6 * value);
                character._status.MinMAttack -= (ushort)(3 * value);
                character._status.BaseRHitrate -= (ushort)(1 * value);
                character.stats.EQUIPMENT.intelligence -= (byte)value;
            }
        }

        public static void ADDITION_CON(ref AdditionValue aval, int value)
        {
            Character character = (Character)aval.sender;

            if (aval.context == AdditionContext.Applied)
            {
                character._status.MaxRAttack += (ushort)(4 * value);
                character._status.MinRAttack += (ushort)(2 * value);
                character._status.BasePHitrate += (ushort)(2 * value);
                character.stats.EQUIPMENT.concentration += (byte)value;
            }
            else if (aval.context == AdditionContext.Deapplied)
            {
                character._status.MaxRAttack -= (ushort)(4 * value);
                character._status.MinRAttack -= (ushort)(2 * value);
                character._status.BasePHitrate -= (ushort)(2 * value);
                character.stats.EQUIPMENT.concentration -= (byte)value;
            }
        }

        public static void ADDITION_LUK(ref AdditionValue aval, int value)
        {
            Character character = (Character)aval.sender;

            if (aval.context == AdditionContext.Applied)
                character.stats.EQUIPMENT.luck += (byte)value;
            else if (aval.context == AdditionContext.Deapplied)
                character.stats.EQUIPMENT.luck -= (byte)value;
        }

        //579        | P. Defence                               | True                 | True
        //580        | Magic Defence                            | True                 | False
        public static void ADDITION_PHYSICALDEFENSE(ref AdditionValue aval, int value)
        {
            Actor character = aval.sender as Actor;
            if (aval.context == AdditionContext.Applied)
                character._status.DefencePhysical += (byte)value;
            else if (aval.context == AdditionContext.Deapplied)
                character._status.DefencePhysical -= (byte)value;
        }

        public static void ADDITION_MAGICALDEFENSE(ref AdditionValue aval, int value)
        {
            Actor character = aval.sender as Actor;
            if (aval.context == AdditionContext.Applied)
                character._status.DefenceMagical += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                character._status.DefenceMagical -= (ushort)value;
        }

        //550        | Maximum HP                        | True                 | True
        //551        | Maximum SP                        | True                 | True
        //552        | Maximum Oxygen                    | False                | True
        public static void ADDITION_MHP(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 533 -  P. Short Hitrate ;

            Actor character = aval.target as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MaxHP += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MaxHP -= value;
            character._status.Updates |= 1;
        }

        public static void ADDITION_MSP(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 533 -  P. Short Hitrate ;

            Actor character = aval.target as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.MaxSP += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.MaxSP -= value;
            character._status.Updates |= 1;
        }

        public static void ADDITION_MBREATHCAPACITY(ref AdditionValue aval, int value)
        {
            Actor character = aval.target as Actor;
            if (aval.context == AdditionContext.Applied)
                character._status.MaximumOxygen += (byte)value;
            else if (aval.context == AdditionContext.Deapplied)
                character._status.MaximumOxygen -= (byte)value;
            character._status.Updates |= 1;
        }

        //553        | Current HP                               | True                 | True
        //554        | Current SP                               | True                 | False
        //555        | Current Oxygen                           | False                | True
        //556        | Current LP                               | True                 | False
        public static void ADDITION_HP(ref AdditionValue aval, int value)
        {
            Actor character = aval.target as Actor;
            if ((aval.context == AdditionContext.Applied || aval.context == AdditionContext.Reapplied) && character != null)
            {
                character._status.CurrentHp += (ushort)value;
                character._status.Updates |= 1;
                if (character._status.CurrentHp > (ushort)character._status.MaxHP)
                {
                    character._status.CurrentHp = (ushort)character._status.MaxHP;
                }
            }
        }

        public static void ADDITION_SP(ref AdditionValue aval, int value)
        {
            Actor character = aval.target as Actor;
            if ((aval.context == AdditionContext.Applied || aval.context == AdditionContext.Reapplied) && character != null)
            {
                character._status.CurrentSp += (ushort)value;
                character._status.Updates |= 1;
                if (character._status.CurrentSp > (ushort)character._status.MaxSP)
                {
                    character._status.CurrentSp = (ushort)character._status.MaxSP;
                }
            }
        }

        public static void ADDITION_BREATH(ref AdditionValue aval, int value)
        {
            Actor character = aval.target as Actor;
            if ((aval.context == AdditionContext.Applied || aval.context == AdditionContext.Reapplied) && character != null)
            {
                character._status.CurrentOxygen += (byte)value;
                character._status.Updates |= 1;
                if (character._status.CurrentOxygen > character._status.MaximumOxygen)
                {
                    character._status.CurrentOxygen = character._status.MaximumOxygen;
                }
            }
        }

        public static void ADDITION_LP(ref AdditionValue aval, int value)
        {
            Actor character = aval.target as Actor;
            if ((aval.context == AdditionContext.Applied || aval.context == AdditionContext.Reapplied) && character != null)
            {
                character._status.CurrentLp += (byte)value;
                character._status.Updates |= 1;
                if (character._status.CurrentLp > 7)
                {
                    character._status.CurrentLp = 7;
                }

                //Do lp effect
                if (value > 0)
                    Common.Skills.SendSkillEffect(character, aval.additionid, 3, (uint)value);
            }
        }

        //557        | HP Recovery Amount                       | True                 | False
        //558        | SP Recovery Amount                       | True                 | False
        //561        | HP Recover Rate		                    | True                 | True
        //562        | SP Recover Rate                          | True                 | True
        //564        | Block SP Recovery                        | True                 | False
        //565        | Block HP Recovery                        | True                 | False
        public static void ADDITION_HPRECOVERYRATE(ref AdditionValue aval, int value)
        {
            Actor character = aval.sender as Actor;
            if (aval.context == AdditionContext.Applied)
                character._status.HpRecoveryRate += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                character._status.HpRecoveryRate -= (ushort)value;
        }

        public static void ADDITION_SPRECOVERYRATE(ref AdditionValue aval, int value)
        {
            Actor character = aval.sender as Actor;
            if (aval.context == AdditionContext.Applied)
                character._status.SpRecoveryRate += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                character._status.SpRecoveryRate -= (ushort)value;
        }

        public static void ADDITION_OXYGENRECOVERYRATE(ref AdditionValue aval, int value)
        {
            Actor character = aval.sender as Actor;
            if (aval.context == AdditionContext.Applied)
                character._status.OxygenRecoveryRate += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                character._status.OxygenRecoveryRate -= (ushort)value;
        }

        public static void ADDITION_HPRECOVERQUANTITY(ref AdditionValue aval, int value)
        {
            Actor character = aval.sender as Actor;
            if (aval.context == AdditionContext.Applied)
                character._status.HpRecoveryQuantity += (short)value;
            else if (aval.context == AdditionContext.Deapplied)
                character._status.HpRecoveryQuantity -= (short)value;
        }

        public static void ADDITION_SPRECOVERQUANTITY(ref AdditionValue aval, int value)
        {
            Actor character = aval.sender as Actor;
            if (aval.context == AdditionContext.Applied)
                character._status.SpRecoveryQuantity += (short)value;
            else if (aval.context == AdditionContext.Deapplied)
                character._status.SpRecoveryQuantity -= (short)value;
        }

        public static void BLOCKHPRECOVERY(ref AdditionValue aval, int value)
        {
            Character starget = aval.sender as Character;
            if (starget != null)
            {
                if (aval.context == AdditionContext.Applied)
                    starget.blockhprecovery += (byte)(value ^ 1);
                else if (aval.context == AdditionContext.Deapplied)
                    starget.blockhprecovery -= (byte)(value ^ 1);
            }
        }

        public static void BLOCKSPRECOVERY(ref AdditionValue aval, int value)
        {
            Character starget = aval.sender as Character;
            if (starget != null)
            {
                if (aval.context == AdditionContext.Applied)
                    starget.blocksprecovery += (byte)(value ^ 1);
                else if (aval.context == AdditionContext.Deapplied)
                    starget.blockhprecovery -= (byte)(value ^ 1);
            }
        }

        //Addition 586
        public static void ADDITION_DROPRATE(ref AdditionValue aval, int value)
        {
        }

        // Addition 603
        public static void ADDITION_CANNOTMOVE(ref AdditionValue aval, int value)
        {
            Actor character = aval.sender as Actor;
            if (aval.context == AdditionContext.Applied && value == 0)
                character._status.CannotMove += 1;
            else if (aval.context == AdditionContext.Deapplied)
                character._status.CannotMove -= 1;
        }

        // Addition 604
        public static void ADDITION_CANNOTATTACK(ref AdditionValue aval, int value)
        {
            Actor character = aval.sender as Actor;
            if (aval.context == AdditionContext.Applied && value == 0)
                character._status.CannotAttack += 1;
            else if (aval.context == AdditionContext.Deapplied)
                character._status.CannotAttack -= 1;
        }

        // Addition 611
        public static void ADDITION_CANSEERESSHOLD(ref AdditionValue aval, int value)
        {
            Actor starget = aval.sender as Actor;
            if (starget != null)
            {
                if (aval.context == AdditionContext.Applied)
                    starget._status.ChaseTresshold += (byte)(value ^ 1);
                else if (aval.context == AdditionContext.Deapplied)
                    starget._status.ChaseTresshold -= (byte)(value ^ 1);
            }
        }

        // Addition 612
        public static void ADDITION_WEXP(ref AdditionValue aval, int value)
        {
            Character character = aval.sender as Character;
            if (value <= -20000)
            {
                double valueb = ((double)(value + 20000) / (double)1000);
                if (aval.context == AdditionContext.Applied)
                    character._WexpModifier += valueb;
                else if (aval.context == AdditionContext.Deapplied)
                    character._WexpModifier -= valueb;
            }
            else if (value >= 20000)
            {
                double valueb = ((double)(value - 20000) / (double)1000);
                if (aval.context == AdditionContext.Applied)
                    character._WexpModifier += valueb;
                else if (aval.context == AdditionContext.Deapplied)
                    character._WexpModifier -= valueb;
            }
        }

        // Addition 613
        public static void ADDITION_CEXP(ref AdditionValue aval, int value)
        {
            Character character = aval.sender as Character;

            if (value <= -20000)
            {
                double valueb = ((double)(value + 20000) / (double)1000);
                if (aval.context == AdditionContext.Applied)
                    character._CexpModifier += valueb;
                else if (aval.context == AdditionContext.Deapplied)
                    character._CexpModifier -= valueb;
            }
            else if (value >= 20000)
            {
                double valueb = ((double)(value - 20000) / (double)1000);
                if (aval.context == AdditionContext.Applied)
                    character._CexpModifier += valueb;
                else if (aval.context == AdditionContext.Deapplied)
                    character._CexpModifier -= valueb;
            }
        }

        // Addition 614
        public static void ADDITION_JEXP(ref AdditionValue aval, int value)
        {
            Character character = aval.sender as Character;
            if (value <= -20000)
            {
                double valueb = ((double)(value + 20000) / (double)1000);
                if (aval.context == AdditionContext.Applied)
                    character._JexpModifier += valueb;
                else if (aval.context == AdditionContext.Deapplied)
                    character._JexpModifier -= valueb;
            }
            else if (value >= 20000)
            {
                double valueb = ((double)(value - 20000) / (double)1000);
                if (aval.context == AdditionContext.Applied)
                    character._JexpModifier += valueb;
                else if (aval.context == AdditionContext.Deapplied)
                    character._JexpModifier -= valueb;
            }
        }

        // Addition 663
        public static void ADDITION_CANATTACKTRESSHOLD(ref AdditionValue aval, int value)
        {
            Actor starget = aval.sender as Actor;
            if (starget != null)
            {
                if (aval.context == AdditionContext.Applied)
                    starget._status.AttackTresshold += (byte)(value ^ 1);
                else if (aval.context == AdditionContext.Deapplied)
                    starget._status.AttackTresshold -= (byte)(value ^ 1);
            }
        }

        // Addition 664
        public static void ADDITION_CURRENTHPTARGET(ref AdditionValue aval, int value)
        {
            Actor character = aval.target as Actor;
            if ((aval.context == AdditionContext.Applied || aval.context == AdditionContext.Reapplied) && character != null)
            {
                character._status.CurrentHp += (ushort)value;
                character._status.Updates |= 1;
                if (character._status.CurrentHp > (ushort)character._status.MaxHP)
                {
                    character._status.CurrentHp = (ushort)character._status.MaxHP;
                }

                Common.Skills.SendSkillEffect(character, aval.additionid, 1, (uint)value);
            }
        }

        // Addition 665
        public static void ADDITION_CURRENTSPTARGET(ref AdditionValue aval, int value)
        {
            Actor character = aval.target as Actor;
            if ((aval.context == AdditionContext.Applied || aval.context == AdditionContext.Reapplied) && character != null)
            {
                character._status.CurrentSp += (ushort)value;
                character._status.Updates |= 1;
                if (character._status.CurrentSp > (ushort)character._status.MaxSP)
                {
                    character._status.CurrentSp = (ushort)character._status.MaxSP;
                }

                Common.Skills.SendSkillEffect(character, aval.additionid, 2, (uint)value);
            }
        }

        // Addition 670
        public static void ADDITION_CHATBAN(ref AdditionValue aval, int value)
        {
            Character starget = aval.sender as Character;
            if (starget != null)
            {
                if (aval.context == AdditionContext.Applied)
                    starget.chatmute += (byte)(value ^ 1);
                else if (aval.context == AdditionContext.Deapplied)
                    starget.chatmute -= (byte)(value ^ 1);
            }
        }

        public static void ADDITION_BLOCKRATE(ref AdditionValue aval, int value)
        {
            Actor character = (Actor)aval.sender;
            if (aval.context == AdditionContext.Applied)
                character.Status.BlockratePhysical += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                character.Status.BlockratePhysical -= (ushort)value;
        }

        public static void RandomizeItem(ref AdditionValue aval, int value)
        {
            if (aval.context == AdditionContext.Applied)
            {
                Character character = (Character)aval.sender;
                List<uint> Itemlist = new List<uint>();
                switch (value)
                {
                    case 1:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 2:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 3:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 4:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 5:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 6:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 7:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 8:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 9:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 10:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 11:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 12:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 13:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 14:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 15:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 16:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 17:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 18: Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;

                    case 19:
                        Itemlist.Clear();
                        Itemlist.Add(100000);
                        Itemlist.Add(100001);
                        Itemlist.Add(100002);
                        Itemlist.Add(100003);
                        break;
                }

                if (Itemlist.Count == 0) return;
                int rand = Singleton.WorldTasks.random.Next(0, 100) % Itemlist.Count;
                Common.Items.GiveItem(character, Itemlist[rand], 1);
            }
        }

        public static void ActDead(ref AdditionValue aval, int value)
        {
            Actor starget = aval.sender as Actor;
            if (starget != null)
            {
                //DO SOMETHING
                Console.WriteLine("Act dead");
            }
        }

        /// <summary>
        /// Adds the specified value of fire-restistance to the specified aval.sender.
        /// </summary>
        /// <param name="aval.sender"></param>
        /// <param name="Value"></param>
        /// <param name="aval.context"></param>
        public static void FireResistance(ref AdditionValue aval, int value)
        {
            //CHARACTER TO INVOKE
            Actor targeta = aval.sender as Actor;
            targeta._status.Updates |= 2;

            lock (targeta._status)
            {
                //CHECKS IF WE ARE aval.contextING OR DEaval.contextING
                if (aval.context == AdditionContext.Applied)
                    targeta._status.FireResistance += (ushort)value;
                else if (aval.context == AdditionContext.Deapplied)
                    targeta._status.FireResistance -= (ushort)value;
            }
        }

        /// <summary>
        /// Adds the specified value of ice-resistance to the specified aval.sender
        /// </summary>
        /// <param name="aval.sender"></param>
        /// <param name="Value"></param>
        /// <param name="aval.context"></param>
        public static void IceResistance(ref AdditionValue aval, int value)
        {
            //CHARACTER TO INVOKE
            Actor targeta = aval.sender as Actor;
            targeta._status.Updates |= 2;

            lock (targeta._status)
            {
                //CHECKS IF WE ARE aval.contextING OR DEaval.contextING
                if (aval.context == AdditionContext.Applied)
                    targeta._status.IceResistance += (ushort)value;
                else if (aval.context == AdditionContext.Deapplied)
                    targeta._status.IceResistance -= (ushort)value;
            }
        }

        /// <summary>
        /// Adds the specified value of thunder-resistance to the specified aval.sender
        /// </summary>
        /// <param name="aval.sender"></param>
        /// <param name="Value"></param>
        /// <param name="aval.context"></param>
        public static void ThunderResistance(ref AdditionValue aval, int value)
        {
            //CHARACTER TO INVOKE
            Actor targeta = aval.sender as Actor;
            targeta._status.Updates |= 2;

            lock (targeta._status)
            {
                //CHECKS IF WE ARE aval.contextING OR DEaval.contextING
                if (aval.context == AdditionContext.Applied)
                    targeta._status.ThunderResistance += (ushort)value;
                else if (aval.context == AdditionContext.Deapplied)
                    targeta._status.ThunderResistance -= (ushort)value;
            }
        }

        /// <summary>
        /// Adds the specified value of holy-resistance to the specified aval.sender
        /// </summary>
        /// <param name="aval.sender"></param>
        /// <param name="Value"></param>
        /// <param name="aval.context"></param>
        public static void HolyResistance(ref AdditionValue aval, int value)
        {
            //CHARACTER TO INVOKE
            Actor targeta = aval.sender as Actor;
            targeta._status.Updates |= 2;

            lock (targeta._status)
            {
                //CHECKS IF WE ARE aval.contextING OR DEaval.contextING
                if (aval.context == AdditionContext.Applied)
                    targeta._status.HolyResistance += (ushort)value;
                else if (aval.context == AdditionContext.Deapplied)
                    targeta._status.HolyResistance -= (ushort)value;
            }
        }

        /// <summary>
        /// Adds the specified value of dark-resistance to the specified aval.sender
        /// </summary>
        /// <param name="aval.sender"></param>
        /// <param name="Value"></param>
        /// <param name="aval.context"></param>
        public static void DarkResistance(ref AdditionValue aval, int value)
        {
            //CHARACTER TO INVOKE
            Actor targeta = aval.sender as Actor;
            targeta._status.Updates |= 2;

            lock (targeta._status)
            {
                //CHECKS IF WE ARE aval.contextING OR DEaval.contextING
                if (aval.context == AdditionContext.Applied)
                    targeta._status.DarkResistance += (ushort)value;
                else if (aval.context == AdditionContext.Deapplied)
                    targeta._status.DarkResistance -= (ushort)value;
            }
        }

        /// <summary>
        /// Adds the specified value of soul-resistance to the specified aval.sender
        /// </summary>
        /// <param name="aval.sender"></param>
        /// <param name="Value"></param>
        /// <param name="aval.context"></param>
        public static void SoulResistance(ref AdditionValue aval, int value)
        {
            //CHARACTER TO INVOKE
            Actor targeta = aval.sender as Actor;
            targeta._status.Updates |= 2;

            lock (targeta._status)
            {
                //CHECKS IF WE ARE aval.contextING OR DEaval.contextING
                if (aval.context == AdditionContext.Applied)
                    targeta._status.SoulResistance += (ushort)value;
                else if (aval.context == AdditionContext.Deapplied)
                    targeta._status.SoulResistance -= (ushort)value;
            }
        }

        /// <summary>
        /// Adds the specified value of spirit-resistance to the specified aval.sender
        /// </summary>
        /// <param name="aval.sender"></param>
        /// <param name="Value"></param>
        /// <param name="aval.context"></param>
        public static void SpiritResistance(ref AdditionValue aval, int value)
        {
            //CHARACTER TO INVOKE
            Actor targeta = aval.sender as Actor;
            targeta._status.Updates |= 2;

            lock (targeta._status)
            {
                //CHECKS IF WE ARE aval.contextING OR DEaval.contextING
                if (aval.context == AdditionContext.Applied)
                    targeta._status.SpiritResistance += (ushort)value;
                else if (aval.context == AdditionContext.Deapplied)
                    targeta._status.SpiritResistance -= (ushort)value;
            }
        }

        /// <summary>
        /// Speed additions
        /// </summary>
        /// <param name="aval.sender"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="aval.context"></param>
        public static void Speed(ref AdditionValue aval, int value)
        {
            //Memory subfunction
            //Addition 533 -  P. Short Critrate ;
            Actor character = aval.target as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.WalkingSpeed += value;
            else if (aval.context == AdditionContext.Deapplied)
                status.WalkingSpeed -= value;
        }

        /// <summary>
        /// Casting Time
        /// </summary>
        /// <param name="aval.sender"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="aval.context"></param>
        public static void CastTime(ref AdditionValue aval, int value)
        {
            Actor character = aval.target as Actor;
            BattleStatus status = character.Status;
            if (aval.context == AdditionContext.Applied)
                status.CastingTime += (ushort)value;
            else if (aval.context == AdditionContext.Deapplied)
                status.CastingTime -= (ushort)value;
        }
    }
}