using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This function add a specified addition to the actor. Once duration reaches
    /// 0 it does not automaticly delete it waits till you send a packet to delete
    /// the specified addition.
    /// </remarks>
    /// <id>
    /// 051C
    /// </id>
    internal class SMSG_ADDITIONBEGIN : RelayPacket
    {
        public SMSG_ADDITIONBEGIN()
        {
            this.Cmd = 0x0601;
            this.Id = 0x051C;
            this.data = new byte[12];
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public uint StatusID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 4); }
        }

        public uint Duration
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 8, 4); }
        }
    }
}