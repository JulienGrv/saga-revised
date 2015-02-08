using Saga.Map;
using Saga.PrimaryTypes;
using System;

namespace Saga.Tasks
{
    /// <summary>
    /// Dynamic Weather Task process all the weather information for
    /// every hosted zone.
    /// </summary>
    internal static class DynamicWeather
    {
        #region Private Members

        /// <summary>
        /// Next tick to process the weathers.
        /// This is randomized.
        /// </summary>
        private static int NextTick;

        /// <summary>
        /// LastTick  count
        /// </summary>
        private static int Tick;

        #endregion Private Members

        #region Internal Members

        /// <summary>
        /// Resets the tick to process the weather again.
        /// </summary>
        internal static void Reset()
        {
            NextTick = Singleton.WorldTasks.random.Next(180000, 600000);
            Tick = Environment.TickCount;
        }

        /// <summary>
        /// Updates the weather of each zone
        /// </summary>
        /// <remarks>
        /// This function proccessed all zones to check for dynamic weather changes.
        ///
        /// This is meant to be running on a low priority background thread as it is far from
        /// beeing important enough.
        ///
        /// We'll process weather changes on a random time based upon 180000 and between 600000
        /// millisecconds which is equiavelent to a time  between 3 minutes and 10 minutes. There's
        /// a 90% change of the weather is stays sunny.
        ///
        /// Only the weather that get's changed will be processed.
        /// </remarks>
        internal static void Process()
        {
            if (Environment.TickCount - Tick > NextTick)
            {
                Reset();
                foreach (Zone zone in Singleton.Zones.HostedZones())
                {
                    int Weather = 0;
                    if (Singleton.WorldTasks.random.Next(0, 99) <= 90)
                    {
                        Weather = 1;
                    }
                    else
                    {
                        Weather = Singleton.WorldTasks.random.Next(1, 8);
                    }
                    if (Weather != zone.Weather)
                    {
                        zone.OnChangeWeather(Weather);
                    }
                }
            }
        }

        #endregion Internal Members

        #region Constructor / Decontructor

        /// <summary>
        /// Starts the dynamic weather pattern
        /// </summary>
        static DynamicWeather()
        {
            Reset();
        }

        #endregion Constructor / Decontructor
    }
}