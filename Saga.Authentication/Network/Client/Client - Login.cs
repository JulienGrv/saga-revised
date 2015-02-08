using Saga.Authentication.Common;
using Saga.Authentication.Packets;
using Saga.Authentication.Utils;
using Saga.Network.Packets;
using System;
using System.Diagnostics;
using System.Threading;

namespace Saga.Authentication.Network
{
    partial class LogonClient
    {
        private static volatile uint NextSession;
        private static volatile int LoginEntry;

        public void Login(CMSG_REQUESTLOGIN cpkt)
        {
            LoginResult result;
            LoginError error = LoginError.UNIDENTIFIED_LOGIN_ERROR;
            DateTime LastLogin = DateTime.Now;
            LoginSession session;

            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
            {
                //CHECK SERVERS STATE
                if (Saga.Managers.ConsoleCommands.InTestmode == true && session.NLoginAttempts < 1)
                {
                    session.NLoginAttempts++;
                    error = LoginError.SERVER_IN_TESTMODE;
                    Trace.TraceInformation("Server in testmode");
                    goto Reply;
                }

                //CHECK THE NUMBER LOGIN ATTEMPTS
                else if (session.NLoginAttempts > 100)
                {
                    error = LoginError.EXCEEDED_RETRY_ENTRY;
                    Trace.TraceError("Exceeded maximum login");
                    goto Reply;
                }

                //QUEUEES A NEW LOGIN REQUEST
                else
                {
                    try
                    {
                        if (LoginEntry > 70)
                        {
                            SMSG_LOGINAWNSER spkt = new SMSG_LOGINAWNSER();
                            spkt.SessionId = cpkt.SessionId;
                            spkt.LoginError = LoginError.LOGIN_DELAYED;
                            this.Send((byte[])spkt);

                            while (LoginEntry > 70)
                            {
                                Thread.Sleep(1);
                            }
                        }

                        //Encrement entry
                        LoginEntry++;
                        session.NLoginAttempts++;
                        if (Singleton.Database.Login(cpkt.Username.ToLowerInvariant(), out result))
                        {
                            #region PASSWORD VERIFYCATION

                            //CHECKS IF THE PASSWORD IS CORRECT
                            if (!result.lg_password.StartsWith(cpkt.Password))
                            {
                                Trace.TraceWarning("Wrong password");
                                error = LoginError.WRONG_PASS;
                                goto Reply;
                            }

                            #endregion PASSWORD VERIFYCATION

                            #region CHECK AGREEMENT

                            //CHECK IF PLAYER HAS AGREED TERMS
                            else if (!result.has_agreed)
                            {
                                Trace.TraceWarning("Account not agreed with agreement");
                                error = LoginError.CONFIRM_AGGEEMENT;
                                goto Reply;
                            }

                            #endregion CHECK AGREEMENT

                            #region CHECK ACTIVATION

                            //CHECK IF ACCOUNT IS ACTIVATED
                            else if (!result.is_activated)
                            {
                                Trace.TraceWarning("Account not activated");
                                error = LoginError.ACCOUNT_NOT_ACTIVATED;
                                goto Reply;
                            }

                            #endregion CHECK ACTIVATION

                            #region CHECK BANNED

                            //CHECK IF ACCOUNT IS BANNED
                            else if (result.is_banned == true)
                            {
                                Trace.TraceWarning("Account is banned");
                                error = LoginError.ACCOUNT_SUSSPENDED;
                                goto Reply;
                            }

                            //CHECK IF IP IS BANNED
                            else if (!Singleton.Database.IsIpAllowed(session.Adress))
                            {
                                Trace.TraceWarning("Ip is banned");
                                error = LoginError.ACCOUNT_SUSSPENDED;
                                goto Reply;
                            }

                            #endregion CHECK BANNED

                            #region TEST ENVIRMONT VERIFYCATION

                            //CHECK IF THE SERVER IS IN TESTMODE AND THE USER CAN LOGIN (GM'S)
                            else if (Saga.Managers.ConsoleCommands.InTestmode && result.is_testaccount == false)
                            {
                                Trace.TraceWarning("Not a test account");
                                error = LoginError.NOT_TEST_ACCOUNT;
                                goto Reply;
                            }

                            #endregion TEST ENVIRMONT VERIFYCATION

                            #region CHECK ACTIVE SESSION

                            else if (result.ative_session > 0)
                            {
                                Trace.TraceWarning("Already connected");
                                error = LoginError.ALREADY_CONNECTED;
                                session.playerid = result.userid;
                                session.ActiveSession = result.ative_session;
                                session.LastPlayedWorld = (byte)result.last_server;
                                goto Reply;
                            }

                            #endregion CHECK ACTIVE SESSION

                            #region LOGIN SUCESS

                            //ELSE SET THE USER ID
                            else
                            {
                                LastLogin = result.lg_entry;
                                error = LoginError.NO_ERROR;
                                session.GmLevel = result.gmlevel;
                                session.Gender = result.lg_gender;
                                session.playerid = result.userid;
                                session.Age = (byte)Utillities.CalculateAge(result.DateOfBirth);
                                Singleton.Database.UpdateLoginEntry(session.playerid, session.Adress);
                                goto Reply;
                            }

                            #endregion LOGIN SUCESS
                        }
                        else
                        {
                            #region USERNAME VERIFICATION

                            //USERNAME DOES NOT EXISTS
                            error = LoginError.WRONG_USER;
                            goto Reply;

                            #endregion USERNAME VERIFICATION
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.Message);
                        error = LoginError.DATABASE_ERROR;
                        goto Reply;
                    }
                    finally
                    {
                        //Decrement entry
                        LoginEntry--;
                    }
                }

            //REPLY LOGIN AWNSER
            Reply:
                SMSG_LOGINAWNSER spkt2 = new SMSG_LOGINAWNSER();
                spkt2.SessionId = cpkt.SessionId;
                spkt2.LoginError = error;
                spkt2.Gender = (byte)session.Gender;
                spkt2.LastLogin = LastLogin;
                spkt2.LoginError = error;
                spkt2.MaxChars = (byte)session.NMaxCharacters;
                spkt2.SessionId = cpkt.SessionId;
                spkt2.Advertisment = (byte)((Saga.Managers.ConsoleCommands.ShowAdvertisment) ? 1 : 0);
                this.Send((byte[])spkt2);
            }
        }

