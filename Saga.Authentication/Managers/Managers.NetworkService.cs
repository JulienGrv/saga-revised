using Saga.Authentication;
using Saga.Authentication.Network;
using Saga.Configuration;
using Saga.Map.Client;
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

        #endregion Ctor/Dtor

        #region Internal Members

        //Managers
        private static Manager<LogonClient> networkManger;

        private static Manager<InternalClient> networkManger2;

        //Settings
        internal static string _host_1 = string.Empty;

        internal static string _host_2 = string.Empty;
        internal static int _port_1 = 0;
        internal static int _port_2 = 0;

        #endregion Internal Members

        #region Protected Methods

        protected override void QuerySettings()
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            NetworkSettings section = config.GetSection("Saga.NetworkSettings") as NetworkSettings;
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
            }
        }

        protected override void FinishedLoading()
        {
            this.HostContext.OnLoaded +=
                new EventHandler(HostContext_OnLoaded);
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
            }
            else
            {
                OpenPublicNetworkConnections();
                OpenInternalNetworkConnections();
                Singleton.ConsoleCommands.Start();
            }
        }

        public void OpenPublicNetworkConnections()
        {
            //HELPER VARIABLES
            bool success = false;
            while (success == false)
            {
                try
                {
                    //START LISTENING TO NETWORK
                    networkManger = new Manager<LogonClient>(_host_1, _port_1);
                    networkManger.Start();
                    success = true;
                    Console.WriteLine("Accpeting gateway connections from: {0}", _port_1);
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == 10049)
                    {
                        WriteWarning("The ip adress {0}:{1} is not an local ip adress", _host_1, _port_1);
                        Console.WriteLine("The ip adress {0}:{1} is not an local ip adress", _host_1, _port_1);
                        Thread.Sleep(60000);
                    }
                    else if (e.ErrorCode == 10048)
                    {
                        WriteWarning("The port number is already in use: {0}:{1}", _host_1, _port_1);
                        Console.WriteLine("The port number is already in use: {0}:{1}", _host_1, _port_1);
                        Thread.Sleep(60000);
                    }
                    else
                    {
                        Console.WriteLine(e);
                        WriteWarning("NetworkManager", e.Message);
                        Thread.Sleep(60000);
                    }
                }
                catch (FormatException)
                {
                    WriteWarning("NetworkManager", "The ip adress {0}:{1} is invalid formatted", _host_1, _port_1);
                    Console.WriteLine("The ip adress {0}:{1} is invalid formatted", _host_1, _port_1);
                    Thread.Sleep(60000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    WriteError("NetworkManager", e.Message);
                    Thread.Sleep(60000);
                }
            }
        }

        public void OpenInternalNetworkConnections()
        {
            //HELPER VARIABLES
            bool success = false;
            while (success == false)
            {
                try
                {
                    //START LISTENING TO NETWORK
                    networkManger2 = new Manager<InternalClient>(_host_2, _port_2);
                    networkManger2.Start();
                    success = true;
                    Console.WriteLine("Accpeting map connections from: {0}", _port_2);
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == 10049)
                    {
                        Trace.TraceError("The ip adress {0}:{1} is not an local ip adress", _host_1, _port_1);
                        Console.WriteLine("The ip adress {0}:{1} is not an local ip adress", _host_1, _port_1);
                        Thread.Sleep(60000);
                    }
                    else if (e.ErrorCode == 10048)
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
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Trace.TraceError(e.Message);
                    Thread.Sleep(60000);
                }
            }
        }

        #endregion Event Callbacks
    }
}