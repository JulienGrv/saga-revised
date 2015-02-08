using Saga.Enumarations;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;
using System.Collections.Generic;

namespace Saga.Map
{
    public sealed class Regiontree
    {
        #region Private Members

        /// <summary>
        /// Lookup table based on the region code
        /// </summary>
        private volatile Dictionary<uint, Region> regions;

        /// <summary>
        /// List of always visible actors
        /// </summary>
        private List<MapObject> alwaysvisible;

        /// <summary>
        /// Next map item.
        /// </summary>
        private uint MapItemID = 0x40000000;

        /// <summary>
        /// Next npc id
        /// </summary>
        private uint MapNPCID = 0x80000000;

        /// <summary>
        /// Generates a region code from the current X,Y,Z cooords
        /// </summary>
        /// <returns>Squared regioncode</returns>
        public static uint GetRegionCode(MapObject regionobject)
        {
            Point pos = regionobject.Position;
            /*
            float x = pos.x;
            float y = pos.y;
            uint REGION_DIAMETER = 2000;
            bool nx = false;
            bool ny = false;

            short hx = (short)(ux / REGION_DIAMETER);
            short hy = (short)(uy / REGION_DIAMETER);
             *
            if (x < 0) { x = x - (2 * x); nx = true; }
            if (y < 0) { y = y - (2 * y); ny = true; }

            uint ux = (uint)x;
            uint uy = (uint)y;

            ux = (uint)(ux / REGION_DIAMETER);
            uy = (uint)(uy / REGION_DIAMETER);

            if (ux > 49999) ux = 49999;
            if (!nx) ux = ux + 50000;
            else ux = 50000 - ux;

            if (uy > 49999) uy = 49999;
            if (!ny) uy = uy + 50000;
            else uy = 50000 - uy;

            return (uint)((ux * 1000000) + uy);
            */

            //Shifting by 10 means division by 1024
            //Shifting by 11 means division by 2048
            //Shifting by 12 means division by 4048
            return ((((uint)pos.y >> 12) & 0x0000FFFF) << 16) |
                     ((uint)pos.x >> 12) & 0x0000FFFF;
        }

        public static uint GetRegionCode(Point pos)
        {
            //Shifting by 10 means division by 1024
            //Shifting by 11 means division by 2048
            //Shifting by 12 means division by 4048
            return ((((uint)pos.y >> 12) & 0x0000FFFF) << 16) |
                     ((uint)pos.x >> 12) & 0x0000FFFF;
        }

        /// <summary>
        /// Generates a unique instance id for the mapobject on the specified regiontree
        /// </summary>
        /// <param name="instance">Instance to generate a unique id</param>
        /// <returns>Instance id</returns>
        private uint ObtainNextUniqueId(MapObject instance)
        {
            if (instance.id == 0)
            {
                if (instance is MapItem)
                {
                    return MapItemID++;
                }
                else
                {
                    return MapNPCID++;
                }
            }
            else
            {
                return instance.id;
            }
        }

        /// <summary>
        /// Find all near regions near the mapobect.
        /// The current region is always selected first as optimalisation.
        /// </summary>
        /// <param name="region">RegionCode</param>
        /// <returns>Enumaration of regions</returns>
        private IEnumerable<Region> GetNearRegions(uint region)
        {
            Region fregion;
            if (regions.TryGetValue(region, out fregion))
                yield return fregion;
            /*
            for (short deltaY = -1; deltaY <= 1; deltaY++)
                for (short deltaX = -1; deltaX <= 1; deltaX++)
                    if( !(deltaX == 0 && deltaY == 0) )
                        if (regions.TryGetValue((uint)(region + (deltaX * 1000000) + deltaY), out fregion))
                            yield return fregion;
            */

            short by = (short)(region >> 16);
            short bx = (short)(region & 0x0000FFFF);
            for (int y = -1; y <= 1; y++) for (int x = -1; x <= 1; x++)
                {
                    if (y == 0 && x == 0) continue;
                    uint value = (((uint)(by + y) & 0x0000FFFF) << 16) | ((uint)(bx + x) & 0x0000FFFF);
                    if (regions.TryGetValue(value, out fregion))
                        yield return fregion;
                }
        }

