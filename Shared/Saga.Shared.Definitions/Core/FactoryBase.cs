using Saga.Configuration;
using System;
using System.Configuration;
using System.IO;

namespace Saga
{
    public abstract class FactoryBase : ManagerBase2
    {
        #region Ctor/Dtor

        public FactoryBase()
        {
        }

        ~FactoryBase()
        {
            Console.WriteLine("~ManagerBase");
        }

        #endregion Ctor/Dtor

        #region Delegates

        protected delegate void ReportProgress();

        #endregion Delegates

        #region Methods

        protected void LoadParameterizedStreamContent(string parameter, string format)
        {
            long end = Environment.TickCount;
            try
            {
                using (Stream DerivedFileStream = ObtainStream(parameter))
                {
                    ReportProgress ReportProgress = delegate()
                    {
                        if (Environment.TickCount - end > 25)
                        {
                            ConsoleUtils.ProgressBarShow((uint)DerivedFileStream.Position, (uint)DerivedFileStream.Length, this.Notification);
                            end = Environment.TickCount;
                        }
                    };

                    ConsoleUtils.ProgressBarShow((uint)DerivedFileStream.Position, (uint)DerivedFileStream.Length, this.Notification);
                    switch (format.ToLowerInvariant())
                    {
                        case "text/csv":
                            ParseAsCsvStream(DerivedFileStream, ReportProgress); break;
                        case "text/xml":
                            ParseAsXmlStream(DerivedFileStream, ReportProgress); break;
                        default:
                            if (!ParseAsOtherStream(DerivedFileStream, ReportProgress, format))
                                HostContext.AddUnhandeldException(new Exception(string.Format("Unsupported fileformat: {0}", this)));
                            break;
                    }
                    ConsoleUtils.ProgressBarHide(this.ReadyState);
                }
            }
            catch (IOException e)
            {
                //Trace.TraceError("Error opening: {0}", Filename);
                e.Source += this.ToString();
                HostContext.AddUnhandeldException(e);
            }
            catch (Exception e)
            {
                //Trace.TraceError("Unhandeld exception {0}", e);
                e.Source += this.ToString();
                HostContext.AddUnhandeldException(new SystemException(string.Format("Unhandeld exception occured on {0} {1}", parameter, format), e));
            }
        }

        protected virtual void ParseAsCsvStream(Stream stream, ReportProgress progress)
        {
            return;
        }

        protected virtual void ParseAsXmlStream(Stream stream, ReportProgress progress)
        {
            return;
        }

        protected virtual bool ParseAsOtherStream(Stream stream, ReportProgress progress, string format)
        {
            return false;
        }

        protected virtual Stream ObtainStream(string filename)
        {
            return new FileStream(filename, FileMode.Open, FileAccess.Read);
        }

        #endregion Methods

        #region Properties

        protected virtual string Notification
        {
            get
            {
                return string.Empty;
            }
        }

        protected virtual string ReadyState
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion Properties

        #region Static Methods

        public new static T ProvideManager<T>(string configurationSettings)
            where T : FactoryBase, new()
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
                    myObject = new T();
                }
                return myObject;
            }
            else
            {
                T myObject = new T();
                return myObject;
            }
        }

        #endregion Static Methods
    }
}