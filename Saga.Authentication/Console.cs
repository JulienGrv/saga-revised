using Saga.Authentication.Utils.Definitions.Misc;
using Saga.Configuration;
using Saga.Managers;
using Saga.Shared.Definitions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;

namespace Saga.Authentication
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
                    //do nothing
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

        private static Saga.Managers.ConsoleCommands _ConsoleCommands;
        private static Saga.Managers.Database _Database;
        private static Saga.Managers.NetworkService _NetworkService;

        #endregion Private Members

        #region Public Members

        public static TraceLog generaltracelog = new TraceLog("General", "Entire Application", 4);

        #endregion Public Members

        #region Public Properties

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

        public static Saga.Managers.Database Database
        {
            get
            {
                return _Database;
            }
        }

        #endregion Public Properties

        #region Constructor

        static Singleton()
        {
            try
            {
                _ConsoleCommands = ManagerBase2.ProvideManager<ConsoleCommands>("Saga.Manager.ConsoleSettings");
                _NetworkService = ManagerBase2.ProvideManager<NetworkService>("Saga.Manager.NetworkSettings");
                _Database = ManagerBase2.ProvideManager<Database>("Saga.Manager.Database");
            }
            catch (Exception e)
            {
                HostContext.Current.AddUnhandeldException(e);
            }
        }

        #endregion Constructor

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
            int gatewayport = 64001;
            IPAddress mapip = IPAddress.Loopback;
            int mapport = 64002;
            string databaseusername = "root";
            string databasepassword = "root";
            uint dbport = 3306;
            string databasename = "saga_auth";
            string dbhost = "localhost";
            string provider = "Saga.Authentication.Data.Mysql.dll, Saga.Authentication.Data.Mysql.MysqlProvider";
            ConsoleReader reader = new ConsoleReader();
            reader.Clear(null);

            System.Configuration.Configuration b = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (CheckConfigExists() == false)
            {
                Console.WriteLine("First time run-configuration");
                char key;

                provider = FindPlugin(typeof(IDatabase), "Saga.Authentication.Data.Mysql.dll, Saga.Authentication.Data.Mysql.MysqlProvider");

            ConfigureGatewayNetwork:
                Console.WriteLine("Do you wan to configure the authentication-gateway network settings? Y/N");
                key = Console.ReadKey(true).KeyChar;
                if (key == 'y')
                {
                    Console.WriteLine("What ip should the authentication-gateway server listen to?");
                    while (!IPAddress.TryParse(Console.ReadLine(), out gatewayip))
                    {
                        Console.WriteLine("Incorrect value please use an ipv4 adress, recommended 0.0.0.0");
                    }

                    Console.WriteLine("What port should the authentication-gateway server listen to?");
                    while (!int.TryParse(Console.ReadLine(), out gatewayport))
                    {
                        Console.WriteLine("Incorrect value please use an number between 1024�49151, recommended 64001");
                    }
                }
                else if (key != 'n') goto ConfigureGatewayNetwork;

            ConfigureWorldNetwork:
                Console.WriteLine("Do you wan to configure the authentication-map network settings? Y/N");
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
                        Console.WriteLine("Incorrect value please use an number between 1024�49151, recommended 64000");
                    }
                }
                else if (key != 'n') goto ConfigureWorldNetwork;

            DatabaseName:
                Console.WriteLine("Do you wan to configure the database settings? Y/N");
                key = Console.ReadKey(true).KeyChar;
                if (key == 'y')
                {
                    Console.WriteLine("What is the database name?");
                    databasename = Console.ReadLine();

                    Console.WriteLine("What is the database username?");
                    databaseusername = Console.ReadLine();

                    Console.WriteLine("What is the database password?");
                    databasepassword = Console.ReadLine();

                    Console.WriteLine("What is the database port?");
                    while (!uint.TryParse(Console.ReadLine(), out dbport))
                    {
                        Console.WriteLine("Incorrect value please use an number between 1024�49151, recommended 3306");
                    }

                    Console.WriteLine("What is the database host?");
                    dbhost = Console.ReadLine();
                }
                else if (key != 'n') goto DatabaseName;

                //Adjust network settings
                NetworkSettings networkSettings = b.Sections["Saga.NetworkSettings"] as NetworkSettings;
                if (networkSettings == null) networkSettings = new NetworkSettings();

                NetworkFileCollection collection = networkSettings.Connections;
                collection["public"] = new NetworkElement("public", gatewayip.ToString(), gatewayport);
                collection["internal"] = new NetworkElement("internal", mapip.ToString(), mapport);
                b.Sections.Remove("Saga.NetworkSettings");
                b.Sections.Add("Saga.NetworkSettings", networkSettings);

                //Adjust database settings
                DatabaseSettings databaseSettings = b.Sections["Saga.Manager.Database"] as DatabaseSettings;
                if (databaseSettings == null) databaseSettings = new DatabaseSettings();
                databaseSettings.Database = databasename;
                databaseSettings.Username = databaseusername;
                databaseSettings.Password = databasepassword;
                databaseSettings.Port = dbport;
                databaseSettings.Host = dbhost;
                databaseSettings.DbType = provider;
                b.Sections.Remove("Saga.Manager.Database");
                b.Sections.Add("Saga.Manager.Database", databaseSettings);
                b.Save();
                ConfigurationManager.RefreshSection("Saga.NetworkSettings");
                ConfigurationManager.RefreshSection("Saga.Manager.Database");
                Console.WriteLine("Everything configured");
            }
        }

        private static string FindPlugin(Type type, string pluginDefault)
        {
            PluginSandbox plugins = PluginSandbox.FindPlugins(type);

            int a = 0;
            foreach (string plugin in plugins.foundPlugins)
            {
                string[] arr = plugin.Split(new char[] { ',' }, 2);
                string assembly = arr[0];
                string typed = arr[1];
                Console.WriteLine("{0} {1}", ++a, typed);
            }

            Console.WriteLine("Enter the number corresponding the plugin");
            while (!int.TryParse(Console.ReadLine(), out a))
            {
                Console.WriteLine("Please enter a number");
            }

            return (a > 0 && a <= plugins.foundPlugins.Count) ? plugins.foundPlugins[a - 1] : pluginDefault;
        }

        private static void Main(string[] args)
        {
            //Set managers
            ManagerBase2.SetTraceLog(generaltracelog);

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
            Trace.WriteLine(string.Format("Saga Authtentication Server starting on: {0}", DateTime.Now));
            Trace.WriteLine(string.Format("OS Information: {0}", Environment.OSVersion));
            Trace.WriteLine(string.Format("Number of Processors: {0}", Environment.ProcessorCount));
            Trace.WriteLine(string.Format("CLR Version: {0}", Environment.Version));
            Trace.WriteLine(string.Format("Working set: {0}", Environment.WorkingSet));
            Trace.WriteLine(string.Format("OS Bit Version: {0} Bit", IntPtr.Size * 8));
            Trace.WriteLine("#############################################################################");

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            FirstRunConfiguration();
            HostContext.Current.Initialize();
            HostContext.Current.BeforeQuerySettings();
            HostContext.Current.AfterQuerySettings();
            HostContext.Current.Load();
            HostContext.Current.Loaded();
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

            Trace.WriteLine("#############################################################################");
        }

        #endregion Entry-point
    }
}