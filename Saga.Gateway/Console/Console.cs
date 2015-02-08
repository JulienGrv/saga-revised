using Saga.Configuration;
using Saga.Core;
using Saga.Gateway.Network;
using Saga.Shared.Definitions;
using Saga.Shared.NetworkCore;
using Saga.Shared.PacketLib.Other;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Saga.Gateway
{
    public static partial class Program
    {
        #region Private Members

        private static int gatewayport = 64000;
        private static IPAddress gatewayip = IPAddress.Any;
        private static int authenticationport = 64001;
        private static IPAddress authenticationip = IPAddress.Loopback;
        private static string guidkey = "A928CDC9DBE8751B3BC99EB65AE07E0C849CE739";
        private static string crckey = "ED90AA25AE906FB36308C8523A4737A7E7B1FC6F";
        private static EncryptedManager<GatewayClient> networkManger;
        private static ConsoleReader reader;

        #endregion Private Members

        #region Public Properties

        public static string CrcKey
        {
            get
            {
                return crckey;
            }
        }

        public static string GuidKey
        {
            get
            {
                return guidkey;
            }
        }

        #endregion Public Properties

        #region Others

        private static void reader_Initialize(object sender, EventArgs e)
        {
            //HELPER VARIABLES
            bool success = false;
            string host = "127.0.0.1";
            int port = 0;

            //GET THE NETWORK SETTINGS
            NetworkSettings section = (NetworkSettings)ConfigurationManager.GetSection("Saga.NetworkSettings");
            if (section != null)
            {
                NetworkElement Element = section.Connections["public"];
                if (Element != null)
                {
                    host = Element.Host;
                    port = Element.Port;
                }
            }

            while (success == false)
            {
                try
                {
                    //START LISTENING TO NETWORK
                    networkManger = new EncryptedManager<GatewayClient>(host, port);
                    networkManger.Start();
                    success = true;
                    Console.WriteLine("Accepting connections from: {0}:{1}", host, port);
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10049)
                    {
                        Trace.TraceError("The ip adress {0}:{1} is not an local ip adress", host, port);
                        Console.WriteLine("The ip adress {0}:{1} is not an local ip adress", host, port);
                        Thread.Sleep(60000);
                    }
                    else if (ex.ErrorCode == 10048)
                    {
                        Trace.TraceError("The port number is already in use: {0}:{1}", host, port);
                        Console.WriteLine("The port number is already in use: {0}:{1}", host, port);
                        Thread.Sleep(60000);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (FormatException)
                {
                    Trace.TraceError("The ip adress {0}:{1} is invalid formatted", host, port);
                    Console.WriteLine("The ip adress {0}:{1} is invalid formatted", host, port);
                    Thread.Sleep(60000);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    Thread.Sleep(60000);
                }
            }
        }

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

        [STAThread()]
        [LoaderOptimizationAttribute(LoaderOptimization.SingleDomain)]
        private static void Main(string[] args)
        {
            reader = new ConsoleReader();
            reader.Title = "Gateway server, type help for commands";
            reader.Initialize += new EventHandler(reader_Initialize);
            reader.Register(new ConsoleCommandHandler(Version));
            reader.Register(new ConsoleCommandHandler(CrcValidationCheck));
            reader.Register(new ConsoleCommandHandler(CheckHost));
            reader.Register(new ConsoleCommandHandler(Shutdown));
            reader.Clear(null);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            //SagaConfigurationManager.read();
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
            Trace.WriteLine(string.Format("Saga Gateway Server starting on: {0}", DateTime.Now));
            Trace.WriteLine(string.Format("OS Information: {0}", Environment.OSVersion));
            Trace.WriteLine(string.Format("Number of Processors: {0}", Environment.ProcessorCount));
            Trace.WriteLine(string.Format("CLR Version: {0}", Environment.Version));
            Trace.WriteLine(string.Format("Working set: {0}", Environment.WorkingSet));
            Trace.WriteLine(string.Format("OS Bit Version: {0} Bit", IntPtr.Size * 8));
            Trace.WriteLine("#############################################################################");

            System.Configuration.Configuration b =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (CheckConfigExists() == false)
            {
                Console.WriteLine("First time run-configuration");
                char key;

            ConfigureGatewayNetwork:
                Console.WriteLine("Do you wan to configure the gateway-network settings? Y/N");
                key = Console.ReadKey(true).KeyChar;
                if (key == 'y')
                {
                    Console.WriteLine("What ip should the gateway server listen to?");
                    while (!IPAddress.TryParse(Console.ReadLine(), out gatewayip))
                    {
                        Console.WriteLine("Incorrect value please use an ipv4 adress, recommended 0.0.0.0");
                    }

                    Console.WriteLine("What port should the gateway server listen to?");
                    while (!int.TryParse(Console.ReadLine(), out gatewayport))
                    {
                        Console.WriteLine("Incorrect value please use an number between 1024–49151, recommended 64000");
                    }
                }
                else if (key != 'n') goto ConfigureGatewayNetwork;

            ConfigureAuthenticationNetwork:
                Console.WriteLine("Do you wan to configure the authentication-network settings? Y/N");
                key = Console.ReadKey(true).KeyChar;
                if (key == 'y')
                {
                    Console.WriteLine("What is the ip of the authentication server");
                    while (!IPAddress.TryParse(Console.ReadLine(), out authenticationip))
                    {
                        Console.WriteLine("Incorrect value please use an ipv4 adress, recommended 0.0.0.0");
                    }

                    Console.WriteLine("On what port is the authentication server listening");
                    while (!int.TryParse(Console.ReadLine(), out authenticationport))
                    {
                        Console.WriteLine("Incorrect value please use an number between 1024–49151, recommended 64000");
                    }
                }
                else if (key != 'n') goto ConfigureAuthenticationNetwork;

            ConfigureGUID:
                Console.WriteLine("Do you wan to configure the gateway-guid settings? Y/N");
                key = Console.ReadKey(true).KeyChar;
                if (key == 'y')
                {
                    Console.WriteLine("What is the crc key");
                    byte[] ncrckey;
                    while (!Conversions.TryParse(Console.ReadLine(), out ncrckey) || crckey.Length != 20)
                    {
                        Console.WriteLine("Crc key key must be 20 hex digit string, recommended: A928CDC9DBE8751B3BC99EB65AE07E0C849CE739");
                    }

                    Console.WriteLine("What is the guid key");
                    byte[] nguidkey;
                    while (!Conversions.TryParse(Console.ReadLine(), out nguidkey) || guidkey.Length != 20)
                    {
                        Console.WriteLine("Guid key key must be 20 hex digit string, recommended: ED90AA25AE906FB36308C8523A4737A7E7B1FC6F");
                    }

                    crckey = Conversions.ByteToHexString(ncrckey);
                    guidkey = Conversions.ByteToHexString(nguidkey);
                }
                else if (key != 'n') goto ConfigureGUID;

                NetworkSettings networkSettings = new NetworkSettings();
                NetworkFileCollection collection = networkSettings.Connections;
                collection["public"] = new NetworkElement("public", gatewayip.ToString(), gatewayport);
                collection["internal"] = new NetworkElement("internal", authenticationip.ToString(), authenticationport);
                networkSettings.Crckey = crckey;
                networkSettings.Guidkey = guidkey;
                b.Sections.Remove("Saga.NetworkSettings");
                b.Sections.Add("Saga.NetworkSettings", networkSettings);
                b.Save();
                ConfigurationManager.RefreshSection("Saga.NetworkSettings");

                Console.WriteLine("Everything configured");
                LoginClient client;
                for (int i = 0; i < 3; i++)
                {
                    if (NetworkManager.TryGetLoginClient(out client))
                    {
                        Console.WriteLine("Test connection created");
                        client.Close();
                        break;
                    }
                    else
                    {
                        Thread.Sleep(3000);
                        Console.WriteLine("Test connection failed retrying in 3 secconds");
                    }
                }
            }
            else
            {
                Console.WriteLine("Configuration file exists");
            }

            reader.Start();
        }

        [ConsoleAttribute("shutdown", "Shutdowns the server.")]
        private static void Shutdown(string[] args)
        {
            char b;

            do
            {
                Console.WriteLine("This will shutdown the server do you want to contiue y/n");
                b = Console.ReadKey(true).KeyChar;
            }
            while (b != 'y' && b != 'n');

            if (b == 'y')
                Environment.Exit(1);
        }

        [ConsoleAttribute("version", "Shows the version of the assembly.")]
        private static void Version(string[] args)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            AssemblyName myAssemblyName = myAssembly.GetName();
            Console.WriteLine(myAssemblyName.Version.ToString());
        }

        [ConsoleAttribute("togglecrc", "Enables/Disables crc checking")]
        private static void CrcValidationCheck(string[] args)
        {
            GatewayClient.CheckCrc ^= true;
            Console.WriteLine("Crc check is now {0}", (GatewayClient.CheckCrc) ? "enabled" : "disabled");
        }

        [ConsoleAttribute("host", "Checks if the host is available")]
        private static void CheckHost(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            LoginClient client;
            switch (args[1].ToUpperInvariant())
            {
                case "-CONNECT":
                    if (NetworkManager.TryGetLoginClient(out client))
                    {
                        client.ExchangeIpAdress(IPAddress.Loopback);
                        Console.WriteLine("Connection to authentication-server is create");
                    }
                    else
                    {
                        switch (NetworkManager.LastError)
                        {
                            case NetworkManager.NetworkError.InvalidHost:
                                Console.WriteLine("Host is invalid");
                                break;

                            case NetworkManager.NetworkError.Refused:
                                Console.WriteLine("Target machine activly refused the connection");
                                break;

                            case NetworkManager.NetworkError.Unknown:
                                Console.WriteLine("Unknown exception occured");
                                break;

                            case NetworkManager.NetworkError.Unreachable:
                                Console.WriteLine("Target machin is unreachable");
                                break;
                        }
                    }

                    break;

                case "-DISCONNECT":
                    if (NetworkManager.TryGetLoginClient(out client))
                    {
                        client.Close();
                        Console.WriteLine("Connection to authentication-server is closed");
                    }
                    else
                        Console.WriteLine("Cannot close connection to authentication-server");
                    break;
            }
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

        #endregion Others
    }

    public class TraceLog : ITrace
    {
        private int errorcount = 0;

        #region ITrace Members

        public void WriteInformation(string category, string message)
        {
            if (managers.TraceInfo)
                __WriteLine(category, _levelInfo, message);
        }

        public void WriteInformation(string category, string message, params object[] format)
        {
            if (managers.TraceInfo)
                __WriteLine(category, _levelInfo, message, format);
        }

        public void WriteWarning(string category, string message)
        {
            if (managers.TraceWarning)
                __WriteLine(category, _levelWarning, message);
        }

        public void WriteWarning(string category, string message, params object[] format)
        {
            if (managers.TraceWarning)
                __WriteLine(category, _levelWarning, message, format);
        }

        public void WriteError(string category, string message)
        {
            if (managers.TraceError)
            {
                errorcount++;
                __WriteLine(category, _levelError, message);
            }
        }

        public void WriteError(string category, string message, params object[] format)
        {
            if (managers.TraceError)
            {
                errorcount++;
                __WriteLine(category, _levelError, message, format);
            }
        }

        public void WriteLine(string category, string message)
        {
            if (managers.TraceVerbose)
                __WriteLine(category, _levelVerbose, message);
        }

        public void WriteLine(string category, string message, params object[] format)
        {
            if (managers.TraceVerbose)
                __WriteLine(category, _levelVerbose, message, format);
        }

        #endregion ITrace Members

        #region ITrace Members

        void ITrace.WriteInformation(string category, string message)
        {
            WriteInformation(category, message);
        }

        void ITrace.WriteInformation(string category, string message, params object[] format)
        {
            WriteInformation(category, message, format);
        }

        void ITrace.WriteWarning(string category, string message)
        {
            WriteWarning(category, message);
        }

        void ITrace.WriteWarning(string category, string message, params object[] format)
        {
            WriteWarning(category, message, format);
        }

        void ITrace.WriteError(string category, string message)
        {
            WriteError(category, message);
        }

        void ITrace.WriteError(string category, string message, params object[] format)
        {
            WriteError(category, message, format);
        }

        void ITrace.WriteLine(string category, string message)
        {
            WriteLine(category, message);
        }

        void ITrace.WriteLine(string category, string message, params object[] format)
        {
            WriteLine(category, message, format);
        }

        #endregion ITrace Members

        #region Private Members

        private TraceSwitch managers = new TraceSwitch("General", "Entire Application");
        private const string _levelVerbose = "verbose   ";
        private const string _levelWarning = "warning   ";
        private const string _levelError = "error     ";
        private const string _levelInfo = "ïnfo      ";

        private void __WriteLine(string category, string level, string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(level);
            builder.Append(message);
            System.Diagnostics.Trace.WriteLine(builder.ToString(), category);
        }

        private void __WriteLine(string category, string level, string message, params object[] format)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(level);
                builder.AppendFormat(message, format);
                System.Diagnostics.Trace.WriteLine(builder.ToString(), category);
            }
            catch (FormatException)
            {
                System.Diagnostics.Trace.Fail("Cannot format message");
            }
        }

        #endregion Private Members

        #region Public Members

        public int CountOfErrors
        {
            get
            {
                return errorcount;
            }
        }

        public int LogLevel
        {
            get
            {
                return (int)this.managers.Level;
            }
        }

        #endregion Public Members

        #region Public Properties

        public bool TraceVerbose
        {
            get
            {
                return managers.TraceVerbose;
            }
        }

        public bool TraceError
        {
            get
            {
                return managers.TraceError;
            }
        }

        public bool TraceWarning
        {
            get
            {
                return managers.TraceWarning;
            }
        }

        public bool TraceInfo
        {
            get
            {
                return managers.TraceInfo;
            }
        }

        #endregion Public Properties

        #region Constructor

        public TraceLog(string switchname, string description)
        {
            managers = new TraceSwitch(switchname, description);
        }

        public TraceLog(string switchname, string description, int defaultlevel)
        {
            managers = new TraceSwitch(switchname, description, defaultlevel.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        #endregion Constructor
    }

    public interface ITrace
    {
        void WriteInformation(string category, string message);

        void WriteInformation(string category, string message, params object[] format);

        void WriteWarning(string category, string message);

        void WriteWarning(string category, string message, params object[] format);

        void WriteError(string category, string message);

        void WriteError(string category, string message, params object[] format);

        void WriteLine(string category, string message);

        void WriteLine(string category, string message, params object[] format);
    }
}