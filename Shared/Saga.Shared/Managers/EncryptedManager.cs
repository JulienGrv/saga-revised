using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Saga.Shared.NetworkCore
{
    public class EncryptedManager<T> where T : EncryptedClient
    {
        private readonly TcpListener listner;
        private bool AmShuttingDown = false;
        private Thread thread;

        public EncryptedManager(string ip, int port)
        {
            this.listner = new TcpListener(IPAddress.Parse(ip), port);
            this.listner.Start();
            thread = new Thread(new ThreadStart(Loop));
        }

        public void Start()
        {
            if (thread.IsAlive == false)
                thread.Start();
        }

        private void Loop()
        {
            this.AmShuttingDown = false;
            while (AmShuttingDown == false)
            {
                for (int i = 0; listner.Pending() && i < 25; i++)
                {
                    Accept();
                }
                System.Threading.Thread.Sleep(1);
            }
            this.listner.Stop();
        }

        public void Stop()
        {
            this.AmShuttingDown = true;
        }

        private T Accept()
        {
            Socket socket = this.listner.AcceptSocket();
            Type t = typeof(T);
            ConstructorInfo cinfo = t.GetConstructor(new Type[] { typeof(Socket) });
            T myClassObj = (T)cinfo.Invoke(new Object[] { socket });

            Trace.WriteLine(string.Format("Connection accepted from: {0}", socket.RemoteEndPoint), "Network");
            return myClassObj;
        }
    }
}