        /*
        internal void Login_Callback(uint sessionId, string username, string password)
        {
            LoginSession session;
            if (LoginSessionHandler.sessions.TryGetValue(sessionId, out session))
            {
                LoginResult result;
                LoginError error = LoginError.NO_ERROR;
                DateTime LastLogin = DateTime.Now;

                try
                {
                    if (Singleton.Database.Login(username.ToLowerInvariant(), out result))
                    {
                        #region PASSWORD VERIFYCATION

                        //CHECKS IF THE PASSWORD IS CORRECT
                        if (!result.lg_password.StartsWith(password))
                        {
                            Trace.TraceWarning("Wrong password");
                            error = LoginError.WRONG_PASS;
                        }

                        #endregion PASSWORD VERIFYCATION

                        #region CHECK AGREEMENT

                        //CHECK IF PLAYER HAS AGREED TERMS
                        else if (!result.has_agreed)
                        {
                            Trace.TraceWarning("Account not agreed with agreement");
                            error = LoginError.CONFIRM_AGGEEMENT;
                        }

                        #endregion CHECK AGREEMENT

                        #region CHECK ACTIVATION

                        //CHECK IF ACCOUNT IS ACTIVATED
                        else if (!result.is_activated)
                        {
                            Trace.TraceWarning("Account not activated");
                            error = LoginError.ACCOUNT_NOT_ACTIVATED;
                        }

                        #endregion CHECK ACTIVATION

                        #region CHECK BANNED

                        //CHECK IF ACCOUNT IS BANNED
                        else if (result.is_banned == true)
                        {
                            Trace.TraceWarning("Account is banned");
                            error = LoginError.ACCOUNT_SUSSPENDED;
                        }

                        #endregion CHECK BANNED



                        #region TEST ENVIRMONT VERIFYCATION

                        //CHECK IF THE SERVER IS IN TESTMODE AND THE USER CAN LOGIN (GM'S)
                        else if (Saga.Managers.ConsoleCommands.InTestmode && result.is_testaccount == false)
                        {
                            Trace.TraceWarning("Not a test account");
                            error = LoginError.NOT_TEST_ACCOUNT;
                        }

                        #endregion TEST ENVIRMONT VERIFYCATION

                        #region CHECK ACTIVE SESSION

                        else if (result.ative_session > 0)
                        {
                            Trace.TraceWarning("Already connected");
                            error = LoginError.ALREADY_CONNECTED;
                            session.playerid = result.userid;
                            session.ActiveSession = result.ative_session;
                            session.LastPlayedWorld = (byte)result.last_server;
                        }

                        #endregion CHECK ACTIVE SESSION

                        #region LOGIN SUCESS

                        //ELSE SET THE USER ID
                        else
                        {
                            LastLogin = result.lg_entry;
                            session.GmLevel = result.gmlevel;
                            session.Gender = result.lg_gender;
                            session.playerid = result.userid;
                            session.Age = (byte)Utillities.CalculateAge(result.DateOfBirth);
                            Singleton.Database.UpdateLoginEntry(session.playerid);
                        }

                        #endregion LOGIN SUCESS
                    }
                    else
                    {
                        #region USERNAME VERIFICATION

                        //USERNAME DOES NOT EXISTS
                        error = LoginError.WRONG_USER;

                        #endregion USERNAME VERIFICATION
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Database error");
                    error = LoginError.DATABASE_ERROR;
                }
                try
                {
                    SMSG_LOGINAWNSER spkt = new SMSG_LOGINAWNSER();
                    spkt.Gender = (byte)session.Gender;
                    spkt.LastLogin = LastLogin;
                    spkt.LoginError = error;
                    spkt.MaxChars = (byte)session.NMaxCharacters;
                    spkt.SessionId = sessionId;
                    spkt.Advertisment = (byte)((Saga.Managers.ConsoleCommands.ShowAdvertisment) ? 1 : 0);
                    this.Send((byte[])spkt);
                }
                catch (Exception)
                {
                    this.Close();
                }
            }
        }
        */

        private void CM_KILLEXISTINGCONNECTION(RelayPacket packet)
        {
            ServerInfo2 info;
            LoginSession session;
            if (LoginSessionHandler.sessions.TryGetValue(packet.SessionId, out session))
                if (ServerManager2.Instance.server.TryGetValue(session.LastPlayedWorld, out info)
                && info.client != null && info.client.IsConnected)
                {
                    //Clears the active session
                    info.client.SM_KILLSESSION(session.ActiveSession);
                }
                else
                {
                    Trace.TraceWarning("World was not found release session");

                    //If world was not found
                    Singleton.Database.ReleaseSessionId(session.ActiveSession);
                }
        }
    }
}