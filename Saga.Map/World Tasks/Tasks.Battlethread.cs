using System;
using System.Collections.Generic;
using System.Threading;

namespace Saga.Tasks
{
    public static class BattleThread
    {
        #region BattleThread Private Members

        /// <summary>
        /// A list with all thread states. We use multiple thread-states
        /// so we can do loadbalacing over threads. In total a average
        /// server has 4 cpu's (Quad-Core) therefor 4 threads would be
        /// sufficient for now.
        /// </summary>
        private static List<ThreadState> ThreadStateList = new List<ThreadState>(4);

        /// <summary>
        /// Finds the most suiteable threadstate to subscribe to.
        /// </summary>
        /// <returns></returns>
        private static int FindBestThreadStateToSubscribe()
        {
            //Index of the best thread count
            int index = 0;

            //Number of activated ai for the best ai thread
            int aicount = 0;

            for (int i = 0; i < ThreadStateList.Count; i++)
            {
                //Get the current thread state
                ThreadState current = ThreadStateList[i];

                //Get the current activated ai count
                int count = current.Count;

                //If it fits in the default capacity
                if (count < current.Capacity)
                {
                    return i;
                }
                else
                {
                    if (IsLower(i, count, aicount))
                    {
                        aicount = count;
                        index = i;
                    }
                }
            }

            //Returns the best found AI
            return index;
        }