        public IEnumerable<uint> GetNearRegionCodes(uint region)
        {
            short by = (short)(region >> 16);
            short bx = (short)(region & 0x0000FFFF);
            for (int y = -1; y <= 1; y++) for (int x = -1; x <= 1; x++)
                {
                    uint value = (((uint)(by + y) & 0x0000FFFF) << 16) | ((uint)(bx + x) & 0x0000FFFF);
                    yield return value;
                }
        }

        /// <summary>
        /// Unregisters the regionobject at the specified regioncode
        /// </summary>
        /// <param name="regionobject">Object to usubscribe</param>
        /// <param name="regioncode">Region code to usubscribe</param>
        private bool Unsubscribe(MapObject regionobject, uint regioncode)
        {
            Region region;
            if (regions.TryGetValue(regioncode, out region))
            {
                return region.Unregister(regionobject);
            }

            return false;
        }

        /// <summary>
        /// Registers the regionobject at the specified regioncode.
        /// If the region does not exists already it will be created
        /// </summary>
        /// <param name="regionobject">Object to subscribe</param>
        /// <param name="regioncode">Region code to subscribe</param>
        private void Subscribe(MapObject regionobject, uint regioncode)
        {
            Region region;
            if (regions.TryGetValue(regioncode, out region))
            {
                region.Register(regionobject);
            }
            else
            {
                region = new Region();
                regions[regioncode] = region;
                region.Register(regionobject);
            }
        }

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Player border
        /// </summary>
        public const uint PlayerBorder = 0x40000000;

        /// <summary>
        /// MapItemBorder
        /// </summary>
        public const uint MapItemBorder = 0x80000000;

        /// <summary>
        /// NpcIndexBorder
        /// </summary>
        public const uint NpcIndexBorder = 10000;

        /// <summary>
        /// Updates the region of the character
        /// </summary>
        public static void UpdateRegion(MapObject regionobject)
        {
            //Calculate new regioncode
            uint oldregion = regionobject.region;
            uint newregion = GetRegionCode(regionobject);

            //If regioncode differs unsubsribe it
            if (newregion != oldregion)
            {
                lock (regionobject)
                {
                    Regiontree tree = regionobject.currentzone.Regiontree;
                    tree.Unsubscribe(regionobject, oldregion);
                    tree.Subscribe(regionobject, newregion);
                    regionobject.region = newregion;
                }
            }
        }

        public static void UpdateRegion(MapObject regionobject, bool forceUpdates)
        {
            //Calculate new regioncode
            uint oldregion = regionobject.region;
            uint newregion = GetRegionCode(regionobject);

            //If regioncode differs unsubsribe it
            if (newregion != oldregion)
            {
                lock (regionobject)
                {
                    if (forceUpdates == true)
                    {
                        Regiontree tree = regionobject.currentzone.Regiontree;
                        tree.Unsubscribe(regionobject, oldregion);
                        tree.Subscribe(regionobject, newregion);
                    }
                    regionobject.region = newregion;
                }
            }
        }

        public static bool TryFind(uint instanceid, MapObject e, out MapObject b)
        {
            Predicate<MapObject> FindActor = delegate(MapObject match)
            {
                return match.id == instanceid;
            };

            Regiontree tree = e.currentzone.Regiontree;
            b = tree.SearchActor(FindActor, e, SearchFlags.DynamicObjects | SearchFlags.StaticObjects);
            if (b == null)
                b = tree.SearchActor(FindActor, SearchFlags.DynamicObjects);
            return b != null;
        }

        public static bool TryFind<T>(uint instanceid, MapObject e, out T b)
            where T : class
        {
            MapObject target;
            Regiontree.TryFind(instanceid, e, out target);
            b = target as T;
            return b != null;
        }

        public IEnumerable<MapObject> Clear()
        {
            foreach (KeyValuePair<uint, Region> pair in regions)
            {
                Region fregion = pair.Value;
                for (int i = 0; i < fregion.npc.Count; i++)
                {
                    yield return fregion.npc[i];
                }
                for (int i = 0; i < fregion.mapItems.Count; i++)
                {
                    yield return fregion.mapItems[i];
                }

                fregion.mapItems = new List<MapObject>();
                fregion.npc = new List<MapObject>();
            }

            for (int i = 0; i < alwaysvisible.Count; i++)
            {
                yield return alwaysvisible[i];
            }

            alwaysvisible = new List<MapObject>();

            //Reset the actor id's
            MapItemID = 0x40000000;
            MapNPCID = 0x80000000;
        }

