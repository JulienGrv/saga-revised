using Saga.Authentication.Structures;
using Saga.Map.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Saga.Authentication
{
    internal class ServerManager2
    {
        public Dictionary<byte, ServerInfo2> server = new Dictionary<byte, ServerInfo2>();

        private static ServerManager2 instance;

        public static ServerManager2 Instance
        {
            get
            {
                if (instance == null) instance = new ServerManager2();
                return instance;
            }
        }

        private ServerManager2()
        {
            List<WorldInfo> list = Singleton.Database.GetWorldInformation();
            foreach (WorldInfo info2 in list)
            {
                ServerInfo2 info = new ServerInfo2();
                info.IP = null;
                info.name = info2.Worldname;
                info.proof = info2.Worldproof;
                this.server.Add(info2.WorldId, info);
            }
        }

        private static Thread PingThread;
        private static int LastTick = 0;

        static ServerManager2()
        {
            LastTick = Environment.TickCount;
            PingThread = new Thread(new ThreadStart(PingCallback));
            PingThread.Start();
        }

        private static void PingCallback()
        {
            //Never stop pinging
            while (!Environment.HasShutdownStarted)
            {
                if (Environment.TickCount - LastTick > 60000)
                {
                    //Try pinging all servers
                    try
                    {
                        foreach (KeyValuePair<byte, ServerInfo2> pair in
                            ServerManager2.Instance.server)
                        {
                            ServerInfo2 info = pair.Value;
                            if (info.client != null)
                                info.client.SM_PING();
                        }
                    }
                    //catch all exceptions but don't handle them
                    catch (Exception)
                    {
                    }
                    //Always update the last tick
                    finally
                    {
                        LastTick = Environment.TickCount;
                    }
                }

                Thread.Sleep(1);
            }
        }
    }

    public class ServerInfo2
    {
        public string proof = string.Empty;
        public string name;
        public IPAddress IP;
        public byte RequiredAge = 0;
        public int Port;
        public int LastPing;
        public int Players;
        public int MaxPlayers;
        public bool InMaintainceMode = false;
        public InternalClient client;
        private byte[] ValidationBytes = new byte[16];

        public byte[] KEY
        {
            get
            {
                return ValidationBytes;
            }
        }

        public bool ClearKey(byte[] key)
        {
            for (int i = 0; i < 16; i++)
            {
                if ((this.ValidationBytes[i] ^ key[i]) != 0)
                    return false;
            }

            return true;
        }

        private static Random rand = new Random();

        public void GenerateKey()
        {
            rand.NextBytes(ValidationBytes);
        }
    }
}