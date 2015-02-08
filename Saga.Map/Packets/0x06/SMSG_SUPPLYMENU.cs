using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This shows a sublist of a trade-menu. You can choose which trade you want to see
    /// and the list of required items is shown.
    /// </remarks>
    /// <id>
    /// 0616
    /// </id>
    internal class SMSG_SUPPLYMENU : RelayPacket
    {
        public SMSG_SUPPLYMENU()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0616;
            this.data = new byte[8];
        }

        public uint MenuId
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }
    }
}