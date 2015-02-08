using Saga.Configuration;
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
using System.Net.Sockets;
using System.Reflection;

namespace Saga.Factory
{
    /// <summary>
    /// Factory to contain spawn information for npc
    /// </summary>
    /// <remarks>
    /// This factory contains spawn information for npc. With ncp both monsters
    /// as regulair npc's are meant.
    /// </remarks>
    public class SpawnTemplate : FactoryBase
    {
        #region Ctor/Dtor

        /// <summary>
        /// Initializes a new SpawnTemplate manager
        /// </summary>
        public SpawnTemplate()
        {
        }

        #endregion Ctor/Dtor

        #region Internal Members

        /// <summary>
        /// Lookup table for templates asociated with modelid
        /// </summary>
        public Dictionary<uint, NpcInfo> templates;

        /// <summary>
        /// Lookup table for mapitems asociated with modelid
        /// </summary>
        public Dictionary<uint, ConstructorInfo> itemtemplates;

        /// <summary>
        /// Creates constructor info lookup table
        /// </summary>
        public Dictionary<string, ConstructorInfo> constructorinfo;

        #endregion Internal Members

        #region Protected Methods

        /// <summary>
        /// Initializes all member variables
        /// </summary>
        protected override void Initialize()
        {
            templates = new Dictionary<uint, NpcInfo>();
            itemtemplates = new Dictionary<uint, ConstructorInfo>();
            constructorinfo = new Dictionary<string, ConstructorInfo>();
        }

        /// <summary>
        /// Loads all listed files that contains folder information
        /// </summary>
        protected override void Load()
        {
            SpawntemplateSettings section = (SpawntemplateSettings)ConfigurationManager.GetSection("Saga.Factory.SpawnTemplate");
            if (section != null)
            {
                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("TemplateFactory", "Loading portal information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }
            }
            else
            {
                WriteWarning("XmlSections", "Section {0} was not found", "Saga.Factory.SpawnTemplate");
            }
        }

        /// <summary>
        /// Unloads unneeded memory
        /// </summary>
        protected override void FinishedLoading()
        {
            constructorinfo.Clear();
            constructorinfo = null;
            base.FinishedLoading();
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
                ProgressReport.Invoke();

                bool isnpcspawns = false;
                int line = 0;
                {
                    String row = c.ReadLine();
                    String[] fields = row.Split(',');
                    isnpcspawns = fields.Length > 20;
                }

                while (c.Peek() > 0)
                {
                    ++line;
                    String row = c.ReadLine();
                    String[] fields = row.Split(',');

                    //Npc Information detected
                    uint id = uint.Parse(fields[0], NumberFormatInfo.InvariantInfo);
                    if (isnpcspawns)
                    {
                        NpcInfo info = new NpcInfo();
                        info.HP = ushort.Parse(fields[3], NumberFormatInfo.InvariantInfo);
                        info.SP = ushort.Parse(fields[4], NumberFormatInfo.InvariantInfo);
                        info.Level = ushort.Parse(fields[5], NumberFormatInfo.InvariantInfo);
                        info.CEXP = ushort.Parse(fields[6], NumberFormatInfo.InvariantInfo);
                        info.JEXP = ushort.Parse(fields[7], NumberFormatInfo.InvariantInfo);
                        info.WEXP = ushort.Parse(fields[8], NumberFormatInfo.InvariantInfo);
                        info.Def = ushort.Parse(fields[9], NumberFormatInfo.InvariantInfo);
                        info.Flee = ushort.Parse(fields[10], NumberFormatInfo.InvariantInfo);
                        info.AtkMin = ushort.Parse(fields[11], NumberFormatInfo.InvariantInfo);
                        info.AtkMax = ushort.Parse(fields[12], NumberFormatInfo.InvariantInfo);
                        info.Cri = ushort.Parse(fields[13], NumberFormatInfo.InvariantInfo);
                        info.Hit = ushort.Parse(fields[14], NumberFormatInfo.InvariantInfo);
                        info.ASPD = ushort.Parse(fields[15], NumberFormatInfo.InvariantInfo);
                        info.Sightrange = ushort.Parse(fields[16], NumberFormatInfo.InvariantInfo);
                        info.Size = ushort.Parse(fields[17], NumberFormatInfo.InvariantInfo);
                        info.Walkspeed = uint.Parse(fields[18], NumberFormatInfo.InvariantInfo);
                        info.Runspeed = uint.Parse(fields[19], NumberFormatInfo.InvariantInfo);
                        info.AIMode = byte.Parse(fields[20], NumberFormatInfo.InvariantInfo);

                        if (TryGetConstructorInfo(fields[1], out info.info) == false)
                        {
                            throw new SystemException(string.Format("Constructor information on type {0} was not found on line: {1}", fields[1], line));
                        }

                        if (this.templates.ContainsKey(id))
                        {
                            throw new SystemException(string.Format("A duplicated entry has been detected: {0}-{1} on line {2}", id, fields[2], line));
                        }

                        this.templates.Add(id, info);
                    }
                    //Item information detected
                    else
                    {
                        ConstructorInfo info;
                        if (TryGetConstructorInfo(fields[1], out info) == false)
                        {
                            throw new SystemException(string.Format("Constructor information on type {0} was not found on line: {1}", fields[1], line));
                        }

                        if (this.itemtemplates.ContainsKey(id))
                        {
                            throw new SystemException(string.Format("A duplicated entry has been detected: {0}-{1} on line {2}", id, fields[2], line));
                        }

                        itemtemplates.Add(id, info);
                    }
                }
            }
        }

