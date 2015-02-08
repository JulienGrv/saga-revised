using Saga.Configuration;
using Saga.Map.Configuration;
using Saga.Structures;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace Saga.Factory
{
    public class Portals : FactoryBase
    {
        #region Ctor/Dtor

        public Portals()
        {
        }

        ~Portals()
        {
            this.portals = null;
        }

        #endregion Ctor/Dtor

        #region Internal Members

        /// <summary>
        /// Lookup table to contain the portals
        /// </summary>
        private Dictionary<byte, Dictionary<byte, Portal>> portals;

        #endregion Internal Members

        #region Protected Methods

        protected override void Initialize()
        {
            this.portals =
                new Dictionary<byte, Dictionary<byte, Portal>>();
        }

        protected override void Load()
        {
            PortalSettings section = (PortalSettings)ConfigurationManager.GetSection("Saga.Factory.Portals");
            if (section != null)
            {
                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("PortalFactory", "Loading portal information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }
            }
            else
            {
                WriteWarning("XmlSections", "Section {0} was not found", "Saga.Factory.Portals");
            }
        }

        protected override void ParseAsCsvStream(System.IO.Stream stream, FactoryBase.ReportProgress ProgressReport)
        {
            using (StreamReader c = new StreamReader(stream))
            {
                c.ReadLine();
                while (c.Peek() > 0)
                {
                    ProgressReport.Invoke();
                    String row = c.ReadLine();
                    String[] fields = row.Split(',');

                    NumberFormatInfo global = System.Globalization.NumberFormatInfo.InvariantInfo;
                    byte toid = byte.Parse(fields[1], global);
                    byte fromid = byte.Parse(fields[0], global);
                    byte tomapid = byte.Parse(fields[5], global);
                    float x = float.Parse(fields[2], global);
                    float y = float.Parse(fields[3], global);
                    float z = float.Parse(fields[4], global);

                    Portal portal = new Portal(tomapid, x, y, z);

                    Dictionary<byte, Portal> portals_tmp;
                    if (this.portals.TryGetValue(fromid, out portals_tmp) == false)
                    {
                        portals_tmp = new Dictionary<byte, Portal>();
                    }
                    portals_tmp[toid] = portal;
                    this.portals[fromid] = portals_tmp;
                }
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Try to find a portal with the specified sourcezone and specified target zone.
        /// </summary>
        /// <param name="TargetZone">Desired mapid</param>
        /// <param name="SourceZone">Current mapid</param>
        /// <param name="portal">Potal information</param>
        /// <returns>True if the portal was found</returns>
        public bool TryFind(byte TargetZone, byte SourceZone, out Portal portal)
        {
            try
            {
                portal = portals[SourceZone][TargetZone];
                return true;
            }
            catch (Exception)
            {
                portal = new Portal();
                return false;
            }
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
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_PORTALS; }
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
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_PORTALS; }
        }

        #endregion Protected Properties

        #region Nested Structures/Classes

        /// <summary>
        /// Structure to contain portal informartion
        /// </summary>
        public struct Portal
        {
            /// <summary>
            /// Initialized the portal with basic information.
            /// </summary>
            /// <remarks>
            /// Portals are mainly used inside this factory and used internal
            /// by the client.
            /// </remarks>
            /// <param name="mapID">Id of the map to warp to</param>
            /// <param name="x">X Coord to spawn</param>
            /// <param name="y">Y Coord to spawn</param>
            /// <param name="z">Z Coord to spawn</param>
            public Portal(byte mapID, float x, float y, float z)
            {
                this.mapID = mapID;
                this.destinaton.x = x;
                this.destinaton.y = y;
                this.destinaton.z = z;
            }

            /// <summary>
            /// Contains the destination mapid
            /// </summary>
            public byte mapID;

            /// <summary>
            /// Contains the destination coordinates
            /// </summary>
            public Point destinaton;
        }

        #endregion Nested Structures/Classes
    }
}