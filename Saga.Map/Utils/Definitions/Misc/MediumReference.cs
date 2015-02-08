using System.Threading;

namespace System.Collections.Generics
{
    /// <summary>
    /// Generic version of the Medium Reference
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Medium Refernece used to cache items on a timer based
    /// activation. The more activation invoked the longer the
    /// lifespan will be for disposing.
    /// </remarks>
    public class MediumReference<T> where T : class
    {
        private T _Target;
        private Timer timer;

        public bool IsAlive
        {
            get
            {
                return _Target != null;
            }
        }

        public T Target
        {
            get
            {
                timer.Change(10000, 0);
                return this._Target;
            }
            set
            {
                timer.Change(10000, 0);
                _Target = value;
            }
        }

        public MediumReference(T target)
        {
            timer = new Timer(timer_Elapsed, null, 10000, 0);
            _Target = target;
        }

        private void timer_Elapsed(object state)
        {
            lock (_Target)
            {
                GC.ReRegisterForFinalize(_Target);
                _Target = null;
                timer.Dispose();
                timer = null;
            }
        }
    }
}