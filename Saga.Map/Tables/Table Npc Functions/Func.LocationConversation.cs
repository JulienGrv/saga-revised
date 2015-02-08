using Saga.Enumarations;
using Saga.IO;
using Saga.Map.Utils.Structures;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Templates;
using System.Collections.Generic;
using System.IO;

namespace Saga.Npc.Functions
{
    /// <summary>
    /// This is a location-guard conversation. Include this in a npc to show a
    /// location dialog and submenu.
    /// </summary>
    /// <remarks>
    /// Location are a new function of kro2 which
    /// aren't included yet hence the button isn't shown as a proper designed
    /// button.
    /// </remarks>
    public class LocationConversation : NpcFunction
    {
        #region Private Members

        /// <summary>
        /// Contains the location-menu id to open.
        /// </summary>
        private uint _locmenuid = 0;

        /// <summary>
        /// Contains a list of point locations.
        /// </summary>
        private List<GuidePoint> _guidelist;

        #endregion Private Members

        #region Protected Methods

        /// <summary>
        /// Registers all the menu's and buttons on the npc.
        /// </summary>
        /// <param name="npc">Npc who to register with</param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.LocationGuide, new FunctionCallback(OnLocationGuide));
            CacheLocations(npc);
        }

        /// <summary>
        /// Refreshes the bookstore
        /// </summary>
        /// <param name="npc">Target npc</param>
        protected internal override void OnRefresh(BaseNPC npc)
        {
            CacheLocations(npc);
        }

        /// <summary>
        /// Loads all dialog dialog information.
        /// </summary>
        /// <remarks>
        /// The information is loaded from the data folder in dialogtemplates/{npcmodelid}.xml
        /// </remarks>
        /// <param name="npc">Npc who requires caching</param>
        /// <param name="name">Name of the found string</param>
        /// <param name="value">Value of the found string</param>
        protected virtual void OnLocationGuide(BaseNPC npc, Character target)
        {
            target.Tag = _guidelist;
            Common.Actions.OpenLocationGuide(target, _locmenuid);
        }

        /// <summary>
        /// Caches the location items.
        /// </summary>
        /// <remarks>
        /// This will load all shop items from the data directory from read from
        /// file Guides/{npcmodelid}.xml.
        /// </remarks>
        /// <param name="npc">Npc who requires caching</param>
        protected void CacheLocations(BaseNPC npc)
        {
            _guidelist = new List<GuidePoint>();
            string filename = Server.SecurePath("~/Guides/{0}.xml", npc.ModelId);
            if (File.Exists(filename))
                using (GuideReader reader = GuideReader.Open(filename))
                    while (reader.Read())
                    {
                        _locmenuid = reader.Menu;
                        switch (reader.Type)
                        {
                            case GuideReader.NodeType.Npc:
                                _guidelist.Add(new GuideNpc(reader.Item, 0));
                                break;

                            case GuideReader.NodeType.Position:
                                _guidelist.Add(new GuidePosition(reader.X, reader.Y, reader.Z, reader.Item, 0));
                                break;
                        }
                    }
        }

        #endregion Protected Methods
    }
}