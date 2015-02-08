using Saga.Packets;
using Saga.Shared.PacketLib;
using Saga.Shared.PacketLib.Gateway;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Saga.Gateway.Network
{
    public class GatewayClient : Shared.NetworkCore.EncryptedClient
    {
        #region CORE

        public static TraceLog log = new TraceLog("GatewayClient.Packetlog", "Log class to log all packet trafic", 0);
        public static bool CheckCrc = false;
        public uint session = 0;
        public WorldClient world;

        public GatewayClient(Socket socket)
            : base(socket)
        {
            OnClose += new EventHandler(GatewayClient_OnClose);
            Console.WriteLine("New client opened");
        }

        private void GatewayClient_OnClose(object sender, EventArgs e)
        {
            ReleaseResources();
        }

        private void ReleaseResources()
        {
            try
            {
                //Close resource on the login
                SMSG_RELEASEAUTH spkt = new SMSG_RELEASEAUTH();
                spkt.Session = this.session;
                spkt.SessionId = 0;

                LoginClient client;
                if (NetworkManager.TryGetLoginClient(out client))
                    client.Send((byte[])spkt);
            }
            catch (SocketException)
            {
                Trace.TraceWarning("Network error");
            }
            catch (Exception)
            {
                Trace.TraceError("Unidentified error");
            }
            finally
            {
                if (world != null) world.Close();
                GatewayPool.Instance.lookup.Remove(this.session);
            }
        }

        protected override void ProcessPacket(ref byte[] body)
        {
            ushort packetIdentifier = (ushort)(body[7] + (body[6] << 8));

#if DEBUG
            switch (log.LogLevel)
            {
                case 1: log.WriteLine("Network debug gateway", "Packet Recieved: {0:X4}", packetIdentifier);
                    break;

                case 2: log.WriteLine("Network debug gateway", "Packet Recieved: {0:X4}", packetIdentifier);
                    Console.WriteLine("Packet received: {0:X4} from: {1}", packetIdentifier, "gateway client");
                    break;

                case 3:
                    log.WriteLine("Network debug gateway", "Packet Recieved: {0:X4} data {1}", packetIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                    break;

                case 4:
                    log.WriteLine("Network debug gateway", "Packet Recieved: {0:X4} data {1}", packetIdentifier, Saga.Shared.PacketLib.Other.Conversions.ByteToHexString(body));
                    Console.WriteLine("Packet received: {0:X4} from: {1}", packetIdentifier, "gateway client");
                    break;
            }
#endif

            switch (packetIdentifier)
            {
                case 0x0105: OnHeader(); return;
                case 0x0101: OnKey((CMSG_SENDKEY)body); return;
                case 0x0102: OnGUID((CMSG_GUID)body); return;
                case 0x0103: OnKey2(); return;
                case 0x0104: OnIdentify(); return;
                case 0x0501: RedirectMap(body); return;
                case 0x0301: RedirectLogin(body); return;
                default: Trace.TraceWarning("Unsupported packet found with id: {0:X4}", packetIdentifier); this.Close(); break;
            }
        }

        #endregion CORE

        #region Packet Handeling

        private void OnHeader()
        {
            Trace.TraceInformation("Header Recieved from {0}", this.socket.RemoteEndPoint);
            try
            {
                byte[] tempServerKey = Encryption.GenerateKey();
                byte[] expandedServerKey = Encryption.GenerateDecExpKey(tempServerKey);
                SMSG_SENDKEY spkt = new SMSG_SENDKEY();
                spkt.Key = expandedServerKey;
                spkt.Collumns = 4;
                spkt.Rounds = 10;
                spkt.Direction = 2;
                this.Send((byte[])spkt);
                this.serverKey = tempServerKey;
            }
            catch (Exception ex)
            {
                Trace.TraceError("An unhandled exception occured {0}", ex.Message);
                this.Close();
            }
        }

        private void OnKey(CMSG_SENDKEY cpkt)
        {
            Trace.TraceInformation("Key Recieved from {0}", this.socket.RemoteEndPoint);
            try
            {
                this.clientKey = cpkt.Key;
                SMSG_GUID spkt = new SMSG_GUID();
                spkt.Key = Program.CrcKey;
                this.Send((byte[])spkt);
            }
            catch (Exception ex)
            {
                Trace.TraceError("An unhandled exception occured {0}", ex.Message);
                this.Close();
            }
        }

        private void OnGUID(CMSG_GUID cpkt)
        {
            Trace.TraceInformation("GUID Recieved from {0}", this.socket.RemoteEndPoint);
            try
            {
                string key = cpkt.Key;
                if (CheckCrc == true && key != Program.GuidKey)
                {
                    IPEndPoint endpoint = (IPEndPoint)this.socket.RemoteEndPoint;
                    Trace.TraceError(String.Format("GUID Mismatch on ip {1} key: {0} ", key, endpoint.Address));
                    this.Close();
                    return;
                }
                else
                {
                    SMSG_IDENTIFY spkt2 = new SMSG_IDENTIFY();
                    this.Send((byte[])spkt2);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("An unhandled exception occured {0}", ex.Message);
                this.Close();
            }
        }

        private void OnKey2()
        {
            Trace.TraceInformation("Key2 received from: {0}", this.socket.RemoteEndPoint);
        }

        /// <summary>
        /// This packet indicates the client requests for a new
        /// session. First we'll set the session to 0 so we can
        /// set the session with the next packet we send.
        ///
        /// If all goes well, the next following code should be
        /// internal: SetSessionId() invoked by the SessionPool.
        /// </summary>
        private void OnIdentify()
        {
            Trace.TraceInformation("Identify Recieved from {0}", this.socket.RemoteEndPoint);
            try
            {
                SMSG_UKNOWN spkt = new SMSG_UKNOWN();
                spkt.Result = 0;
                this.Send((byte[])spkt);

                //Invoke the session pool for an new request
                if (!SessionPool.Instance.Request(this))
                {
                    Trace.TraceInformation("Unable to request an new session for {0} - Reason {1}", this.socket.RemoteEndPoint, NetworkManager.LastError);
                    this.Close();
                }
            }
            catch (SessionRequestException ex)
            {
                Trace.TraceInformation("An unhandled exception occured has raised during session request {0} {1}", ex, ex.InnerException);
                this.Close();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("An unhandled exception occured {0}", ex.Message);
                this.Close();
            }
        }

        #endregion Packet Handeling

        #region Internal Methods

        internal void SetSessionId(uint session)
        {
            try
            {
                Trace.TraceInformation("Set session-id {1} received from: {0}", this.socket.RemoteEndPoint, session);
                if (session > 0)
                {
                    //Update session, add session to lookup table
                    this.session = session;
                    GatewayPool.Instance.lookup.Add(session, this);

                    //Exchange ip adress
                    LoginClient client;
                    if (NetworkManager.TryGetLoginClient(out client))
                    {
                        IPEndPoint endpoint = (IPEndPoint)this.socket.RemoteEndPoint;
                        SMSG_NETWORKADRESSIP spkt = new SMSG_NETWORKADRESSIP();
                        spkt.ConnectionFrom = endpoint.Address;
                        spkt.SessionId = session;
                        client.Send((byte[])spkt);
                    }

                    //Forward to out client
                    SMSG_UKNOWN2 spkt2 = new SMSG_UKNOWN2();
                    spkt2.SessionId = this.session;
                    this.Send((byte[])spkt2);
                }
                else
                {
                    Trace.TraceError("Session with value 0 was given");
                }
            }
            catch (ObjectDisposedException)
            {
                //Closing connection
                this.Close();
            }
            catch (System.Net.Sockets.SocketException)
            {
                //Closing connection
                this.Close();
            }
        }

        private void RedirectLogin(byte[] body)
        {
            try
            {
                LoginClient client;
                if (NetworkManager.TryGetLoginClient(out client))
                {
                    client.Send(body);
                }
                else
                {
                    //Output a error
                    byte[] buffer2 = new byte[] { 0x0B, 0x00, 0x74, 0x17, 0x91, 0x00, 0x02, 0x07, 0x00, 0x00, 0x01 };
                    Array.Copy(BitConverter.GetBytes(session), 0, buffer2, 2, 4);
                    this.Send(buffer2);
                }
            }
            catch (ObjectDisposedException ex)
            {
                Trace.TraceError("Connection with the auth server has been lost, packet are discared");
            }
            catch (Exception)
            {
                Trace.TraceError("Connection with the auth server has been lost, packet are discared");
            }
        }

        private void RedirectMap(byte[] body)
        {
            try
            {
                if (world != null)
                {
                    world.Send(body);
                }
                else
                {
                    Console.WriteLine("world is null");
                }
            }
            catch (ObjectDisposedException)
            {
                Trace.TraceError("Connection with the world server has been lost, packet are discared");
                world = null;
                this.Close();
            }
            catch (Exception)
            {
                Trace.TraceError("Connection with the world server has been lost, packet are discared");
                this.Close();
            }
        }

        #endregion Internal Methods
    }
}