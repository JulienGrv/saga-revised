using System;
using System.Collections.Generic;
using System.Text;
using Saga.Gateway.Network;

namespace Saga.Gateway
{
    class GatewayPool
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

        #endregion

        #region GatewayPool Public Members
        public Dictionary<uint, GatewayClient> lookup = new Dictionary<uint, GatewayClient>();
        #endregion

        #region Constructor / Deconstructor

        ~GatewayPool() { }
        private GatewayPool(){ }

        #endregion

    }
}
