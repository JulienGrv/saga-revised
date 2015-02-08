using Saga.Configuration;
using Saga.Map;
using Saga.Map.Configuration;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Saga.Factory
{
    public class SpawnWorldObjects : FactoryBase
    {
        #region Ctor/Dtor

        public SpawnWorldObjects()
        {
        }

        #endregion Ctor/Dtor

        #region Protected Members

        protected BooleanSwitch npcspawnsaswarnings = new BooleanSwitch("NpcSpawnsAsWarnings", "Forces to detect npc spawns as warnings instead of errors", "0");

        #endregion Protected Members

        #region Protected Methods

        protected override void Load()
        {
            Trace.TraceInformation("Loading worldspawn objects");
            SpawnWorldObjectSettings section = (SpawnWorldObjectSettings)ConfigurationManager.GetSection("Saga.Factory.SpawnWorldObjects");
            if (section != null)
            {
                foreach (FactoryFileElement element in section.FolderItems)
                {
                    Trace.TraceInformation("Loading worldspawn objects information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(element.Path, element.Reader);
                }
            }
        }

        protected override void ParseAsCsvStream(Stream stream, FactoryBase.ReportProgress ProgressReport)
        {
            byte Line = 0;
            using (StreamReader c = new StreamReader(stream))
            {
                c.ReadLine();
                while (c.Peek() > 0)
                {
                    Line++;
                    ProgressReport.Invoke();
                    String row = c.ReadLine();
                    String[] fields = row.Split(',');

                    try
                    {
                        Zone zone;
                        if (!Singleton.Zones.TryGetZone(uint.Parse(fields[0], NumberFormatInfo.InvariantInfo), out zone))
                        {
                            WriteError("WorldObjectsFactory (single)", "Zone of id {0} was not found", fields[0]);
                        }

                        float x = float.Parse(fields[1], NumberFormatInfo.InvariantInfo);
                        float y = float.Parse(fields[2], NumberFormatInfo.InvariantInfo);
                        float z = float.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                        uint modelid = uint.Parse(fields[4], NumberFormatInfo.InvariantInfo);
                        byte ismapitem = byte.Parse(fields[5], NumberFormatInfo.InvariantInfo);
                        int yaw = int.Parse(fields[6], NumberFormatInfo.InvariantInfo);

                        MapObject regionObject;

                        bool iscreated = ismapitem != 1
                            ? Singleton.Templates.SpawnNpcInstance(modelid, new Point(x, y, z), yaw, zone, out regionObject)
                            : Singleton.Templates.SpawnItemInstance(modelid, new Point(x, y, z), yaw, zone, out regionObject);

                        if (!iscreated)
                        {
                            if (npcspawnsaswarnings.Enabled)
                                WriteWarning("WorldObjectsFactory (single)", "Cannot initialize {1} {0}", fields[4], ismapitem == 1 ? "actionobject" : "npc");
                            else
                                WriteError("WorldObjectsFactory (single)", "Cannot initialize {1} {0}", fields[4], ismapitem == 1 ? "actionobject" : "npc");
                        }
                    }
                    catch (FormatException)
                    {
                        WriteError("WorldObjectsFactory (single)", "Incorrect format at line {0}: {1}", Line, row);
                    }
                }
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Reloads all the npc's from the files
        /// </summary>
        public void Reload()
        {
            try
            {
                //Start respawning new mobs
                Load();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                //Do nothing
            }

            //Clear the exception list
            HostContext.UnhandeldExceptionList.Clear();
        }

        #endregion Public Methods

        #region Public Methods

        public void Respawn()
        {
            try
            {
                Load();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        #endregion Public Methods

        #region Protected Properties

        protected override string Notification
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_WORLDOBJECTSSPAWN; }
        }

        protected override string ReadyState
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_WORLDOBJECTSSPAWNS; }
        }

        #endregion Protected Properties
    }
}