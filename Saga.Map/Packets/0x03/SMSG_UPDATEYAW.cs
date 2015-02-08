using Saga.Network.Packets;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet updates the yaw of your character. This packet is used
    /// to notify the surrounding people you have changed your facing
    /// direction.
    /// </remarks>
    /// <id>
    /// 0307
    /// </id>
    internal class SMSG_UPDATEYAW : RelayPacket
    {
        public SMSG_UPDATEYAW()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0307;
            this.data = new byte[8];
        }

        public uint ActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public Rotator Yaw
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value.rotation), 0, this.data, 4, 2);
                Array.Copy(BitConverter.GetBytes(value.unknown), 0, this.data, 6, 2);
            }
        }
    }
}