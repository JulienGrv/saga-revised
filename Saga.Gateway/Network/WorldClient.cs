using Saga.Gateway.Network;
using Saga.Shared.NetworkCore;
using System;
using System.Net.Sockets;

namespace Saga.Gateway
{
    public class WorldClient : Client
    {
        public static TraceLog log = new TraceLog("WorldClient.Packetlog", "Log class to log all packet trafic", 0);

        public WorldClient(Socket sock)
        {
            this.socket = sock;
            base.Connect();
        }

        protected override void ProcessPacket(ref byte[] body)
        {
            ushort packetIdentifier = (ushort)(body[7] + (body[6] << 8));

#if DEBUG
            switch (log.LogLevel)
            {
                case 1: log.WriteLine("Network debug world", "Packet Recieved: {0:X4}", packetIdentifier);
                    break;

                case 2: log.WriteLine("Network debug world", "Packet Recieved: {0:X4}", packetIdentifier);
                    Console.WriteLine("Packet received: {0:X4} from: {1}", packetIdentifier, "world client");
                    break;

                case 3:
                    log.WriteLine("Network debug world", "Packet Recieved: {0:X4} data {1}", packetIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                    break;

                case 4:
                    log.WriteLine("Network debug world", "Packet Recieved: {0:X4} data {1}", packetIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                    Console.WriteLine("Packet received: {0:X4} from: {1}", packetIdentifier, "world client");
                    break;
            }
#endif

            switch (packetIdentifier)
            {
                case 0x0301: ForwardToLoginClient(body); return;
                case 0x0401: ForwardToClient(body); return;
                case 0x0501: ForwardToWorldClient(body); return;
                case 0x0601: ForwardToClient(body); return;
                case 0x0701: ProcessInternal(body); return;
                default: ForwardToClient(body); return;
                //default: Console.WriteLine("UNSUPPORTED FORWARD ADRESS {0}", Ident); return;
            }
        }

        /// <summary>
        /// Forwards raw packets straight to the associated client.
        /// </summary>
        /// <param name="buffer"></param>
        private void ForwardToClient(byte[] buffer)
        {
            GatewayClient client = null;
            try
            {
                uint session = BitConverter.ToUInt32(buffer, 2);
                if (GatewayPool.Instance.lookup.TryGetValue(session, out client))
                {
                    client.Send(buffer);
                }
                else
                {
                    Console.WriteLine("Unable to find client");
                }
            }
            catch (ObjectDisposedException)
            {
                if (client != null)
                {
                    client.world = null;
                    client.Close();
                }
            }
            catch (Exception)
            {
                if (client != null)
                {
                    client.world = null;
                    client.Close();
                }
            }
        }

        /// <summary>
        /// Forwards raw packets straight to the associated
        /// world client.
        /// </summary>
        /// <param name="buffer"></param>
        private void ForwardToWorldClient(byte[] buffer)
        {
            GatewayClient client = null;
            try
            {
                uint session = BitConverter.ToUInt32(buffer, 2);
                if (GatewayPool.Instance.lookup.TryGetValue(session, out client))
                {
                    client.world.Send(buffer);
                }
            }
            catch (ObjectDisposedException)
            {
                if (client != null)
                {
                    client.world = null;
                    client.Close();
                }
            }
            catch (Exception)
            {
                if (client != null)
                {
                    client.world = null;
                    client.Close();
                }
            }
        }

        private void ForwardToLoginClient(byte[] body)
        {
            try
            {
                LoginClient client;
                if (NetworkManager.TryGetLoginClient(out client))
                    client.Send(body);
            }
            catch (Exception)
            {
            }
        }

        private void ProcessInternal(byte[] body)
        {
            ushort subpacketIdentifier = (ushort)(body[13] + (body[12] << 8));
            switch (subpacketIdentifier)
            {
                case 0x0010: Kick(body); break;
            }
        }

        private void Kick(byte[] body)
        {
            GatewayClient client;
            uint session = BitConverter.ToUInt32(body, 2);
            if (GatewayPool.Instance.lookup.TryGetValue(session, out client))
            {
                client.Close();
                client.world.Close();
            }
        }
    }
}