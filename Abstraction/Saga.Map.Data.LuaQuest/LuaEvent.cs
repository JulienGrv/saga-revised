using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using Saga.Events;

namespace Saga.Map.Data.LuaQuest
{
    using Events = Saga.Factory.EventManager;
    using System.Diagnostics;
    using System.IO;
    public class EventInfo : Events.BaseEventInfo, IDisposable
    {

        #region Private Members

        private Lua myLua = new Lua();

        #endregion

        #region Public Methods

        protected override void OnInitialize(string filename)
        {
            try
            {
                //string filename = Path.Combine(BaseDirectory, string.Format("{0}.lua", cid));
                myLua.NewTable("Saga");
                myLua.RegisterFunction("Saga.Participate", null, typeof(EventTable).GetMethod("Participate"));
                myLua.RegisterFunction("Saga.Rewards", null, typeof(EventTable).GetMethod("Rewards"));

                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs, true))
                {
                    myLua.DoString(reader.ReadToEnd());
                }
            }
            catch (LuaException e)
            {
                __dbtracelog.WriteError("event", "In event {0} a error occured while loading: {1}", filename, e.Message);
                if (__dbtracelog.TraceVerbose)
                    Console.WriteLine("Error in event {0}, see tracelog", filename);
            }
            catch (FileNotFoundException)
            {
                __dbtracelog.WriteWarning("event", "Event {0} was not found and could not be loaded", filename);
                if (__dbtracelog.TraceVerbose)
                    Console.WriteLine("Error in event {0}, see tracelog", filename);
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("event", "In event {0} a error unhandeld error occured while loading: {1}", filename, e.Message);
                if (__dbtracelog.TraceVerbose)
                    Console.WriteLine("Error in event {0}, see tracelog", filename);
            }
        }


        protected override int OnEventParticipate(uint cid)
        {
            try
            {
                string luachunk = String.Format(System.Globalization.CultureInfo.InvariantCulture, "return EVENT_PARTICIPATE({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0), System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (LuaException e)
            {
                __dbtracelog.WriteError("event", "In event a error occured in 'check': {1}",  e.Message);
                if (__dbtracelog.TraceVerbose)
                    Console.WriteLine("Error in event see tracelog");
                return -1;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("event", "In event a error unhandeld error occured in 'check': {1}", e.Message);
                if (__dbtracelog.TraceVerbose)
                    Console.WriteLine("Error in event see tracelog");
                return -1;
            }
        }


        #endregion

        #region Constructor

        ~EventInfo()
        {
            Dispose();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if( myLua != null )
                myLua.Dispose();
        }

        #endregion

        #region ITrace Members

        static TraceLog __dbtracelog = new TraceLog("Lua", "Trace switch for the lua plugin", 4);       

        #endregion

    }
}
