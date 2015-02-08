using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Saga.Managers
{
    public class QueedTask
    {
        public QueedTask(System.Threading.WaitCallback callback, object state, int delay)
        {
            this.state = state;
            this.delay = delay;
            this.callback = callback;

            lock (WorldTasks.Tasks)
            {
                Trace.TraceInformation("Queed new task");
                WorldTasks.Tasks.Add(this);
            }
        }

        public bool IsReady
        {
            get
            {
                return Environment.TickCount - starttime > delay;
            }
        }

        public object State
        {
            get
            {
                return state;
            }
        }

        internal System.Threading.WaitCallback callback;
        private int starttime = Environment.TickCount;
        private object state;
        private int delay;
    }

    public class WorldTasks : ManagerBase2
    {
        #region Ctor/Dtor

        public WorldTasks()
        {
        }

        ~WorldTasks()
        {
            Stop();
        }

        #endregion Ctor/Dtor

        #region Internal Members

        //Settings
        internal static Random _random = new Random();

        #endregion Internal Members

        #region Private Members

        internal static List<QueedTask> Tasks = new List<QueedTask>();

        /// <summary>
        /// Thread for general lifespan
        /// </summary>
        /// <remarks>
        /// Lifespan is used to do corpsers, regeneration and respawns.
        /// </remarks>
        private static Thread c;

        /// <summary>
        /// Thread for general mailservice
        /// </summary>
        /// <remarks>
        /// Lifespan is used to do corpsers, regeneration and respawns.
        /// </remarks>
        private static Thread e;

        #endregion Private Members

        #region Private Methods

        /// <summary>
        /// General game thread
        /// </summary>
        /// <remarks>
        /// This is our main GameThread. It is used to process
        /// all stuff we need to process. MobAI, Corpses, respawns, oxygen,
        /// hp recovery.
        /// </remarks>
        private static void GameThread()
        {
            while (!Environment.HasShutdownStarted)
            {
                try
                {
                    Saga.Tasks.LifeCycle.Process();
                    Saga.Tasks.Corpses.Process();
                    Saga.Tasks.Respawns.Process();
                    Saga.Tasks.DynamicWeather.Process();
                    Saga.Tasks.Maintenance.Process();
                    Saga.Tasks.WorldTime.Process();
                    Saga.Tasks.WorldRefresh.Process();
                    UpdateRemoveableTasks();
                    Thread.Sleep(1);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(1);
                }
            }
        }

        private static void UpdateRemoveableTasks()
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                QueedTask task = Tasks[i];
                if (task.IsReady)
                {
                    Trace.TraceInformation("Task is ready");
                    try
                    {
                        if (ThreadPool.QueueUserWorkItem(task.callback, task.State))
                            lock (Tasks)
                            {
                                Trace.TraceInformation("Removing task");
                                Tasks.RemoveAt(i);
                                i--;
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
            }
        }

        /// <summary>
        /// General MailService thread
        /// </summary>
        private static void MailService()
        {
            while (!Environment.HasShutdownStarted)
            {
                try
                {
                    Saga.Tasks.MailService.Process();
                    Thread.Sleep(1);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        #endregion Private Methods

        #region Protected Methods

        protected override void Initialize()
        {
            c = new Thread(GameThread);
            e = new Thread(MailService);
        }

        protected override void QuerySettings()
        {
            e.Priority = ThreadPriority.Normal;
            e.IsBackground = true;
        }

        #endregion Protected Methods

        #region Public Methods

        public void Start()
        {
            c.Start();
            e.Start();
        }

        public void Stop()
        {
            try
            {
                if (c != null && c.IsAlive)
                {
                    GC.SuppressFinalize(e);
                    c.Abort();
                }
            }
            catch (ThreadAbortException) { Console.WriteLine("Security exception"); }
            catch (System.Security.SecurityException) { Console.WriteLine("Security exception"); }
            finally
            {
                GC.ReRegisterForFinalize(c);
            }

            try
            {
                if (e != null && e.IsAlive)
                {
                    GC.SuppressFinalize(e);
                    e.Abort();
                }
            }
            catch (ThreadAbortException) { Console.WriteLine("Security exception"); }
            catch (System.Security.SecurityException) { Console.WriteLine("Security exception"); }
            finally
            {
                GC.ReRegisterForFinalize(e);
            }
        }

        #endregion Public Methods

        #region Public Properties

        public Random random
        {
            get
            {
                return _random;
            }
        }

        #endregion Public Properties
    }
}