using Saga.Configuration;
using Saga.Map.Configuration;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Saga.Factory
{
    /// <summary>
    /// Provides a manager for event information
    /// </summary>
    public class EventManager : FactoryBase
    {
        #region Ctor/Dtor

        public EventManager()
        {
        }

        ~EventManager()
        {
            this._eventdates = null;
            _constructorInfo = null;
        }

        #endregion Ctor/Dtor

        #region Internal Members

        /// <summary>
        /// Lookup table of events by event id
        /// </summary>
        protected Dictionary<byte, EventDateTime> _eventdates;

        /// <summary>
        /// Chached constructor information for optimalisation
        /// </summary>
        protected static ConstructorInfo _constructorInfo;

        #endregion Internal Members

        #region Protected Methods

        protected override void QuerySettings()
        {
            EventSettings section = (EventSettings)ConfigurationManager.GetSection("Saga.Factory.Events");
            if (section != null)
            {
                object mytype;
                if (CoreService.TryFindType(section.Provider, out mytype))
                {
                    if (mytype is BaseEventInfo)
                    {
                        Type _eventtype = mytype.GetType();
                        _constructorInfo = _eventtype.GetConstructor(new Type[] { });
                        if (_constructorInfo == null)
                            WriteError("Event Manager", "Constructor type is not found on the event type of: {0}", _eventtype.FullName);
                    }
                    else
                    {
                        WriteError("Event Manager", "Provided type is not a valid event base");
                    }
                }
                else
                {
                    WriteError("Event Manager", "Provided type was not found");
                }
            }
            else
            {
                WriteError("Event Manager", "Event section was not found please configure the quest configuration");
            }
        }

        protected override void Initialize()
        {
            _eventdates = new Dictionary<byte, EventDateTime>();
        }

        protected override void Load()
        {
            EventSettings section = (EventSettings)ConfigurationManager.GetSection("Saga.Factory.Events");
            if (section != null)
            {
                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("Event Manager", "Loading event information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }
            }
        }

        protected override void ParseAsCsvStream(Stream stream, FactoryBase.ReportProgress ProgressReport)
        {
            using (StreamReader c = new StreamReader(stream))
            {
                c.ReadLine();
                while (c.Peek() > 0)
                {
                    //REPORT PROGRESS
                    ProgressReport.Invoke();
                    String row = c.ReadLine();
                    String[] fields = row.Split(',');

                    try
                    {
                        int Flags = 0;
                        byte eventid = Convert.ToByte(fields[0]);            //Unique id of the event
                        byte UseDate = Convert.ToByte(fields[6]);            //1   - Use a date to check
                        byte UseTime = Convert.ToByte(fields[7]);            //2   - Use a timespan to check
                        byte EnableMonday = Convert.ToByte(fields[8]);       //4   - Is active on monday
                        byte EnableThuesday = Convert.ToByte(fields[9]);     //8   - Is active on thuesday
                        byte EnableWednesday = Convert.ToByte(fields[10]);   //16  - Is active on wednessday
                        byte EnableThursday = Convert.ToByte(fields[11]);    //32  - Is active on thursday
                        byte EnableFriday = Convert.ToByte(fields[12]);      //64  - Is active on friday
                        byte EnableSaturday = Convert.ToByte(fields[13]);    //128 - Is active on saturday
                        byte EnableSunday = Convert.ToByte(fields[14]);      //256 - Is active on sunday
                        string EventName = fields[1];                        //Name of the event

                        if (UseDate == 1)
                            Flags |= 1;
                        if (UseTime == 1)
                            Flags |= 2;
                        if (EnableMonday == 1)
                            Flags |= 4;
                        if (EnableThuesday == 1)
                            Flags |= 8;
                        if (EnableWednesday == 1)
                            Flags |= 16;
                        if (EnableThursday == 1)
                            Flags |= 32;
                        if (EnableFriday == 1)
                            Flags |= 64;
                        if (EnableSaturday == 1)
                            Flags |= 128;
                        if (EnableSunday == 1)
                            Flags |= 256;

                        DateTime eventstart = DateTime.Parse(fields[2], CultureInfo.InvariantCulture);
                        DateTime eventend = DateTime.Parse(fields[3], CultureInfo.InvariantCulture);
                        TimeSpan timestart = TimeSpan.Parse(fields[4]);
                        TimeSpan timetend = TimeSpan.Parse(fields[5]);

                        EventDateTime date = new EventDateTime();
                        date.end = eventend.Add(timetend);
                        date.start = eventstart.Add(timestart);
                        date.eventname = EventName;
                        date.Flags = Flags;
                        if (CanCheckTimespan(date))
                            date.IsActive = IsActiveToday(date) && IsBetweenDateTime(date) && IsBetweenTimeStamp(date);
                        else
                            date.IsActive = true;

                        _eventdates.Add(eventid, date);
                    }
                    catch (Exception e)
                    {
                        HostContext.AddUnhandeldException(e);
                    }
                }
            }
        }

        #endregion Protected Methods

        #region Internal Methods

        private int LastTick = Environment.TickCount;

        internal void CheckEvents()
        {
            if (Environment.TickCount - LastTick > 1000)
            {
                LastTick = Environment.TickCount;
                foreach (KeyValuePair<byte, EventDateTime> pair in _eventdates)
                {
                    if (pair.Value.IsActive == true)
                        Deactivate(pair.Value);
                    else
                        Activate(pair.Value);
                }
            }
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Creates a instance of the associated type and returns
        /// it's base implamentation.
        /// </summary>
        /// <param name="e">Out eventinfo</param>
        /// <returns></returns>
        private bool CreateInstance(out BaseEventInfo e)
        {
            object a = _constructorInfo.Invoke(new object[] { });
            e = (BaseEventInfo)a;
            return e != null;
        }

        /// <summary>
        /// Broadcasts a string to all actors
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        private void DoGlobalAnnounchment(string message)
        {
            //GENERATE BROADCAST
            SMSG_SENDCHAT spkt = new SMSG_SENDCHAT();
            spkt.Name = "GM";
            spkt.MessageType = SMSG_SENDCHAT.MESSAGE_TYPE.SYSTEM_MESSAGE_RED;
            spkt.Message = message;

            //FORWARD THE BROADCAST
            foreach (Character characterTarget in Saga.Tasks.LifeCycle.Characters)
            {
                try
                {
                    spkt.SessionId = characterTarget.id;
                    characterTarget.client.Send((byte[])spkt);
                }
                catch (Exception)
                {
                    //DON'T HANDLE THE EXCEPTION
                }
            }
        }

        /// <summary>
        /// Checks if a event is activated today
        /// </summary>
        /// <param name="e">event to check</param>
        /// <returns>True if the event is scheduled for activation of today</returns>
        private bool IsActiveToday(EventDateTime e)
        {
            DayOfWeek day = DateTime.Today.DayOfWeek;
            if (day == DayOfWeek.Monday)
                return (e.Flags & 4) == 4;
            else if (day == DayOfWeek.Tuesday)
                return (e.Flags & 8) == 8;
            else if (day == DayOfWeek.Wednesday)
                return (e.Flags & 16) == 16;
            else if (day == DayOfWeek.Thursday)
                return (e.Flags & 32) == 32;
            else if (day == DayOfWeek.Friday)
                return (e.Flags & 64) == 64;
            else if (day == DayOfWeek.Saturday)
                return (e.Flags & 128) == 128;
            else if (day == DayOfWeek.Sunday)
                return (e.Flags & 256) == 256;
            return false;
        }

        /// <summary>
        /// Checks if a event is between the correctly setted timestamps
        /// </summary>
        /// <param name="e">event to check</param>
        /// <returns>True if the event is between correct timestamps</returns>
        private bool IsBetweenTimeStamp(EventDateTime e)
        {
            if ((e.Flags & 2) == 2)
            {
                TimeSpan time = DateTime.Now.TimeOfDay;
                return time > e.start.TimeOfDay && time < e.end.TimeOfDay;
            }

            return true;
        }

        /// <summary>
        /// Checks if a event is between the correctly setted dates
        /// </summary>
        /// <param name="e">event to check</param>
        /// <returns>True if the event is between correct dates</returns>
        private bool IsBetweenDateTime(EventDateTime e)
        {
            if ((e.Flags & 1) == 1)
            {
                DateTime today = DateTime.Today;
                return today >= e.start.Date && today <= e.end.Date;
            }

            return true;
        }

        /// <summary>
        /// Checks if the event is scheduled for periodic updates
        /// </summary>
        /// <param name="e">event to check</param>
        /// <returns></returns>
        private bool CanCheckTimespan(EventDateTime e)
        {
            return (e.Flags & 3) == 3;
        }

        /// <summary>
        /// Activates the event and announches the change
        /// </summary>
        /// <param name="e">event to activate</param>
        internal void Activate(EventDateTime e)
        {
            bool result = true;
            if (CanCheckTimespan(e))
                result = IsBetweenTimeStamp(e) && IsBetweenDateTime(e) && IsActiveToday(e);

            if (result == true && e.IsActive == false)
            {
                e.IsActive = true;
                Console.WriteLine("Activate");
                string message = string.Format(CultureInfo.InvariantCulture, "Event {0} started", e.eventname);
                DoGlobalAnnounchment(message);
            }
        }

        /// <summary>
        /// Deactivates the event and announches the change
        /// </summary>
        /// <param name="e">event to activate</param>
        internal void Deactivate(EventDateTime e)
        {
            bool result = true;
            if (CanCheckTimespan(e))
                result = IsBetweenTimeStamp(e) && IsBetweenDateTime(e) && IsActiveToday(e);

            if (result == false && e.IsActive == true)
            {
                e.IsActive = false;
                Console.WriteLine("Deactivate");
                string message = string.Format(CultureInfo.InvariantCulture, "Event {0} ended", e.eventname);
                DoGlobalAnnounchment(message);
            }
        }

        /// <summary>
        /// Creates a event information from a filename
        /// </summary>
        /// <param name="fileName">Filename to open</param>
        /// <returns>Null if failed</returns>
        private BaseEventInfo FromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                BaseEventInfo file;
                if (CreateInstance(out file))
                    file.OnInitialize(fileName);
                return file;
            }
            else
            {
                return null;
            }
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Checks wether a event has started
        /// </summary>
        /// <param name="id">Id of the event to check</param>
        /// <returns>True if the event has started</returns>
        public bool IsStarted(byte id)
        {
            EventDateTime date;
            if (this._eventdates.TryGetValue(id, out date))
                return date.IsActive;
            return false;
        }

        /// <summary>
        /// Lists a enumaration of started events
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> GetVisibleEvents()
        {
            foreach (KeyValuePair<byte, EventDateTime> pair in _eventdates)
                if (IsStarted(pair.Key))
                    yield return pair.Key;
        }

        /// <summary>
        /// Opens the event
        /// </summary>
        /// <param name="eventid">Id of the event to open</param>
        /// <returns></returns>
        public BaseEventInfo FindEventInformation(byte eventid)
        {
            string filename = Server.SecurePath("~/Events/{0}.lua", eventid);
            return FromFile(filename);
        }

        #endregion Public Methods

        #region Protected Properties

        /// <summary>
        /// Get the notification string.
        /// </summary>
        /// <remarks>
        /// We used notification strings from the resource files. This way it's easier
        /// for us to make a multilanguagable server. And a golden rule in C# is that
        /// strings are slow, so rather have it instanted once by the resource file than
        /// reallocting a new string for every progress report.
        /// </remarks>
        protected override string Notification
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_ZONE; }
        }

        /// <summary>
        /// Get the readystate string.
        /// </summary>
        /// <remarks>
        /// We used readystate strings from the resource files. This way it's easier
        /// for us to make a multilanguagable server. And a golden rule in C# is that
        /// strings are slow, so rather have it instanted once by the resource file than
        /// reallocting a new string for every progress report.
        /// </remarks>
        protected override string ReadyState
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_ZONE; }
        }

        #endregion Protected Properties

        #region Nested Classes/Structures

        public class EventDateTime
        {
            public string eventname;
            public DateTime start;
            public DateTime end;
            public int Flags;
            public bool IsActive;
        }

        public abstract class BaseEventInfo
        {
            protected internal abstract void OnInitialize(string filename);

            protected internal abstract int OnEventParticipate(uint cid);
        }

        #endregion Nested Classes/Structures
    }
}