        public IEnumerable<MapObject> SearchActors(SearchFlags flags)
        {
            bool includeCharacter = (flags & SearchFlags.Characters) == SearchFlags.Characters;
            bool includeMapitems = (flags & SearchFlags.MapItems) == SearchFlags.MapItems;
            bool includeNpc = (flags & SearchFlags.Npcs) == SearchFlags.Npcs;
            bool includeAlwaysVisible = (flags & SearchFlags.StaticObjects) == SearchFlags.StaticObjects;

            if (includeCharacter | includeMapitems | includeNpc == true)
                foreach (KeyValuePair<uint, Region> pair in regions)
                {
                    Region fregion = pair.Value;
                    if (includeMapitems == true)
                        for (int i = 0; i < fregion.mapItems.Count; i++)
                            yield return fregion.mapItems[i];
                    if (includeCharacter == true)
                        for (int i = 0; i < fregion.characters.Count; i++)
                            yield return fregion.characters[i];
                    if (includeNpc == true)
                        for (int i = 0; i < fregion.npc.Count; i++)
                            yield return fregion.npc[i];
                }

            if (includeAlwaysVisible == true)
                for (int i = 0; i < alwaysvisible.Count; i++)
                    yield return alwaysvisible[i];
        }

        public IEnumerable<MapObject> SearchActors(MapObject instance, SearchFlags flags)
        {
            bool includeCharacter = (flags & SearchFlags.Characters) == SearchFlags.Characters;
            bool includeMapitems = (flags & SearchFlags.MapItems) == SearchFlags.MapItems;
            bool includeNpc = (flags & SearchFlags.Npcs) == SearchFlags.Npcs;
            bool includeAlwaysVisible = (flags & SearchFlags.StaticObjects) == SearchFlags.StaticObjects;

            if (includeCharacter | includeMapitems | includeNpc == true)
                foreach (Region fregion in GetNearRegions(instance.region))
                {
                    if (includeMapitems == true)
                        for (int i = 0; i < fregion.mapItems.Count; i++)
                            yield return fregion.mapItems[i];
                    if (includeCharacter == true)
                        for (int i = 0; i < fregion.characters.Count; i++)
                            yield return fregion.characters[i];
                    if (includeNpc == true)
                        for (int i = 0; i < fregion.npc.Count; i++)
                            yield return fregion.npc[i];
                }

            if (includeAlwaysVisible == true)
                for (int i = 0; i < alwaysvisible.Count; i++)
                    yield return alwaysvisible[i];
        }

        public IEnumerable<MapObject> SearchActors(MapObject instance, SearchFlags flags, Point positionNew)
        {
            bool includeCharacter = (flags & SearchFlags.Characters) == SearchFlags.Characters;
            bool includeMapitems = (flags & SearchFlags.MapItems) == SearchFlags.MapItems;
            bool includeNpc = (flags & SearchFlags.Npcs) == SearchFlags.Npcs;
            bool includeAlwaysVisible = (flags & SearchFlags.StaticObjects) == SearchFlags.StaticObjects;

            if (includeCharacter | includeMapitems | includeNpc == true)
            {
                List<uint> list = new List<uint>(18);

                short by = (short)(instance.region >> 16);
                short bx = (short)(instance.region & 0x0000FFFF);
                for (int y = -1; y <= 1; y++) for (int x = -1; x <= 1; x++)
                    {
                        uint value = (((uint)(by + y) & 0x0000FFFF) << 16) | ((uint)(bx + x) & 0x0000FFFF);
                        if (!list.Contains(value)) list.Add(value);
                    }

                uint region = GetRegionCode(positionNew);
                by = (short)(region >> 16);
                bx = (short)(region & 0x0000FFFF);
                for (int y = -1; y <= 1; y++) for (int x = -1; x <= 1; x++)
                    {
                        uint value = (((uint)(by + y) & 0x0000FFFF) << 16) | ((uint)(bx + x) & 0x0000FFFF);
                        if (!list.Contains(value)) list.Add(value);
                    }

                for (int a = 0, maxCount = list.Count; a < maxCount; a++)
                {
                    Region fregion;
                    if (regions.TryGetValue(list[a], out fregion))
                    {
                        if (includeMapitems == true)
                            for (int i = 0; i < fregion.mapItems.Count; i++)
                                yield return fregion.mapItems[i];
                        if (includeCharacter == true)
                            for (int i = 0; i < fregion.characters.Count; i++)
                                yield return fregion.characters[i];
                        if (includeNpc == true)
                            for (int i = 0; i < fregion.npc.Count; i++)
                                yield return fregion.npc[i];
                    }
                }
            }

            if (includeAlwaysVisible == true)
                for (int i = 0; i < alwaysvisible.Count; i++)
                    yield return alwaysvisible[i];
        }

