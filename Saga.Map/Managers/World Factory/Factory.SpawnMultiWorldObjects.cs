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
    public class SpawnMultiWorldObjects : FactoryBase
    {
        #region Ctor/Dtor

        public SpawnMultiWorldObjects()
        {
        }

        #endregion Ctor/Dtor

        #region Protected Members

        protected bool SendSpawnPacket = false;
        protected BooleanSwitch mobspawnsaswarnings = new BooleanSwitch("MobSpawnsAsWarnings", "Forces to detect npc spawns as warnings instead of errors", "0");

        #endregion Protected Members

        #region Protected Methods

        protected override void Load()
        {
            Trace.TraceInformation("Loading multi-worldspawn objects");
            SpawnMultiWorldObjectSettings section = (SpawnMultiWorldObjectSettings)ConfigurationManager.GetSection("Saga.Factory.SpawnMultiWorldObjects");
            if (section != null)
            {
                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("WorldObjectsFactory (multi)", "Loading world objects from {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(element.Path, element.Reader);
                }
            }
            SendSpawnPacket = true;
        }

        protected override void ParseAsCsvStream(Stream stream, FactoryBase.ReportProgress ProgressReport)
        {
            using (StreamReader c = new StreamReader(stream))
            {
                c.ReadLine();
                while (c.Peek() > 0)
                {
                    ProgressReport.Invoke();
                    String row = c.ReadLine();
                    String[] fields = row.Split(',');

                    Zone zone;
                    byte count = byte.Parse(fields[5], System.Globalization.NumberFormatInfo.InvariantInfo);
                    if (Singleton.Zones.TryGetZone(uint.Parse(fields[1], NumberFormatInfo.InvariantInfo), out zone))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            float x = float.Parse(fields[2], System.Globalization.NumberFormatInfo.InvariantInfo);
                            float y = float.Parse(fields[3], System.Globalization.NumberFormatInfo.InvariantInfo);
                            float z = float.Parse(fields[4], System.Globalization.NumberFormatInfo.InvariantInfo);
                            int spawnrange = int.Parse(fields[6], System.Globalization.NumberFormatInfo.InvariantInfo);
                            uint modelid = uint.Parse(fields[0], System.Globalization.NumberFormatInfo.InvariantInfo);

                            MapObject regionObject;
                            Point location = GeneratePointInRange(new Point(x, y, z), spawnrange);
                            bool isspawned = Singleton.Templates.SpawnNpcInstance(modelid, location, rand.Next(0, ushort.MaxValue), zone, out regionObject);
                            if (!isspawned)
                            {
                                if (mobspawnsaswarnings.Enabled)
                                    WriteWarning("WorldObjectsFactory (multi)", "Cannot initialize {1} {0}", fields[0], "npc");
                                else
                                    WriteError("WorldObjectsFactory (multi)", "Cannot initialize {1} {0}", fields[0], "npc");
                            }
                        }
                    }
                }
            }
        }

        private static Random rand = new Random();

        protected Point GeneratePointInRange(Point startpoint, int mrange)
        {
            if (mrange > 0)
            {
                float range = (float)rand.Next(0, mrange);
                ushort yaw = (ushort)rand.Next(0, ushort.MaxValue);

                Point Loc = startpoint;
                double rad = Saga.Structures.Yaw.ToRadiants(yaw);
                Loc.x += (float)(range * Math.Cos(rad));
                Loc.y += (float)(range * Math.Sin(rad));
                Loc.z = startpoint.z;
                return Loc;
            }
            else
            {
                return startpoint;
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public void Reload()
        {
            try
            {
                Load();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }

            //Clear the exception list
            HostContext.UnhandeldExceptionList.Clear();
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
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_MOBSPAWNS; }
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
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_MOBSPAWNS; }
        }

        #endregion Protected Properties
    }
}