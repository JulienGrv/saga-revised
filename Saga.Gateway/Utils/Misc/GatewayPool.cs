using Saga.Gateway.Network;
using System.Collections.Generic;

namespace Saga.Gateway
{
    internal class GatewayPool
    {
        #region GatewayPool Singleton

        private static GatewayPool instance;

        public static GatewayPool Instance
        {
            get
            {
                if (instance == null) instance = new GatewayPool();
                return instance;
            }
        }

        #endregion GatewayPool Singleton

        #region GatewayPool Public Members

        public Dictionary<uint, GatewayClient> lookup = new Dictionary<uint, GatewayClient>();

        #endregion GatewayPool Public Members

        #region Constructor / Deconstructor

        ~GatewayPool()
        {
        }

        private GatewayPool()
        {
        }

        #endregion Constructor / Deconstructor
    }
}