        public IEnumerable<MapObject> SearchActors(MapObject instance, Predicate<MapObject> match, SearchFlags flags)
        {
            bool includeCharacter = (flags & SearchFlags.Characters) == SearchFlags.Characters;
            bool includeMapitems = (flags & SearchFlags.MapItems) == SearchFlags.MapItems;
            bool includeNpc = (flags & SearchFlags.Npcs) == SearchFlags.Npcs;
            bool includeAlwaysVisible = (flags & SearchFlags.StaticObjects) == SearchFlags.StaticObjects;

            if (includeCharacter | includeMapitems | includeNpc == true)
                foreach (Region fregion in GetNearRegions(instance.region))
                {
                    if (includeMapitems == true)
                        for (int i = 0; i < fregion.mapItems.Count; i++)
                            if (match.Invoke(fregion.mapItems[i]))
                                yield return fregion.mapItems[i];
                    if (includeCharacter == true)
                        for (int i = 0; i < fregion.characters.Count; i++)
                            if (match.Invoke(fregion.mapItems[i]))
                                yield return fregion.characters[i];
                    if (includeNpc == true)
                        for (int i = 0; i < fregion.npc.Count; i++)
                            if (match.Invoke(fregion.npc[i]))
                                yield return fregion.npc[i];
                }

            if (includeAlwaysVisible == true)
                for (int i = 0; i < alwaysvisible.Count; i++)
                    if (match.Invoke(alwaysvisible[i]))
                        yield return alwaysvisible[i];
        }

        public IEnumerable<MapObject> SearchActors(Predicate<MapObject> match, SearchFlags flags)
        {
            bool includeCharacter = (flags & SearchFlags.Characters) == SearchFlags.Characters;
            bool includeMapitems = (flags & SearchFlags.MapItems) == SearchFlags.MapItems;
            bool includeNpc = (flags & SearchFlags.Npcs) == SearchFlags.Npcs;
            bool includeAlwaysVisible = (flags & SearchFlags.StaticObjects) == SearchFlags.StaticObjects;

            if (includeCharacter | includeMapitems | includeNpc == true)
                foreach (KeyValuePair<uint, Region> pair in regions)
                {
                    Region fregion = pair.Value;
                    if (includeMapitems == true)
                        for (int i = 0; i < fregion.mapItems.Count; i++)
                            if (match.Invoke(fregion.mapItems[i]))
                                yield return fregion.mapItems[i];
                    if (includeCharacter == true)
                        for (int i = 0; i < fregion.characters.Count; i++)
                            if (match.Invoke(fregion.characters[i]))
                                yield return fregion.characters[i];
                    if (includeNpc == true)
                        for (int i = 0; i < fregion.npc.Count; i++)
                            if (match.Invoke(fregion.npc[i]))
                                yield return fregion.npc[i];
                }

            if (includeAlwaysVisible == true)
                for (int i = 0; i < alwaysvisible.Count; i++)
                    if (match.Invoke(alwaysvisible[i]))
                        yield return alwaysvisible[i];
        }

        public MapObject SearchActor(Predicate<MapObject> match, SearchFlags flags)
        {
            bool includeCharacter = (flags & SearchFlags.Characters) == SearchFlags.Characters;
            bool includeMapitems = (flags & SearchFlags.MapItems) == SearchFlags.MapItems;
            bool includeNpc = (flags & SearchFlags.Npcs) == SearchFlags.Npcs;
            bool includeAlwaysVisible = (flags & SearchFlags.StaticObjects) == SearchFlags.StaticObjects;

            if (includeCharacter | includeMapitems | includeNpc == true)
                foreach (KeyValuePair<uint, Region> pair in regions)
                {
                    Region fregion = pair.Value;
                    if (includeMapitems == true)
                        for (int i = 0; i < fregion.mapItems.Count; i++)
                            if (match.Invoke(fregion.mapItems[i]))
                                return fregion.mapItems[i];
                    if (includeCharacter == true)
                        for (int i = 0; i < fregion.characters.Count; i++)
                            if (match.Invoke(fregion.characters[i]))
                                return fregion.characters[i];
                    if (includeNpc == true)
                        for (int i = 0; i < fregion.npc.Count; i++)
                            if (match.Invoke(fregion.npc[i]))
                                return fregion.npc[i];
                }

            if (includeAlwaysVisible == true)
                for (int i = 0; i < alwaysvisible.Count; i++)
                    if (match.Invoke(alwaysvisible[i]))
                        return alwaysvisible[i];

            return null;
        }

