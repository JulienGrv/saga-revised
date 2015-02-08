using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <remarks>
    /// This deleted a item from your owners list. This packet is mostly send
    /// with SMSG_MARKETMAIL which notify's the user of a new mail. The item which
    /// is deleted is sent over it by mail.
    /// </remarks>
    /// <id>
    /// 1105
    /// </id>
    internal class SMSG_MARKETDELETEITEM : RelayPacket
    {
        public SMSG_MARKETDELETEITEM()
        {
            this.Cmd = 0x0601;
            this.Id = 0x1105;
            this.data = new byte[5];
        }

        public byte Reason
        {
            set
            {
                this.data[0] = value;
            }
        }

        public uint ItemID
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4);
            }
        }
    }
}