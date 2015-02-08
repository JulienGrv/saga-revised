using Saga.Gateway.Network;
using System;
using System.Collections.Generic;

namespace Saga.Gateway
{
    internal class SessionPool
    {
        #region SessionPool Singleton

        private static SessionPool instance;

        public static SessionPool Instance
        {
            get
            {
                if (instance == null) instance = new SessionPool();
                return instance;
            }
        }

        #endregion SessionPool Singleton

        #region SessionPool Public Members

        public Queue<GatewayClient> PendingSessionQueuee = new Queue<GatewayClient>();
        public Queue<uint> Sessions = new Queue<uint>();

        #endregion SessionPool Public Members

        #region SessionPool Public Methods

        public void Add(uint session)
        {
            if (PendingSessionQueuee.Count > 0)
            {
                GatewayClient client = PendingSessionQueuee.Dequeue();
                if (client != null)
                    client.SetSessionId(session);
            }
            else
            {
                Sessions.Enqueue(session);
            }
        }

        public bool Request(GatewayClient client)
        {
            try
            {
                if (Sessions.Count == 0)
                {
                    this.PendingSessionQueuee.Enqueue(client);
                    LoginClient clienta;
                    if (NetworkManager.TryGetLoginClient(out clienta))
                    {
                        clienta.RequestSessionId();
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    uint session = Sessions.Dequeue();
                    client.SetSessionId(session);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new SessionRequestException("Requesting the session id has failed - unknown exception occured", ex);
            }
        }

        #endregion SessionPool Public Methods
    }

    internal class SessionRequestException : Exception
    {
        public SessionRequestException(string message)
            : base(message)
        {
        }

        public SessionRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}