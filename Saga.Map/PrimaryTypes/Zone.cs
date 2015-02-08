using Saga.Enumarations;
using Saga.Map;
using Saga.Packets;
using Saga.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;

namespace Saga.PrimaryTypes
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [Serializable()]
    public class Zone : ICloneable
    {
        #region Private Members

        private byte map;
        private ZoneType type;
        private WorldCoordinate cathelaya_location;
        private WorldCoordinate promise_location;
        private uint regioncode;
        private int weather;

        [NonSerialized()]
        private HeightMap heightmap;

        [NonSerialized()]
        private Regiontree regiontree;

        #endregion Private Members

        #region Public Members

        public Regiontree Regiontree
        {
            get
            {
                return regiontree;
            }
            protected internal set
            {
                regiontree = value;
            }
        }

        public ZoneType Type
        {
            get
            {
                return type;
            }
            protected internal set
            {
                type = value;
            }
        }

        public byte Map
        {
            get
            {
                return map;
            }
            protected internal set
            {
                map = value;
            }
        }

        public WorldCoordinate CathelayaLocation
        {
            get
            {
                return cathelaya_location;
            }
            protected internal set
            {
                cathelaya_location = value;
            }
        }

        public WorldCoordinate ProsmiseLocation
        {
            get
            {
                return promise_location;
            }
            protected internal set
            {
                promise_location = value;
            }
        }

        public HeightMap Heightmap
        {
            get
            {
                return heightmap;
            }
            protected internal set
            {
                heightmap = value;
            }
        }

        /// <summary>
        /// Returns the regioncode for the specified map.
        /// </summary>
        /// <remarks>
        /// Each map is associated with his own unique region code or with a
        /// shared regioncode. The region code is use by the internal quest
        /// system to filter out official/personal quests.
        ///
        /// When entering a new zone the personal quest list is filled by personal
        /// quests that are available based upon the regioncode and matches the
        /// criteria for beeing visible.
        /// </remarks>
        public uint RegionCode
        {
            get
            {
                return regioncode;
            }
            protected internal set
            {
                regioncode = value;
            }
        }

        /// <summary>
        /// Get's the current weather as a interger.
        /// </summary>
        public int Weather
        {
            get
            {
                return weather;
            }
        }

        #endregion Public Members

        #region Public methods

        public void Clear()
        {
            lock (this.regiontree)
            {
                List<MapObject> list = new List<MapObject>();
                list.AddRange(regiontree.Clear());

                foreach (Character character in Regiontree.SearchActors(SearchFlags.Characters))
                {
                    foreach (MapObject regionObject in list)
                    {
                        try
                        {
                            SMSG_ACTORDELETE spkt = new SMSG_ACTORDELETE();
                            spkt.ActorID = regionObject.id;
                            spkt.SessionId = character.id;
                            character.client.Send((byte[])spkt);
                        }
                        catch (SocketException)
                        {
                            break;
                        }
                        catch (Exception e)
                        {
                            Trace.TraceWarning(e.ToString());
                        }
                    }
                }

                foreach (MapObject regionObject in list)
                {
                    try
                    {
                        regionObject.OnDeregister();
                    }
                    catch (Exception)
                    {
                        //Do nothing
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when entering a zone.
        /// </summary>
        /// <param name="character"></param>
        public virtual void OnEnter(Character character)
        {
        }

        /// <summary>
        /// Occurs when leaving a zone
        /// </summary>
        /// <param name="character"></param>
        public virtual void OnLeave(Character character)
        {
            if (character.currentzone == this)
            {
                if (!this.Regiontree.Unsubscribe(character))
                {
                    Trace.TraceError("Unsubscribe failed");
                }

                Regiontree tree = character.currentzone.Regiontree;
                foreach (MapObject target in tree.SearchActors(character, Saga.Enumarations.SearchFlags.DynamicObjects))
                {
                    if (MapObject.IsPlayer(target))
                    {
                        Character cTarget = (Character)target;
                        if (cTarget != character && cTarget.client.isloaded == true)
                            character.HideObject(cTarget);
                        target.Disappear(character);
                    }
                    else
                    {
                        target.Disappear(character);
                    }
                }
            }
        }

        public IEnumerable<MapObject> GetObjectsInRegionalRange(MapObject a)
        {
            /*
             * Returns all objects in the regional sightrange.
             *
             * Usefull for updates that should be spread about amongst
             * multiple regions. For example update player information
             *
             */

            foreach (MapObject c in this.regiontree.SearchActors(a, SearchFlags.DynamicObjects))
            {
                yield return c;
            }
        }

        /// <summary>
        /// Returns all objects in the regional sightrange.
        /// </summary>
        /// <param name="a">Object which to check if he can see</param>
        /// <returns>a list of objects which can be seen</returns>
        /// <remarks>
        ///  Usefull for updates that should be spread about amongst
        ///  multiple regions. For example update player information
        /// </remarks>
        public IEnumerable<MapObject> GetObjectsInSightRange(MapObject a)
        {
            foreach (MapObject c in this.regiontree.SearchActors(a, SearchFlags.DynamicObjects))
            {
                if (IsInSightRangeByRadius(c.Position, a.Position))
                    yield return c;
            }
        }

        /// <summary>
        /// Returns all chacters in the regional sightrange.
        /// </summary>
        /// <param name="a">Object which to check if he can see</param>
        /// <returns>a list of objects which can be seen</returns>
        /// <remarks>
        ///  Usefull for updates that should be spread about amongst
        ///  multiple regions. For example update player information
        /// </remarks>
        public IEnumerable<Character> GetCharactersInSightRange(MapObject a)
        {
            foreach (MapObject c in this.regiontree.SearchActors(a, SearchFlags.Characters))
            {
                if (MapObject.IsPlayer(c))
                    if (IsInSightRangeByRadius(c.Position, a.Position))
                        yield return (Character)c;
            }
        }

        /// <summary>
        /// Check if a object is in sightrange of eachother
        /// </summary>
        /// <returns>
        /// Checks if position a is in a range of position b.
        /// This sub function is used to calculate objects we are allowed to
        /// see. Applied for 3 axices: X, Y, Z.///
        /// </returns>
        public bool IsInSightRangeByRadius(Point A, Point B)
        {
            double dx = (double)(A.x - B.x);
            double dy = (double)(A.y - B.y);
            double dz = (double)(A.z - B.z);
            double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return distance < 10000;
        }

        /// <summary>
        /// </summary>
        /// <returns>True if a object is visible by a square bounds</returns>
        /// <remarks>
        /// Checks if position a is in range of position b.
        /// This sub function should be obfuscated, and could serve as a backup
        /// for the Radius.
        /// </remarks>
        public bool IsInSightRangeBySquare(Point A, Point B)
        {
            if (Math.Abs(A.x - B.x) > 10000) return false;
            if (Math.Abs(A.y - B.y) > 10000) return false;
            return true;
        }

        public Point GetZ(Point postion)
        {
            //Correct z-heightmap with z-index
            float z = 0;
            if (this.Heightmap != null)
            {
                if (this.Heightmap.GetZ(postion.x, postion.y, out z))
                {
                    return new Point(postion.x, postion.y, z + 30);
                }
                else
                {
                    return postion;
                }
            }
            else
            {
                return postion;
            }
        }

        public bool SaveLocation(Character character)
        {
            if (character.currentzone.ProsmiseLocation.map > 0)
            {
                character.savelocation = character.currentzone.ProsmiseLocation;
                SMSG_KAFTRAHOMEPOINT spkt = new SMSG_KAFTRAHOMEPOINT();
                spkt.SessionId = character.id;
                spkt.Result = 0;
                spkt.Zone = character.savelocation.map;
                character.client.Send((byte[])spkt);

                WorldCoordinate lpos = character.lastlocation.map > 0 ? character.lastlocation : character.savelocation;
                WorldCoordinate spos = character.savelocation;
                if (spos.map == character.currentzone.Map)
                {
                    SMSG_RETURNMAPLIST spkt2 = new SMSG_RETURNMAPLIST();
                    spkt2.ToMap = lpos.map;
                    spkt2.FromMap = character.currentzone.CathelayaLocation.map;
                    spkt2.IsSaveLocationSet = (lpos.map > 0) ? (byte)1 : (byte)0;
                    spkt2.SessionId = character.id;
                    character.client.Send((byte[])spkt2);
                }
                else
                {
                    SMSG_RETURNMAPLIST spkt2 = new SMSG_RETURNMAPLIST();
                    spkt2.ToMap = spos.map;
                    spkt2.FromMap = character.currentzone.CathelayaLocation.map;
                    spkt2.IsSaveLocationSet = (spos.map > 0) ? (byte)1 : (byte)0;
                    spkt2.SessionId = character.id;
                    character.client.Send((byte[])spkt2);
                }

                return true;
            }
            else
            {
                SMSG_KAFTRAHOMEPOINT spkt = new SMSG_KAFTRAHOMEPOINT();
                spkt.SessionId = character.id;
                spkt.Result = 1;
                character.client.Send((byte[])spkt);

                return false;
            }
        }

        public void UpdateWeather(int Weather)
        {
            this.weather = Weather;
            foreach (Character c in this.regiontree.SearchActors(SearchFlags.Characters))
            {
                CommonFunctions.UpdateTimeWeather(c as Character);
            }
        }

        /// <summary>
        /// Is called when the weather changes
        /// </summary>
        /// <remarks>
        /// This method is called when the weather of the zone is supposed to change.
        /// Our parameter the int weather speciafies the new weather in which it's
        /// supposed to change.
        ///
        /// Override this method when you need to control which types of weathers
        /// should be allowed for the current map.
        /// </remarks>
        /// <param name="Weather"></param>
        public virtual void OnChangeWeather(int Weather)
        {
            UpdateWeather(Weather);
        }

        #endregion Public methods

        #region ICloneable Members

        /// <summary>
        /// Clones the zone with a clean regiontree.
        /// </summary>
        /// <returns>Cloned zone instance</returns>
        object ICloneable.Clone()
        {
            Zone b = new Zone();
            b.ProsmiseLocation = this.ProsmiseLocation;
            b.CathelayaLocation = this.CathelayaLocation;
            b.RegionCode = this.RegionCode;
            b.Regiontree = new Regiontree();
            b.Type = this.Type;
            b.weather = this.Weather;
            b.map = this.map;
            return b;
        }

        #endregion ICloneable Members
    }
}