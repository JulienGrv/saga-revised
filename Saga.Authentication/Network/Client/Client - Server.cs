using Saga.Authentication.Packets;
using Saga.Authentication.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Saga.Authentication.Network
{
    partial class LogonClient
    {
        private void OnServerListRequest(CMSG_REQUESTSERVERLIST cpkt)
        {
            //HELPER VARIABLES
            LoginSession session;
            Dictionary<byte, byte> CharactersWorlds;

            //OBTAIN A LIST OF WORLDS
            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
            {
                session.NCharacterCount = 0;
                SMSG_SERVERLIST spkt = new SMSG_SERVERLIST();
                spkt.SessionId = cpkt.SessionId;

                Singleton.Database.GetCharacterWorldCount(session.playerid, out CharactersWorlds);
                foreach (KeyValuePair<byte, ServerInfo2> info in ServerManager2.Instance.server)
                {
                    byte nChars = 0;

                    byte status = (byte)ServerStatus.OK;
                    status = (info.Value.LastPing > 700) ? (byte)ServerStatus.CROWDED : status;
                    //status = (info.Value.Players > info.Value.MaxPlayers - 50) ? (byte)ServerStatus.CROWDED : status;
                    status = (info.Value.Players >= info.Value.MaxPlayers) ? (byte)ServerStatus.OVERLOADED : status;
                    status = (info.Value.client == null) ? (byte)ServerStatus.UNKNOWN : status;
                    status = (info.Value.InMaintainceMode) ? (byte)ServerStatus.MAINTENANCE : status;

                    CharactersWorlds.TryGetValue(info.Key, out nChars);
                    spkt.Add(
                        new ServerInfo(
                        info.Key,           //ID OF THE WORLD
                        info.Value.name,    //NAME OF THE WORLD
                        nChars,             //NUMBER OF CHARACTERS
                        status              //CURRENT STATE
                        )
                    );

                    session.NCharacterCount += nChars;
                }

                this.Send((byte[])spkt);
            }
        }

        private void OnSelectServer(CMSG_SELECTSERVER cpkt)
        {
            //HELPER VARIABLES
            ServerInfo2 info;
            LoginSession session;

            //TRY TO ESTABLISH A CONNECTION TO OUR BACKEND
            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
                if (ServerManager2.Instance.server.TryGetValue(cpkt.Index, out info))
                {
                    //CHECK  IF CLIENT IS NULL
                    if (info.client == null)
                    {
                        SMSG_CHARACTERLIST spkt = new SMSG_CHARACTERLIST();
                        spkt.Result = (byte)LoginError.NO_ERROR_NO_SERVER_LIST;
                        spkt.SessionId = cpkt.SessionId;
                        this.Send((byte[])spkt);
                    }
                    //CHECK IF PLAYER IS TOO YOUNG
                    else if (session.Age < info.RequiredAge)
                    {
                        SMSG_CHARACTERLIST spkt = new SMSG_CHARACTERLIST();
                        spkt.Result = (byte)LoginError.TOO_YOUNG_TOPLAY;
                        spkt.SessionId = cpkt.SessionId;
                        this.Send((byte[])spkt);
                    }
                    //EVERYTHING OKAY
                    else
                    {
                        session.IsWaiting = true;
                        session.World = cpkt.Index;
                        info.client.SM_SELECT_CHARACTERS(session.playerid, cpkt.SessionId);

                        bool Timedout = false;
                        int LastTick = Environment.TickCount;

                        Trace.TraceError("Start waiting for character list");
                        while (session.IsWaiting)
                        {
                            if (Environment.TickCount - LastTick > 20000)
                            {
                                Timedout = true;
                                session.IsWaiting = false;
                                break;
                            }
                            else
                            {
                                Thread.Sleep(0);
                            }
                        }

                        if (!Timedout)
                        {
                            Trace.TraceError("Received successfull awner after {0}ms", Environment.TickCount - LastTick);
                            CMSG_REQUESTCHARACTERLIST cpkt2 = new CMSG_REQUESTCHARACTERLIST();
                            cpkt2.SessionId = cpkt.SessionId;
                            OnWantCharList(cpkt2);
                        }
                        else
                        {
                            Trace.TraceError("Login server timed out: {0}ms", Environment.TickCount - LastTick);
                            SMSG_CHARACTERLIST spkt = new SMSG_CHARACTERLIST();
                            spkt.Result = (byte)LoginError.UNIDENTIFIED_LOGIN_ERROR;
                            spkt.SessionId = cpkt.SessionId;
                            this.Send((byte[])spkt);
                        }
                    }
                }
                else
                {
                    SMSG_CHARACTERLIST spkt = new SMSG_CHARACTERLIST();
                    spkt.Result = (byte)LoginError.INCORRECT_KEY_OR_SERVERNUMBERVALUE;
                    spkt.SessionId = cpkt.SessionId;
                    this.Send((byte[])spkt);
                }
        }
    }
}