using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// No information yet. To be believed to do something related to the old guard functionallity.
    /// Which will show you some 'hot' places or provide information about them.
    /// </remarks>
    /// <id>
    /// 0606
    /// </id>
    internal class SMSG_NPCASKLOCATION : RelayPacket
    {
        public SMSG_NPCASKLOCATION()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0606;
            this.data = new byte[4];
        }

        public uint Script
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}