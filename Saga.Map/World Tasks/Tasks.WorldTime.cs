using System;

namespace Saga.Tasks
{
    /// <summary>
    /// Taks for world time
    /// </summary>
    public static class WorldTime
    {
        #region Contructor / Decontructor

        /// <summary>
        /// Creates the workd time task
        /// </summary>
        static WorldTime()
        {
            LastTick = Environment.TickCount;
        }

        #endregion Contructor / Decontructor

        #region Private Members

        /// <summary>
        /// Contains the GameTime
        /// </summary>
        private static volatile byte[] gameTime = new byte[3];

        /// <summary>
        /// Last updated tick
        /// </summary>
        private static int LastTick;

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Gets the world time in bytes
        /// </summary>
        public static byte[] Time
        {
            get
            {
                return gameTime;
            }
        }

        #endregion Public Members

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
            if (Environment.TickCount - LastTick > 5000)
            {
                if (gameTime[2] < 59) gameTime[2]++;
                else
                {
                    if (gameTime[1] < 24)
                    {
                        gameTime[1]++;
                        gameTime[2] = 0;
                    }
                    else
                    {
                        if (gameTime[0] < 28)
                        {
                            gameTime[0]++;
                            gameTime[1] = 0;
                            gameTime[2] = 0;
                        }
                        else
                        {
                            gameTime[0] = 1;
                            gameTime[1] = 0;
                            gameTime[2] = 0;
                        }
                    }
                }
            }
        }

        #endregion Internal Members
    }
}