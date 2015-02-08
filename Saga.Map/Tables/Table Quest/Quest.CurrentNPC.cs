using Saga.Managers;
using Saga.Map;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Tasks;
using System;

namespace Saga.Quests
{
    [CLSCompliant(false)]
    static partial class QUEST_TABLE
    {
        /// <title>Saga.GetNPCIndex</title>
        /// <code>
        /// Saga.GetNPCIndex(cid);
        /// </code>
        /// <description>
        /// Returns the npc model id of the current npc.
        /// Returns 0 on no npc selected.
        /// </description>
        /// <example>
        /// function QUEST_STEP_2(cid)
        ///	    -- Talk to mischa
        ///     local NPCIndex = 1000;
        ///     local ret = Saga.GetNPCIndex(cid);
        ///
        ///     Saga.AddWaypoint(cid, QuestID, NPCIndex, -12092, -6490, -8284, 1);
        ///     if ret == NPCIndex then
        ///         Saga.StepComplete(cid, QuestID, 102);
        ///     else
        ///         return  -1;
        ///     end
        ///
        ///     Saga.ClearWaypoints(cid, QuestID);
        ///     return 0;
        /// end
        /// </example>
        public static uint GetNPCIndex(uint cid)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
                if (value.Target != null && MapObject.IsNpc(value.Target)) return value.Target.ModelId;
            return 0;
        }

        public static uint GetActionObjectIndex(uint cid)
        {
            Character value;
            if (LifeCycle.TryGetById(cid, out value))
                if (value.Target != null && MapObject.IsMapItem(value.Target)) return value.Target.ModelId;
            return 0;
        }

        public static void Spawn(uint npcid, byte map, float x, float y, float z)
        {
            Zone zone;
            if (Singleton.Zones.TryGetZone(map, out zone))
            {
                MapObject regionObject = null;
                System.Threading.WaitCallback callback = delegate(object state)
                {
                    Singleton.Templates.UnspawnInstance(regionObject);
                    GC.Collect(0, GCCollectionMode.Optimized);
                };

                if (Singleton.Templates.SpawnNpcInstance(npcid, new Point(x, y, z), new Rotator(0, 0), zone, false, out regionObject))
                {
                    QueedTask timer = new QueedTask(callback, regionObject, 300000);
                }
            }
        }
    }
}