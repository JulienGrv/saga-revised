using Saga.Network.Packets;
using System;

namespace Saga.Authentication.Packets
{
    public class SMSG_WORLDINSTANCEACK : RelayPacket
    {
        public SMSG_WORLDINSTANCEACK()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0001;
            this.data = new byte[17];
        }

        /// <summary>
        /// Result why connection wasn't established
        /// </summary>
        /// <remarks>
        /// 0   -   OK
        /// 1   -   Invalid proof
        /// 2   -   Server already online
        /// </remarks>
        public byte Result
        {
            set
            {
                this.data[0] = value;
            }
        }

        public byte[] NextKey
        {
            set
            {
                Array.Copy(value, 0, this.data, 1, 16);
            }
        }
    }
}