using Saga.Shared.PacketLib;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Saga.Shared.NetworkCore
{
    public abstract class EncryptedClient
    {
        public Socket socket;

        public event EventHandler OnClose;

        public byte[] clientKey = new byte[16];
        public byte[] serverKey = new byte[16];

        public EncryptedClient(Socket socket)
        {
            Encryption.StaticKey.CopyTo(this.clientKey, 0);
            Encryption.StaticKey.CopyTo(this.serverKey, 0);
            if (socket.Connected == true)
            {
                this.socket = socket;
                byte[] buffer = new byte[2];
                this.socket.BeginReceive(buffer, 0, 2, SocketFlags.Peek, new AsyncCallback(OnRead), buffer);
            }
        }

        private void OnRead(IAsyncResult asyn)
        {
            try
            {
                #region Basic-buffer check

                int num_bytes = this.socket.EndReceive(asyn);
                if (num_bytes == 2)
                {
                    goto ProcessPacket;
                }
                else
                {
                    this.Close();
                }

                #endregion Basic-buffer check

                return;
            ProcessPacket:

                #region Packet-processing

                SocketError error;
                byte[] length_buffer = new byte[2];
                this.socket.Receive(length_buffer, 0, 2, SocketFlags.Peek, out error);
                ushort packet_length = BitConverter.ToUInt16(length_buffer, 0);
                if (packet_length < 10)
                {
                    Trace.TraceError("Packet was less than 10 bytes. Disconnecting client.");
                    this.Close();
                }
                else
                {
                    byte[] packet = null;
                    bool process = this.socket.Available >= packet_length;
                    if (process)
                    {
                        packet = new byte[packet_length];
                        this.socket.Receive(packet, SocketFlags.None);
                    }

                    byte[] buffer = new byte[2];
                    if (socket.Connected == true) this.socket.BeginReceive(buffer, 0, 2, SocketFlags.Peek, new AsyncCallback(OnRead), buffer);
                    if (process)
                    {
                        WaitCallback callback = delegate(object state)
                        {
                            packet = Encryption.Decrypt(packet, 2, this.clientKey);
                            ProcessPacket(ref packet);
                        };

                        ThreadPool.QueueUserWorkItem(callback);
                    }
                }

                #endregion Packet-processing
            }
            catch (ObjectDisposedException)
            {
                this.Close();
            }
            catch (SocketException)
            {
                this.Close();
            }
            catch (Exception ex)
            {
                this.Close();
                Trace.TraceError(ex.Message);
            }
        }

        public void Close()
        {
            if (socket != null)
            {
                if (socket.Connected == true)
                {
                    Trace.WriteLine(string.Format("Connection closed from: {0}", socket.RemoteEndPoint), "Network");
                }
                if (OnClose != null)
                {
                    OnClose.Invoke(this, null);
                }
                if (socket.Connected == true)
                {
                    this.socket.Close();
                }
            }
            else
            {
                if (OnClose != null)
                {
                    OnClose.Invoke(this, null);
                }
            }
        }

        public void Send(byte[] buffer)
        {
            try
            {
                buffer = Encryption.Encrypt(buffer, 2, this.serverKey);
                this.socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, null);
            }
            catch (ObjectDisposedException)
            {
                this.Close();
            }
        }

        public void Send(ref byte[] buffer)
        {
            try
            {
                byte[] tmpbuffer = Encryption.Encrypt(buffer, 2, this.serverKey);
                this.socket.BeginSend(tmpbuffer, 0, tmpbuffer.Length, SocketFlags.None, null, null);
            }
            catch (ObjectDisposedException)
            {
                this.Close();
            }
        }

        protected abstract void ProcessPacket(ref byte[] body);
    }
}