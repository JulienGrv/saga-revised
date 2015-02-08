using Saga.Packets;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Saga.Map.Client
{
    public partial class InternalClient : Saga.Shared.NetworkCore.Client
    {
        ~InternalClient()
        {
        }

        public InternalClient(Socket h)
            : base(h)
        {
        }

        protected override void OnConnect()
        {
            this.OnClose += new EventHandler(InternalClient_OnClose);
            base.OnConnect();
        }

        private void InternalClient_OnClose(object sender, EventArgs e)
        {
            GC.SuppressFinalize(this);
            Trace.WriteLine("Authentication server disconnected", "Network");
            while (base.IsConnected == false)
            {
                Thread.Sleep(5000);

                try
                {
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(Managers.NetworkService._host_2, Managers.NetworkService._port_2);
                    if (sock.Connected)
                    {
                        Managers.NetworkService.InterNetwork = new InternalClient(sock);
                        Managers.NetworkService.InterNetwork.OnConnect();
                        Managers.NetworkService.InterNetwork.WORLD_RECONNECT();
                    }
                    break;
                }
                catch (Exception)
                {
                    Trace.WriteLine("Network reconnection failed", "Network");
                }
            }

            GC.ReRegisterForFinalize(this);
        }

        protected override void ProcessPacket(ref byte[] body)
        {
            ushort subpacketIdentifier = (ushort)(body[13] + (body[12] << 8));
            switch (subpacketIdentifier)
            {
                case 0x0001: CM_WORLDINSTANCEACK((CMSG_WORLDINSTANCEACK)body); return;
                case 0x0004: CM_KILLSESSION((CMSG_KILLSESSION)body); return;
                case 0x000E: CM_PING((CMSG_PING)body); return;
                case 0x0106: DELETE_CHARACTER((CMSG_INTERNAL_CHARACTERDELETE)body); return;
                case 0x0105: CHARACTER_CREATE((CMSG_INTERNAL_CHARACTERCREATE)body); return;
                case 0x0002: SELECT_CHARACTERS((CMSG_FINDCHARACTERS)body); return;
                case 0x0003: SELECT_CHARACTER((CMSG_FINDCHARACTERDETAILS)body); return;
                case 0xFF02: CM_LOADCHARACTER((CMSG_CHARACTERLOGIN)body); return;
                case 0xFF03: CM_SETRATES((CMSG_SETRATES)body); return;
                default: Console.WriteLine("Packet received {0:X4}", subpacketIdentifier); return;
            }
        }

        private static byte[] NextProof = new byte[16];

        private void CM_WORLDINSTANCEACK(CMSG_WORLDINSTANCEACK cpkt)
        {
            if (cpkt.Reason == 0)
            {
                Console.WriteLine("Everything okay");
            }
            else if (cpkt.Reason == 1)
            {
                Console.WriteLine("Invalid proof");
            }
            else
            {
                Console.WriteLine("Server already online");
            }
        }

        private void CM_KILLSESSION(CMSG_KILLSESSION cpkt)
        {
            //Try finding the session
            Character c;
            if (Tasks.LifeCycle.TryGetById(cpkt.Session, out c))
            {
                c.client.Close();
                c.client.Dispose();
            }

            //Always release session
            SM_RELEASESESSION(cpkt.Session);
        }

        private void CM_LOADCHARACTER(CMSG_CHARACTERLOGIN cpkt)
        {
            Console.WriteLine("Load character: {0} {1}", cpkt.Session, cpkt.CharacterId);

            try
            {
                Character newCharacter = new Character(cpkt.CharacterId);
                if (Singleton.Database.TransLoad(newCharacter)
                 && Singleton.Zones.TryGetZone((uint)newCharacter.map, out newCharacter._currentzone))
                {
                    //this.SendStart();
                    //LifeCycle.Subscribe(this.character);

                    SMSG_CHARACTERLOGINREPLY spkt = new SMSG_CHARACTERLOGINREPLY();
                    spkt.SessionId = cpkt.Session;
                    spkt.Result = 0;
                    this.Send((byte[])spkt);
                }
                else
                {
                    //WorldConnectionError();
                    //LifeCycle.Unsubscribe(this.character);

                    SMSG_CHARACTERLOGINREPLY spkt = new SMSG_CHARACTERLOGINREPLY();
                    spkt.SessionId = cpkt.Session;
                    spkt.Result = 1;
                    this.Send((byte[])spkt);

                    Trace.WriteLine(string.Format("Cannot find zone instance {0}", newCharacter.map));
                }
            }
            catch (Exception e)
            {
                //WRITE OUT ANY ERRORS
                //WorldConnectionError();
                //LifeCycle.Unsubscribe(this.character);
                Trace.TraceWarning(e.ToString());

                SMSG_CHARACTERLOGINREPLY spkt = new SMSG_CHARACTERLOGINREPLY();
                spkt.SessionId = cpkt.Session;
                spkt.Result = 2;
                this.Send((byte[])spkt);
            }
        }

        private void CM_SETRATES(CMSG_SETRATES cpkt)
        {
            //If previous is not enabled but does enables now
            if (!Managers.ConsoleCommands.isaddisplayed && cpkt.IsAdDisplayed == 1)
            {
                Singleton.experience.Modifier_Cexp += 2;
                Singleton.experience.Modifier_Jexp += 2;
                Singleton.experience.Modifier_Drate += 3;
                Managers.ConsoleCommands.isaddisplayed = true;
                Trace.TraceInformation("Apply increased stats settings");
            }
            //If previous is enabled but disables it now.
            else if (Managers.ConsoleCommands.isaddisplayed && cpkt.IsAdDisplayed == 0)
            {
                Singleton.experience.Modifier_Cexp -= 2;
                Singleton.experience.Modifier_Jexp -= 2;
                Singleton.experience.Modifier_Drate -= 3;
                Managers.ConsoleCommands.isaddisplayed = false;
                Trace.TraceInformation("Deapply increased stats stat settings");
            }
        }

        private void CM_PING(CMSG_PING cpkt)
        {
            SMSG_PONG spkt = new SMSG_PONG();
            spkt.SessionId = cpkt.SessionId;
            this.Send((byte[])spkt);
        }

        private static Queue<uint> ReleasedSessions = new Queue<uint>();

        internal void SM_RELEASESESSION(uint session)
        {
            Console.WriteLine("Release session");

            if (this.IsConnected)
            {
                SMSG_RELEASESESSION spkt = new SMSG_RELEASESESSION();
                spkt.Session = session;
                this.Send((byte[])spkt);
            }
            else
            {
                Trace.WriteLine("Auth server not online session stacked", "Network");
                ReleasedSessions.Enqueue(session);
            }
        }
    }
}