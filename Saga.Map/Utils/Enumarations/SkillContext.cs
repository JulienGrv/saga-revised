using System;

namespace Saga.Enumarations
{
    [CLSCompliant(false)]
    public enum SkillContext
    {
        /// <summary>
        /// Inidicates tge skill is used by skilll-cast
        /// </summary>
        SkillCast,

        /// <summary>
        /// Inidicates tge skill is used by skill-use
        /// </summary>
        SkillUse,

        /// <summary>
        /// Inidicates tge skill is used by skilll-togle
        /// </summary>
        SkillToggle,

        /// <summary>
        /// Iniditactes the skill is used by a item
        /// </summary>
        ItemUsage
    }

    public enum AdditionContext
    {
        /// <summary>
        /// Indicates the addition is applying
        /// </summary>
        Applied,

        /// <summary>
        /// Indicates the addition is reapplying
        /// </summary>
        Reapplied,

        /// <summary>
        /// Indicates the addition is deapplying
        /// </summary>
        Deapplied
    }
}