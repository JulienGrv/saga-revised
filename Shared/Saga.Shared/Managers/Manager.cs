using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Saga.Shared.NetworkCore
{
    public class Manager<T> where T : Client, new()
    {
        private readonly TcpListener listner;
        private bool AmShuttingDown = false;
        private Thread thread;

        public Manager(string ip, int port)
        {
            this.listner = new TcpListener(IPAddress.Parse(ip), port);
            thread = new Thread(new ThreadStart(Loop));
        }

        public void Start()
        {
            if (thread.IsAlive == false)
            {
                this.listner.Start();
                this.AmShuttingDown = false;
                thread.Start();
            }
        }

        private void Loop()
        {
            while (AmShuttingDown == false || Environment.HasShutdownStarted == true)
            {
                for (int i = 0; this.listner.Pending() && i < 25; i++)
                {
                    try
                    {
                        Accept();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
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
            T obj = new T();
            obj.socket = socket;
            obj.Connect();

            Trace.WriteLine(string.Format("Connection accepted from: {0}", socket.RemoteEndPoint), "Network");
            return obj;
        }
    }

    public class Manager2<T> where T : Client, new()
    {
        private readonly TcpListener listner;
        private bool AmShuttingDown = false;

        public Manager2(string ip, int port)
        {
            this.listner = new TcpListener(IPAddress.Parse(ip), port);
        }

        public void Start()
        {
            this.listner.Start();
            this.AmShuttingDown = false;
            while (AmShuttingDown == false)
            {
                for (int i = 0; listner.Pending() && i < 25; i++)
                {
                    try
                    {
                        Accept();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
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
            T obj = new T();
            obj.socket = socket;
            obj.Connect();
            Console.WriteLine("Connection accepted from: {0}", socket.RemoteEndPoint);
            return obj;
        }
    }
}