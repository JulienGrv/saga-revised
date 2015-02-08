using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Saga.Shared.NetworkCore
{
    public abstract class Client : IDisposable
    {
        public Socket socket;

        public event EventHandler OnClose;

        ~Client()
        {
            Dispose();
        }

        public Client()
        {
        }

        public Client(Socket socket)
        {
            this.socket = socket;
            Connect();
            OnConnect();
        }

        public Client(string host, int port)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(host, port);
            this.socket = sock;
            Connect();
            OnConnect();
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

        public void Connect()
        {
            if (this.socket.Connected == true)
            {
                byte[] buffer = new byte[2];
                this.socket.BeginReceive(buffer, 0, 2, SocketFlags.Peek, new AsyncCallback(OnRead), buffer);
            }
        }

        public void Close()
        {
            if (OnClose != null)
            {
                OnClose.Invoke(this, null);
            }
            if (this.socket.Connected == true)
            {
                Trace.WriteLine(string.Format("Connection closed from: {0}", socket.RemoteEndPoint), "Network");
                this.socket.Close();
            }
        }

        public bool IsConnected
        {
            get
            {
                if (socket != null)
                    return socket.Connected;
                else
                    return false;
            }
        }

        public virtual void Send(byte[] buffer)
        {
            try
            {
                this.socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, null);
            }
            catch (ObjectDisposedException ex)
            {
                this.Close();
            }
        }

        public virtual void Send(ref byte[] buffer)
        {
            try
            {
                this.socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, null);
            }
            catch (ObjectDisposedException ex)
            {
                this.Close();
            }
        }

        protected virtual void OnConnect()
        {
        }

        protected abstract void ProcessPacket(ref byte[] body);

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                GC.SuppressFinalize(this);
                if (this.socket != null && this.socket.Connected == true)
                    this.socket.Disconnect(false);
            }
            catch (ObjectDisposedException)
            {
                this.socket = null;
                this.OnClose = null;
            }
        }

        #endregion IDisposable Members
    }
}