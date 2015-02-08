using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to request new love comitment.
    /// </remarks>
    /// <id>
    /// 031D
    /// </id>
    internal class SMSG_REQUESTSHOWLOVE : RelayPacket
    {
        public SMSG_REQUESTSHOWLOVE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x031D;
            this.data = new byte[4];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }
    }
}