using Saga.Authentication;

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