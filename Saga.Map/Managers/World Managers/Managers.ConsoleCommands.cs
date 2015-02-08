using Saga.Configuration;
using Saga.Core;
using Saga.Map;
using Saga.Map.Client;
using Saga.Map.Configuration;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Shared.Definitions;
using Saga.Tasks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Reflection;
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

        //Non console GM commands
        private static Dictionary<string, GmCommand> CommandList = new Dictionary<string, GmCommand>();

        private delegate void GmCommandHandler(Character character, Match arguments);

        /// <summary>
        /// Used to determine if the changes should reflect of the auth server.
        /// </summary>
        internal static bool isaddisplayed = false;

        /// <summary>
        /// By default don't kick people with fatal errors
        /// </summary>
        internal static bool DisconnectClientOnException = false;

        #endregion Internal Members

        #region Protected Methods

        protected override void Initialize()
        {
            reader = new ConsoleReader();
            reader.Title = "Zone server, type help for commands";
            reader.Clear(null);
        }

        protected override void Load()
        {
            RegisterCommands();
            RegisterGMCommands();
        }

        protected override void FinishedLoading()
        {
            this.HostContext.OnLoaded += new EventHandler(RegisterScriptedCommands);
        }

        protected virtual void RegisterScriptedCommands(object sender, EventArgs e)
        {
            try
            {
                ConsoleSettings section = ConfigurationManager.GetSection("Saga.Manager.ConsoleSettings") as ConsoleSettings;
                if (section != null)
                {
                    foreach (FactoryFileElement Element in section.Commands)
                    {
                        RegisterExternal(Element.Path);
                    }

                    foreach (FactoryFileElement Element in section.GmCommands)
                    {
                        RegisterExternalGmCommand(Element.Path);
                    }
                }
            }
            catch (Exception x)
            {
                HostContext.UnhandeldExceptionList.Add(x);
            }
        }

        protected virtual void RegisterCommands()
        {
            Register(new ConsoleCommandHandler(Version));
            Register(new ConsoleCommandHandler(Player));
            Register(new ConsoleCommandHandler(Server));
            Register(new ConsoleCommandHandler(Maintenance));
            Register(new ConsoleCommandHandler(RestoreCentre));
            Register(new ConsoleCommandHandler(Shutdown));
        }

        protected virtual void RegisterGMCommands()
        {
            /*
            CommandList.Add("InternalReply",new InternalGMCommand(new CommandFunc(this.SendCommandReply),101));
            //CommandList.Add("heal",new InternalGMCommand(new CommandFunc(this.ProcessTest),0));
            */
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
        protected void RegisterExternal(string path)
        {
            try
            {
                ConsoleCommandHandler handler = CoreService.Find<ConsoleCommandHandler>(path);
                if (handler != null)
                {
                    Register(handler);
                }
                else
                {
                    HostContext.AddUnhandeldException(new SystemException(string.Format("Cannot find console command: {0}", path)));
                }
            }
            catch (Exception e)
            {
                //do nothing here
                HostContext.AddUnhandeldException(e);
            }
        }

        [DebuggerNonUserCode()]
        private void Register(GmCommandHandler handler)
        {
            GmAttribute[] attribute = handler.Method.GetCustomAttributes(typeof(GmAttribute), true) as GmAttribute[];
            if (attribute != null && attribute.Length > 0)
            {
                GmCommand command = new GmCommand();
                command.attribute = attribute[0];
                command.handler = handler;
                string name = attribute[0].Name.ToUpperInvariant();
                CommandList.Add(name, command);
            }
            else
            {
                throw new SystemException("Cannot register command: no information found");
            }
        }

        [DebuggerNonUserCode()]
        protected void RegisterExternalGmCommand(string path)
        {
            try
            {
                GmCommandHandler handler = CoreService.Find<GmCommandHandler>(path);
                if (handler != null)
                {
                    Register(handler);
                }
                else
                {
                    HostContext.AddUnhandeldException(new SystemException(string.Format("Cannot find console command: {0}", path)));
                }
            }
            catch (Exception e)
            {
                //do nothing here
                HostContext.AddUnhandeldException(e);
            }
        }

        [DebuggerNonUserCode()]
        public void Clear()
        {
            reader.Clear(null);
        }

        #endregion Public Methods

        #region Console Commands: Maintenance

        [ConsoleAttribute("Maintaince.Reset", "Resets the maintaince schedule")]
        protected void Maintenance_Reset(string[] args)
        {
            Saga.Tasks.Maintenance.IsScheduled = false;
            Saga.Tasks.Maintenance.NextSceduledMaintenance = DateTime.Now;
        }

        #endregion Console Commands: Maintenance

        #region Console Commands: Misc

        [ConsoleAttribute("Version", "Shows the version of the assembly")]
        protected void Version(string[] args)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            AssemblyName myAssemblyName = myAssembly.GetName();
            Console.WriteLine(myAssemblyName.Version.ToString());
        }

        #endregion Console Commands: Misc

        #region Player Console Commands

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
            {
                Tasks.LifespanAI.Stop();
                Tasks.BattleThread.Stop();
                Singleton.NetworkService.StopServers();
                Singleton.WorldTasks.Stop();
                Environment.Exit(1);
            }
        }

        [ConsoleAttribute("player", "Provides functions to interact with the players", "player -rescue player mapid\n\t\t player -isonline playername\n\t\t player -kick playername\n\t\t player -zeny playername zeny\n\t\t player -save playername")]
        protected void Player(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            switch (args[1].ToUpperInvariant())
            {
                case "-RESCUE": WarpToMap(args); break;
                case "-ISONLINE": IsOnline(args); break;
                case "-KICK": Kick(args); break;
                case "-ZENY": GiveZeny(args); break;
                case "-SAVE": ManualSave(args); break;
                case "-SHOWALL": ShowAll(args); break;
                case "-REGION": ShowRegionInformation(args); break;
                //case "-CHANNEL": ShowChannels(args); break;       Temp untill we figure out how channels are used
            }
        }

        [ConsoleAttribute("asr", "Provides functions to interact with a automated system restore", "asr -create playername\n\t\t asr -restore\n\t\t asr -repair playername")]
        protected void RestoreCentre(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            switch (args[1].ToUpperInvariant())
            {
                case "-CREATE": __RESTOREPOINTS(args); break;
                case "-RESTORE": Singleton.Database.Restore(); break;
                case "-REPAIR": __REPAIR__(args); break;
            }
        }

        protected void ShowAll(string[] args)
        {
            List<string> list = new List<string>();
            foreach (Character character in Tasks.LifeCycle.Characters)
                list.Add(character.Name);

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

        protected void ShowRegionInformation(string[] args)
        {
            Character characterSource;
            if (!Tasks.LifeCycle.TryGetByName(args[2], out characterSource)) return;

            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Generic information");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("position.x: {0}", characterSource.Position.x);
            Console.WriteLine("position.y: {0}", characterSource.Position.x);
            Console.WriteLine("position.z: {0}", characterSource.Position.x);
            Console.WriteLine("rhash code: {0:X8}", characterSource.region);
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Region found nearby character");
            Console.WriteLine("-----------------------------------");
            foreach (uint areacode in characterSource.currentzone.Regiontree.GetNearRegionCodes(characterSource.region))
            {
                Console.WriteLine("{0:X8}", areacode);
            }
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("NPC Found in nearby regions");
            Console.WriteLine("-----------------------------------");
            foreach (MapObject mobject in characterSource.currentzone.Regiontree.SearchActors(characterSource, Saga.Enumarations.SearchFlags.Npcs))
            {
                Console.WriteLine("{0:X8} {1} {2}", mobject.region, mobject.ToString(), Saga.Structures.Point.GetDistance3D(characterSource.Position, mobject.Position));
            }
        }

        protected void IsOnline(string[] args)
        {
            Character character;
            if (Tasks.LifeCycle.TryGetByName(args[2], out character))
            {
                Console.WriteLine("Character is online");
            }
            else
            {
                Console.WriteLine("Character is ofline");
            }
        }

        protected void Kick(string[] args)
        {
            Character character;
            if (Tasks.LifeCycle.TryGetByName(args[2], out character))
            {
                SMSG_KICKSESSION spkt = new SMSG_KICKSESSION();
                spkt.SessionId = character.id;
                character.client.Send((byte[])spkt);
            }
        }

        protected void GiveZeny(string[] args)
        {
            int Zeny;
            if (!int.TryParse(args[3], out Zeny))
            {
                Console.WriteLine("Not a valid number");
            }
            else
            {
                Character character;
                if (Tasks.LifeCycle.TryGetByName(args[2], out character))
                {
                    character.ZENY += (uint)Zeny;
                    if (character.client.isloaded == true)
                        CommonFunctions.UpdateZeny(character);
                }
                else
                {
                    Console.WriteLine("Character not found");
                }
            }
        }

        public static void WarpToMap(string[] args)
        {
            Character characterSource;
            byte map = Convert.ToByte(args[3]);
            if (Tasks.LifeCycle.TryGetByName(args[2], out characterSource))
                CommonFunctions.Warp(characterSource, map);
        }

        protected void ManualSave(string[] args)
        {
            Character character;
            if (Tasks.LifeCycle.TryGetByName(args[2], out character))
            {
                lock (character)
                {
                    if (character.client.isloaded == true)
                        Singleton.Database.TransSave(character);
                }
            }
            else
            {
                Console.WriteLine("Character not found");
            }
        }

        protected void __RESTOREPOINTS(string[] args)
        {
            try
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Wrong argument count");
                    return;
                }

                uint playerid = 0;
                if (!Singleton.Database.GetCharacterId(args[2], out playerid))
                {
                    Console.WriteLine("Could not find player with name: '{0}'", args[2]);
                    return;
                }

                Character tempCharacter = new Character(null, playerid, 1);
                if (Singleton.Database.TransLoad(tempCharacter))
                {
                    Singleton.Database.PostLoad(tempCharacter);
                    byte[] b = Singleton.Database.Serialize(tempCharacter);
                    Singleton.Database.WriteBytes(tempCharacter.Name, playerid, b);
                }
                else
                {
                    Console.WriteLine("Character not found");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected void __REPAIR__(string[] args)
        {
            try
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Wrong argument count");
                    return;
                }

                uint playerid = 0;
                if (!Singleton.Database.GetCharacterId(args[2], out playerid))
                {
                    Console.WriteLine("Could not find player with name: '{0}'", args[2]);
                    return;
                }

                Character tempCharacter = new Character(null, playerid, 1);
                if (Singleton.Database.TransLoad(tempCharacter, true))
                {
                    Singleton.Database.PostLoad(tempCharacter);
                    Singleton.Database.TransRepair(tempCharacter);
                }
                else
                {
                    Console.WriteLine("Character not found");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected void ShowChannels(string[] args)
        {
            Character character;
            if (Tasks.LifeCycle.TryGetByName(args[2], out character))
            {
                SMSG_SHOWCHANNELS spkt = new SMSG_SHOWCHANNELS();
                spkt.SessionId = character.id;
                spkt.Add(1, 0);
                spkt.Add(2, 0);
                spkt.Add(3, 0);
                character.client.Send((byte[])spkt);
            }
            else
            {
                Console.WriteLine("Character not found");
            }
        }

        #endregion Player Console Commands

        #region Server Console Commands

        [ConsoleAttribute("server", "Interacts with the general server settings", "server -toggledc\n\t\t server -broadcast message\n\t\t server -showrates")]
        protected void Server(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            switch (args[1].ToUpperInvariant())
            {
                case "-TOGGLEDC": ToggleDc(args); break;
                case "-SHOWRATES": ShowRates(args); break;
                case "-BROADCAST": Broadcast(args); break;
                case "-EXCEPTION": throw new SagaTestException();// break;
            }
        }

        protected void Broadcast(string[] args)
        {
            if (args.Length < 3) return;
            //GENERATE BROADCAST
            SMSG_SENDCHAT spkt = new SMSG_SENDCHAT();
            spkt.Name = "GM";
            spkt.MessageType = SMSG_SENDCHAT.MESSAGE_TYPE.SYSTEM_MESSAGE;
            spkt.Message = args[2];

            //FORWARD THE BROADCAST
            foreach (Character characterTarget in LifeCycle.Characters)
            {
                try
                {
                    spkt.SessionId = characterTarget.id;
                    characterTarget.client.Send((byte[])spkt);
                }
                catch (SocketException)
                {
                    //do nothing client was busy disconnecting or such
                }
            }
        }

        protected void ToggleDc(string[] args)
        {
            DisconnectClientOnException ^= true;
        }

        protected void ShowRates(string[] args)
        {
            Console.WriteLine("Server rates: x{0}, x{1} x{2} x{3}", Singleton.experience.Modifier_Cexp, Singleton.experience.Modifier_Jexp, Singleton.experience.Modifier_Wexp, Singleton.experience.Modifier_Drate);
        }

        #endregion Server Console Commands

        #region Maintenance Console Functions

        [ConsoleAttribute("maintenance", "Interacts with the servers ability for maintenance", "maintenance -force -enter\n\t\t maintenance -force -release\n\t\t maintenance -schedule date")]
        protected void Maintenance(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            switch (args[1].ToUpperInvariant())
            {
                case "-FORCE": Maintenance_Force(args); break;
                case "-SCHEDULE": Maintenance_Schedule(args); break;
            }
        }

        //Sub function for scheduling & canceling
        protected void Maintenance_Schedule(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            DateTime result;
            if (DateTime.TryParse(args[2], out result))
            {
                if (DateTime.Now < result)
                {
                    Saga.Tasks.Maintenance.NextSceduledMaintenance = result;
                    Saga.Tasks.Maintenance.IsScheduled = true;
                }
                else
                {
                    Saga.Tasks.Maintenance.NextSceduledMaintenance = result;
                    Saga.Tasks.Maintenance.IsScheduled = false;
                }
            }
            else
            {
                Console.WriteLine("Not a valid format");
            }
        }

        //Sub function for enforced updates
        protected void Maintenance_Force(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Wrong argument count");
                return;
            }

            string q = args[2].ToUpperInvariant();
            if (q == "-ENTER")
            {
                if (Saga.Managers.NetworkService.InterNetwork.IsConnected == true)
                {
                    SMSG_MAINTENANCEENTER spkt = new SMSG_MAINTENANCEENTER();
                    spkt.SessionId = uint.MaxValue;
                    Saga.Managers.NetworkService.InterNetwork.Send((byte[])spkt);
                }
            }
            else if (q == "-RELEASE")
            {
                if (Saga.Managers.NetworkService.InterNetwork.IsConnected == true)
                {
                    SMSG_MAINTENANCELEAVE spkt = new SMSG_MAINTENANCELEAVE();
                    spkt.SessionId = uint.MaxValue;
                    Saga.Managers.NetworkService.InterNetwork.Send((byte[])spkt);
                }
            }
            else
            {
                Console.WriteLine("Expected -force or -release switch");
            }
        }

        #endregion Maintenance Console Functions

        #region GM Commands

        public static bool IsGMCommand(string identifier)
        {
            if (identifier.Length > 0 && (identifier[0] == '!' || identifier[0] == '.' || identifier[0] == '/'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ParseGMCommand(string command, Character character)
        {
            try
            {
                string[] commands = command.Split(new char[] { ' ' }, 2);
                string commandname = commands[0].Length > 1 ? command.Substring(1, commands[0].Length - 1) : string.Empty;
                string body = commands.Length > 1 ? commands[1] : string.Empty;
                if (commandname == string.Empty) return;

                GmCommand commanda;
                if (CommandList.TryGetValue(commandname.ToUpperInvariant(), out commanda))
                {
                    Regex matchingrex = commanda.attribute.Regex;
                    Match match = matchingrex.Match(body);

                    if (match.Success == false)
                    {
                        string warning = string.Format(CultureInfo.InvariantCulture, "Command: {0} was not successfull.", commandname);
                        SendCommandReply(character.client, warning);
                    }
                    //else if (commanda.attribute.Gmlevel >= character.GmLevel)
                    else if (commanda.attribute.Gmlevel > character.GmLevel)
                    {
                        string warning = string.Format(CultureInfo.InvariantCulture, "You are not authorised to use this command.");
                        SendCommandReply(character.client, warning);
                    }
                    else
                    {
                        commanda.handler.Invoke(character, match);
                    }
                }
                else
                {
                    string warning = string.Format(CultureInfo.InvariantCulture, "Command: {0} was not found.", commandname);
                    SendCommandReply(character.client, warning);
                }
            }
            catch (Exception)
            {
                Trace.TraceWarning("Error parsing commands: {0}", command);
            }
        }

        #region Command Parsing

        private void ProcessHeal(Client srcc, string args)
        {
            srcc.character.HP = srcc.character.HPMAX;
            srcc.character.OnRegenerateHP();
        }

        private static void SendCommandReply(Client tc, string message)
        {
            SMSG_SENDCHAT spkt = new SMSG_SENDCHAT();
            spkt.Message = message;
            spkt.Name = "Saga";
            spkt.MessageType = SMSG_SENDCHAT.MESSAGE_TYPE.SYSTEM_MESSAGE;
            spkt.SessionId = tc.character.id;
            tc.Send((byte[])spkt);
        }

        #endregion Command Parsing

        #endregion GM Commands

        #region Internal Methods

        internal void Start()
        {
            reader.Start();
        }

        #endregion Internal Methods

        #region Nested Types/Structures

        private struct GmCommand
        {
            public GmCommandHandler handler;
            public GmAttribute attribute;
        }

        #endregion Nested Types/Structures
    }
}