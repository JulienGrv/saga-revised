using System;
using System.Collections.Generic;

namespace Saga
{
    public sealed class HostContext
    {
        private static TraceLog managers = new TraceLog("General", "Entire Application", 4);

        #region Ctor/Dtor

        private HostContext()
        {
        }

        ~HostContext()
        {
            if (OnClose != null)
                OnClose.Invoke(this, EventArgs.Empty);
        }

        #endregion Ctor/Dtor

        #region Internal Methods

        internal void Initialize()
        {
            managers.WriteLine("Calling initialize-event on factories & managers", "HostContext");
            if (OnInitialize != null)
                OnInitialize.Invoke(this, EventArgs.Empty);
        }

        internal void Load()
        {
            managers.WriteLine("Calling onload-event on factories & managers", "HostContext");
            if (OnLoad != null)
                OnLoad.Invoke(this, EventArgs.Empty);
        }

        internal void Loaded()
        {
            managers.WriteLine("Calling loaded-event on factories & managers", "HostContext");
            if (OnLoaded != null)
                OnLoaded.Invoke(this, EventArgs.Empty);
        }

        internal void BeforeQuerySettings()
        {
            managers.WriteLine("Calling beforequery-event on factories & managers", "HostContext");
            if (OnBeforeQuerySettings != null)
                OnBeforeQuerySettings.Invoke(this, EventArgs.Empty);
        }

        internal void AfterQuerySettings()
        {
            managers.WriteLine("Calling afterquery-event on factories & managers", "HostContext");
            if (OnAfterQuerySettings != null)
                OnAfterQuerySettings.Invoke(this, EventArgs.Empty);
        }

        internal void Close()
        {
            managers.WriteLine("Calling close-event on factories & managers", "HostContext");
            if (OnLoad != null)
                OnLoad.Invoke(this, EventArgs.Empty);
        }

        #endregion Internal Methods

        #region Private Static Members

        private static HostContext _current = null;
        private static List<Exception> _exceptionlist = new List<Exception>();

        #endregion Private Static Members

        #region Public Events

        public event EventHandler OnInitialize;

        public event EventHandler OnBeforeQuerySettings;

        public event EventHandler OnAfterQuerySettings;

        public event EventHandler OnLoad;

        public event EventHandler OnLoaded;

        public event EventHandler OnClose;

        #endregion Public Events

        #region Public Properties

        public List<Exception> UnhandeldExceptionList
        {
            get
            {
                return _exceptionlist;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void AddUnhandeldException(Exception e)
        {
            _exceptionlist.Add(e);
        }

        public void ClearUnhandeldExceptions()
        {
            _exceptionlist.Clear();
        }

        #endregion Public Methods

        #region Public Static Methods

        public static HostContext Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new HostContext();
                    return _current;
                }
                else
                {
                    return _current;
                }
            }
        }

        #endregion Public Static Methods
    }
}