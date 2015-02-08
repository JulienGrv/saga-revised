using Saga.Enumarations;
using Saga.PrimaryTypes;
using Saga.Structures;

namespace Saga.Skills
{
    static partial class Spelltable
    {
        /// <remarks>
        /// Last time edited: 10-01-2008
        ///
        /// This script is invoked when player x uses his/hers
        /// promise stone. Basicly it's used to warp fast to your
        /// save location and warp back.
        ///
        /// Dungeon -> Village -> Dungeon: goes to dungeon lvl 1
        /// Village -> Village -> Village: stays on the current village
        /// Map -> Village -> Map: goes to the latest map restore point
        /// </remarks>
        public static void GENERAL_PROMISESTONE(SkillBaseEventArgs bargument)
        {
            if (bargument.Context == Saga.Enumarations.SkillContext.SkillUse)
            {
                SkillUsageEventArgs arguments = (SkillUsageEventArgs)bargument;
                Character current = arguments.Sender as Character;
                Zone zone = current.currentzone;

                uint requiredzeny = (uint)((current.Level - 4) * 10);
                if (requiredzeny > current.ZENY)
                {
                    Common.Errors.GeneralErrorMessage(current, (uint)Generalerror.NotEnoughMoney);
                }

                WorldCoordinate lpos = current.lastlocation.map > 0 ? current.lastlocation : current.savelocation;
                WorldCoordinate spos = current.savelocation;
                if (current.currentzone.Type == ZoneType.Village)
                {
                    arguments.Failed = !CommonFunctions.Warp(current, lpos.map);
                }
                else
                {
                    arguments.Failed = !CommonFunctions.Warp(current, spos.map);
                }

                if (arguments.Failed == false)
                {
                    current.ZENY -= (uint)((current.Level - 4) * 10);
                    CommonFunctions.UpdateZeny(current);
                }
            }
        }
    }
}