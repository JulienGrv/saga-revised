using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    /// Sends a list of skillbooks
    /// </summary>
    /// <remarks>
    /// The list is not limited to showing just skillbooks. It can show
    /// all kinds of items they will be shown in catagories of users
    /// who can use them.
    /// </remarks>
    /// <id>
    /// 091B
    /// </id>
    internal class SMSG_SENDBOOKLIST : RelayPacket
    {
        private ushort count;

        public SMSG_SENDBOOKLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x091B;
            this.data = new byte[1030];
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 6, 4); }
        }

        public uint Zeny
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 2, 4); }
        }

        public void Add(uint item)
        {
            Array.Copy(BitConverter.GetBytes(item), 0, this.data, 10 + (count * 4), 4);
            Array.Copy(BitConverter.GetBytes(++count), 0, this.data, 0, 2);
        }
    }
}