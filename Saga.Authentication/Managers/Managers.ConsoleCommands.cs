using Saga.Authentication;
using Saga.Core;
using Saga.Packets;
using Saga.Shared.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Saga.Managers
{
    public class ConsoleCommands : ManagerBase2
    {
        #region Ctor/Dtor

        public ConsoleCommands()
        {
        }

        #endregion Ctor/Dtor

        #region Internal Members

        //Settings
        private ConsoleReader reader;

        public static bool InTestmode = false;
        public static bool ShowAdvertisment = false;

        #endregion Internal Members

        #region Protected Methods

        protected override void Initialize()
        {
            reader = new ConsoleReader();
            reader.Title = "Authentication server, type help for commands";
            reader.Clear(null);
        }

        protected override void Load()
        {
            RegisterCommands();
        }

        protected virtual void RegisterCommands()
        {
            Register(new ConsoleCommandHandler(Version));
            Register(new ConsoleCommandHandler(CreateUser));
            Register(new ConsoleCommandHandler(Server));
            Register(new ConsoleCommandHandler(DumpAcl));
            Register(new ConsoleCommandHandler(CreateWorld));
            Register(new ConsoleCommandHandler(Shutdown));
        }

        #endregion Protected Methods

        #region Public Methods

        [DebuggerNonUserCode()]
        public void Register(string command, ConsoleCommandHandler handler)
        {
            reader.Register(command, handler);
        }

        [DebuggerNonUserCode()]
        public void Register(ConsoleCommandHandler handler)
        {
            reader.Register(handler);
        }

        [DebuggerNonUserCode()]
        public void Clear()
        {
            reader.Clear(null);
        }

        #endregion Public Methods

        #region Console Commands: GameObjects

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

        [ConsoleAttribute("Version", "Shows the version of the assembly")]
        private static void Version(string[] args)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            AssemblyName myAssemblyName = myAssembly.GetName();
            Console.WriteLine(myAssemblyName.Version.ToString());
        }

        [ConsoleAttribute("acl", "Interacts with the access control list", "acl -add deny|allow IP[/cidr|/mask]\n\t\t acl -remove deny|allow IP[/cidr|/mask]\n\t\t acl -dump [IP]")]
        private static void DumpAcl(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            switch (args[1].ToUpperInvariant())
            {
                case "-DUMP": DumpAcl2(args); break;
                case "-ADD": AclAdd(args); break;
                case "-REMOVE": AclRemove(args); break;
            }
        }

        [ConsoleAttribute("account", "Interacts with the accounts", "account -create username password [male|female] [gmlevel]\n\t\t account -ban username\n\t\t account -isonline username\n\t\t account -kick username\n\t\t account -ip username\n\t\t account -time username")]
        private static void CreateUser(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            switch (args[1].ToUpperInvariant())
            {
                case "-CREATE": CreateUser2(args); break;
                case "-BAN": BanUser(args); break;
                case "-ISONLINE": IsOnline(args); break;
                case "-KICK": SessionKill(args); break;
                case "-IP": GetIpAddress(args); break;
                case "-TIME": GetGameTime(args); break;
                case "-SHOWALL": ShowAllOnline(args); break;
            }
        }

        [ConsoleAttribute("world", "Interacts with the worldlist", "world -add worldname worldpassword\n\t\t world -remove worldname\n\t\t world -rename worldname newworldname")]
        private static void CreateWorld(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            switch (args[1].ToUpperInvariant())
            {
                case "-ADD": AddWorld(args); break;
                case "-REMOVE": RemoveWorld(args); break;
                case "-RENAME": RenameWorld(args); break;
                default: Console.WriteLine("Unregonized argument"); break;
            }
        }

        [ConsoleAttribute("server", "Interacts with the server modes", "server -showworlds\n\t\t server -togglemode\n\t\t server -togglead\n\t\t server -state")]
        private static void Server(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            switch (args[1].ToUpperInvariant())
            {
                case "-SHOWWORLDS": ShowWorld(args); break;
                case "-TOGGLEMODE": ToggleTestMode(args); break;
                case "-TOGGLEAD": ToggleAdvertisment(args); break;
                case "-STATE": State(args); break;
            }
        }

        #endregion Console Commands: GameObjects

        #region World Sub Functions

        public static void AddWorld(string[] args)
        {
            MD5 md5 = MD5.Create();
            byte[] block = Encoding.UTF8.GetBytes(args[3]);
            byte[] md5blcok = md5.ComputeHash(block);

            StringBuilder builder = new StringBuilder();
            foreach (byte b in md5blcok)
                builder.AppendFormat("{0:X2}", b);

            int worldid = 0;
            if (args.Length < 4) Console.WriteLine("Wrong argument count");
            if (Singleton.Database.AddWorld(args[2], builder.ToString(), out worldid))
            {
                Console.WriteLine("World is created. World-id is {0}", worldid);
            }
            else
            {
                Console.WriteLine("Failed creating the world. A world with the same name is already present or serverlist is full");
            }
        }

        public static void RemoveWorld(string[] args)
        {
            if (Singleton.Database.RemoveWorld(args[2]))
            {
                Console.WriteLine("World is deleted.");
            }
            else
            {
                Console.WriteLine("Failed deleting the world.");
            }
        }

        public static void RenameWorld(string[] args)
        {
            if (args.Length < 4) Console.WriteLine("Wrong argument count");
            if (Singleton.Database.RenameWorld(args[2], args[3]))
            {
                Console.WriteLine("World is renamed.");
                foreach (KeyValuePair<byte, ServerInfo2> pair in ServerManager2.Instance.server)
                {
                    if (pair.Value.name == args[2])
                        pair.Value.name = args[3];
                }
            }
            else
            {
                Console.WriteLine("Failed renamed the world.");
            }
        }

        #endregion World Sub Functions

        #region Account Sub Functions

        private static void SessionKill(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            LoginResult result;
            if (!Singleton.Database.Login(args[2], out result))
            {
                Console.WriteLine("User does not exists");
                return;
            }
            else
            {
                ServerInfo2 info = null;
                if (result.ative_session > 0)
                {
                    //SERVER DOES NOT EXISTS
                    if (!ServerManager2.Instance.server.TryGetValue((byte)result.last_server, out info))
                        Console.WriteLine("Server does not exists");
                    //CHECK IF SERVER IS ALIVE
                    else if (info.client != null && info.client.IsConnected)
                    {
                        info.client.SM_KILLSESSION(result.ative_session);
                    }
                    else
                    {
                        Console.WriteLine("Server is not online");
                    }
                }
            }
        }

        private static void IsOnline(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            LoginResult result;
            if (!Singleton.Database.Login(args[2], out result))
            {
                Console.WriteLine("User does not exists");
                return;
            }
            else
            {
                Console.WriteLine("User is: {0}", result.ative_session > 0 ? "online" : "offline");
            }
        }

        private static void ShowAllOnline(string[] args)
        {
            List<string> list = Singleton.Database.GetAllCharactersOnline();
            Console.WriteLine("players online detected: {0}", list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i]);

                if (i % 20 == 19)
                {
                    char a = Console.ReadKey(true).KeyChar;
                    if (a == 's') i += 20;
                }
            }
        }

        private static void CreateUser2(string[] args)
        {
            int gmlevel = 0;
            byte gender = 1;

            if (args.Length > 5)
            {
                if (!int.TryParse(args[5], out gmlevel))
                {
                    Console.WriteLine("Invalid argument");
                    return;
                }
            }

            if (args.Length > 4)
            {
                if (args[4].ToLowerInvariant() == "female")
                    gender = 2;
            }

            if (args.Length > 3)
            {
                if (args[2].Length > 0 && args[3].Length > 0)
                {
                    if (Singleton.Database.CreateUserEntry(args[2], args[3], gender, gmlevel))
                    {
                        Console.WriteLine("Account created");
                    }
                    else
                    {
                        Console.WriteLine("Account creation failed");
                    }
                }
            }
        }

        private static void BanUser(string[] args)
        {
        }

        private static void GetIpAddress(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            LoginResult result;
            if (!Singleton.Database.Login(args[2], out result))
            {
                Console.WriteLine("User does not exists");
                return;
            }
            else
            {
                IPAddress adr = Singleton.Database.GetAdressOfUser(result.userid);
                if (adr == IPAddress.None)
                {
                    Console.WriteLine("User hasn't logged in before");
                }
                else
                {
                    Console.WriteLine("Users last ip adress: {0}", adr);
                }
            }
        }

        private static void GetGameTime(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            LoginResult result;
            if (!Singleton.Database.Login(args[2], out result))
            {
                Console.WriteLine("User does not exists");
                return;
            }
            else
            {
                long time = Singleton.Database.GetPlayedTimeOfUser(result.userid);
                if (time == 0)
                {
                    Console.WriteLine("User has never played");
                }
                else
                {
                    TimeSpan dt = TimeSpan.FromSeconds((double)time);
                    Console.WriteLine("Users has played {0} days - {1} hours - {2} minutes", dt.Days, dt.Hours, dt.Minutes);
                }
            }
        }

        #endregion Account Sub Functions

        #region ACL Sub Functions

        private static void DumpAcl2(string[] args)
        {
            if (args.Length == 2)
            {
                using (FileStream fs = new FileStream("acl.txt", FileMode.Create, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine("Access Control Lists");
                    writer.WriteLine("===================================================================================");
                    writer.WriteLine(string.Empty);
                    writer.WriteLine("Rule policy:");
                    writer.WriteLine("A explicit allow declaration can override a deny operation");
                    writer.WriteLine("All ip's are implicit allowed.");
                    writer.WriteLine(string.Empty);
                    writer.WriteLine(string.Empty);
                    writer.WriteLine("{0}\t{1}\t{2}\t{3}", "RULE#", "ALLOW/DENY".PadRight(20), "IP".PadRight(15), "MASK".PadRight(15));

                    List<AclEntry> entries = Singleton.Database.ListAclEntries();
                    for (int i = 0; i < entries.Count; i++)
                    {
                        AclEntry entry = entries[i];
                        writer.WriteLine("{0}\t{1}\t{2}\t{3}", entry.RuleId, entry.Operation == 0 ? "DENY".PadRight(20) : "ALLOW".PadRight(20), entry.IP.PadRight(15), entry.Mask.PadRight(15));
                    }
                }
            }
            else if (args.Length == 3)
            {
                Regex IsIpOnly = new Regex("^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$");

                if (IsIpOnly.IsMatch(args[2]))
                {
                    using (FileStream fs = new FileStream("acl.txt", FileMode.Create, FileAccess.Write))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine("Access Control Lists");
                        writer.WriteLine("===================================================================================");
                        writer.WriteLine(string.Empty);
                        writer.WriteLine("Rule policy:");
                        writer.WriteLine("A explicit allow declaration can override a deny operation");
                        writer.WriteLine("All ip's are implicit allowed.");
                        writer.WriteLine(string.Empty);
                        writer.WriteLine("Filtering all entries on matching: {0}", args[2]);
                        writer.WriteLine(string.Empty);
                        writer.WriteLine(string.Empty);
                        writer.WriteLine("{0}\t{1}\t{2}\t{3}", "RULE#", "ALLOW/DENY".PadRight(20), "IP".PadRight(15), "MASK".PadRight(15));

                        List<AclEntry> entries = Singleton.Database.FindMatchingAclEntries(args[2]);
                        for (int i = 0; i < entries.Count; i++)
                        {
                            AclEntry entry = entries[i];
                            writer.WriteLine("{0}\t{1}\t{2}\t{3}", entry.RuleId, entry.Operation == 0 ? "DENY".PadRight(20) : "ALLOW".PadRight(20), entry.IP.PadRight(15), entry.Mask.PadRight(15));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Argument is not a valid ip adress");
                }
            }
            else
            {
                Console.WriteLine("Wrong argument count");
            }
        }

        private static void AclAdd(string[] args)
        {
            Regex IsIpOnly = new Regex("^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$");
            Regex IsIpWithCIDR = new Regex("^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})/(\\d{1,2})$");
            Regex IsIpWithMask = new Regex("^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})/(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})$");

            if (args.Length != 4)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }
            else if (args[2].ToUpperInvariant() != "DENY" && args[2].ToUpperInvariant() != "ALLOW")
            {
                Console.WriteLine("Argument 2 expected to be deny or allow");
                return;
            }
            else if (IsIpOnly.IsMatch(args[3]))
            {
                byte op = (byte)(args[2].ToUpperInvariant() == "DENY" ? 0 : 1);
                if (!Singleton.Database.AddEntry(args[3], "255.255.255.255", op))
                {
                    Console.WriteLine("Failed to add entry");
                }
                else
                {
                    Console.WriteLine("Entry added");
                }
            }
            else if (IsIpWithCIDR.IsMatch(args[3]))
            {
                string cidr_string = string.Empty;
                Match match = IsIpWithCIDR.Match(args[3]);
                byte cidr = byte.Parse(match.Groups[2].Value);
                switch (cidr)
                {
                    case 32: cidr_string = "255.255.255.255"; break;
                    case 31: cidr_string = "255.255.255.254"; break;
                    case 30: cidr_string = "255.255.255.252"; break;
                    case 29: cidr_string = "255.255.255.248"; break;
                    case 28: cidr_string = "255.255.255.240"; break;
                    case 27: cidr_string = "255.255.255.224"; break;
                    case 26: cidr_string = "255.255.255.192"; break;
                    case 25: cidr_string = "255.255.255.128"; break;
                    case 24: cidr_string = "255.255.255.000"; break;
                    case 23: cidr_string = "255.255.254.000"; break;
                    case 22: cidr_string = "255.255.252.000"; break;
                    case 21: cidr_string = "255.255.248.000"; break;
                    case 20: cidr_string = "255.255.240.000"; break;
                    case 19: cidr_string = "255.255.224.000"; break;
                    case 18: cidr_string = "255.255.192.000"; break;
                    case 17: cidr_string = "255.255.128.000"; break;
                    case 16: cidr_string = "255.255.000.000"; break;
                    case 15: cidr_string = "255.254.000.000"; break;
                    case 14: cidr_string = "255.252.000.000"; break;
                    case 13: cidr_string = "255.248.000.000"; break;
                    case 12: cidr_string = "255.240.000.000"; break;
                    case 11: cidr_string = "255.224.000.000"; break;
                    case 10: cidr_string = "255.192.000.000"; break;
                    case 9: cidr_string = "255.128.000.000"; break;
                    case 8: cidr_string = "255.000.000.000"; break;
                    case 7: cidr_string = "254.000.000.000"; break;
                    case 6: cidr_string = "252.000.000.000"; break;
                    case 5: cidr_string = "248.000.000.000"; break;
                    case 4: cidr_string = "240.000.000.000"; break;
                    case 3: cidr_string = "224.000.000.000"; break;
                    case 2: cidr_string = "192.000.000.000"; break;
                    case 1: cidr_string = "128.000.000.000"; break;
                    case 0: cidr_string = "000.000.000.000"; break;
                    default: Console.WriteLine("Invalid CIDR"); return;
                }

                byte op = (byte)(args[2].ToUpperInvariant() == "DENY" ? 0 : 1);
                if (!Singleton.Database.AddEntry(match.Groups[1].Value, cidr_string, op))
                {
                    Console.WriteLine("Failed to add entry");
                }
                else
                {
                    Console.WriteLine("Entry added");
                }
            }
            else if (IsIpWithMask.IsMatch(args[3]))
            {
                Match match = IsIpWithMask.Match(args[3]);
                byte op = (byte)(args[2].ToUpperInvariant() == "DENY" ? 0 : 1);
                if (!Singleton.Database.AddEntry(match.Groups[1].Value, match.Groups[2].Value, op))
                {
                    Console.WriteLine("Failed to add entry");
                }
                else
                {
                    Console.WriteLine("Entry added");
                }
            }
            else
            {
                Console.WriteLine("Not valid arguments");
            }
        }

        private static void AclRemove(string[] args)
        {
            Regex IsIpOnly = new Regex("^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$");
            Regex IsIpWithCIDR = new Regex("^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})/(\\d{1,2})$");
            Regex IsIpWithMask = new Regex("^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})/(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})$");

            if (args.Length != 4)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }
            else if (args[2].ToUpperInvariant() != "DENY" && args[2].ToUpperInvariant() != "ALLOW")
            {
                return;
            }
            else if (IsIpOnly.IsMatch(args[3]))
            {
                byte op = (byte)(args[2].ToUpperInvariant() == "DENY" ? 0 : 1);
                if (!Singleton.Database.RemoveEntry(args[3], "255.255.255.255", op))
                {
                    Console.WriteLine("Failed to remove entry");
                }
                else
                {
                    Console.WriteLine("Entry removed");
                }
            }
            else if (IsIpWithCIDR.IsMatch(args[3]))
            {
                string cidr_string = string.Empty;
                Match match = IsIpWithCIDR.Match(args[3]);
                byte cidr = byte.Parse(match.Groups[2].Value);
                switch (cidr)
                {
                    case 32: cidr_string = "255.255.255.255"; break;
                    case 31: cidr_string = "255.255.255.254"; break;
                    case 30: cidr_string = "255.255.255.252"; break;
                    case 29: cidr_string = "255.255.255.248"; break;
                    case 28: cidr_string = "255.255.255.240"; break;
                    case 27: cidr_string = "255.255.255.224"; break;
                    case 26: cidr_string = "255.255.255.192"; break;
                    case 25: cidr_string = "255.255.255.128"; break;
                    case 24: cidr_string = "255.255.255.000"; break;
                    case 23: cidr_string = "255.255.254.000"; break;
                    case 22: cidr_string = "255.255.252.000"; break;
                    case 21: cidr_string = "255.255.248.000"; break;
                    case 20: cidr_string = "255.255.240.000"; break;
                    case 19: cidr_string = "255.255.224.000"; break;
                    case 18: cidr_string = "255.255.192.000"; break;
                    case 17: cidr_string = "255.255.128.000"; break;
                    case 16: cidr_string = "255.255.000.000"; break;
                    case 15: cidr_string = "255.254.000.000"; break;
                    case 14: cidr_string = "255.252.000.000"; break;
                    case 13: cidr_string = "255.248.000.000"; break;
                    case 12: cidr_string = "255.240.000.000"; break;
                    case 11: cidr_string = "255.224.000.000"; break;
                    case 10: cidr_string = "255.192.000.000"; break;
                    case 9: cidr_string = "255.128.000.000"; break;
                    case 8: cidr_string = "255.000.000.000"; break;
                    case 7: cidr_string = "254.000.000.000"; break;
                    case 6: cidr_string = "252.000.000.000"; break;
                    case 5: cidr_string = "248.000.000.000"; break;
                    case 4: cidr_string = "240.000.000.000"; break;
                    case 3: cidr_string = "224.000.000.000"; break;
                    case 2: cidr_string = "192.000.000.000"; break;
                    case 1: cidr_string = "128.000.000.000"; break;
                    case 0: cidr_string = "000.000.000.000"; break;
                    default: Console.WriteLine("Invalid CIDR"); return;
                }

                byte op = (byte)(args[2].ToUpperInvariant() == "DENY" ? 0 : 1);
                if (!Singleton.Database.RemoveEntry(match.Groups[1].Value, cidr_string, op))
                {
                    Console.WriteLine("Failed to remove entry");
                }
                else
                {
                    Console.WriteLine("Entry removed");
                }
            }
            else if (IsIpWithMask.IsMatch(args[3]))
            {
                Match match = IsIpWithMask.Match(args[3]);
                byte op = (byte)(args[2].ToUpperInvariant() == "DENY" ? 0 : 1);
                if (!Singleton.Database.RemoveEntry(match.Groups[1].Value, match.Groups[2].Value, op))
                {
                    Console.WriteLine("Failed to remove entry");
                }
                else
                {
                    Console.WriteLine("Entry removed");
                }
            }
            else
            {
                Console.WriteLine("Not valid arguments");
            }
        }

        #endregion ACL Sub Functions

        #region Server Console Functions

        /// <summary>
        /// Shows the current state modes
        /// </summary>
        /// <param name="args">Arguments</param>
        private static void State(string[] args)
        {
            Console.WriteLine("Primairy login running in {0}",
                (InTestmode) ? "testmode" : "normalmode");

            Console.WriteLine("Advertisment is {0}",
                (ShowAdvertisment) ? "shown" : "hidden");
        }

        /// <summary>
        /// Toggles the server between test mode. During this mode only test accounts
        /// are allowed to enter.
        /// </summary>
        /// <param name="args">Arguments</param>
        private static void ToggleTestMode(string[] args)
        {
            InTestmode ^= true;
            Console.WriteLine("Primairy login now running in: {0}",
                (InTestmode) ? "testmode" : "normalmode");
        }

        /// <summary>
        /// Toggles the server between advertisement mode. Advertisement would
        /// add increased rates for the servers
        /// </summary>
        /// <param name="args">Arguments</param>
        private static void ToggleAdvertisment(string[] args)
        {
            ShowAdvertisment ^= true;
            Console.WriteLine("Advertisment is: {0}",
                (ShowAdvertisment) ? "shown" : "hidden");

            lock (ServerManager2.Instance.server)
            {
                foreach (KeyValuePair<byte, ServerInfo2> b in ServerManager2.Instance.server)
                {
                    if (b.Value.client != null && b.Value.client.IsConnected == true)
                        try
                        {
                            SMSG_SETRATES spkt = new SMSG_SETRATES();
                            spkt.IsAdDisplayed = (ShowAdvertisment) ? (byte)1 : (byte)0;
                            b.Value.client.Send((byte[])spkt);
                        }
                        catch (Exception)
                        {
                            Trace.TraceWarning("Error notifying server");
                        }
                }
            }
        }

        /// <summary>
        /// Displayes a list of workds
        /// </summary>
        /// <param name="args">Arguments</param>
        private static void ShowWorld(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("Worlds\tPlayers\tIp");
            Console.WriteLine("================================================");
            foreach (KeyValuePair<byte, ServerInfo2> pair in ServerManager2.Instance.server)
            {
                ServerInfo2 info = pair.Value;
                Console.WriteLine("{0}\t{1}\t{2}", info.name, info.Players, info.IP);
            }
        }

        #endregion Server Console Functions

        #region Internal Methods

        internal void Start()
        {
            reader.Start();
        }

        #endregion Internal Methods
    }
}