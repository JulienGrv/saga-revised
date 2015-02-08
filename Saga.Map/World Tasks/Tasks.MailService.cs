using Saga.Map;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Saga.Tasks
{
    internal static class MailService
    {
        #region Private Members

        /// <summary>
        /// List of AI pending for deletion
        /// </summary>
        private static Queue<KeyValuePair<string, uint>> ProcessingQueuee = new Queue<KeyValuePair<string, uint>>();

        /// <summary>
        /// Last tickcount when the MailService was refreshed
        /// </summary>
        private static int LastRefreshTick = Environment.TickCount;

        /// <summary>
        /// Object to synclock all items
        /// </summary>
        private static object synclock = new object();

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
            try
            {
                //SET PRIORITY OF THE THREAD
                int count = ProcessingQueuee.Count;
                lock (synclock)
                {
                    if (count > 200)
                        Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                    else if (count > 20)
                        Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                }

                int i = 0;
                while (ProcessingQueuee.Count > 0)
                {
                    //HELPER VARIABLES
                    Character character;
                    KeyValuePair<string, uint> pair = ProcessingQueuee.Dequeue();

                    //CHECK IF THE USER IS ONLINE
                    if (LifeCycle.TryGetByName(pair.Key, out character))
                    {
                        Common.Internal.MailArrived(character, pair.Value);
                    }

                    //CLEARS THE PENDING MAIL
                    Singleton.Database.ClearPendingMails(pair.Key);
                    i++;
                    if (i > 10000) break;
                }

                //REFRESHES THE MAILING QUEUEE
                if (Environment.TickCount - LastRefreshTick > 6000)
                {
                    ProcessingQueuee.Clear();
                    LastRefreshTick = Environment.TickCount;

                    foreach (KeyValuePair<string, uint> pair in Singleton.Database.GetPendingMails())
                        ProcessingQueuee.Enqueue(pair);
                }
            }
            catch (ThreadAbortException ex)
            {
                throw ex;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion Internal Members
    }
}