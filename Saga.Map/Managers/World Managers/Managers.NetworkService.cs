using Saga.Configuration;
using Saga.Map;
using Saga.Map.Client;
using Saga.Map.Configuration;
using Saga.Shared.NetworkCore;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Saga.Managers
{
    public class NetworkService : ManagerBase2
    {
        #region Ctor/Dtor

        public NetworkService()
        {
        }

        ~NetworkService()
        {
            StopServers();
        }

        #endregion Ctor/Dtor

        #region Internal Members

        //Settings
        internal static string _host_1 = string.Empty;

        internal static string _host_2 = string.Empty;
        internal static int _port_1 = 0;
        internal static int _port_2 = 0;
        internal static int _worldid = 0;
        internal static int _playerlimit = 0;
        internal static byte _requiredage = 0;
        internal static string _proof = string.Empty;
        internal static InternalClient InterNetwork;
        internal Manager<Saga.Map.Client.Client> networkManger;

        #endregion Internal Members

        #region Public Properties

        /// <summary>
        /// Get's the binding port of the world server
        /// </summary>
        public int WorldPort
        {
            get
            {
                return _port_1;
            }
        }

        /// <summary>
        /// Get's the to be connected port for the authentication server
        /// </summary>
        public int AuthenticationPort
        {
            get
            {
                return _port_2;
            }
        }

        /// <summary>
        /// Get's the player limit of the server
        /// </summary>
        public int PlayerLimit
        {
            get
            {
                return _playerlimit;
            }
        }

        /// <summary>
        /// Get's the world's proof
        /// </summary>
        public string Proof
        {
            get
            {
                return _proof;
            }
        }

        /// <summary>
        /// Get's the authentication client.
        /// </summary>
        /// <remarks>
        /// This is the fixed connection between world and authentication server.
        /// We use this connection to do secure commands such as adding/deleting a character.
        /// </remarks>
        internal InternalClient AuthenticationClient
        {
            get
            {
                return InterNetwork;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void QuerySettings()
        {
            NetworkSettings section = (NetworkSettings)ConfigurationManager.GetSection("Saga.Manager.NetworkSettings");
            if (section != null)
            {
                NetworkElement Element = section.Connections["public"];
                if (Element != null)
                {
                    _host_1 = Element.Host;
                    _port_1 = Element.Port;
                }

                NetworkElement Element2 = section.Connections["internal"];
                if (Element2 != null)
                {
                    _host_2 = Element2.Host;
                    _port_2 = Element2.Port;
                }

                _worldid = section.WorldId;
                _playerlimit = section.PlayerLimit;
                _requiredage = (byte)section.Agelimit;
                _proof = section.Proof;
            }
        }

        protected override void FinishedLoading()
        {
            this.HostContext.OnLoaded += new EventHandler(HostContext_OnLoaded);
        }

        #endregion Protected Methods

        #region Event Callbacks

        private void HostContext_OnLoaded(object sender, EventArgs e)
        {
            Singleton.ConsoleCommands.Clear();

            if (Singleton.generaltracelog.CountOfErrors > 0)
            {
                Console.WriteLine("{0} errors occured when loading", Singleton.generaltracelog.CountOfErrors);
                Console.ReadKey(true);
                Environment.Exit(1);
            }
            else
            {
                Singleton.Database.AutoRestore();

                StartLoginServer();
                StartWorldServer();
                Singleton.WorldTasks.Start();
                Singleton.ConsoleCommands.Start();
            }
        }

        internal void StartLoginServer()
        {
            bool success = false;
            //NOTIFY LOGIN SERVER
            while (success == false)
            {
                try
                {
                    Console.WriteLine("Connect to authentication server: {0}:{1}", _host_2, _port_2);
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(_host_2, _port_2);
                    InterNetwork = new InternalClient(sock);
                    InterNetwork.WORLD_NOTIFYCATION();
                    success = true;
                }
                catch (SocketException)
                {
                    Console.WriteLine("Cannot create connection with authentication server");
                    WriteError("NetworkManager", "Cannot create connection with authentication server");
                    Thread.Sleep(60000);
                }
                catch (Exception ex)
                {
                    WriteError("NetworkManager", "A unknown exception occurred: {0} {1}", ex.Source, ex.Message);
                    Thread.Sleep(60000);
                }
            }
        }

        internal void StartWorldServer()
        {
            //START LISTENING
            bool success = false;
            success = false;
            while (success == false)
            {
                try
                {
                    //START LISTENING TO NETWORK
                    Console.WriteLine("Accepting gateway connections on: {0}:{1}", _host_1, _port_1);
                    networkManger = new Manager<Saga.Map.Client.Client>(_host_1, _port_1);
                    networkManger.Start();
                    success = true;
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10049)
                    {
                        Trace.TraceError("The ip adress {0}:{1} is not an local ip adress", _host_1, _port_1);
                        Console.WriteLine("The ip adress {0}:{1} is not an local ip adress", _host_1, _port_1);
                        Thread.Sleep(60000);
                    }
                    else if (ex.ErrorCode == 10048)
                    {
                        Trace.TraceError("The port number is already in use: {0}:{1}", _host_1, _port_1);
                        Console.WriteLine("The port number is already in use: {0}:{1}", _host_1, _port_1);
                        Thread.Sleep(60000);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (FormatException)
                {
                    Trace.TraceError("The ip adress {0}:{1} is invalid formatted", _host_1, _port_1);
                    Console.WriteLine("The ip adress {0}:{1} is invalid formatted", _host_1, _port_1);
                    Thread.Sleep(60000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while connecting retying in 1 minute");
                    Trace.TraceError(ex.Message);
                    Thread.Sleep(60000);
                }
            }
        }

        internal void StopServers()
        {
            //Close the internal network client if any exists
            if (InterNetwork != null)
                InterNetwork.Close();

            //Stop the network manager if any exists
            if (networkManger != null)
                networkManger.Stop();
        }

        #endregion Event Callbacks
    }
}