using Saga.Authentication;
using Saga.Packets;
using Saga.Shared.PacketLib.Login;
using Saga.Shared.PacketLib.Map;
using System;
using System.Threading;

namespace Saga.Map.Client
{
    public partial class InternalClient : Saga.Shared.NetworkCore.Client
    {
        #region Constructor / Deconstructor

        public InternalClient()
        {
            OnClose += new EventHandler(InternalClient_OnClose);

            WaitCallback callback = delegate(object obj)
            {
                int LastTick = Environment.TickCount;
                bool Timeout = false;

                //Forces to receive the header in 10 secconds if not kill the connection.
                while (WorldId == 0)
                {
                    if (Environment.TickCount - LastTick > 10000)
                    {
                        Timeout = true;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(0);
                    }
                }

                if (Timeout == true)
                {
                    Console.WriteLine("Connection did not authenticate himself within 10 secconds. Closing connection.");
                    this.Close();
                }
            };

            ThreadPool.QueueUserWorkItem(callback);
        }

        #endregion Constructor / Deconstructor

        #region Events

        private void InternalClient_OnClose(object sender, EventArgs e)
        {
            ServerInfo2 info;
            if (ServerManager2.Instance.server.TryGetValue(WorldId, out info))
            {
                info.client = null;
                info.IP = null;
                info.MaxPlayers = 0;
                info.Players = 0;
            }
        }

        #endregion Events

        protected override void ProcessPacket(ref byte[] body)
        {
            ushort subpacketIdentifier = (ushort)(body[13] + (body[12] << 8));
            switch (subpacketIdentifier)
            {
                case 0x0001: CM_WORLDINSTANCE((CMSG_WORLDINSTANCE)body); return;
                case 0x000A: CM_PONG((CMSG_PONG)body); return;
                case 0x000B: CM_RELEASESESSION((CMSG_RELEASESESSION)body); return;
                case 0x000E: CM_MAINTAINCELEAVE(); return;
                case 0x000F: CM_MAINTAINCEENTER(); return;
                case 0x0002: CM_SELECT_CHARACTERS((Saga.Shared.PacketLib.Login.CMSG_FINDCHARACTERS)body); return;
                case 0x0003: CM_SELECT_CHARACTER((Saga.Shared.PacketLib.Login.CMSG_FINDCHARACTERDETAILS)body); return;
                case 0x0103: CM_CHARACTER_CREATE((CMSG_INTERNAL_CHARCREATIONREPLY)body); return;
                case 0x0106: CM_DELETE_CHARACTER((CMSG_INTERNAL_DELETIONREPLY)body); return;
                case 0xFF02: CM_CHARACTERLOGINREPLY((CMSG_CHARACTERLOGINREPLY)body); return;
                //case 0x0105: CHARACTER_CREATE(body); return;
            }
            return;
        }
    }
}