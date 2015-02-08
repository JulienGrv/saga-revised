using Saga.PrimaryTypes;
using Saga.Tasks;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <title>Saga.GiveZeny</title>
        /// <code>
        /// Saga.GiveZeny(cid, Zeny);
        /// </code>
        /// <description>
        /// Gives a certain amount of money as part of beeing
        /// a quest reward.
        /// </description>
        /// <example>
        ///function QUEST_FINISH(cid)
        ///	    -- Gives all rewards
        ///
        ///	    Saga.GiveItem(cid, RewItem1, RewItemCount1 );
        ///	    Saga.GiveZeny(cid, RewZeny);
        ///	    Saga.GiveExp(cid, RewCxp, RewJxp, RewWxp);
        ///	    return 0;
        /// end
        /// </example>
        public static void GiveZeny(uint cid, uint zeny)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                value.ZENY += zeny;
                CommonFunctions.UpdateZeny(value);
            }
        }
    }
}