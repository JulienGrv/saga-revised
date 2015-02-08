using System;

namespace Saga.Enumarations
{
    /// <remarks>
    /// Interally we use this enum to generate a dictornairy of a number with
    /// a vaue. So internally is uses numbers but from the db it uses plain text
    /// or a number.
    ///
    /// This is a benefitial approach which doesn't require any casting and thus
    /// saves time where not needed.
    /// </remarks>
    [CLSCompliant(false)]
    public enum AdditionsBonus : uint
    {
        Null = 0,
        LPOnSkillUsage = 84,
        MagicalAtkMin = 502,
        AtkMin = 509,
        RangedAtkMax = 510,
        MagicalAtkMax = 511,
        AtkMax = 518,
        MinRWDamage = 519,
        MinMWDamage,
        MinWDamage,
        MaxRWDamage,
        MaxMWDamage,
        PhysicalATK = 524,
        MagicalATK = 525,
        RangedATK = 526,
        PhysicalHit = 533,
        RangedHit = 534,
        MagicalHit = 535,
        PhysicalFlee = 536,
        RangedFlee = 537,
        MagicalFlee = 538,
        PhysicalCri = 539,
        RangedCri = 540,
        MagicalCri = 541,
        STR = 545,
        DEX = 546,
        INT = 547,
        CON = 548,
        LUK = 549,
        HPMax = 550,
        SPMax = 551,
        OxygenMax = 552,
        CurrentHP = 553,
        OxygenMax2 = 555,
        LPPoint = 556,
        HPRecover = 561,
        SPRecover = 562,
        FireResist = 567,
        IceResist = 568,
        ThunderResist = 569,
        HolyResist = 570,
        DarkResist,
        SoulResist = 573,
        SpiritResist,
        RESISTANCE_SOUL = 575,
        WalkSpeed = 576,
        PhysicalDef = 579,
        MagicalDefense = 580,
        AtkSpeed = 581,
        PhysicalBlock = 582,
        ShieldPhysicalBlock = 584,
        MagicBlock,
        DropRate = 586,
        PhysicalATK2 = 594,
        RangedATK2 = 595,
        MagicalATK2 = 596,
        HPRecoverRate = 601,
        SPRecoverRate = 602,
        WExpRate = 612,
        CExpRate = 613,
        JExpRate = 614,
        FireAtk = 662,
        Def = 645,
        ApplyAddition = 669
    }
}