using Saga.Authentication.Packets;
using Saga.Authentication.Utils;
using Saga.Network.Packets;
using Saga.Packets;
using Saga.Shared.PacketLib.Login;
using System;
using System.Diagnostics;
using System.Threading;

namespace Saga.Authentication.Network
{
    partial class LogonClient : Shared.NetworkCore.Client
    {
        public LogonClient()
        {
            Trace.TraceInformation("Client create starting timeout session");
            WaitCallback callback = delegate(object obj)
            {
                int LastTick = Environment.TickCount;
                bool Timeout = false;

                //Forces to receive the header in 10 secconds if not kill the connection.
                while (securityToken == 0)
                {
                    if (Environment.TickCount - LastTick > 10000)
                    {
                        Timeout = true;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }

                if (Timeout == true)
                {
                    Console.WriteLine("Unautorized connection detetected. Make sure you connect on the gateway server not the authentication server");
                    Trace.WriteLine("Connection did not authenticate himself within 10 secconds. Closing connection.");
                    this.Close();
                }
            };

            ThreadPool.QueueUserWorkItem(callback);
            this.OnClose += new EventHandler(LogonClient_OnClose);
        }

        private void LogonClient_OnClose(object sender, EventArgs e)
        {
            if (securityToken == 1)
            {
                Console.WriteLine("Gateway connection is closed from {0}", this.socket.RemoteEndPoint);
            }
        }

        protected override void ProcessPacket(ref byte[] body)
        {
            try
            {
                //Only allow listening for the the initial header handshake
                if (securityToken == 0)
                {
                    //Skip this packet if it's not orginated from 0401
                    ushort packetIdentifier = (ushort)(body[7] + (body[6] << 8));
                    if (packetIdentifier != 0x0401) return;

                    //Check for header handshake
                    ushort subpacketIdentifier = (ushort)(body[13] + (body[12] << 8));
                    switch (subpacketIdentifier)
                    {
                        case 0xFFFF: HEADERRETRIEVED(); return;
                        default: Trace.TraceWarning("Unsupported packet found with id: {0:X4}", subpacketIdentifier); break;
                    }
                }
                else
                {
                    ushort subpacketIdentifier = (ushort)(body[13] + (body[12] << 8));
                    switch (subpacketIdentifier)
                    {
                        //Custom
                        case 0xFF01: OnNetworkExchange((CMSG_NETWORKADRESSIP)body); break;
                        case 0xFF02: OnReleaseResources((CMSG_RELEASERESOURCES)body); break;
                        case 0xFF03: OnSetSession(body); return;

                        //Official
                        case 0x0101: Login((CMSG_REQUESTLOGIN)body); return;
                        case 0x010B: Login((CMSG_REQUESTLOGIN)body); return;
                        case 0x0102: OnServerListRequest((CMSG_REQUESTSERVERLIST)body); return;
                        case 0x0103: OnSelectServer((CMSG_SELECTSERVER)body); return;
                        case 0x0104: OnSelectChar((CMSG_SELECTCHARACTER)body); return;
                        case 0x0105: OnCreateChar((CMSG_CREATECHARACTER)body); return;
                        case 0x0106: OnWantCharList((CMSG_REQUESTCHARACTERLIST)body); return;
                        case 0x0107: OnGetCharData((CMSG_REQUESTCHARACTERDETAILS)body); return;
                        case 0x0108: OnDeleteChar((CMSG_DELETECHARACTER)body); return;
                        case 0x010A: CM_KILLEXISTINGCONNECTION((RelayPacket)body); return;
                        case 0x0110: OnLogin((RelayPacket)body); return;
                        default: Trace.TraceWarning("Unsupported packet found with id: {0:X4}", subpacketIdentifier); break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnSetSession(byte[] body)
        {
            uint session = 0;
            session = ++NextSession;
            if (session >= 0x40000000)
            {
                NextSession = 1;
                session = 1;
            }

            if (session > 0)
            {
                LoginSession logonsession = new LoginSession();
                logonsession.client = this;
                LoginSessionHandler.sessions.Add(session, logonsession);

                SMSG_SETSESSION spkt = new SMSG_SETSESSION();
                spkt.SessionId = session;
                this.Send((byte[])spkt);
            }
            else
            {
                Trace.TraceError("No session found");
            }
        }

        private void OnNetworkExchange(CMSG_NETWORKADRESSIP cpkt)
        {
            //TRY TO ESTABLISH A CONNECTION TO OUR BACKEND
            LoginSession session;
            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
            {
                session.Adress = cpkt.ConnectionFrom;
            }
            else
            {
                Trace.TraceError("Session was not found: {0}", cpkt.SessionId);
            }
        }

        private byte securityToken = 0;

        private void HEADERRETRIEVED()
        {
            securityToken = 1;
            Console.WriteLine("Gateway connection accepted from {0}", this.socket.RemoteEndPoint);
        }

        private void OnReleaseResources(CMSG_RELEASERESOURCES cpkt)
        {
            lock (LoginSessionHandler.sessions)
            {
                if (!LoginSessionHandler.sessions.Remove(cpkt.Session))
                    Trace.TraceInformation("Resources are not found: {0}", cpkt.Session);
            }
        }

        private void OnLogin(RelayPacket cpkt)
        {
            LoginSession session;
            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
            {
                ServerInfo2 info;
                if (!ServerManager2.Instance.server.TryGetValue(session.World, out info))
                {
                    Trace.TraceError(string.Format("Server not found: {0}", session.World));
                }
                else if (info.client == null || info.client.IsConnected == false)
                {
                    Trace.TraceError(string.Format("World server not connected: {0}", session.World));
                }
                else
                {
                    Singleton.Database.UpdateSession(session.playerid, cpkt.SessionId);
                    Singleton.Database.UpdateLastplayerWorld(session.playerid, session.World);
                    info.Players++;

                    SMSG_LOGIN spkt = new SMSG_LOGIN();
                    spkt.GmLevel = session.GmLevel;
                    spkt.Gender = (byte)session.Gender;
                    spkt.CharacterId = session.characterid;
                    spkt.SessionId = cpkt.SessionId;
                    this.Send((byte[])spkt);

                    //Send login packet to world
                    /*
                    SMSG_CHARACTERLOGIN spkt2 = new SMSG_CHARACTERLOGIN();
                    spkt2.CharacterId = session.characterid;
                    spkt2.SessionId = cpkt.SessionId;
                    spkt2.Session = cpkt.SessionId;
                    info.client.Send((byte[])spkt2);*/
                }
            }
        }
    }
}