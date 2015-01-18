using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Saga.Shared.Definitions;
using Saga.Shared.PacketLib.Map;
using Saga.Authentication;
using Saga.Authentication.Utils;
using Saga.Shared.PacketLib.Login;

namespace Saga.Map.Client
{
    partial class InternalClient
    {
        public void CM_MAINTAINCEENTER()
        {
            ServerInfo2 info;
            if (ServerManager2.Instance.server.TryGetValue(WorldId, out info))
            {
                info.InMaintainceMode = true;
            }
        }

        public void CM_MAINTAINCELEAVE()
        {
            ServerInfo2 info;
            if (ServerManager2.Instance.server.TryGetValue(WorldId, out info))
            {
                info.InMaintainceMode = false;
            }
        }

    }
}
