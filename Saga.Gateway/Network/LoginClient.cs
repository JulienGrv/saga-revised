using Saga.Gateway.Network;
using Saga.Network.Packets;
using Saga.Packets;
using Saga.Shared.NetworkCore;
using Saga.Shared.PacketLib.Login;
using System;
using System.Net;
using System.Net.Sockets;

namespace Saga.Gateway
{
    public class LoginClient : Client
    {
        public static TraceLog log = new TraceLog("LoginClient.Packetlog", "Log class to log all packet trafic", 0);

        internal LoginClient(Socket h)
            : base(h)
        {
        }

        internal LoginClient(string host, int port)
            : base(host, port)
        {
        }

        protected override void ProcessPacket(ref byte[] body)
        {
            ushort packetIdentifier = (ushort)(body[7] + (body[6] << 8));

#if DEBUG
            switch (log.LogLevel)
            {
                case 1: log.WriteLine("Network debug login", "Packet Recieved: {0:X4}", packetIdentifier);
                    break;

                case 2: log.WriteLine("Network debug login", "Packet Recieved: {0:X4}", packetIdentifier);
                    Console.WriteLine("Packet received: {0:X4} from: {1}", packetIdentifier, "login client");
                    break;

                case 3:
                    log.WriteLine("Network debug login", "Packet Recieved: {0:X4} data {1}", packetIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                    break;

                case 4:
                    log.WriteLine("Network debug login", "Packet Recieved: {0:X4} data {1}", packetIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                    Console.WriteLine("Packet received: {0:X4} from: {1}", packetIdentifier, "login client");
                    break;
            }
#endif

            switch (packetIdentifier)
            {
                case 0x0001: ObtainedSessionId((CMSG_SESSIONREQUEST)body); return;
                case 0x0002: ConnectToWorldServer((CMSG_ESTABLISHWORLDCONNECTION)body); return;
                case 0x0301: ForwardToLoginClient(body); return;
                case 0x0401: ForwardToClient(body); return;
                case 0x0501: ForwardToWorldClient(body); return;
                case 0x0601: ForwardToClient(body); return;
                default: ForwardToClient(body); return;
            }
        }

        /// <summary>
        /// Occurs when our login server notifies us we obtained a new
        /// session id we can give away.
        /// </summary>
        private void ObtainedSessionId(CMSG_SESSIONREQUEST cpkt)
        {
            SessionPool.Instance.Add(cpkt.SessionId);
        }

        private void ConnectToWorldServer(CMSG_ESTABLISHWORLDCONNECTION cpkt)
        {
            GatewayClient client;
            uint session = cpkt.SessionId;

            if (GatewayPool.Instance.lookup.TryGetValue(session, out client))
            {
                try
                {
                    IPAddress address = new IPAddress(cpkt.IPAddres);
                    int port = cpkt.Port;

                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(address, port);
                    WorldClient instance = new WorldClient(sock);
                    client.world = instance;

                    RelayPacket pkt = new RelayPacket();
                    pkt.Cmd = 0x0301;
                    pkt.Id = 0x0110;
                    pkt.SessionId = session;
                    this.Send((byte[])pkt);

                    byte[] buffer2 = new byte[] {
                         0x0B, 	0x00, 	0x74, 	0x17, 	0x91, 	0x00,
                         0x02, 	0x07, 	0x00, 	0x00, 	0x00
                    };

                    Array.Copy(BitConverter.GetBytes(session), 0, buffer2, 2, 4);
                    client.Send(buffer2);
                }
                catch (Exception)
                {
                    byte[] buffer2 = new byte[] {
                         0x0B, 	0x00, 	0x74, 	0x17, 	0x91, 	0x00,
                         0x02, 	0x07, 	0x00, 	0x00, 	0x01
                    };
                    Array.Copy(BitConverter.GetBytes(session), 0, buffer2, 2, 4);
                    client.Send(buffer2);
                }
            }
        }

        /// <summary>
        /// This function is invoked by our sessionpool once it claims it
        /// needs more session.
        /// </summary>
        internal void RequestSessionId()
        {
            SMSG_SESSIONREQUEST spkt = new SMSG_SESSIONREQUEST();
            this.Send((byte[])spkt);
        }

        /// <summary>
        /// This function is invoked by our sessionpool once it claims it
        /// needs more session.
        /// </summary>
        internal void ReleaseSessionId()
        {
            /*
            SMSG_SESSIONREQUEST spkt = new SMSG_SESSIONREQUEST();
            this.Send((byte[])spkt);
            */
        }

        internal void ExchangeIpAdress(IPAddress adrr)
        {
            SMSG_NETWORKADRESSIP spkt = new SMSG_NETWORKADRESSIP();
            spkt.ConnectionFrom = adrr;
            spkt.SessionId = 0;
            this.Send((byte[])spkt);
        }

        //Initialisation Protocol.....

        internal void CreateLoginConnection(IPAddress adress)
        {
            Console.WriteLine("Create login connection: {0}", adress);
        }

        //Forwards......

        /// <summary>
        /// Forwards raw packets straight to the associated
        /// client.
        /// </summary>
        /// <param name="buffer"></param>
        private void ForwardToClient(byte[] buffer)
        {
            GatewayClient client;
            uint session = BitConverter.ToUInt32(buffer, 2);
            if (GatewayPool.Instance.lookup.TryGetValue(session, out client))
            {
                client.Send(buffer);
            }
        }

        /// <summary>
        /// Forwards raw packets straight to the associated
        /// world client.
        /// </summary>
        /// <param name="buffer"></param>
        private void ForwardToWorldClient(byte[] buffer)
        {
            GatewayClient client;
            uint session = BitConverter.ToUInt32(buffer, 2);
            if (GatewayPool.Instance.lookup.TryGetValue(session, out client))
            {
                client.world.Send(buffer);
            }
        }

        private void ForwardToLoginClient(byte[] body)
        {
            LoginClient client;
            if (NetworkManager.TryGetLoginClient(out client))
                client.Send(body);
        }
    }
}