        /// <summary>
        /// Get's the constructor info by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        protected bool TryGetConstructorInfo(string name, out ConstructorInfo info)
        {
            if (constructorinfo.TryGetValue(name, out info))
                return true;

            object obj;

            if (CoreService.TryFindType(name, out obj))
                info = obj.GetType().GetConstructor(Type.EmptyTypes);
            else
                WriteError("Type not found {0}", name);

            bool isnotempty = info != null;
            if (isnotempty) constructorinfo[name] = info;
            return isnotempty;
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Fills the actor with data from the templates
        /// </summary>
        /// <param name="id">Id of the actor</param>
        /// <param name="mob">Npc instance</param>
        /// <returns></returns>
        public bool FillByTemplate(uint id, Monster mob)
        {
            NpcInfo info;
            bool result = templates.TryGetValue(id, out info);
            if (result)
            {
                mob._status = new BattleStatus();
                mob._status.CurrentHp = info.HP;
                mob._status.MaxHP = info.HP;
                mob._status.CurrentSp = info.SP;
                mob._status.MaxSP = info.SP;
                mob._level = (byte)info.Level;
                mob._CEXP = info.CEXP;
                mob._WEXP = info.WEXP;
                mob._JEXP = info.JEXP;
                mob._SIGHTRANGE = info.Sightrange;
                mob._status.MaxPAttack = info.AtkMax;
                mob._status.MinPAttack = info.AtkMin;
                mob._status.DefencePhysical = info.Def;
                mob._status.BasePEvasionrate = info.Flee;
                mob._status.BasePCritrate = info.Cri;
                mob._status.WalkingSpeed = (ushort)info.Walkspeed;

                //mob._DEF = info.Def;
                //mob._FLEE = info.Flee;
                //mob._ATKMIN = info.AtkMin;
                //mob._ATKMAX = info.AtkMax;
                //mob._CRIT = info.Cri;

                mob._ASPD = info.ASPD;
                mob._SIZE = info.Size;
                mob._AIMODE = info.AIMode;
                mob._WALKSPEED = (ushort)info.Walkspeed;
                mob._RUNSPEED = (ushort)info.Runspeed;
            }

            return result;
        }

        /// <summary>
        /// Fills the actor with data from the templates
        /// </summary>
        /// <param name="id">Id of the actor</param>
        /// <param name="npc">Npc instance</param>
        /// <returns></returns>
        public bool FillByTemplate(uint id, BaseNPC npc)
        {
            NpcInfo info;
            bool result = templates.TryGetValue(id, out info);
            if (result)
            {
                npc._status = new BattleStatus();
                npc._status.CurrentHp = info.HP;
                npc._status.MaxHP = info.HP;
                npc._status.CurrentSp = info.SP;
                npc._status.MaxSP = info.SP;
            }

            return result;
        }

        /// <summary>
        /// Spawns an instance of an npc
        /// </summary>
        /// <param name="id">Id of the npc id</param>
        /// <param name="position">Position of the npc</param>
        /// <param name="yaw">Yaw of the npc</param>
        /// <param name="zone">Zone where to register the npc</param>
        /// <param name="regionObject">returned object</param>
        /// <returns>Returns true if the object doesn't fail</returns>
        public bool SpawnNpcInstance(uint id, Point position, Rotator yaw, Zone zone, out MapObject regionObject)
        {
            if (CreateInstance(id, 0, out regionObject))
            {
                regionObject.Position = position;
                regionObject.Yaw = yaw;
                regionObject.currentzone = zone;
                regionObject.OnInitialize(position);
                regionObject.OnLoad();
                regionObject.OnSpawn();
                regionObject.OnRegister();

                if (regionObject.id > 0)
                {
                    Regiontree tree = regionObject.currentzone.Regiontree;
                    foreach (Character regionCharacter in tree.SearchActors(regionObject, Saga.Enumarations.SearchFlags.Characters))
                    {
                        try
                        {
                            if (regionCharacter.client.isloaded == false) continue;
                            if (Point.IsInSightRangeByRadius(regionCharacter.Position, regionObject.Position))
                                regionObject.ShowObject(regionCharacter);
                        }
                        catch (SocketException)
                        {
                            //Do nothing
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Spawns an instance of an npc
        /// </summary>
        /// <param name="id">Id of the npc id</param>
        /// <param name="position">Position of the npc</param>
        /// <param name="yaw">Yaw of the npc</param>
        /// <param name="zone">Zone where to register the npc</param>
        /// <param name="regionObject">returned object</param>
        /// <returns>Returns true if the object doesn't fail</returns>
        public bool SpawnNpcInstance(uint id, Point position, Rotator yaw, Zone zone, bool canRespawn, out MapObject regionObject)
        {
            if (CreateInstance(id, 0, out regionObject))
            {
                regionObject.Position = position;
                regionObject.Yaw = yaw;
                regionObject.currentzone = zone;
                regionObject.CanRespawn = canRespawn;
                regionObject.OnInitialize(position);
                regionObject.OnLoad();
                regionObject.OnSpawn();
                regionObject.OnRegister();

                if (regionObject.id > 0)
                {
                    Regiontree tree = regionObject.currentzone.Regiontree;
                    foreach (Character regionCharacter in tree.SearchActors(regionObject, Saga.Enumarations.SearchFlags.Characters))
                    {
                        try
                        {
                            if (regionCharacter.client.isloaded == false) continue;
                            if (Point.IsInSightRangeByRadius(regionCharacter.Position, regionObject.Position))
                                regionObject.ShowObject(regionCharacter);
                        }
                        catch (SocketException)
                        {
                            //Do nothing
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Spawns an instance of an item
        /// </summary>
        /// <param name="id">Id of the npc id</param>
        /// <param name="position">Position of the npc</param>
        /// <param name="yaw">Yaw of the npc</param>
        /// <param name="zone">Zone where to register the npc</param>
        /// <param name="regionObject">returned object</param>
        /// <returns>Returns true if the object doesn't fail</returns>
        public bool SpawnItemInstance(uint id, Point position, Rotator yaw, Zone zone, out MapObject regionObject)
        {
            if (CreateInstance(id, 1, out regionObject))
            {
                regionObject.Position = position;
                regionObject.Yaw = yaw;
                regionObject.currentzone = zone;
                regionObject.OnInitialize(position);
                regionObject.OnLoad();
                regionObject.OnSpawn();
                regionObject.OnRegister();

                if (regionObject.id > 0)
                {
                    Regiontree tree = regionObject.currentzone.Regiontree;
                    foreach (Character regionCharacter in tree.SearchActors(regionObject, Saga.Enumarations.SearchFlags.Characters))
                    {
                        try
                        {
                            if (regionCharacter.client.isloaded == false) continue;
                            if (Point.IsInSightRangeByRadius(regionCharacter.Position, regionObject.Position))
                                regionObject.ShowObject(regionCharacter);
                        }
                        catch (SocketException)
                        {
                            //Do nothing
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Unspawns a item
        /// </summary>
        /// <param name="regionObject"></param>
        public void UnspawnInstance(MapObject regionObject)
        {
            regionObject.OnDeregister();
            Regiontree tree = regionObject.currentzone.Regiontree;
            foreach (Character character in tree.SearchActors(regionObject, Saga.Enumarations.SearchFlags.Characters))
            {
                try
                {
                    if (character.client.isloaded == false) continue;
                    if (character.Target == regionObject) character.Target = null;
                    regionObject.HideObject(character);
                }
                catch (SocketException)
                {
                    //Do nothing
                }
            }
        }

        /// <summary>
        /// Creates an instance of the specified npcid
        /// </summary>
        /// <param name="id">Npc id to lookup</param>
        /// <param name="e">Object to return</param>
        /// <returns>True if the type was found</returns>
        public bool CreateInstance(uint id, byte type, out MapObject e)
        {
            e = null;
            if (type == 0)
            {
                NpcInfo d;
                if (templates.TryGetValue(id, out d))
                    e = d.info.Invoke(new object[] { }) as MapObject;
                if (e != null)
                    e.ModelId = id;
            }
            else if (type == 1)
            {
                ConstructorInfo d;
                if (itemtemplates.TryGetValue(id, out d))
                    e = d.Invoke(new object[] { }) as MapObject;
                if (e != null)
                    e.ModelId = id;
            }

            return e != null;
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
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_ADDITION; }
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
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_ADDITION; }
        }

        #endregion Protected Properties

        #region Nested Classes/Structures

        /// <summary>
        /// Structure used internal as storage for the spawntemplates.
        /// </summary>
        public struct NpcInfo
        {
            /// <summary>
            /// Npcid of the npc
            /// </summary>
            public uint ID;

            /// <summary>
            /// Constructor information
            /// </summary>
            public ConstructorInfo info;

            /// <summary>
            /// Maximum Hp of the actor
            /// </summary>
            public ushort HP;

            /// <summary>
            /// Maximum Sp of the actor
            /// </summary>
            public ushort SP;

            /// <summary>
            /// Level of the actor
            /// </summary>
            public ushort Level;

            /// <summary>
            /// Cexp reward of the actor
            /// </summary>
            public ushort CEXP;

            /// <summary>
            /// Jexp reward of the actor
            /// </summary>
            public ushort JEXP;

            /// <summary>
            /// Wexp reward of the actor
            /// </summary>
            public ushort WEXP;

            /// <summary>
            /// Physical Defense of the actor
            /// </summary>
            public ushort Def;

            /// <summary>
            /// Physical Evasion of the actor
            /// </summary>
            public ushort Flee;

            /// <summary>
            /// Physical minimum attack of the actor
            /// </summary>
            public ushort AtkMin;

            /// <summary>
            /// Physical maximum attack of the actor
            /// </summary>
            public ushort AtkMax;

            /// <summary>
            /// Physical critical rate of the actor
            /// </summary>
            public ushort Cri;

            /// <summary>
            /// Physical hitrate of the actor
            /// </summary>
            public ushort Hit;

            /// <summary>
            /// Attack speed of the actor
            /// </summary>
            public ushort ASPD;

            /// <summary>
            /// Sightrange of the actor
            /// </summary>
            public ushort Sightrange;

            /// <summary>
            /// Size of the actor
            /// </summary>
            /// <remarks>
            /// This property is used by the default AI to determine wether a
            /// monster/npc has reached it's destination.
            /// </remarks>
            public ushort Size;

            /// <summary>
            /// Walking speed of the actor
            /// </summary>
            public uint Walkspeed;

            /// <summary>
            /// Running speed of the actor
            /// </summary>
            /// <remarks>
            /// This speed is used when a mob is in chasing mode.
            /// </remarks>
            public uint Runspeed;

            /// <summary>
            /// Byte setting of ai modes.
            /// </summary>
            public byte AIMode;
        }

        #endregion Nested Classes/Structures
    }
}