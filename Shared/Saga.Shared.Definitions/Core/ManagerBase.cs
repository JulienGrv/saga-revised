using Saga.Configuration;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace Saga
{
    public class ManagerBase2
    {
        private static TraceLog managers = new TraceLog("General", "Entire Application", 4);

        #region Ctor/Dtor

        public ManagerBase2()
        {
            HostContext.Current.OnInitialize += delegate(object sender, EventArgs a)
            {
                try
                {
                    Initialize();
                }
                catch
                {
                }
                if (OnInitialize != null)
                {
                    try
                    {
                        OnInitialize.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception)
                    {
                    }
                }
            };

            HostContext.Current.OnBeforeQuerySettings += delegate(object sender, EventArgs a)
            {
                if (OnBeforeQuerySettings != null)
                {
                    try
                    {
                        OnBeforeQuerySettings.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception)
                    {
                    }
                }
                try
                {
                    QuerySettings();
                }
                catch (Exception)
                {
                }
            };

            HostContext.Current.OnAfterQuerySettings += delegate(object sender, EventArgs a)
            {
                if (OnAfterQuerySettings != null)
                {
                    try
                    {
                        OnAfterQuerySettings.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception)
                    {
                    }
                }
            };

            HostContext.Current.OnLoad += delegate(object sender, EventArgs a)
            {
                if (OnLoad != null)
                {
                    try
                    {
                        OnLoad.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception)
                    {
                    }
                }
                try
                {
                    Load();
                }
                catch (Exception)
                {
                }
            };

            HostContext.Current.OnLoad += delegate(object sender, EventArgs a)
            {
                try
                {
                    FinishedLoading();
                }
                catch (Exception)
                {
                }
                if (OnFinishedLoad != null)
                {
                    try
                    {
                        OnFinishedLoad.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception)
                    {
                    }
                }
            };
        }

        #endregion Ctor/Dtor

        #region Events

        public event EventHandler OnInitialize;

        public event EventHandler OnBeforeQuerySettings;

        public event EventHandler OnAfterQuerySettings;

        public event EventHandler OnLoad;

        public event EventHandler OnFinishedLoad;

        #endregion Events

        #region Methods

        protected virtual void Load()
        {
        }

        protected virtual void FinishedLoading()
        {
        }

        protected virtual void QuerySettings()
        {
        }

        protected virtual void Initialize()
        {
        }

        #endregion Methods

        #region ITrace Members

        protected void WriteInformation(string category, string message)
        {
            managers.WriteInformation(category, message);
        }

        protected void WriteInformation(string category, string message, params object[] format)
        {
            managers.WriteInformation(category, message, format);
        }

        protected void WriteWarning(string category, string message)
        {
            managers.WriteWarning(category, message);
        }

        protected void WriteWarning(string category, string message, params object[] format)
        {
            managers.WriteWarning(category, message, format);
        }

        protected void WriteError(string category, string message)
        {
            managers.WriteError(category, message);
        }

        protected void WriteError(string category, string message, params object[] format)
        {
            managers.WriteError(category, message, format);
        }

        protected void WriteLine(string category, string message)
        {
            managers.WriteLine(category, message);
        }

        protected void WriteLine(string category, string message, params object[] format)
        {
            managers.WriteLine(category, message, format);
        }

        #endregion ITrace Members

        #region Properties

        public HostContext HostContext
        {
            get
            {
                return HostContext.Current;
            }
        }

        #endregion Properties

        #region Static Methods

        public static void SetTraceLog(TraceLog tracelog)
        {
            managers = tracelog;
        }

        public static T ProvideManager<T>(string configurationSettings)
            where T : ManagerBase2, new()
        {
            ManagerProviderBaseConfiguration configurationSection =
                ConfigurationManager.GetSection(configurationSettings) as ManagerProviderBaseConfiguration;
            if (configurationSection != null)
            {
                object a;
                CoreService.TryFindType(configurationSection.DerivedType, out a);

                T myObject = a as T;
                if (a == null)
                {
                    managers.WriteWarning("XmlSections", "Section {0} was not found using default manager/factory", configurationSettings);
                    myObject = new T();
                }
                return myObject;
            }
            else
            {
                managers.WriteWarning("XmlSections", "Section {0} was not found using default manager/factory", configurationSettings);
                T myObject = new T();
                return myObject;
            }
        }

        public static T ProvideManagerFromTypeString<T>(string typestring)
            where T : ManagerBase2, new()
        {
            object a;
            CoreService.TryFindType(typestring, out a);

            T myObject = a as T;
            if (a == null) myObject = new T();
            return myObject;
        }

        #endregion Static Methods
    }

    public class TraceLog : ITrace
    {
        private int errorcount = 0;

        #region ITrace Members

        public void WriteInformation(string category, string message)
        {
            if (managers.TraceInfo)
                __WriteLine(category, _levelInfo, message);
        }

        public void WriteInformation(string category, string message, params object[] format)
        {
            if (managers.TraceInfo)
                __WriteLine(category, _levelInfo, message, format);
        }

        public void WriteWarning(string category, string message)
        {
            if (managers.TraceWarning)
                __WriteLine(category, _levelWarning, message);
        }

        public void WriteWarning(string category, string message, params object[] format)
        {
            if (managers.TraceWarning)
                __WriteLine(category, _levelWarning, message, format);
        }

        public void WriteError(string category, string message)
        {
            if (managers.TraceError)
            {
                errorcount++;
                __WriteLine(category, _levelError, message);
            }
        }

        public void WriteError(string category, string message, params object[] format)
        {
            if (managers.TraceError)
            {
                errorcount++;
                __WriteLine(category, _levelError, message, format);
            }
        }

        public void WriteLine(string category, string message)
        {
            if (managers.TraceVerbose)
                __WriteLine(category, _levelVerbose, message);
        }

        public void WriteLine(string category, string message, params object[] format)
        {
            if (managers.TraceVerbose)
                __WriteLine(category, _levelVerbose, message, format);
        }

        #endregion ITrace Members

        #region ITrace Members

        void ITrace.WriteInformation(string category, string message)
        {
            WriteInformation(category, message);
        }

        void ITrace.WriteInformation(string category, string message, params object[] format)
        {
            WriteInformation(category, message, format);
        }

        void ITrace.WriteWarning(string category, string message)
        {
            WriteWarning(category, message);
        }

        void ITrace.WriteWarning(string category, string message, params object[] format)
        {
            WriteWarning(category, message, format);
        }

        void ITrace.WriteError(string category, string message)
        {
            WriteError(category, message);
        }

        void ITrace.WriteError(string category, string message, params object[] format)
        {
            WriteError(category, message, format);
        }

        void ITrace.WriteLine(string category, string message)
        {
            WriteLine(category, message);
        }

        void ITrace.WriteLine(string category, string message, params object[] format)
        {
            WriteLine(category, message, format);
        }

        #endregion ITrace Members

        #region Private Members

        private TraceSwitch managers = new TraceSwitch("General", "Entire Application");
        private const string _levelVerbose = "verbose   ";
        private const string _levelWarning = "warning   ";
        private const string _levelError = "error     ";
        private const string _levelInfo = "ïnfo      ";

        private void __WriteLine(string category, string level, string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(level);
            builder.Append(message);
            System.Diagnostics.Trace.WriteLine(builder.ToString(), category);
        }

        private void __WriteLine(string category, string level, string message, params object[] format)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(level);
                builder.AppendFormat(message, format);
                System.Diagnostics.Trace.WriteLine(builder.ToString(), category);
            }
            catch (FormatException)
            {
                System.Diagnostics.Trace.Fail("Cannot format message");
            }
        }

        #endregion Private Members

        #region Public Members

        public int CountOfErrors
        {
            get
            {
                return errorcount;
            }
        }

        #endregion Public Members

        #region Public Properties

        public bool TraceVerbose
        {
            get
            {
                return managers.TraceVerbose;
            }
        }

        public bool TraceError
        {
            get
            {
                return managers.TraceError;
            }
        }

        public bool TraceWarning
        {
            get
            {
                return managers.TraceWarning;
            }
        }

        public bool TraceInfo
        {
            get
            {
                return managers.TraceInfo;
            }
        }

        public int LogLevel
        {
            get
            {
                return (int)managers.Level;
            }
        }

        #endregion Public Properties

        #region Constructor

        public TraceLog(string switchname, string description)
        {
            managers = new TraceSwitch(switchname, description);
        }

        public TraceLog(string switchname, string description, int defaultlevel)
        {
            managers = new TraceSwitch(switchname, description, defaultlevel.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        #endregion Constructor
    }

    public interface ITrace
    {
        void WriteInformation(string category, string message);

        void WriteInformation(string category, string message, params object[] format);

        void WriteWarning(string category, string message);

        void WriteWarning(string category, string message, params object[] format);

        void WriteError(string category, string message);

        void WriteError(string category, string message, params object[] format);

        void WriteLine(string category, string message);

        void WriteLine(string category, string message, params object[] format);
    }
}