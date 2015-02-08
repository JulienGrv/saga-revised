using Saga.Configuration;
using Saga.Map.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace Saga.Factory
{
    /// <summary>
    /// Warp factory supplies information regarding warp lookups per id.
    /// </summary>
    public class Warps : FactoryBase
    {
        #region Ctor/Dtor

        public Warps()
        {
        }

        #endregion Ctor/Dtor

        #region Internal Members

        private Dictionary<ushort, Info> warps;

        #endregion Internal Members

        #region Protected Methods

        protected override void Initialize()
        {
            this.warps = new Dictionary<ushort, Info>();
        }

        protected override void Load()
        {
            WarpSettings section = (WarpSettings)ConfigurationManager.GetSection("Saga.Factory.Warps");
            if (section != null)
            {
                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("WarpFactory", "Loading warp information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }
            }
            else
            {
                WriteWarning("XmlSections", "Section {0} was not found", "Saga.Factory.Warps");
            }
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

                    Info info = new Info();
                    info.price = uint.Parse(fields[1], NumberFormatInfo.InvariantInfo);
                    info.x = float.Parse(fields[2], NumberFormatInfo.InvariantInfo);
                    info.y = float.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                    info.z = float.Parse(fields[4], NumberFormatInfo.InvariantInfo);
                    info.map = byte.Parse(fields[5], NumberFormatInfo.InvariantInfo);
                    warps.Add(ushort.Parse(fields[0], NumberFormatInfo.InvariantInfo), info);
                }
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Try to get a warp information based on the warpid
        /// </summary>
        /// <param name="WarpLocation">Warpid used to lookup the information</param>
        /// <param name="WarpInfo">Information used when warping</param>
        /// <returns>True if the warp was found</returns>
        public bool TryFind(ushort WarpLocation, out Info WarpInfo)
        {
            return warps.TryGetValue(WarpLocation, out WarpInfo);
        }

        #endregion Public Methods

        #region Protected Properties

        protected override string Notification
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_WARPS; }
        }

        protected override string ReadyState
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_WARPS; }
        }

        #endregion Protected Properties

        #region Nested Classes/Structures

        /// <summary>
        /// Contains the warp information.
        /// </summary>
        public class Info
        {
            /// <summary>
            /// Get's or set's the price you require to warp
            /// </summary>
            public uint price;

            /// <summary>
            /// Get's or set's the x-Axis of the position where to warp to
            /// </summary>
            public float x;

            /// <summary>
            /// Get's or set's the y-Axis of the position where to warp to
            /// </summary>
            public float y;

            /// <summary>
            /// Get's or set's the z-Axis of the position where to warp to
            /// </summary>
            public float z;

            /// <summary>
            /// Get's or set's the mapId of the map where to warp to
            /// </summary>
            public byte map;
        }

        #endregion Nested Classes/Structures
    }
}