using Saga.Authentication.Packets;
using Saga.Authentication.Utils;
using Saga.Packets;
using Saga.Shared.PacketLib.Login;
using System;

namespace Saga.Authentication.Network
{
    partial class LogonClient
    {
        internal void OnWantCharList(CMSG_REQUESTCHARACTERLIST cpkt)
        {
            //HELPER VARIABLES
            ServerInfo2 info;
            LoginSession session;

            //CREATE CHARACTER-LIST
            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
                if (ServerManager2.Instance.server.TryGetValue(session.World, out info))
                {
                    SMSG_CHARACTERLIST spkt = new SMSG_CHARACTERLIST();
                    spkt.Result = 0;
                    spkt.CountAllServer = (byte)session.NCharacterCount;
                    spkt.ServerName = info.name;
                    spkt.SessionId = cpkt.SessionId;

                    foreach (CharInfo info2 in session.list)
                        spkt.AddChar(info2.charId, info2.name, 1, info2.cexp, info2.job, 0, info2.map);
                    this.Send((byte[])spkt);
                }
        }

        internal void OnGetCharData(CMSG_REQUESTCHARACTERDETAILS cpkt)
        {
            //HELPER VARIABLES
            CharInfo info;
            ServerInfo2 info2;
            byte[] cachedbuffer;
            LoginSession session;

            //GET CHARACTER DETAILS - WE CACHE THE DETAILS
            //SO WE DON'T OVERLOAD THE BACKEND, BUT INSTEAD THE FRONTEND
            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
                if (cpkt.Index <= session.list.Count)
                {
                    info = session.list[cpkt.Index];

                    if (session.CachedCharacterStates.TryGetValue(info.charId, out cachedbuffer))
                    {
                        this.Send(cachedbuffer);
                    }
                    else
                    {
                        if (ServerManager2.Instance.server.TryGetValue(session.World, out info2))
                            info2.client.SM_SELECT_CHARACTER(info.charId, cpkt.SessionId);
                    }
                }
        }

        internal void OnCreateChar(CMSG_CREATECHARACTER cpkt)
        {
            //HELPER VARIABLES
            CharInfo info = new CharInfo();
            ServerInfo2 info2;
            LoginSession session;

            //START CREATING THE CHARACTER ON OUR SERVER
            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
                if (ServerManager2.Instance.server.TryGetValue(session.World, out info2))
                {
                    SMSG_INTERNAL_CHARACTERCREATE cpkt2 = new SMSG_INTERNAL_CHARACTERCREATE();
                    cpkt2.CharacterId = session.playerid;
                    cpkt2.Name = cpkt.Name;
                    cpkt2.Eye = cpkt.Eye;
                    cpkt2.EyeBrowse = cpkt.EyeBrowse;
                    cpkt2.EyeColor = cpkt.EyeColor;
                    cpkt2.Hair = cpkt.Hair;
                    cpkt2.HairColor = cpkt.HairColor;
                    cpkt2.WeaponAffix = cpkt.WeaponAffix;
                    cpkt2.WeaponName = cpkt.WeaponName;
                    cpkt2.SessionId = cpkt.SessionId;

                    info.cexp = 0;
                    info.charId = 0;
                    info.gender = (byte)session.Gender;
                    info.name = cpkt.Name;
                    info.job = 1;
                    session.PendingCharInfo = info;
                    info2.client.SM_CHARACTER_CREATE(cpkt2);
                }
        }

        private void OnSelectChar(CMSG_SELECTCHARACTER cpkt)
        {
            try
            {
                //HELPER VARIABLES
                LoginSession session;
                ServerInfo2 info;

                //GET THE SELECTED CHARACTER
                if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
                    if (cpkt.Index <= session.list.Count)
                    {
                        //MAKE THE GATEWAY ESTABLISH A CONNECTION TO THE WORLD SERVER
                        session.characterid = session.list[cpkt.Index].charId;
                        SMSG_ESTABLISHWORLDCONNECTION spkt = new SMSG_ESTABLISHWORLDCONNECTION();
                        spkt.SessionId = cpkt.SessionId;

                        //SET WORLD INFO
                        if (ServerManager2.Instance.server.TryGetValue(session.World, out info))
                        {
                            spkt.IpAddr = info.IP.GetAddressBytes();
                            spkt.Port = info.Port;
                            this.Send((byte[])spkt);
                        }
                    }
            }
            catch (Exception)
            {
                //Output a error
                byte[] buffer2 = new byte[] { 0x0B, 0x00, 0x74, 0x17, 0x91, 0x00, 0x02, 0x07, 0x00, 0x00, 0x01 };
                Array.Copy(BitConverter.GetBytes(cpkt.SessionId), 0, buffer2, 2, 4);
                this.Send(buffer2);
            }
        }

        private void OnDeleteChar(CMSG_DELETECHARACTER cpkt)
        {
            //HELPER VARIABLES
            ServerInfo2 info2;
            LoginSession session;

            //START CREATING THE CHARACTER ON OUR SERVER
            if (LoginSessionHandler.sessions.TryGetValue(cpkt.SessionId, out session))
                if (ServerManager2.Instance.server.TryGetValue(session.World, out info2))
                {
                    //CHECK IF INDEX IS VALID
                    if (cpkt.Index < 0 || cpkt.Index >= session.list.Count)
                    {
                        SMSG_DELETECHARACTER spkt = new SMSG_DELETECHARACTER();
                        spkt.Result = (byte)LoginError.INCORRECT_KEY_OR_SERVERNUMBERVALUE;
                        spkt.SessionId = cpkt.SessionId;
                        this.Send((byte[])spkt);
                    }
                    //CHECK IF NAME MATCHES OUR INDEX VALUE
                    else if (session.list[cpkt.Index].name.ToLowerInvariant() != cpkt.Name.ToLowerInvariant())
                    {
                        SMSG_DELETECHARACTER spkt = new SMSG_DELETECHARACTER();
                        spkt.Result = (byte)LoginError.INCORRECT_KEY_OR_SERVERNUMBERVALUE;
                        spkt.SessionId = cpkt.SessionId;
                        this.Send((byte[])spkt);
                    }
                    //DELETE CHARACTER
                    else
                    {
                        uint charid = session.list[cpkt.Index].charId;
                        session.PedingCharDeletion = charid;
                        info2.client.SM_DELETE_CHARACTER(charid, cpkt.SessionId);
                    }
                }
        }

        private void ConnectionEstablished()
        {
            byte[] buffer2 = new byte[] {
                         0x0B, 	0x00, 	0x74, 	0x17, 	0x91, 	0x00,
                         0x00, 	0x02, 	0x00, 	0x00, 	0x00
                    };
        }
    }
}