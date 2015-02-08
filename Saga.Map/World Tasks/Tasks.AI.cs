using Saga.Shared.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Saga.Tasks
{
    public static class LifespanAI
    {
        #region Private Members

        /// <summary>
        /// List of LifespanThreads where monster objects can be assigned to.
        /// </summary>
        private static List<ThreadState> LifespanThreads = new List<ThreadState>();

        /// <summary>
        /// A zero based index of the next thread where to register with
        /// </summary>
        /// <remarks>
        /// This is a volatile field containing the next threadstate where
        /// to register the AI to. Tbe next threadstate will should indicate
        /// that the AI is registered on several lists. Reducing the
        /// error amount.
        /// </remarks>
        private static volatile int NextThreadState = 0;

        /// <summary>
        /// Number representing the number of activated monsters.
        /// </summary>
        private static volatile int NumberSubscribedMobs = 0;

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Subscribes a AI controler for it's lifespan updates.
        /// </summary>
        /// <remarks>
        /// The primairy target for this is to make them moveable.
        /// </remarks>
        /// <param name="e">Target who to subscribe</param>
        public static void Subscribe(IArtificialIntelligence e)
        {
            if (!e.Lifespan.IsRegistered)
            {
                int ThreadA = NextThreadState;
                int ThreadB = ThreadA + 1;
                LifespanThreads[ThreadA].Register(e);
                e.Lifespan.LifespanThread = ThreadB;
                NextThreadState = ++NextThreadState % LifespanThreads.Count;
                NumberSubscribedMobs++;
            }
        }

        /// <summary>
        /// Unsubscribes a AI controler for it's lifespan updates.
        /// </summary>
        /// <param name="e"></param>
        public static void Unsubscribe(IArtificialIntelligence e)
        {
            if (e.Lifespan.IsRegistered)
            {
                NextThreadState = (e.Lifespan.LifespanThread - 1);
                LifespanThreads[(e.Lifespan.LifespanThread - 1)].Unregister(e);
                e.Lifespan.LifespanThread = 0;
                NumberSubscribedMobs--;
            }
        }

        /// <summary>
        /// Checks if the AI controler is subscribed.
        /// </summary>
        /// <param name="e">AI controler to check</param>
        /// <returns>True if the controller is subscribed</returns>
        public static bool IsSubscribed(IArtificialIntelligence e)
        {
            return e.Lifespan.IsRegistered;
        }

        #endregion Public Members

        #region Constructor / Deconstructor

        /// <summary>
        /// Initializes all the threadstates.
        /// </summary>
        /// <remarks>
        /// It creates 4 thread states by default.
        /// </remarks>
        static LifespanAI()
        {
            LifespanThreads.Add(new ThreadState());
            LifespanThreads.Add(new ThreadState());
            LifespanThreads.Add(new ThreadState());
            LifespanThreads.Add(new ThreadState());
        }

        internal static void Stop()
        {
            //remove all lifespan threads
            for (int i = 0; i < LifespanThreads.Count; i++)
                LifespanThreads[i].Stop();
            LifespanThreads.Clear();
        }

        #endregion Constructor / Deconstructor

        #region Nested Classes/Structures

        private sealed class ThreadState
        {
            private Thread d;

            public ThreadState()
            {
                d = new Thread(new ThreadStart(Process));
                d.Start();
            }

            ~ThreadState()
            {
                Stop();
                d = null;
            }

            internal void Stop()
            {
                try
                {
                    if (d != null && d.IsAlive)
                    {
                        GC.SuppressFinalize(d);
                        d.Abort();
                    }
                }
                finally
                {
                    if (d != null)
                        GC.ReRegisterForFinalize(d);
                }
            }

            /// <summary>
            /// List of activated AI
            /// </summary>
            internal List<IArtificialIntelligence> ActivatedAI = new List<IArtificialIntelligence>(25);

            /// <summary>
            /// Indication if the AI is processing.
            /// </summary>
            private volatile bool IsWritting = false;

            /// <summary>
            /// Registers a AI object
            /// </summary>
            /// <param name="c"></param>
            internal void Register(IArtificialIntelligence c)
            {
                try
                {
                    IsWritting = true;
                    //Add a random delay to make mobs appear they move pure random
                    int rand = Saga.Managers.WorldTasks._random.Next(0, 1000);
                    c.Lifespan.lasttick = Environment.TickCount + rand;
                    ActivatedAI.Add(c);
                }
                finally
                {
                    IsWritting = false;
                }
            }

            /// <summary>
            /// Unregisters a AI object
            /// </summary>
            /// <param name="c"></param>
            internal void Unregister(IArtificialIntelligence c)
            {
                try
                {
                    IsWritting = true;
                    c.Lifespan.lasttick = Environment.TickCount;
                    ActivatedAI.Remove(c);
                }
                finally
                {
                    IsWritting = false;
                }
            }

            /// <summary>
            /// Processes all threads
            /// </summary>
            internal void Process()
            {
                while (!Environment.HasShutdownStarted)
                {
                    try
                    {
                        for (int i = 0; i < ActivatedAI.Count; i++)
                        {
                            try
                            {
                                if (IsWritting == false)
                                    ActivatedAI[i].Process();
                            }
                            catch (SocketException)
                            {
                                //DO NOT PROCESS THIS
                            }
                            catch (ThreadAbortException e)
                            {
                                throw e;
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }
                        }

                        Thread.Sleep(1);
                    }
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Supplies as interaction base for IArtificialIntelligence objects.
        /// </summary>
        public sealed class Lifespan
        {
            /// <summary>
            /// Index to which ThreadState object the map
            /// was assigned to
            /// </summary>
            internal int LifespanThread = 0;

            /// <summary>
            /// Tick when it was last updated.
            /// </summary>
            public int lasttick;

            /// <summary>
            /// Checks if the Lifespan object was registered
            /// </summary>
            public bool IsRegistered
            {
                get
                {
                    return LifespanThread > 0;
                }
            }

            /// <summary>
            /// Subscribes the Lifespan Object
            /// </summary>
            public void Subscribe(IArtificialIntelligence ai)
            {
                LifespanAI.Subscribe(ai);
            }

            /// <summary>
            /// Ubsubscribes the Lifespan Object
            /// </summary>
            public void Unsubscribe(IArtificialIntelligence ai)
            {
                LifespanAI.Unsubscribe(ai);
            }
        }

        #endregion Nested Classes/Structures
    }
}