        private static bool IsLower(int index, int a, int b)
        {
            //If it is the first index
            //Or find the thread with the lowest ai's to equalize them
            if (index == 0 || a < b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void Subscribe(IBattleArtificialIntelligence e, int index)
        {
            if (index < ThreadStateList.Count)
            {
                ThreadState current = ThreadStateList[index];
                current.Add(e);
                e.BattleState.Subscribe(index);
            }
        }

        private static void Unsubscribe(IBattleArtificialIntelligence e, int index)
        {
            if (index < ThreadStateList.Count)
            {
                //Actual ubsubscribement
                ThreadState current = ThreadStateList[index];
                current.Remove(e);
                e.BattleState.Unsubscribe(index);
            }
        }

        private static void AddNewThread()
        {
            //Adds a new threadstate
            ThreadStateList.Add(
                new ThreadState()
            );
        }

        internal static void Stop()
        {
            for (int i = 0; i < ThreadStateList.Count; i++)
                ThreadStateList[i].Abort();
            ThreadStateList.Clear();
        }

        #endregion BattleThread Private Members

        #region BattleThread Public Members

        /// <summary>
        /// Subscribes a IBattleArtificialIntelligence member on one of the designated lists
        /// to be processed.
        /// </summary>
        /// <param name="e"></param>
        public static void Subscribe(IBattleArtificialIntelligence e)
        {
            //If the object is subscribed cancel
            if (e.BattleState.IsSubscribed == true) return;
            int index = FindBestThreadStateToSubscribe();
            Subscribe(e, index);
        }

        /// <summary>
        /// Unsubscribes a IBattleArtificialIntelligence member on one of the designated lists
        /// to be processed.
        /// </summary>
        /// <param name="e"></param>
        public static void Unsubscribe(IBattleArtificialIntelligence e)
        {
            //If the object is not subscribed cancel
            if (e.BattleState.IsSubscribed == false) return;
            int index = e.BattleState.ThreadIndex;
            Unsubscribe(e, index);
        }

        #endregion BattleThread Public Members

        #region BattleThread Nested Classes

        /// <summary>
        /// Interfaces our BattleThread requires for a object to have
        /// </summary>
        public interface IBattleArtificialIntelligence
        {
            /// <summary>
            /// Battle state containing whether
            /// </summary>
            IBattlestate BattleState { get; }

            /// <summary>
            /// Processing statement used to fire
            /// </summary>
            void Process();
        }

        public class IBattlestate
        {
            private int _listupdate = 0;
            private byte _threadindex;

            internal byte ThreadIndex
            {
                get
                {
                    return (byte)(_threadindex - 1);
                }
            }

            public bool IsSubscribed
            {
                get
                {
                    return _threadindex > 0;
                }
            }

            public int LastUpdate
            {
                get
                {
                    return this._listupdate;
                }
                set
                {
                    this._listupdate = value;
                }
            }

            internal void Subscribe(int threadindex)
            {
                if (_threadindex > 0) return;
                _threadindex = (byte)(threadindex + 1);
                _listupdate = Environment.TickCount;
            }

            internal void Unsubscribe(int threadindex)
            {
                if (_threadindex == 0) return;
                _threadindex = 0;
            }
        }

        /// <summary>
        /// Thread state: a internal helper class containing there own
        /// processing list and thread.
        /// </summary>
        private class ThreadState
        {
            #region ThreadState Private Members

            /// <summary>
            /// A list of to be proccessed IBattleArtificialIntelligence
            /// </summary>
            private List<IBattleArtificialIntelligence> processinglist;

            /// <summary>
            /// The thread to execute it on.
            /// </summary>
            private Thread ProcessingThread;

            /// <summary>
            /// Processes all list members
            /// </summary>
            private void Process()
            {
                while (!Environment.HasShutdownStarted)
                {
                    try
                    {
                        //Process all mobs
                        for (int i = 0; i < this.processinglist.Count; i++)
                        {
                            try
                            {
                                IBattleArtificialIntelligence ai = this.processinglist[i];
                                ai.Process();
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

                        //Sleep one sec
                        Thread.Sleep(1);
                    }
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                }
            }

            #endregion ThreadState Private Members

            #region ThreadState Public Members

            /// <summary>
            /// Adds the requested IBattleArtificialIntelligence.
            /// </summary>
            /// <param name="e"></param>
            public void Add(IBattleArtificialIntelligence e)
            {
                lock (e)
                {
                    this.processinglist.Add(e);
                }
            }

            /// <summary>
            /// Removes the requested
            /// IBattleArtificialIntelligence
            /// </summary>
            /// <param name="e"></param>
            public void Remove(IBattleArtificialIntelligence e)
            {
                lock (e)
                {
                    this.processinglist.Remove(e);
                }
            }

            /// <summary>
            /// Checks if a threadstate contains the requested
            /// IBattleArtificialIntelligence
            /// </summary>
            /// <param name="e"></param>
            public void Contains(IBattleArtificialIntelligence e)
            {
                this.processinglist.Contains(e);
            }

            /// <summary>
            /// Gets the count of AI's that are subscribed
            /// </summary>
            public int Count
            {
                get
                {
                    return this.processinglist.Count;
                }
            }

            /// <summary>
            /// Get the default capacity
            /// </summary>
            public int Capacity
            {
                get
                {
                    return this.processinglist.Capacity;
                }
            }

            #endregion ThreadState Public Members

            #region ThreadState Contructor/Deconstructor

            /// <summary>
            /// Initialize a new thread state
            /// </summary>
            public ThreadState()
            {
                //Set new processing list with 20 stack
                this.processinglist = new List<IBattleArtificialIntelligence>(20);

                //Set the thread
                this.ProcessingThread = new Thread(new ThreadStart(Process));
                this.ProcessingThread.Name = "BattleThread";
                this.ProcessingThread.Start();
            }

            ~ThreadState()
            {
                try
                {
                    if (this.ProcessingThread != null && this.ProcessingThread.IsAlive)
                    {
                        GC.SuppressFinalize(this.ProcessingThread);
                        this.ProcessingThread.Abort();
                    }
                    this.processinglist.Clear();
                }
                finally
                {
                    if (this.ProcessingThread != null)
                        GC.ReRegisterForFinalize(this.ProcessingThread);
                    ProcessingThread = null;
                    processinglist = null;
                }
            }

            internal void Abort()
            {
                try
                {
                    if (this.ProcessingThread != null && this.ProcessingThread.IsAlive)
                    {
                        GC.SuppressFinalize(this.ProcessingThread);
                        this.ProcessingThread.Abort();
                    }
                }
                finally
                {
                    if (this.ProcessingThread != null)
                        GC.ReRegisterForFinalize(this.ProcessingThread);
                }
            }

            #endregion ThreadState Contructor/Deconstructor
        }

        #endregion BattleThread Nested Classes

        #region BattleThread Constructor/Descontructor

        static BattleThread()
        {
            //Add 4 threadstates to the procssing list
            for (int i = 0; i < 4; i++)
            {
                AddNewThread();
            }
        }

        #endregion BattleThread Constructor/Descontructor
    }
}