        public MapObject SearchActor(Predicate<MapObject> match, MapObject target, SearchFlags flags)
        {
            bool includeCharacter = (flags & SearchFlags.Characters) == SearchFlags.Characters;
            bool includeMapitems = (flags & SearchFlags.MapItems) == SearchFlags.MapItems;
            bool includeNpc = (flags & SearchFlags.Npcs) == SearchFlags.Npcs;
            bool includeAlwaysVisible = (flags & SearchFlags.StaticObjects) == SearchFlags.StaticObjects;

            if (includeCharacter | includeMapitems | includeNpc == true)
                foreach (Region fregion in GetNearRegions(target.region))
                {
                    if (includeMapitems == true)
                        for (int i = 0; i < fregion.mapItems.Count; i++)
                            if (match.Invoke(fregion.mapItems[i]))
                                return fregion.mapItems[i];
                    if (includeCharacter == true)
                        for (int i = 0; i < fregion.characters.Count; i++)
                            if (match.Invoke(fregion.characters[i]))
                                return fregion.characters[i];
                    if (includeNpc == true)
                        for (int i = 0; i < fregion.npc.Count; i++)
                            if (match.Invoke(fregion.npc[i]))
                                return fregion.npc[i];
                }

            if (includeAlwaysVisible == true)
                for (int i = 0; i < alwaysvisible.Count; i++)
                    if (match.Invoke(alwaysvisible[i]))
                        return alwaysvisible[i];

            return null;
        }

        public static int GetCharacterCount(MapObject instance)
        {
            Region region;
            Regiontree tree = instance.currentzone.Regiontree;

            if (tree.regions.TryGetValue(instance.region, out region))
            {
                return region.characters.Count;
            }

            return 0;
        }

        public static int GetCharacterCount(MapObject instance, uint regionCode)
        {
            Region region;
            Regiontree tree = instance.currentzone.Regiontree;

            if (tree.regions.TryGetValue(regionCode, out region))
            {
                return region.characters.Count;
            }

            return 0;
        }

        #endregion Public Members

        #region Public Subscriptions: Always Visible

        /// <summary>
        /// Subscribes a actor as always visible
        /// </summary>
        /// <param name="regionobject">Object to subscribe </param>
        private void AlwaysVisibleSubscribe(MapObject regionobject)
        {
            uint instanceid = ObtainNextUniqueId(regionobject);
            if (instanceid > 0)
            {
                regionobject.id = instanceid;
                this.alwaysvisible.Add(regionobject);
            }
        }

        /// <summary>
        /// Unsubscribe a ctor as always visible
        /// </summary>
        /// <param name="regionobject">Object to unsubscribe</param>
        private void AlwaysVisibleUnsubscribe(MapObject regionobject)
        {
            Predicate<MapObject> FindActors = delegate(MapObject match)
            {
                return match.id == regionobject.id;
            };

            uint instanceid = ObtainNextUniqueId(regionobject);
            if (instanceid > 0)
            {
                this.alwaysvisible.RemoveAll(FindActors);
            }
        }

        #endregion Public Subscriptions: Always Visible

        #region Public Subscriptions: Locally Visible

