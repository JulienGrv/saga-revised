using Saga.Managers;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Quests;
using Saga.Shared.PacketLib.Login;
using Saga.Tasks;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace Saga.Map.Client
{
    [System.Reflection.Obfuscation(Exclude = false, StripAfterObfuscation = true, ApplyToMembers = true)]
    public partial class Client : Saga.Shared.NetworkCore.Client
    {
        #region Private Members

        /// <summary>
        /// Boolean defining if the client is first loading for
        /// the first or seccond time
        /// </summary>
        private bool IsFirstTimeLoad = true;

        /// <summary>
        /// Booleasn indicated if the map is loaded.
        /// </summary>
        /// <remarks>
        /// All packets objects that are not on demand generated e.d. monster updates
        /// should check if this is set to true prior to the update sending.
        /// </remarks>
        internal volatile bool isloaded = false;

        /// <summary>
        /// Character associated with the current client
        /// </summary>
        internal Character character;

        internal QuestBase pendingquest;

        #endregion Private Members

        #region Constructor / Deconstructor

        public Client()
        {
            //.this.chatLua.RegisterFunction("Create", this, typeof(Client).GetMethod("Create"));
            this.OnClose += new EventHandler(Client_OnClose);
        }

        public Client(Socket h)
            : base(h)
        {
            //.this.chatLua.RegisterFunction("Create", this, typeof(Client).GetMethod("Create"));
            this.OnClose += new EventHandler(Client_OnClose);
        }

        #endregion Constructor / Deconstructor

        #region Events

        protected override void OnConnect()
        {
        }

        private void Client_OnClose(object sender, EventArgs e)
        {
            try
            {
                CM_PARTYQUIT(null);

                //Release current session
                Singleton.NetworkService.AuthenticationClient.SM_RELEASESESSION(this.character.id);

                if (this.IsConnected == true)
                {
                    SMSG_KICKSESSION spkt2 = new SMSG_KICKSESSION();
                    spkt2.SessionId = this.character.id;
                    this.Send((byte[])spkt2);
                }
            }
            catch (Exception) { }

            try
            {
                if (this.character != null)
                {
                    GC.SuppressFinalize(this.character);                    //Disallows garbage collector to clean up

                    if (this.character.currentzone != null)                //Leave the current zone if any
                        this.character.currentzone.OnLeave(this.character);     //zone you are registered on.

                    LifeCycle.Unsubscribe(this.character);                  //Unsubscribe yourself on life thread
                    //Singleton.Database.SaveCharacter(this.character);

                    if (Singleton.Database.TransSave(this.character) == false)
                    {
                        Trace.WriteLine("Creating restore point could not save into database");
                        Singleton.Database.CreateRestorePoint(this.character);
                    }

                    GC.ReRegisterForFinalize(this.character);               //Allow garbage collector to clean up
                }

                //Set character instance to null
                this.character = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Login(CMSG_LOGIN cpkt)
        {
            if (cpkt.Cmd != 0x0501)
            {
                Trace.TraceError("Attempt to hack login protocol");
                WorldConnectionError();
                return;
            }

            try
            {
                Character tempCharacter = new Character(this, cpkt.CharacterId, cpkt.SessionId);
                if (Singleton.Database.TransLoad(tempCharacter) &&
                    Singleton.Zones.TryGetZone((uint)tempCharacter.map, out tempCharacter._currentzone))
                {
                    Singleton.Database.PostLoad(tempCharacter);
                    this.character = tempCharacter;
                    this.SendStart();
                    LifeCycle.Subscribe(this.character);
                    this.character.GmLevel = cpkt.GmLevel;
                    this.character.gender = cpkt.Gender;
                }
                else
                {
                    WorldConnectionError();
                    LifeCycle.Unsubscribe(this.character);
                }
            }
            catch (Exception e)
            {
                //WRITE OUT ANY ERRORS
                Trace.TraceWarning(e.ToString());
                WorldConnectionError();
                LifeCycle.Unsubscribe(this.character);
            }
        }

        public void SendStart()
        {
            SMSG_SENDSTART spkt = new SMSG_SENDSTART();
            spkt.Channel = 1;
            spkt.MapId = this.character.map;
            spkt.SessionId = this.character.id;
            spkt.X = this.character.Position.x;
            spkt.Y = this.character.Position.y;
            spkt.Z = this.character.Position.z;
            spkt.Unknown = (byte)(this.character.map + 0x65);
            this.Send((byte[])spkt);
        }

        #endregion Events

        #region Protected Methods

        public TraceLog log = new TraceLog("WorldClient.Packetlog", "Log class to log all packet trafic", 0);

#if DEBUG

        public override void Send(byte[] body)
        {
            ushort subpacketIdentifier = (ushort)(body[13] + (body[12] << 8));

            switch (log.LogLevel)
            {
                case 1: log.WriteLine("Network debug world", "Packet Sent: {0:X4}", subpacketIdentifier);
                    break;

                case 2: log.WriteLine("Network debug world", "Packet Sent: {0:X4}", subpacketIdentifier);
                    Console.WriteLine("Packet Sent: {0:X4} from: {1}", subpacketIdentifier, "world client");
                    break;

                case 3:
                    log.WriteLine("Network debug world", "Packet Sent: {0:X4} data {1}", subpacketIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                    break;

                case 4:
                    log.WriteLine("Network debug world", "Packet Sent: {0:X4} data {1}", subpacketIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                    Console.WriteLine("Packet Sent: {0:X4} from: {1}", subpacketIdentifier, "world client");
                    break;
            }

            base.Send(body);
        }

        public override void Send(ref byte[] body)
        {
            ushort subpacketIdentifier = (ushort)(body[13] + (body[12] << 8));

            switch (log.LogLevel)
            {
                case 1: log.WriteLine("Network debug world", "Packet Sent: {0:X4}", subpacketIdentifier);
                    break;

                case 2: log.WriteLine("Network debug world", "Packet Sent: {0:X4}", subpacketIdentifier);
                    Console.WriteLine("Packet Sent: {0:X4} from: {1}", subpacketIdentifier, "world client");
                    break;

                case 3:
                    log.WriteLine("Network debug world", "Packet Sent: {0:X4} data {1}", subpacketIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                    break;

                case 4:
                    log.WriteLine("Network debug world", "Packet Sent: {0:X4} data {1}", subpacketIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                    Console.WriteLine("Packet Sent: {0:X4} from: {1}", subpacketIdentifier, "world client");
                    break;
            }

            base.Send(body);
        }

#endif

        protected override void ProcessPacket(ref byte[] body)
        {
            try
            {
                #region Identification

                ushort subpacketIdentifier = (ushort)(body[13] + (body[12] << 8));

#if DEBUG
                switch (log.LogLevel)
                {
                    case 1: log.WriteLine("Network debug world", "Packet Recieved: {0:X4}", subpacketIdentifier);
                        break;

                    case 2: log.WriteLine("Network debug world", "Packet Recieved: {0:X4}", subpacketIdentifier);
                        Console.WriteLine("Packet received: {0:X4} from: {1}", subpacketIdentifier, "world client");
                        break;

                    case 3:
                        log.WriteLine("Network debug world", "Packet Recieved: {0:X4} data {1}", subpacketIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                        break;

                    case 4:
                        log.WriteLine("Network debug world", "Packet Recieved: {0:X4} data {1}", subpacketIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                        Console.WriteLine("Packet received: {0:X4} from: {1}", subpacketIdentifier, "world client");
                        break;
                }
#endif

                switch (body[12])
                {
                    case 0x00: goto Px00;
                    case 0x03: goto Px03;
                    case 0x04: goto Px04;
                    case 0x05: goto Px05;
                    case 0x06: goto Px06;
                    case 0x07: goto Px07;
                    case 0x08: goto Px08;
                    case 0x09: goto Px09;
                    case 0x0A: goto Px0A;
                    case 0x0C: goto Px0C;
                    case 0x0E: goto Px0E;
                    case 0x0F: goto Px0F;
                    case 0x10: goto Px10;
                    case 0x11: goto Px11;
                    case 0x12: goto Px12;
                    default: goto Px10;
                }

                #endregion Identification

                #region 0x00 - Internal Packets

            Px00:
                switch (subpacketIdentifier)
                {
                    case 0x0001: Login((CMSG_LOGIN)body); return;
                } return;

                #endregion 0x00 - Internal Packets

                #region 0x03 - Misc. Movement Packets

            Px03:

                switch (subpacketIdentifier)
                {
                    case 0x0301: this.CM_CHARACTER_MAPLOADED(); break;
                    case 0x0302: this.CM_CHARACTER_MOVEMENTSTART((CMSG_MOVEMENTSTART)body); break;
                    case 0x0303: this.CM_CHARACTER_MOVEMENTSTOPPED((CMSG_MOVEMENTSTOPPED)body); break;
                    case 0x0304: this.CM_CHARACTER_UPDATEYAW((CMSG_SENDYAW)body); break;
                    case 0x0305: this.CM_CHARACTER_CHANGESTATE((CMSG_ACTORCHANGESTATE)body); break;
                    case 0x0306: this.CM_CHARACTER_USEPORTAL((CMSG_USEPORTAL)body); break;
                    case 0x0307: this.CM_CHARACTER_JOBCHANGE((CMSG_CHANGEJOB)body); break;
                    case 0x0308: this.CM_CHARACTER_RETURNTOHOMEPOINT((CMSG_SENDHOMEPOINT)body); break;
                    case 0x0309: this.CM_CHARACTER_STATPOINTUPDATE((CMSG_STATUPDATE)body); break;
                    case 0x030A: this.CM_CHARACTER_DIVEUP(); break; //newly packet
                    case 0x030B: this.CM_CHARACTER_JUMP(); break;
                    case 0x030C: this.CM_CHARACTER_SHOWLOVE((CMSG_SHOWLOVE)body); break;
                    case 0x030D: this.CM_CHARACTER_CONFIRMSHOWLOVE((CMSG_SHOWLOVECONFIRM)body); break;
                    case 0x030E: this.CM_CHARACTER_FALL((CMSG_ACTORFALL)body); break;
                    case 0x030F: this.CM_CHARACTER_DIVEDOWN(); break;
                    case 0x0310: this.CM_CHARACTER_DIVEUP(); break;
                    case 0x0311: this.CM_CHARACTER_EXPENSIVESFORJOBCHANGE((CMSG_SELECTJOB)body); break;
                    default: Console.WriteLine("{0:X4}", subpacketIdentifier); break;
                } return;

                #endregion 0x03 - Misc. Movement Packets

                #region 0x04 - Chat packets

            Px04:
                switch (subpacketIdentifier)
                {
                    case 0x0401: this.CM_SENDCHAT((CMSG_SENDCHAT)body); break;
                    case 0x0402: this.CM_SENDWHISPER((CMSG_SENDWHISPER)body); break;
                    default: Console.WriteLine("{0:X4}", subpacketIdentifier); break;
                } return;

                #endregion 0x04 - Chat packets

                #region 0x05 - Misc. Item packets

            Px05:
                switch (subpacketIdentifier)
                {
                    case 0x0501: this.CM_MOVEITEM((CMSG_MOVEITEM)body); break;
                    case 0x0504: this.CM_IVENTORY_SORT((CMSG_SORTINVENTORYLIST)body); break;
                    case 0x0505: this.CM_STORAGE_SORT((CMSG_SORTSTORAGELIST)body); break;
                    case 0x0507: this.CM_DISCARDITEM((CMSG_DELETEITEM)body); break;
                    case 0x0508: this.CM_REPAIREQUIPMENT((CMSG_REPAIRITEM)body); break;
                    case 0x050B: this.CM_WEAPONMOVE((CMSG_WEAPONMOVE)body); break;
                    case 0x050D: this.CM_WEAPONSWITCH((CMSG_WEAPONSWITCH)body); break;
                    case 0x050F: this.CM_WEAPONARY_UPGRADE((CMSG_WEAPONUPGRADE)body); break;
                    case 0x0512: this.CM_WEAPONAUGE((CMSG_WEAPONAUGE)body); break;
                    case 0x0515: this.CM_WEAPONARY_CHANGESUFFIX((CMSG_WEAPONCHANGESUFFIX)body); break;
                    case 0x0516: this.CM_WEAPONARY_NEWCHANGESUFFIX((CMSG_WEAPONCHANGESUFFIX2)body); break;
                    case 0x0517: this.CM_USEMAPITEM((CMSG_USEMAP)body); break;
                    case 0x0518: this.CM_USEADMISSIONWEAPON((CMSG_USEWEAPONADMISSION)body); break;
                    case 0x0519: this.CM_USEDYEITEM((CMSG_USEDYEITEM)body); break;
                    case 0x051A: this.CM_USESTATRESETITEM((CMSG_STATRESETPOTION)body); break;
                    case 0x051B: this.CM_USESUPLEMENTSTONE((CMSG_USESUPPLEMENTSTONE)body); break;
                    default: Console.WriteLine("Recieved {0:X4}", subpacketIdentifier); break;
                } return;

                #endregion 0x05 - Misc. Item packets

                #region 0x06 - Misc. NPC Packets

            Px06:
                switch (subpacketIdentifier)
                {
                    case 0x0601: this.CM_NPCCHAT((CMSG_NPCCHAT)body); break;
                    case 0x0602: this.CM_ONSELECTBUTTON((CMSG_SELECTBUTTON)body); break;
                    case 0x0603: this.CM_LEAVENPC(); break;
                    case 0x0604: this.CM_ONSELECTMENUSUBITEM((CMSG_NPCMENU)body); break;
                    case 0x0605: this.CM_PERSONALREQUESTCONFIRMATION((CMSG_PERSONALREQUEST)body); break;
                    case 0x0606: this.CM_REQUESTSHOWNPCLOCATION((CMSG_NPCLOCATIONSELECT)body); break;
                    case 0x0607: this.CM_GETHATEINFO((CMSG_HATEINFO)body); break;
                    case 0x0608: this.CM_GETATTRIBUTE((CMSG_ATTRIBUTE)body); break;
                    case 0x0609: this.CM_RELEASEATTRIBUTE(); break;
                    case 0x060B: this.CM_SELECTSUPPPLYMENU((CMSG_SUPPLYSELECT)body); break;
                    case 0x060D: this.CM_EXCHANGEGOODS((CMSG_SUPPLYEXCHANGE)body); break;
                    case 0x060F: this.CM_CLEARCORPSE(); break;
                    case 0x0610: this.CM_REQUESTITEMDROPLIST((CMSG_DROPLIST)body); break;
                    case 0x0611: this.CM_NPCINTERACTION_SHOPSELL((CMSG_NPCSHOPSELL)body); break;
                    case 0x0612: this.CM_NPCINTERACTION_SHOPBUY((CMSG_NPCSHOPBUY)body); break;
                    case 0x0613: this.CM_NPCINTERACTION_SHOPREBUY((CMSG_NPCREBUY)body); break;
                    case 0x0614: this.CM_DROPSELECT((CMSG_DROPSELECT)body); break;
                    case 0x0615: this.CM_EVENTPARTICIPATE((CMSG_SELECTEVENTINFO)body); break;
                    case 0x0616: this.CM_EVENTRECIEVEITEM((CMSG_RECEIVEEVENTITEM)body); break;
                    case 0x0617: this.CM_NPCINTERACTION_WARPSELECT((CMSG_WARP)body); break;
                    default: Console.WriteLine("{0:X4}", subpacketIdentifier); break;
                } return;

                #endregion 0x06 - Misc. NPC Packets

                #region 0x07 - Questing packets

            Px07:
                switch (subpacketIdentifier)
                {
                    case 0x0701: this.OnWantQuestGroupList(); break;
                    case 0x0704: this.CM_QUESTCONFIRMCANCEL((CMSG_QUESTCONFIRMCANCEL)body); break;
                    case 0x0705: this.CM_QUESTCONFIRM((CMSG_QUESTCONFIRMED)body); break;
                    case 0x0706: this.CM_QUESTCOMPLETE((CMSG_QUESTCOMPLETE)body); break;
                    case 0x0707: this.OnQuestRewardChoice(); break;
                    case 0x0709: this.CM_QUESTITEMSTART((CMSG_QUESTITEMSTART)body); break;
                    default: Console.WriteLine("{0:X4}", subpacketIdentifier); break;
                } return;

                #endregion 0x07 - Questing packets

                #region 0x08 - Trading Packets

            Px08:
                switch (subpacketIdentifier)
                {
                    case 0x0801: this.CM_TRADEINVITATION((CMSG_REQUESTTRADE)body); break;
                    case 0x0802: this.CM_TRADEINVITATIONREPLY((CMSG_TRADEINVITATIONRESULT)body); break;
                    case 0x0803: this.CM_TRADEITEM((CMSG_TRADEITEM)body); break;
                    case 0x0804: this.CM_TRADEMONEY((CMSG_TRADEZENY)body); break;
                    case 0x0805: this.CM_TRADECONTENTAGREE((CMSG_TRADECONTENTCONFIRM)body); break;
                    case 0x0806: this.CM_TRADECONFIRM((CMSG_TRADECONFIRM)body); break;
                    case 0x0807: this.CM_TRADECANCEL((CMSG_TRADECANCEL)body); break;
                    default: Console.WriteLine("{0:X4}", subpacketIdentifier); break;
                } return;

                #endregion 0x08 - Trading Packets

                #region 0x09 - Skill packets

            Px09:
                switch (subpacketIdentifier)
                {
                    case 0x0903: this.CM_SKILLCAST((CMSG_SKILLCAST)body); break;
                    case 0x0904: this.CM_SKILLCASTCANCEL((CMSG_SKILLCASTCANCEL)body); break;
                    case 0x0905: this.CM_USEOFFENSIVESKILL((CMSG_OFFENSIVESKILL)body); break;
                    case 0x0906: this.CM_SKILLTOGGLE((CMSG_SKILLTOGLE)body); break;
                    case 0x090A: this.CM_ITEMTOGGLE((CMSG_ITEMTOGLE)body); break;
                    case 0x090C: this.CM_SKILLS_LEARNFROMSKILLBOOK((CMSG_SKILLLEARN)body); break;
                    case 0x090D: this.CM_SKILLS_ADDSPECIAL((CMSG_SETSPECIALSKILL)body); break;
                    case 0x090E: this.CM_SKILLS_REMOVESPECIAL((CMSG_REMOVESPECIALSKILL)body); break;
                    case 0x0911: this.CM_SKILLS_REQUESTSPECIALSET((CMSG_WANTSETSPECIALLITY)body); break;
                    default: Console.WriteLine("{0:X4}", subpacketIdentifier); break;
                } return;

                #endregion 0x09 - Skill packets

                #region 0x0A - Shortcut packets

            Px0A:
                switch (subpacketIdentifier)
                {
                    case 0x0A01: this.OnAddShortcut(); break;
                    case 0x0A02: this.OnDelShortcut(); break;
                    default: Console.WriteLine("{0:X4}", subpacketIdentifier); break;
                } return;

                #endregion 0x0A - Shortcut packets

                #region 0x0C - Mail packets

            Px0C:
                switch (subpacketIdentifier)
                {
                    case 0x0C01: this.CM_REQUESTINBOXMAILLIST((CMSG_REQUESTINOX)body); break;
                    case 0x0C02: this.CM_NEWMAILITEM((CMSG_SENDMAIL)body); break;
                    case 0x0C03: this.CM_INBOXMAILITEM((CMSG_GETMAIL)body); break;
                    case 0x0C04: this.CM_RETRIEVEITEMATTACHMENT((CMSG_GETITEMATTACHMENT)body); break;
                    case 0x0C05: this.CM_RETRIEVEZENYATTACHMENT((CMSG_GETZENYATTACHMENT)body); break;
                    case 0x0C06: this.CM_MAILDELETE((CMSG_DELETEMAIL)body); break;
                    case 0x0C07: this.CM_REQUESTOUTBOXMAILLIST((CMSG_REQUESTOUTBOX)body); break;
                    case 0x0C08: this.CM_MAILCANCEL((CMSG_MAILCANCEL)body); break;
                    case 0x0C09: this.CM_OUTBOXMAILITEM((CMSG_GETMAILOUTBOX)body); break;
                    case 0x0C0A: this.CM_MAILCLEAR((CMSG_MAILCLEAR)body); break;
                    default: Console.WriteLine("{0:X4}", subpacketIdentifier); break;
                } return;

                #endregion 0x0C - Mail packets

                #region 0x0E - Party packets

            Px0E:
                switch (subpacketIdentifier)
                {
                    case 0x0E01: this.CM_PARTYREQUEST((CMSG_PARTYINVITE_LOCAL)body); break;
                    case 0x0E02: this.CM_PARTYREQUEST((CMSG_PARTYINVITE)body); break;
                    case 0x0E03: this.CM_PARTYINVITATIONACCEPT((CMSG_PARTYINVITATIONACCEPT)body); break;
                    case 0x0E04: this.CM_PARTYQUIT((CMSG_PARTYQUIT)body); break;
                    case 0x0E05: this.CM_PARTYKICK((CMSG_PARTYKICK)body); break;
                    case 0x0E06: this.CM_PARTYMODE((CMSG_PARTYMODE)body); break;
                    case 0x0E07: this.CM_PARTYSETLEADER((CMSG_PARTYSETLEADER)body); break;
                    case 0x0E08: this.CM_PARTYINVITATIONCANCELED((CMSG_PARTYINVITECANCEL)body); break;
                    default: Console.WriteLine("{0:X4}", subpacketIdentifier); break;
                } return;

                #endregion 0x0E - Party packets

                #region 0x0F - Market packets

            Px0F:
                switch (subpacketIdentifier)
                {
                    case 0x0F01: this.CM_MARKET_SEARCH((CMSG_MARKETSEARCH)body); break;
                    case 0x0F02: this.CM_MARKET_BUY((CMSG_MARKETBUY)body); break;
                    case 0x0F03: this.CM_MARKET_SEARCHOWNERITEMS(); break;
                    case 0x0F04: this.CM_MARKET_REGISTERITEM((CMSG_MARKETREGISTER)body); break;
                    case 0x0F05: this.CM_MARKET_DELETEITEM((CMSG_MARKETDELETEITEM)body); break;
                    case 0x0F06: this.CM_MARKET_CHANGECOMMENT((CMSG_MARKETMESSAGE)body); break;
                    case 0x0F07: this.CM_MARKET_COMMENT((CMSG_MARKETGETCOMMENT)body); break;
                    case 0x0F08: this.CM_MARKET_OWNERCOMMENT((CMSG_MARKETOWNERCOMMENT)body); break;
                    default: Console.WriteLine("{0} - {0:X2}", subpacketIdentifier); break;
                } return;

                #endregion 0x0F - Market packets

                #region 0x10 - Scenario packets

            Px10:
                switch (subpacketIdentifier)
                {
                    case 0x1001: CM_SCENARIO_EVENTEND(body); break;
                    default: Console.WriteLine("{0} - {0:X2}", subpacketIdentifier); break;
                } return;

                #endregion 0x10 - Scenario packets

                #region 0x11 - Misc packets

            Px11:
                switch (subpacketIdentifier)
                {
                    case 0x1101: CM_SELECTCHANNEL((CMSG_SELECTCHANNEL)body); break;
                    case 0x1102: CM_TAKESCREENSHOT((CMSG_TAKESCREENSHOT)body); break;
                    default: Console.WriteLine("{0} - {0:X2}", subpacketIdentifier); break;
                } return;

                #endregion 0x11 - Misc packets

                #region 0x12 - Friendlist / Blacklist Packets

            Px12:
                switch (subpacketIdentifier)
                {
                    case 0x1201: this.CM_FRIENDLIST_REGISTER((CMSG_FRIENDLIST_REGISTER)body); return;
                    case 0x1202: this.CM_FRIENDLIST_UNREGISTER((CMSG_FRIENDLIST_UNREGISTER)body); return;
                    case 0x1203: this.CM_FRIENDLIST_REFRESH((CMSG_FRIENDLIST_REFRESH)body); return;
                    case 0x1204: this.CM_BLACKLIST_REGISTER((CMSG_BLACKLIST_REGISTER)body); return;
                    case 0x1205: this.CM_BLACKLIST_UNREGISTER((CMSG_BLACKLIST_UNREGISTER)body); return;
                    case 0x1206: this.CM_BLACKLIST_REFRESH((CMSG_BLACKLIST_REFRESH)body); break;
                    default: Console.WriteLine("{0:X4}", subpacketIdentifier); return;
                } return;

                #endregion 0x12 - Friendlist / Blacklist Packets
            }
            catch (Exception e)
            {
                if (ConsoleCommands.DisconnectClientOnException == true)
                    throw e;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Notifies the gateway that we're not able to make a connection.
        /// </summary>
        private void WorldConnectionError()
        {
            byte[] buffer2 = new byte[] { 0x0B, 0x00, 0x74, 0x17, 0x91, 0x00, 0x02, 0x07, 0x00, 0x00, 0x01 };
            Array.Copy(BitConverter.GetBytes(this.character.id), 0, buffer2, 2, 4);
            this.Send(buffer2);
        }

        #endregion Private Methods
    }
}