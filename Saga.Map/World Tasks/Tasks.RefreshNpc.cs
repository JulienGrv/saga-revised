using Saga.Map;
using System;

namespace Saga.Tasks
{
    /// <summary>
    /// Taks for world time
    /// </summary>
    internal static class WorldRefresh
    {
        #region Contructor / Decontructor

        /// <summary>
        /// Creates the world refresh task
        /// </summary>
        static WorldRefresh()
        {
            LastTick = Environment.TickCount;
        }

        #endregion Contructor / Decontructor

        #region Private Members

        /// <summary>
        /// Last updated tick
        /// </summary>
        private static int LastTick;

        /// <summary>
        /// Susspended time
        /// </summary>
        private static int SusspendedTime;

        #endregion Private Members

        #region Internal Members

        /// <summary>
        /// Processes all AI threads.
        /// </summary>
        /// <remarks>
        /// If the qeuee get's to big reprioritize the
        /// AI thread with higher priviledges.
        /// </remarks>
        internal static void Process()
        {
            //Refresh npc's
            int t_diff = Environment.TickCount - LastTick;
            if (t_diff > 300000)
            {
                SusspendedTime += t_diff;
                LastTick = Environment.TickCount;

                if (SusspendedTime > 3600000)
                {
                    SusspendedTime = 0;
                    Singleton.Zones.RefreshActors();
                }
            }

            //Check all events
            Singleton.EventManager.CheckEvents();
        }

        #endregion Internal Members
    }
}