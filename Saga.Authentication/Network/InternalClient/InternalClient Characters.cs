using Saga.Authentication;
using Saga.Authentication.Packets;
using Saga.Authentication.Utils;
using Saga.Packets;
using Saga.Shared.PacketLib.Login;
using Saga.Shared.PacketLib.Map;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Saga.Map.Client
{
    partial class InternalClient
    {
        //Internal Members

        #region CREATE CHARACTER

        public void SM_CHARACTER_CREATE(SMSG_INTERNAL_CHARACTERCREATE spkt)
        {
            this.Send((byte[])spkt);
        }

        public void CM_CHARACTER_CREATE(CMSG_INTERNAL_CHARCREATIONREPLY cpkt)
        {
            SMSG_CREATECHARACTER spkt = new SMSG_CREATECHARACTER();
            spkt.SessionId = cpkt.SessionId;

            LoginSession session;
            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
            {
                //CHARACTER CREATION SUCCESS
                if (cpkt.Result == 0)
                {
                    Singleton.Database.UpdateCharacterOnWorld(session.playerid, (uint)session.World);
                    session.NCharacterCount++;
                    session.PendingCharInfo.charId = cpkt.CharacterId;
                    session.list.Add(session.PendingCharInfo);
                    session.client.Send((byte[])spkt);
                }
                else
                {
                    spkt.Result = cpkt.Result;
                    session.client.Send((byte[])spkt);
                }
            }
        }

        #endregion CREATE CHARACTER

        #region DELETE CHARACTER

        public void CM_DELETE_CHARACTER(CMSG_INTERNAL_DELETIONREPLY cpkt)
        {
            ServerInfo2 info;
            LoginSession session;

            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
                if (ServerManager2.Instance.server.TryGetValue(session.World, out info))
                {
                    Predicate<CharInfo> callback = delegate(CharInfo info2)
                    {
                        return info2.charId == session.PedingCharDeletion;
                    };

                    int index = session.list.FindIndex(callback);

                    SMSG_DELETECHARACTER spkt = new SMSG_DELETECHARACTER();
                    spkt.SessionId = cpkt.SessionId;
                    spkt.Index = (byte)index;

                    //CHARACTER CREATION SUCCESS
                    if (cpkt.Result == 0)
                    {
                        Singleton.Database.RemoveCharacterOnWorld(session.playerid, session.World);
                        session.CachedCharacterStates.Remove(session.PedingCharDeletion);
                        if (index > -1)
                            session.list.RemoveAt(index);
                        session.PedingCharDeletion = 0;
                        session.client.Send((byte[])spkt);

                        SMSG_CHARACTERLIST spkt2 = new SMSG_CHARACTERLIST();
                        spkt2.Result = 0;
                        spkt2.CountAllServer = (byte)--session.NCharacterCount;
                        spkt2.ServerName = info.name;
                        spkt2.SessionId = cpkt.SessionId;
                        foreach (CharInfo info2 in session.list)
                            spkt2.AddChar(info2.charId, info2.name, 1, info2.cexp, info2.job, 0, info2.map);
                        session.client.Send((byte[])spkt2);
                    }
                    else
                    {
                        session.PedingCharDeletion = 0;
                        spkt.Result = cpkt.Result;
                        session.client.Send((byte[])spkt);
                    }
                }
        }

        public void SM_DELETE_CHARACTER(uint charid, uint sessionId)
        {
            SMSG_INTERNAL_CHARACTERDELETE spkt = new SMSG_INTERNAL_CHARACTERDELETE();
            spkt.CharacterId = charid;
            spkt.SessionId = sessionId;
            this.Send((byte[])spkt);
        }

        #endregion DELETE CHARACTER

        #region SELECT CHARACTER

        public void SM_SELECT_CHARACTER(uint CharacterId, uint sessionId)
        {
            Saga.Shared.PacketLib.Login.SMSG_FINDCHARACTERDETAILS spkt = new Saga.Shared.PacketLib.Login.SMSG_FINDCHARACTERDETAILS();
            spkt.CharacterId = CharacterId;
            spkt.SessionId = sessionId;
            this.Send((byte[])spkt);
        }

        public void CM_SELECT_CHARACTER(Saga.Shared.PacketLib.Login.CMSG_FINDCHARACTERDETAILS cpkt)
        {
            LoginSession session;

            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
            {
                //ASSIGN RAW BYTES
                SMSG_CHARACTERDETAILS spkt = new SMSG_CHARACTERDETAILS();
                spkt.SetRawBytes(cpkt.GetRawBytes());
                spkt.SessionId = cpkt.SessionId;
                session.CachedCharacterStates[cpkt.CharacterId] = (byte[])spkt;

                //GET CHARACTER-INDEX
                int index = session.list.FindIndex(
                    delegate(CharInfo info)
                    {
                        return (info.charId == cpkt.CharacterId);
                    }
                );

                //INVOKE CHARACTER GET
                if (index < 0) return;
                CMSG_REQUESTCHARACTERDETAILS cpkt2 = new CMSG_REQUESTCHARACTERDETAILS();
                cpkt2.Index = (byte)index;
                cpkt2.SessionId = cpkt.SessionId;
                session.client.OnGetCharData(cpkt2);
            }
        }

        public void SM_SELECT_CHARACTERS(uint playerid, uint sessionId)
        {
            Trace.TraceError("Request character list");
            Saga.Shared.PacketLib.Login.SMSG_FINDCHARACTERS spkt = new Saga.Shared.PacketLib.Login.SMSG_FINDCHARACTERS();
            spkt.UserId = playerid;
            spkt.SessionId = sessionId;
            this.Send((byte[])spkt);
        }

        public void CM_SELECT_CHARACTERS(Saga.Shared.PacketLib.Login.CMSG_FINDCHARACTERS cpkt)
        {
            Trace.TraceError("Received character list from world server: {0} session {1}", this.WorldId, cpkt.SessionId);

            //HELPER ARGUMENTS
            CharInfo info;
            LoginSession session;

            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
            {
                lock (session)
                {
                    Trace.TraceError("Clear previous results", this.WorldId, cpkt.SessionId);
                    session.list.Clear();
                    for (byte i = 0; i < cpkt.Count; i++)
                    {
                        cpkt.GetCharInfo(out info);
                        session.list.Add(info);
                    }

                    Trace.TraceError("Ready receiving", this.WorldId, cpkt.SessionId);
                    session.IsWaiting = false;
                }
            }
        }

        #endregion SELECT CHARACTER

        #region VERIFY CHARACTER

        public void VERIFY_CHARACTERNAME_EXISTS()
        {
        }

        #endregion VERIFY CHARACTER

        #region SMSG

        // 0x0004
        public void SM_KILLSESSION(uint session)
        {
            Console.WriteLine("Kill session: {0}", session);
            SMSG_KILLSESSION spkt = new SMSG_KILLSESSION();
            spkt.Session = session;
            this.Send((byte[])spkt);
        }

        // 0x000E
        public void SM_PING()
        {
            //Update the ping tick
            PingTick = Environment.TickCount;

            //Requests a new pong reply
            SMSG_PING spkt = new SMSG_PING();
            this.Send((byte[])spkt);
        }

        public void CM_CHARACTERLOGINREPLY(CMSG_CHARACTERLOGINREPLY cpkt)
        {
            Console.WriteLine("Character Login: {0}", cpkt.Result);
        }

        #endregion SMSG

        #region CMSG

        // 0x0001
        public void CM_WORLDINSTANCE(CMSG_WORLDINSTANCE cpkt)
        {
            ServerInfo2 info = null;
            byte error = 0;
            byte WorldId = 0;

            try
            {
                //socket.RemoteEndPoint.
                IPEndPoint IPEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                WorldId = cpkt.WorldId;

                //SERVER DOES NOT EXISTS
                if (!ServerManager2.Instance.server.TryGetValue(WorldId, out info))
                    error = 3;
                //IF PROOF IS VALID
                else if (info.proof != cpkt.Proof)
                    error = 1;
                //CHECK IF SERVER IS ALIVE
                else if (info.client != null && info.client.IsConnected && cpkt.IsReconnected == 0)
                    error = 2;
                //IF EVERYTHING IS OKAY
                else
                {
                    this.WorldId = WorldId;
                    info.GenerateKey();
                    info.client = this;
                    info.MaxPlayers = cpkt.MaximumPlayers;
                    info.Players = 0;
                    info.InMaintainceMode = false;
                    info.IP = IPEndPoint.Address;
                    info.Port = cpkt.Port;
                    info.RequiredAge = cpkt.RequiredAge;

                    Console.WriteLine("world connection established");

                    if (cpkt.IsReconnected == 0)
                        Singleton.Database.ClearWorldSessions(this.WorldId);

                    SMSG_SETRATES spkt = new SMSG_SETRATES();
                    spkt.IsAdDisplayed = Managers.ConsoleCommands.ShowAdvertisment ? (byte)1 : (byte)0;
                    this.Send((byte[])spkt);
                }
            }
            finally
            {
                Thread.Sleep(500);
                SMSG_WORLDINSTANCEACK spkt = new SMSG_WORLDINSTANCEACK();
                spkt.Result = error;
                if (error == 0) spkt.NextKey = info.KEY;
                this.Send((byte[])spkt);
            }
        }

        // 0x000A
        public void CM_PONG(CMSG_PONG cpkt)
        {
            //Delay between ping and pong reply
            int delay = Environment.TickCount - PingTick;

            //Update the delay
            ServerInfo2 info = null;
            if (ServerManager2.Instance.server.TryGetValue(WorldId, out info))
                info.LastPing = delay;
        }

        // 0x000B
        public void CM_RELEASESESSION(CMSG_RELEASESESSION cpkt)
        {
            Trace.TraceWarning(string.Format("Release session: {0}", cpkt.Session));
            //Update the delay
            ServerInfo2 info = null;
            if (Singleton.Database.ReleaseSessionId(cpkt.Session)
             && ServerManager2.Instance.server.TryGetValue(WorldId, out info))
                info.Players = info.Players > 0 ? info.Players - 1 : 0;
        }

        #endregion CMSG

        #region Members

        public byte WorldId = 0;
        private int PingTick = 0;

        #endregion Members
    }
}