        /// <summary>
        /// Subscribes a local visible actor
        /// </summary>
        /// <param name="regionobject">Object to subscribe</param>
        public void Subscribe(MapObject regionobject)
        {
            bool isalwaysVisible = false;
            foreach (RegionVisibility att in regionobject.GetType().GetCustomAttributes(typeof(RegionVisibility), true))
                isalwaysVisible = att.Level == VisibilityLevel.Always;

            Regiontree tree = regionobject.currentzone.Regiontree;
            if (isalwaysVisible == false)
            {
                regionobject.id = ObtainNextUniqueId(regionobject);
                if (regionobject.id > 0)
                {
                    //Calculate new regioncode
                    uint newregion = GetRegionCode(regionobject);
                    tree.Unsubscribe(regionobject, regionobject.region);
                    tree.Subscribe(regionobject, newregion);
                    regionobject.region = newregion;
                }
            }
            else
            {
                tree.AlwaysVisibleSubscribe(regionobject);
            }
        }

        /// <summary>
        /// Unsubscribe a local visible actor
        /// </summary>
        /// <param name="regionobject">Object to unsubscribe</param>
        public bool Unsubscribe(MapObject regionobject)
        {
            bool isalwaysVisible = false;
            foreach (RegionVisibility att in regionobject.GetType().GetCustomAttributes(typeof(RegionVisibility), true))
                isalwaysVisible = att.Level == VisibilityLevel.Always;

            Regiontree tree = regionobject.currentzone.Regiontree;
            if (isalwaysVisible == false)
            {
                uint newregion = GetRegionCode(regionobject);
                bool result = tree.Unsubscribe(regionobject, regionobject.region);
                regionobject.region = newregion;
                return result;
            }
            else
            {
                tree.AlwaysVisibleUnsubscribe(regionobject);
                return true;
            }
        }

        #endregion Public Subscriptions: Locally Visible

        #region Nested Classes/Structures

        private class Region
        {
            #region Internal Members

            /// <summary>
            /// List containing all characters
            /// </summary>
            internal List<MapObject> characters;

            /// <summary>
            /// List containing all npc's
            /// </summary>
            internal List<MapObject> npc;

            /// <summary>
            /// List containing all mapitems
            /// </summary>
            internal List<MapObject> mapItems;

            #endregion Internal Members

            #region Internal Methods

            /// <summary>
            /// Registers a new mapobject to the region
            /// </summary>
            /// <param name="regionobject">Object to register</param>
            internal void Register(MapObject regionobject)
            {
                if (regionobject.id < PlayerBorder)
                {
                    lock (characters)
                    {
                        characters.Add(regionobject);
                    }
                }
                else if (regionobject.id < MapItemBorder)
                {
                    lock (mapItems)
                    {
                        mapItems.Add(regionobject);
                    }
                }
                else
                {
                    lock (npc)
                    {
                        npc.Add(regionobject);
                    }
                }
            }

            /// <summary>
            /// Unregisters a new mapobject to the region
            /// </summary>
            /// <param name="regionobject">Object to unregister</param>
            internal bool Unregister(MapObject regionobject)
            {
                //Use a predicate to match only on Id
                Predicate<MapObject> FindObject = delegate(MapObject match)
                {
                    return match.id == regionobject.id;
                };

                if (regionobject.id < PlayerBorder)
                {
                    lock (characters)
                    {
                        return characters.RemoveAll(FindObject) > 0;
                    }
                }
                else if (regionobject.id < MapItemBorder)
                {
                    lock (mapItems)
                    {
                        return mapItems.RemoveAll(FindObject) > 0;
                    }
                }
                else
                {
                    lock (npc)
                    {
                        return npc.RemoveAll(FindObject) > 0;
                    }
                }
            }

            #endregion Internal Methods

            #region Constructor

            internal Region()
            {
                this.characters = new List<MapObject>();
                this.mapItems = new List<MapObject>();
                this.npc = new List<MapObject>();
            }

            #endregion Constructor
        }

        #endregion Nested Classes/Structures

        #region Constructor

        public Regiontree()
        {
            this.regions = new Dictionary<uint, Region>();
            this.alwaysvisible = new List<MapObject>();
        }

        #endregion Constructor
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class RegionVisibility : System.Attribute
    {
        private VisibilityLevel isregionalvisible = VisibilityLevel.Region;

        public VisibilityLevel Level
        {
            get
            {
                return this.isregionalvisible;
            }
            set
            {
                this.isregionalvisible = value;
            }
        }
    }

    /// <summary>
    /// Defines the how the mapitem is shown visible
    /// </summary>
    public enum VisibilityLevel
    {
        /// <summary>
        /// Item is always visible
        /// </summary>
        Always = 1,

        /// <summary>
        /// Item is region based visible
        /// </summary>
        Region = 2
    }
}