using Saga.PrimaryTypes;
using Saga.Tasks;

namespace Saga.Quests
{
    static partial class QUEST_TABLE
    {
        /// <title>Saga.GiveExp</title>
        /// <code>
        /// Saga.GiveExp(cid, RewCxp, RewJxp, RewWxp);
        /// </code>
        /// <description>
        /// Gives a certain Cexp, Jexp, Wexp as part of a quest reward.
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
        public static void GiveExp(uint cid, uint Cxp, uint Jxp, uint Wxp)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
            {
                Common.Experience.Add(value, Cxp, Jxp, Wxp);
                //TODO: ADD WEXP ADD
            }
        }
    }
}