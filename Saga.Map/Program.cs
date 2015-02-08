#pragma warning disable 3001

using Saga.Configuration;
using Saga.Factory;
using Saga.Managers;
using Saga.Map.Configuration;
using Saga.Shared.Definitions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Saga.Map
{
    [Serializable()]
    public class PluginSandbox
    {
        public List<string> foundPlugins = null;

        public static PluginSandbox FindPlugins(Type basetype)
        {
            PluginSandbox c = new PluginSandbox();
            c.foundPlugins = new List<string>();

            //GET THE CURRENT ASSEMBLY AND PATH
            string file = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = Path.GetDirectoryName(file);

            //CREATE A SANDBOX TO LOAD ALL PLUGINS FOR EXAMINATION
            AppDomain sandbox = AppDomain.CreateDomain("sandbox");
            PluginSandbox b = (PluginSandbox)sandbox.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(PluginSandbox).FullName);
            foreach (string d in b.test(path, basetype))
            {
                c.foundPlugins.Add(d);
            }

            AppDomain.Unload(sandbox);
            return c;
        }

        private IEnumerable<string> test(string path, Type checkType)
        {
            List<string> foundPlugins = new List<string>();
            string[] mfiles = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < mfiles.Length; i++)
            {
                try
                {
                    Assembly current = Assembly.LoadFile(mfiles[i]);
                    Type[] types = current.GetExportedTypes();

                    foreach (Type type in current.GetExportedTypes())
                    {
                        if (type.IsSubclassOf(checkType))
                        {
                            foundPlugins.Add(string.Format("{0}, {1}", type.Assembly.Location, type.FullName));
                            continue;
                        }

                        TypeFilter b = delegate(Type filterType, object filter)
                        {
                            return filterType.FullName == checkType.FullName;
                        };

                        Type[] arr = type.FindInterfaces(b, null);
                        if (arr.Length > 0)
                        {
                            foundPlugins.Add(string.Format("{0}, {1}", type.Assembly.Location, type.FullName));
                        }
                    }
                }
                catch (TypeLoadException)
                {
                    //Do nothing;
                }
                catch (BadImageFormatException)
                {
                    //do nothing;
                }
            }

            for (int i = 0; i < foundPlugins.Count; i++)
            {
                yield return foundPlugins[i];
            }
        }
    }

    public static partial class Singleton
    {
        #region Private Members

        private static Saga.Factory.ItemsDrops _ItemDrops;
        private static Saga.Factory.CharacterConfiguration _CharacterConfiguration;
        private static Saga.Factory.SpawnMultiWorldObjects _SpawnMultiWorldObjectSettings;
        private static Saga.Factory.SpawnWorldObjects _SpawnWorldObjects;
        private static Saga.Factory.SpawnTemplate _SpawnTemplates;
        private static Saga.Factory.Weaponary _Weaponary;
        private static Saga.Factory.Zones _Zones;
        private static Saga.Factory.Warps _Warps;
        private static Saga.Factory.StatusByLevel _StatusByLevel;
        private static Saga.Factory.Spells _Spells;
        private static Saga.Factory.Portals _Portals;
        private static Saga.Factory.Additions _Additions;
        private static Saga.Factory.ItemsFactory _ItemFactory;
        private static Saga.Factory.EventManager _EventManager;
        private static Saga.Managers.ConsoleCommands _ConsoleCommands;
        private static Saga.Managers.Database _Database;
        private static Saga.Managers.NetworkService _NetworkService;
        private static Saga.Managers.ScriptCompiler _ScriptCompiler;
        private static Saga.Managers.WorldTasks _WorldTasks;
        private static Saga.Managers.Quests _Quests;
        private static Dictionary<string, ManagerBase2> _CustomManagers;

        #endregion Private Members

        #region Public Members

        public static TraceLog generaltracelog = new TraceLog("General", "Entire Application", 4);

        #endregion Public Members

        #region Constructor / Deconstructor

        static Singleton()
        {
            try
            {
                _CustomManagers = new Dictionary<string, ManagerBase2>();
            }
            catch (Exception e)
            {
                HostContext.Current.AddUnhandeldException(e);
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }

        #endregion Constructor / Deconstructor

        #region Built-in Managers

        public static Saga.Factory.Additions Additions
        {
            get
            {
                return _Additions;
            }
        }

        public static Saga.Factory.StatusByLevel experience
        {
            get
            {
                return _StatusByLevel;
            }
        }

        public static Saga.Managers.NetworkService NetworkService
        {
            get
            {
                return _NetworkService;
            }
        }

        public static Saga.Managers.ConsoleCommands ConsoleCommands
        {
            get
            {
                return _ConsoleCommands;
            }
        }

        public static Saga.Factory.Weaponary Weapons
        {
            get
            {
                return _Weaponary;
            }
        }

        public static Saga.Factory.Warps Warps
        {
            get
            {
                return _Warps;
            }
        }

        public static Saga.Factory.Spells SpellManager
        {
            get
            {
                return _Spells;
            }
        }

        public static Saga.Factory.Zones Zones
        {
            get
            {
                return _Zones;
            }
        }

        public static ItemsFactory Item
        {
            get
            {
                return _ItemFactory;
            }
        }

        public static Saga.Factory.Portals portal
        {
            get
            {
                return _Portals;
            }
        }

        public static Saga.Factory.SpawnWorldObjects QuestBoardSpawnManager
        {
            get
            {
                return _SpawnWorldObjects;
            }
        }

        public static Saga.Factory.SpawnMultiWorldObjects NpcSpawnManager
        {
            get
            {
                return _SpawnMultiWorldObjectSettings;
            }
        }

        public static Saga.Factory.SpawnTemplate Templates
        {
            get
            {
                return _SpawnTemplates;
            }
        }

        public static Saga.Managers.ScriptCompiler Scripting
        {
            get
            {
                return _ScriptCompiler;
            }
        }

        public static Saga.Factory.CharacterConfiguration CharacterConfiguration
        {
            get
            {
                return _CharacterConfiguration;
            }
        }

        public static Saga.Factory.ItemsDrops Itemdrops
        {
            get
            {
                return _ItemDrops;
            }
        }

        public static Saga.Managers.Database Database
        {
            get
            {
                return _Database;
            }
        }

        public static Saga.Managers.Quests Quests
        {
            get
            {
                return _Quests;
            }
        }

        public static Saga.Managers.WorldTasks WorldTasks
        {
            get
            {
                return _WorldTasks;
            }
        }

        public static Saga.Factory.EventManager EventManager
        {
            get
            {
                return _EventManager;
            }
        }

        #endregion Built-in Managers

        #region Entry-point

        private static bool CheckConfigExists()
        {
            //GET THE ASSEMBLY'S DIRECTORY
            string file = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string aname = Path.GetFileName(file);
            string bname = Path.GetFileNameWithoutExtension(file);
            aname = Path.Combine(Environment.CurrentDirectory, aname + ".config");
            bname = Path.Combine(Environment.CurrentDirectory, bname + ".config");
            return File.Exists(aname) | File.Exists(bname);
        }

        private static void FirstRunConfiguration()
        {
            IPAddress gatewayip = IPAddress.Loopback;
            int gatewayport = 64003;
            IPAddress mapip = IPAddress.Loopback;
            byte worldid = 0;
            int mapport = 64002;
            int playerlimit = 60;
            string databaseusername = "root";
            string databasepassword = "root";
            uint dbport = 3306;
            int cexprates = 1;
            int jexprates = 1;
            int wexprates = 1;
            int droprates = 1;
            string databasename = "saga_world";
            string dbhost = "localhost";
            string dbprovider = "Saga.Map.Data.Mysql.dll, Saga.Map.Data.Mysql.MysqlProvider";
            string proof = "c4ca4238a0b923820dcc509a6f75849b";
            string questplugin = "Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.LuaQuestProvider";
            string scenarioquestplugin = "Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.ScenarioLuaQuest";
            string worldspawn = "Saga.Map.Plugins.dll, Saga.Map.Plugins.MultifileSpawnWorldObjects";
            string multiworldspawn = "Saga.Map.Plugins.dll, Saga.Map.Plugins.MultifileSpawnMultiWorldObjects";
            string eventprovider = "Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.EventInfo";
            ConsoleReader reader = new ConsoleReader();
            reader.Clear(null);

            System.Configuration.Configuration b = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (CheckConfigExists() == false)
            {
                Console.WriteLine("First time run-configuration");
                char key;

                #region Always Configure

                ConfigureRequired("What is the world id of this server?");
                while (!byte.TryParse(Console.ReadLine(), out worldid))
                {
                    Console.WriteLine("Incorrect value please use an number between 0–255");
                }

                ConfigureRequired("What is the player limit of this server?");
                while (!int.TryParse(Console.ReadLine(), out playerlimit))
                {
                    Console.WriteLine("Incorrect value please use an valid number");
                }

                ConfigureRequired("What is the authentication proof of this server?");
                MD5 md5 = MD5.Create();
                byte[] block = Encoding.UTF8.GetBytes(Console.ReadLine());
                byte[] md5block = md5.ComputeHash(block);
                StringBuilder builder = new StringBuilder();
                foreach (byte c in md5block)
                    builder.AppendFormat("{0:X2}", c);
                proof = builder.ToString();

                ConfigureRequired("What are the cexp-rates?");
                while (!int.TryParse(Console.ReadLine(), out cexprates))
                {
                    Console.WriteLine("Incorrect value please use an between 1 and 20");
                }
                cexprates = Math.Min(20, Math.Max(cexprates, 1));

                ConfigureRequired("What are the jexp-rates?");
                while (!int.TryParse(Console.ReadLine(), out jexprates))
                {
                    Console.WriteLine("Incorrect value please use an between 1 and 20");
                }
                jexprates = Math.Min(20, Math.Max(jexprates, 1));

                ConfigureRequired("What are the wexp-rates?");
                while (!int.TryParse(Console.ReadLine(), out wexprates))
                {
                    Console.WriteLine("Incorrect value please use an between 1 and 20");
                }
                wexprates = Math.Min(20, Math.Max(wexprates, 1));

                ConfigureRequired("What are the item drop-rates?");
                while (!int.TryParse(Console.ReadLine(), out droprates))
                {
                    Console.WriteLine("Incorrect value please use an between 1 and 20");
                }
                droprates = Math.Min(20, Math.Max(droprates, 1));

                ConfigureRequired("Detect database plugin");
                dbprovider = FindPlugin(typeof(IDatabase), dbprovider);

                ConfigureRequired("Detect quest plugin");
                questplugin = FindPlugin(typeof(Saga.Quests.IQuest), questplugin);

                ConfigureRequired("Detect scenarion quest plugin");
                scenarioquestplugin = FindPlugin(typeof(Saga.Quests.ISceneraioQuest), scenarioquestplugin);

                ConfigureRequired("Detect npc & map spawn plugin");
                worldspawn = FindPlugin(typeof(Saga.Factory.SpawnWorldObjects), worldspawn);

                ConfigureRequired("Detect mob spawn plugin");
                multiworldspawn = FindPlugin(typeof(Saga.Factory.SpawnMultiWorldObjects), multiworldspawn);

                ConfigureRequired("Detect event plugin");
                eventprovider = FindPlugin(typeof(Saga.Factory.EventManager.BaseEventInfo), eventprovider);

                #endregion Always Configure

                #region Network Settings

            ConfigureGatewayNetwork:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Do you wan to configure the gateway-map network settings? Y/N");
                Console.ResetColor();
                key = Console.ReadKey(true).KeyChar;
                if (key == 'y')
                {
                    Console.WriteLine("What ip should the gateway-map server listen to?");
                    while (!IPAddress.TryParse(Console.ReadLine(), out gatewayip))
                    {
                        Console.WriteLine("Incorrect value please use an ipv4 adress, recommended 0.0.0.0");
                    }

                    Console.WriteLine("What port should the gateway-map server listen to?");
                    while (!int.TryParse(Console.ReadLine(), out gatewayport))
                    {
                        Console.WriteLine("Incorrect value please use an number between 1024–49151, recommended 64003");
                    }
                }
                else if (key != 'n') goto ConfigureGatewayNetwork;

            ConfigureWorldNetwork:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Do you wan to configure the authentication-map network settings? Y/N");
                Console.ResetColor();
                key = Console.ReadKey(true).KeyChar;
                if (key == 'y')
                {
                    Console.WriteLine("What ip should the authentication-map server listen to?");
                    while (!IPAddress.TryParse(Console.ReadLine(), out mapip))
                    {
                        Console.WriteLine("Incorrect value please use an ipv4 adress, recommended 0.0.0.0");
                    }

                    Console.WriteLine("On what port is the authentication server listening");
                    while (!int.TryParse(Console.ReadLine(), out mapport))
                    {
                        Console.WriteLine("Incorrect value please use an number between 1024–49151, recommended 64002");
                    }
                }
                else if (key != 'n') goto ConfigureWorldNetwork;

                #endregion Network Settings

                #region Database Settings

            DatabaseName:
                ConfigureOptional("Do you want to configure the database settings? Y/N");
                key = Console.ReadKey(true).KeyChar;
                if (key == 'y')
                {
                    ConfigureRequired("What is the database name?");
                    databasename = Console.ReadLine();

                    ConfigureRequired("What is the database username?");
                    databaseusername = Console.ReadLine();

                    ConfigureRequired("What is the database password?");
                    databasepassword = Console.ReadLine();

                    ConfigureRequired("What is the database port?");
                    while (!uint.TryParse(Console.ReadLine(), out dbport))
                    {
                        Console.WriteLine("Incorrect value please use an number between 1024–49151, recommended 3306");
                    }

                    ConfigureRequired("What is the database host?");
                    dbhost = Console.ReadLine();
                }
                else if (key != 'n') goto DatabaseName;

                #endregion Database Settings



                #region Plugin detection

            PluginDetection:
                ConfigureOptional("Do you want to dectect other plugins?");
                key = Console.ReadKey(true).KeyChar;
                if (key == 'y')
                {
                    Console.WriteLine("no plugins detected");
                }
                else if (key != 'n') goto PluginDetection;

                #endregion Plugin detection

                //CONFIGURE SERVER SETTINGS
                ServerVars serverVarsConfiguration = new ServerVars();
                serverVarsConfiguration.DataDirectory = "../Data/";
                b.Sections.Add("Saga.ServerVars", serverVarsConfiguration);

                //CONFIGURE NETWORK SETTINGS
                NetworkSettings networkSettings = new NetworkSettings();
                NetworkFileCollection collection = networkSettings.Connections;
                collection["public"] = new NetworkElement("public", gatewayip.ToString(), gatewayport);
                collection["internal"] = new NetworkElement("internal", mapip.ToString(), mapport);
                b.Sections.Remove("Saga.Manager.NetworkSettings");
                b.Sections.Add("Saga.Manager.NetworkSettings", networkSettings);
                networkSettings.WorldId = worldid;
                networkSettings.Proof = proof;
                networkSettings.PlayerLimit = playerlimit;

                //CONFIGURE CONSOLE SETTING
                ConsoleSettings consoleSettings = new ConsoleSettings();
                consoleSettings.CommandPrefix = "@";
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.Broadcast"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.Position"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.ChatMute"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.GmWarptomap"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.PlayerJump"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.PlayerCall"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.Speed"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.GarbageCollector"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.ClearNpc"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.KickAll"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.Kick"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.Time"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.ShowMaintenance"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.ScheduleMaintenance"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.SetGmLevel"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.Spawn"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.Unspawn"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.GiveItem"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.QStart"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.Kill"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.Worldload"));
                consoleSettings.GmCommands.Add(new FactoryFileElement("Saga.Scripting.Console.Gmgo"));
                b.Sections.Add("Saga.Manager.ConsoleSettings", consoleSettings);

                //PORTALS
                PortalSettings portalSettings = new PortalSettings();
                portalSettings.FolderItems.Add(new FactoryFileElement("~/portal_data.csv", "text/csv"));
                b.Sections.Remove("Saga.Factory.Portals");
                b.Sections.Add("Saga.Factory.Portals", portalSettings);

                //CHARACTERCONFIGURATION
                CharacterConfigurationSettings characterconfigurationSettings = new CharacterConfigurationSettings();
                characterconfigurationSettings.FolderItems.Add(new FactoryFileElement("~/character-template.csv", "text/csv"));
                b.Sections.Remove("Saga.Factory.CharacterConfiguration");
                b.Sections.Add("Saga.Factory.CharacterConfiguration", characterconfigurationSettings);

                //ADDITION
                AdditionSettings additionSettings = new AdditionSettings();
                additionSettings.FolderItems.Add(new FactoryFileElement("~/Addition_t.xml", "text/xml"));
                additionSettings.Reference = "~/addition_reference.csv";
                b.Sections.Remove("Saga.Factory.Addition");
                b.Sections.Add("Saga.Factory.Addition", additionSettings);

                //SPELLS
                SpellSettings spellSettings = new SpellSettings();
                spellSettings.FolderItems.Add(new FactoryFileElement("~/spell_data.xml", "text/xml"));
                spellSettings.Reference = "~/skill_reference.csv";
                b.Sections.Remove("Saga.Factory.Spells");
                b.Sections.Add("Saga.Factory.Spells", spellSettings);

                //STATUSBYLEVEL
                StatusByLevelSettings statusbylevelSettings = new StatusByLevelSettings();
                statusbylevelSettings.FolderItems.Add(new FactoryFileElement("~/experience.csv", "text/csv"));
                statusbylevelSettings.Cexp = cexprates;
                statusbylevelSettings.Jexp = jexprates;
                statusbylevelSettings.Wexp = wexprates;
                statusbylevelSettings.Droprate = droprates;

                b.Sections.Remove("Saga.Factory.StatusByLevel");
                b.Sections.Add("Saga.Factory.StatusByLevel", statusbylevelSettings);

                //WARPSETTINGS
                WarpSettings warpSettings = new WarpSettings();
                warpSettings.FolderItems.Add(new FactoryFileElement("~/warp_data.csv", "text/csv"));
                b.Sections.Remove("Saga.Factory.Warps");
                b.Sections.Add("Saga.Factory.Warps", warpSettings);

                //ZONES
                ZoneSettings zoneSettings = new ZoneSettings();
                zoneSettings.FolderItems.Add(new FactoryFileElement("~/zone_data.csv", "text/csv"));
                zoneSettings.Directory = "../Data/heightmaps";
                b.Sections.Remove("Saga.Factory.Zones");
                b.Sections.Add("Saga.Factory.Zones", zoneSettings);

                //ITEMS
                ItemSettings itemSettings = new ItemSettings();
                itemSettings.FolderItems.Add(new FactoryFileElement("~/item_data.xml", "text/xml"));
                b.Sections.Remove("Saga.Factory.Items");
                b.Sections.Add("Saga.Factory.Items", itemSettings);

                //WEAPONARY
                WeaponarySettings weaponarySettings = new WeaponarySettings();
                weaponarySettings.FolderItems.Add(new FactoryFileElement("~/weapon_data.csv", "text/csv"));
                b.Sections.Remove("Saga.Factory.Weaponary");
                b.Sections.Add("Saga.Factory.Weaponary", weaponarySettings);

                //SPAWNTEMPLATE
                SpawntemplateSettings spawntemplateSettings = new SpawntemplateSettings();
                spawntemplateSettings.FolderItems.Add(new FactoryFileElement("~/npc_templates.csv", "text/csv"));
                spawntemplateSettings.FolderItems.Add(new FactoryFileElement("~/item_templates.csv", "text/csv"));
                b.Sections.Remove("Saga.Factory.SpawnTemplate");
                b.Sections.Add("Saga.Factory.SpawnTemplate", spawntemplateSettings);

                //SPAWNS NPC & MAP
                SpawnWorldObjectSettings spawnworldobjectSettings = new SpawnWorldObjectSettings();
                spawnworldobjectSettings.FolderItems.Add(new FactoryFileElement("~/npc-spawns/", "text/csv"));
                spawnworldobjectSettings.FolderItems.Add(new FactoryFileElement("~/item-spawns/", "text/csv"));
                spawnworldobjectSettings.DerivedType = worldspawn;
                b.Sections.Remove("Saga.Factory.SpawnWorldObjects");
                b.Sections.Add("Saga.Factory.SpawnWorldObjects", spawnworldobjectSettings);

                //SPAWNS MOBS
                SpawnMultiWorldObjectSettings spawnmultiworldobjectSettings = new SpawnMultiWorldObjectSettings();
                spawnmultiworldobjectSettings.FolderItems.Add(new FactoryFileElement("~/mob-spawns/", "text/csv"));
                spawnmultiworldobjectSettings.DerivedType = multiworldspawn;
                b.Sections.Remove("Saga.Factory.SpawnMultiWorldObjects");
                b.Sections.Add("Saga.Factory.SpawnMultiWorldObjects", spawnmultiworldobjectSettings);

                //SCRIPTING
                ScriptingSettings scriptingSettings = new ScriptingSettings();
                scriptingSettings.Directory = "../Saga.Scripting";
                scriptingSettings.Assemblies.Add(new FactoryFileElement("System.dll", "text/csv"));
                scriptingSettings.Assemblies.Add(new FactoryFileElement("System.Data.dll", "text/csv"));
                scriptingSettings.Assemblies.Add(new FactoryFileElement("System.Xml.dll", "text/csv"));
                b.Sections.Remove("Saga.Manager.Scripting");
                b.Sections.Add("Saga.Manager.Scripting", scriptingSettings);

                //EVENTS
                EventSettings eventSettings = new EventSettings();
                eventSettings.FolderItems.Add(new FactoryFileElement("~/eventlist.csv", "text/csv"));
                eventSettings.Provider = eventprovider;
                b.Sections.Remove("Saga.Factory.Events");
                b.Sections.Add("Saga.Factory.Events", eventSettings);

                //QUUESTS
                QuestSettings questSettings = new QuestSettings();
                questSettings.Directory = "../Quests/";
                questSettings.SDirectory = "~/Scenario.Quests/";
                questSettings.Provider = questplugin;
                questSettings.ScenarioProvider = scenarioquestplugin;
                b.Sections.Remove("Saga.Manager.Quest");
                b.Sections.Add("Saga.Manager.Quest", questSettings);

                //DATABASE SETTINGS
                DatabaseSettings databaseSettings = new DatabaseSettings();
                databaseSettings.Database = databasename;
                databaseSettings.Username = databaseusername;
                databaseSettings.Password = databasepassword;
                databaseSettings.Port = dbport;
                databaseSettings.Host = dbhost;
                databaseSettings.DBType = dbprovider;
                b.Sections.Remove("Saga.Manager.Database");
                b.Sections.Add("Saga.Manager.Database", databaseSettings);

                //SAVE CONFIGURATION AND REFRESH ALL SECTIONS
                b.Save();

                //REFRESH ALL SECTIONS
                ConfigurationManager.RefreshSection("Saga.Factory.SpawnMultiWorldObjects");
                ConfigurationManager.RefreshSection("Saga.Manager.Database");
                ConfigurationManager.RefreshSection("Saga.Manager.Quest");
                ConfigurationManager.RefreshSection("Saga.Manager.Scripting");
                ConfigurationManager.RefreshSection("Saga.Factory.Events");
                ConfigurationManager.RefreshSection("Saga.Factory.SpawnWorldObject");
                ConfigurationManager.RefreshSection("Saga.ServerVars");
                ConfigurationManager.RefreshSection("Saga.Manager.NetworkSettings");
                ConfigurationManager.RefreshSection("Saga.Manager.ConsoleSettings");
                ConfigurationManager.RefreshSection("Saga.Factory.Portals");
                ConfigurationManager.RefreshSection("Saga.Factory.CharacterConfiguration");
                ConfigurationManager.RefreshSection("Saga.Factory.Addition");
                ConfigurationManager.RefreshSection("Saga.Factory.Spells");
                ConfigurationManager.RefreshSection("Saga.Factory.StatusByLevel");
                ConfigurationManager.RefreshSection("Saga.Factory.Warps");
                ConfigurationManager.RefreshSection("Saga.Factory.Zones");
                ConfigurationManager.RefreshSection("Saga.Factory.Items");
                ConfigurationManager.RefreshSection("Saga.Factory.Weaponary");
                ConfigurationManager.RefreshSection("Saga.Factory.SpawnTemplate");

                Console.WriteLine("Everything configured");
            }
        }

        private static void ConfigureRequired(string title)
        {
            // Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(title);
            //Console.ResetColor();
        }

        private static void ConfigureOptional(string title)
        {
            // Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(title);
            // Console.ResetColor();
        }

        private static string FindPlugin(Type type, string pluginDefault)
        {
            PluginSandbox plugins = PluginSandbox.FindPlugins(type);

            int a = 0;
            if (plugins.foundPlugins.Count > 0)
            {
                foreach (string plugin in plugins.foundPlugins)
                {
                    string[] arr = plugin.Split(new char[] { ',' }, 2);
                    string assembly = arr[0];
                    string typed = arr[1];
                    Console.WriteLine("{0} {1}", ++a, typed);
                }

                Console.WriteLine("Enter the number corresponding the plugin");
                while (!int.TryParse(Console.ReadLine(), out a) || a == 0)
                {
                    Console.WriteLine("Please enter a number");
                }

                return (a > 0 && a <= plugins.foundPlugins.Count) ? plugins.foundPlugins[a - 1] : pluginDefault;
            }
            else
            {
                Console.WriteLine("No plugin found");
                return pluginDefault;
            }
        }

        [LoaderOptimizationAttribute(LoaderOptimization.SingleDomain)]
        private static void Main(string[] args)
        {
            //Set managers
            ManagerBase2.SetTraceLog(generaltracelog);
            CoreService.SetTraceLog(generaltracelog);

            //GET THE ASSEMBLY'S DIRECTORY
            string file = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fname = Path.GetFileNameWithoutExtension(file);

            //TRACELOG
            Trace.AutoFlush = true;
            Trace.IndentSize = 4;
            if (System.Diagnostics.Trace.Listeners.Count <= 1 && (System.Diagnostics.Trace.Listeners["Default"] == null ||
                System.Diagnostics.Trace.Listeners["Default"].GetType() == typeof(System.Diagnostics.DefaultTraceListener)))
            {
                DelimitedListTraceListener del = new System.Diagnostics.DelimitedListTraceListener((fname + ".log.csv"), "text");
                del.Delimiter = ",";
                System.Diagnostics.Trace.Listeners.Add(del);
            }

            Trace.WriteLine("#############################################################################");
            Trace.WriteLine(string.Format("Saga Map Server starting on: {0}", DateTime.Now));
            Trace.WriteLine(string.Format("OS Information: {0}", Environment.OSVersion));
            Trace.WriteLine(string.Format("Number of Processors: {0}", Environment.ProcessorCount));
            Trace.WriteLine(string.Format("CLR Version: {0}", Environment.Version));
            Trace.WriteLine(string.Format("Working set: {0}", Environment.WorkingSet));
            Trace.WriteLine(string.Format("OS Bit Version: {0} Bit", IntPtr.Size * 8));
            Trace.WriteLine("#############################################################################");

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            //Do initial configuration
            FirstRunConfiguration();

            _ConsoleCommands = ManagerBase2.ProvideManager<ConsoleCommands>("Saga.Manager.ConsoleSettings");
            _NetworkService = ManagerBase2.ProvideManager<NetworkService>("Saga.Manager.NetworkSettings");
            _WorldTasks = ManagerBase2.ProvideManager<WorldTasks>("Saga.Manager.WorldTasks");
            _Database = ManagerBase2.ProvideManager<Database>("Saga.Manager.Database");
            _Quests = ManagerBase2.ProvideManager<Saga.Managers.Quests>("Saga.Manager.Quest");
            _ScriptCompiler = ManagerBase2.ProvideManager<ScriptCompiler>("Saga.Manager.Scripting");
            _EventManager = FactoryBase.ProvideManager<EventManager>("Saga.Factory.Events");
            _Additions = FactoryBase.ProvideManager<Additions>("Saga.Factory.Addition");
            _Portals = FactoryBase.ProvideManager<Portals>("Saga.Factory.Portals");
            _Spells = FactoryBase.ProvideManager<Saga.Factory.Spells>("Saga.Factory.Spells");
            _StatusByLevel = FactoryBase.ProvideManager<Saga.Factory.StatusByLevel>("Saga.Factory.StatusByLevel");
            _Warps = FactoryBase.ProvideManager<Warps>("Saga.Factory.Warps");
            _Zones = FactoryBase.ProvideManager<Zones>("Saga.Factory.Zones");
            _ItemFactory = FactoryBase.ProvideManager<ItemsFactory>("Saga.Factory.Items");
            _Weaponary = FactoryBase.ProvideManager<Weaponary>("Saga.Factory.Weaponary");
            _SpawnTemplates = FactoryBase.ProvideManager<SpawnTemplate>("Saga.Factory.SpawnTemplate");
            _SpawnWorldObjects = FactoryBase.ProvideManager<SpawnWorldObjects>("Saga.Factory.SpawnWorldObjects");
            _SpawnMultiWorldObjectSettings = FactoryBase.ProvideManager<SpawnMultiWorldObjects>("Saga.Factory.SpawnMultiWorldObjects");
            _CharacterConfiguration = FactoryBase.ProvideManager<CharacterConfiguration>("Saga.Factory.CharacterConfiguration");
            _ItemDrops = FactoryBase.ProvideManager<ItemsDrops>("Saga.Factory.ItemDrops");

            try
            {
                ManagerCollection section = (ManagerCollection)ConfigurationManager.GetSection("Saga.Managers");
                if (section != null)
                {
                    foreach (SingletonManagerElement element in section)
                    {
                        ManagerBase2 managerBase = ManagerBase2.ProvideManagerFromTypeString<ManagerBase2>(element.Type);
                        _CustomManagers.Add(element.Name, managerBase);
                    }
                }
            }
            catch (Exception e)
            {
                HostContext.Current.AddUnhandeldException(e);
            }
            finally
            {
                HostContext.Current.Initialize();
                HostContext.Current.BeforeQuerySettings();
                HostContext.Current.AfterQuerySettings();
                HostContext.Current.Load();
                HostContext.Current.Loaded();
            }

            Console.ReadLine();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.WriteLine("#############################################################################");
            Trace.WriteLine("A unhandeld exception was thrown");
            if (e.IsTerminating)
                Trace.WriteLine("CLR is terminating");
            Trace.WriteLine("");
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                Trace.WriteLine(ex.ToString());
            }
            Trace.WriteLine("Starting backup procedure");
            foreach (Saga.PrimaryTypes.Character tempCharacter in Tasks.LifeCycle.Characters)
            {
                Trace.WriteLine(string.Format("backup: {0}", tempCharacter.Name));
                byte[] b = Singleton.Database.Serialize(tempCharacter);
                Singleton.Database.WriteBytes(tempCharacter.Name, tempCharacter.ModelId, b);
            }
            Trace.WriteLine("backup complete");
            Trace.WriteLine("");
            Trace.WriteLine("#############################################################################");
        }

        #endregion Entry-point
    }
}