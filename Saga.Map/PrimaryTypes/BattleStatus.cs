using Saga.Structures;
using System;

namespace Saga.PrimaryTypes
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [Serializable()]
    public class BattleStatus
    {
        public volatile byte Updates;                    //Internal usage

        private AdditionInteger minPAttack = 0;
        private AdditionInteger minRAttack = 0;
        private AdditionInteger minMAttack = 0;
        private AdditionInteger maxPAttack = 0;
        private AdditionInteger maxRAttack = 0;
        private AdditionInteger maxMAttack = 0;

        public AdditionInteger MinPAttack
        {
            get
            {
                if (noDamage > 0)
                    return 0;
                return minPAttack;
            }
            set { minPAttack = value; }
        }

        public AdditionInteger MinRAttack
        {
            get
            {
                if (noDamage > 0)
                    return 0;
                return minRAttack;
            }
            set { minRAttack = value; }
        }

        public AdditionInteger MinMAttack
        {
            get
            {
                if (noDamage > 0)
                    return 0;
                return minMAttack;
            }
            set { minMAttack = value; }
        }

        public AdditionInteger MaxPAttack
        {
            get
            {
                if (noDamage > 0)
                    return 0;
                return maxPAttack;
            }
            set { maxPAttack = value; }
        }

        public AdditionInteger MaxRAttack
        {
            get
            {
                if (noDamage > 0)
                    return 0;
                return maxRAttack;
            }
            set { maxRAttack = value; }
        }

        public AdditionInteger MaxMAttack
        {
            get
            {
                if (noDamage > 0)
                    return 0;
                return maxMAttack;
            }
            set { maxMAttack = value; }
        }

        public ushort MinWPAttack = 0;
        public ushort MinWRAttack = 0;
        public ushort MinWMAttack = 0;
        public ushort MaxWPAttack = 0;
        public ushort MaxWRAttack = 0;
        public ushort MaxWMAttack = 0;

        public AdditionInteger BaseMaxPAttack = 0;
        public AdditionInteger BaseMaxRAttack = 0;
        public AdditionInteger BaseMaxMAttack = 0;
        public AdditionInteger BaseMinPAttack = 0;
        public AdditionInteger BaseMinRAttack = 0;
        public AdditionInteger BaseMinMAttack = 0;
        public AdditionInteger BasePHitrate = 0;
        public AdditionInteger BaseRHitrate = 0;
        public AdditionInteger BaseMHitrate = 0;
        public AdditionInteger BasePEvasionrate = 0;
        public AdditionInteger BaseREvasionrate = 0;
        public AdditionInteger BaseMEvasionrate = 0;
        public AdditionInteger BasePCritrate = 0;
        public AdditionInteger BaseRCritrate = 0;
        public AdditionInteger BaseMCritrate = 0;

        public AdditionInteger MaxHP = 0;
        public AdditionInteger MaxSP = 0;
        public AdditionInteger MaxOxygen = 0;

        public byte MaximumOxygen = 45;                // 552
        public ushort CurrentHp;                         // 553
        public ushort CurrentSp;                         // 554
        public byte CurrentOxygen = 45;                // 555
        public int LASTLP_ADD;
        public byte currentLp;                           // 556

        public byte CurrentLp
        {
            set
            {
                if (currentLp == 0 && value > 0)
                    LASTLP_ADD = System.Environment.TickCount;
                currentLp = value;
            }
            get { return currentLp; }
        }

        public ushort HpRecoveryRate;                    // 557
        public ushort SpRecoveryRate;                    // 558
        public ushort OxygenRecoveryRate;                // 559

        public bool NoHpSpRecovery;                    // 565

        public ushort SoulResistance = 0;                // 567
        public ushort SpiritResistance = 0;              // 568
        public ushort ThunderResistance = 0;             // 569
        public ushort DarkResistance = 0;                 // 570
        public ushort FireResistance = 0;                // 571
        public ushort IceResistance = 0;                 // 572
        public ushort HolyResistance = 0;                // 573
        public ushort GhostResitance = 0;                // 573
        public ushort WindResitance = 0;                 // 573

        public AdditionInteger WalkingSpeed = 500;                  // 576

        public ushort CastingTime = 0;                   // 577

        public AdditionInteger DefencePhysical;                   // 578
        public AdditionInteger DefenceRanged;                   // 579
        public AdditionInteger DefenceMagical;                    // 580

        public ushort BlockratePhysical;                 // 580
        public ushort BlockrateRanged;                   // 581
        public ushort BlockrateShield;                   // 582
        public ushort BlockrateMagical;                  // 583

        public ushort DroprateItems;                     // 586

        public ushort SkillsPhysicalMinimalAttack;       // 594
        public ushort SkillsRangedMinimalAttack;         // 595
        public ushort SkillsMagicalMinimalAttack;        // 596
        public ushort SkillsPhysicalMaximalAttack;       // 597
        public ushort SkillsRangedMaximalAttack;         // 598
        public ushort SkillsMagicalMaximalAttack;        // 599

        public short HpRecoveryQuantity;                 // 601
        public short SpRecoveryQuantity;                 // 602
        public byte ChaseTresshold;                      // 611
        public byte AttackTresshold;                     // 663

        public byte CannotAttack;                        // 603
        public byte CannotMove;                          // 604

        public int LPAtkAdd = 0;
        public byte noDamage = 0;
    }
}