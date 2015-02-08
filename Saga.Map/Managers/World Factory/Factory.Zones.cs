using Saga.Configuration;
using Saga.Enumarations;
using Saga.Map;
using Saga.Map.Configuration;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Templates;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Saga.Factory
{
    /// <summary>
    /// Factory to hold all hosted zones.
    /// </summary>
    public class Zones : FactoryBase
    {
        #region Ctor/Dtor

        /// <summary>
        /// Initializes a new Zone Factory
        /// </summary>
        public Zones()
        {
        }

        #endregion Ctor/Dtor

        #region Internal Members

        /// <summary>
        /// Default loopup table for all hosted zones/maps for the current Zone-Server.
        /// </summary>
        /// <remarks>
        /// This member should be made internal protected in a more advancanced release
        /// client and thus should be completly cloacked for the scripts assembly we
        /// compile on the fly.
        /// </remarks>
        public Dictionary<uint, Zone> maps;

        /// <summary>
        /// Directory to search for the heightmaps
        /// </summary>
        public string dirHeightmap = "";

        #endregion Internal Members

        #region Protected Members

        protected bool TryFindZoneString(string name, out Zone zone)
        {
            object bzone;
            CoreService.TryFindType(name, out bzone);
            zone = bzone as Zone;
            return zone != null;
        }

        protected void SetMembers(Zone zone, byte zoneid, HeightMap heightmap, ZoneType type, byte cathelaya_map, Point cathelaya_location, byte promise_map, Point promise_location, uint regioncode)
        {
            zone.Map = zoneid;
            zone.Regiontree = new Regiontree();
            zone.Type = type;
            zone.Heightmap = heightmap;
            zone.CathelayaLocation = new WorldCoordinate(cathelaya_location, cathelaya_map);
            zone.ProsmiseLocation = new WorldCoordinate(promise_location, promise_map);
            zone.RegionCode = regioncode;
        }

        #endregion Protected Members

        #region Protected Methods

        /// <summary>
        /// Initializes all member variables
        /// </summary>
        protected override void Initialize()
        {
            maps = new Dictionary<uint, Zone>();
        }

        /// <summary>
        /// Queries configuration file for configurable settings
        /// </summary>
        protected override void QuerySettings()
        {
            ZoneSettings section = (ZoneSettings)ConfigurationManager.GetSection("Saga.Factory.Zones");
            if (section != null)
            {
                if (section.Directory.Length == 0)
                    WriteError("ZoneFactory", "Heightmap directory is not configured");
                else
                    dirHeightmap = Saga.Structures.Server.SecurePath(section.Directory);
            }
            else
            {
                WriteWarning("XmlSections", "Section {0} was not found", "Saga.Factory.Zones");
            }
        }

        /// <summary>
        /// Loads all listed files that contains folder information
        /// </summary>
        protected override void Load()
        {
            ZoneSettings section = (ZoneSettings)ConfigurationManager.GetSection("Saga.Factory.Zones");
            if (section != null)
            {
                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("ZoneFactory", "Loading zone information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }
            }
            else
            {
                WriteWarning("XmlSections", "Section {0} was not found", "Saga.Factory.Zones");
            }
        }

        /// <summary>
        /// Default included event that invokes a csv based stream.
        /// </summary>
        /// <param name="stream">Stream to read data from</param>
        /// <param name="ProgressReport">Class to report the state of reading</param>
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

                    //LOAD A HEIGHTMAP
                    HeightMap heightmap = new HeightMap();
                    HeightMap.HeightMapInfo info = new HeightMap.HeightMapInfo();

                    try
                    {
                        //FILL OUT HEIGHTMAP INFORMATION
                        string filename = Path.Combine(Environment.CurrentDirectory, dirHeightmap);
                        if (fields[10].Length > 0)
                        {
                            info.location[0] = float.Parse(fields[12], NumberFormatInfo.InvariantInfo);
                            info.location[1] = float.Parse(fields[13], NumberFormatInfo.InvariantInfo);
                            info.location[2] = float.Parse(fields[14], NumberFormatInfo.InvariantInfo);
                            info.scale[0] = int.Parse(fields[15], NumberFormatInfo.InvariantInfo);
                            info.scale[1] = int.Parse(fields[16], NumberFormatInfo.InvariantInfo);
                            info.scale[2] = int.Parse(fields[17], NumberFormatInfo.InvariantInfo);
                            info.size = int.Parse(fields[11], NumberFormatInfo.InvariantInfo);
                            filename = Path.Combine(filename, fields[10]);

                            //IF HEIGHTMAP IS NOT LOADED PROCEED
                            HeightMap.LoadFromFile(filename, info, out heightmap);
                        }

                        float catheleyax = float.Parse(fields[2], NumberFormatInfo.InvariantInfo);
                        float catheleyay = float.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                        float catheleyaz = float.Parse(fields[4], NumberFormatInfo.InvariantInfo);
                        float promisex = float.Parse(fields[6], NumberFormatInfo.InvariantInfo);
                        float promisey = float.Parse(fields[7], NumberFormatInfo.InvariantInfo);
                        float promizez = float.Parse(fields[8], NumberFormatInfo.InvariantInfo);
                        byte catheleyamap = byte.Parse(fields[5], NumberFormatInfo.InvariantInfo);
                        byte promisemap = byte.Parse(fields[9], NumberFormatInfo.InvariantInfo);
                        uint regioncode = uint.Parse(fields[19], NumberFormatInfo.InvariantInfo);
                        uint zoneid = uint.Parse(fields[0], NumberFormatInfo.InvariantInfo);
                        ZoneType zonetype = (ZoneType)Enum.Parse(typeof(ZoneType), fields[18], true);
                        Zone zone;

                        if (TryFindZoneString(fields[1], out zone))
                        {
                            SetMembers(zone, (byte)zoneid, heightmap, zonetype,
                                catheleyamap, new Point(catheleyax, catheleyay, catheleyaz),
                                promisemap, new Point(promisex, promisey, promizez),
                                regioncode);

                            maps.Add(zoneid, zone);
                        }
                    }
                    catch (Exception e)
                    {
                        HostContext.AddUnhandeldException(e);
                    }
                }
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Removes all actors from a region tree
        /// </summary>
        public void RefreshActors()
        {
            WaitCallback callback = delegate(object state)
            {
                foreach (KeyValuePair<uint, Zone> pair in this.maps)
                {
                    Regiontree tree = pair.Value.Regiontree;
                    foreach (MapObject regionObject in tree.SearchActors(SearchFlags.Npcs))
                    {
                        BaseNPC npc = regionObject as BaseNPC;
                        if (npc != null)
                            npc.OnRefresh();
                    }
                }
            };

            ThreadPool.QueueUserWorkItem(callback);
        }

        /// <summary>
        /// Checks if the specified zone is hosted by the server.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsZoneHosted(uint id)
        {
            return maps.ContainsKey(id);
        }

        /// <summary>
        /// Try to get the specified zone.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool TryGetZone(uint id, out Zone map)
        {
            return maps.TryGetValue(id, out map);
        }

        /// <summary>
        /// Tries to get a cloned zone
        /// </summary>
        /// <param name="zoneid">id to find</param>
        /// <param name="zone">Zone to find</param>
        /// <returns></returns>
        public bool TryFindClonedZone(byte zoneid, out Zone zone)
        {
            this.TryGetZone(zoneid, out zone);
            zone = (Zone)((ICloneable)zone).Clone();
            return zone != null;
        }

        /// <summary>
        /// Obtain all zones so they can be iterated through a foreach statement.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Zone> HostedZones()
        {
            foreach (KeyValuePair<uint, Zone> pair in this.maps)
                yield return pair.Value;
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
    }
}