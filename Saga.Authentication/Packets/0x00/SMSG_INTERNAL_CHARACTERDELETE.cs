using System;
using System.Collections.Generic;
using System.Text;
using Saga.Network.Packets;

namespace Saga.Shared.PacketLib.Login
{

    public class SMSG_INTERNAL_CHARACTERDELETE : RelayPacket
    {

        public SMSG_INTERNAL_CHARACTERDELETE()
        {
            this.Cmd = 0x0501;
            this.Id = 0x0106;
            this.data = new byte[4];
        }

        public uint CharacterId
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4);
            }
        }

    }
}
