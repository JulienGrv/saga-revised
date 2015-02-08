using Saga.Configuration;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Sockets;

namespace Saga.Gateway
{
    internal static class NetworkManager
    {
        public enum NetworkError
        {
            None = 0,
            Unreachable = 1,
            Refused = 2,
            InvalidHost = 3,
            Unknown = 4
        }

        private static NetworkError error;
        private static LoginClient instance;

        public static NetworkError LastError
        {
            get
            {
                return error;
            }
        }

        public static bool TryGetLoginClient(out LoginClient client)
        {
            try
            {
                if ((instance != null && instance.IsConnected))
                {
                    client = instance;
                    return client != null;
                }
                else
                {
                    CreateLoginClient();
                    client = instance;
                    return client != null;
                }
            }
            catch (Exception)
            {
                client = null;
                return false;
            }
        }

        private static void CreateLoginClient()
        {
            //HELPER VARIABLES
            string host = "127.0.0.1";
            int port = 64001;

            try
            {
                //GET THE NETWORK SETTINGS
                NetworkSettings section = (NetworkSettings)ConfigurationManager.GetSection("Saga.NetworkSettings");
                if (section != null)
                {
                    NetworkElement Element = section.Connections["internal"];
                    if (Element != null)
                    {
                        host = Element.Host;
                        port = Element.Port;
                    }
                }

                lock (Synroot)
                {
                    error = NetworkError.Unknown;
                    instance = new LoginClient(host, port);

                    Trace.TraceInformation("Sending header token");
                    Saga.Packets.SMSG_HEADERTOKEN p = new Saga.Packets.SMSG_HEADERTOKEN();
                    instance.Send((byte[])p);
                    error = NetworkError.None;
                }
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    error = NetworkError.Refused;
                    Trace.TraceError("Target machine refused connection on: {0}-{1}", host, port);
                    throw;
                }
                else if (e.SocketErrorCode == SocketError.HostUnreachable)
                {
                    error = NetworkError.Unreachable;
                    Trace.TraceError("Target machine is unreachable: {0}-{1}", host, port);
                    throw;
                }
                else if (e.SocketErrorCode == SocketError.HostNotFound)
                {
                    error = NetworkError.InvalidHost;
                    Trace.TraceError("Target machine has an invalid host name: {0}-{1}", host, port);
                    throw;
                }
            }
            catch (Exception e)
            {
                error = NetworkError.Unknown;
                Trace.WriteLine(e);
            }
        }

        private static object Synroot = new object();
    }
}