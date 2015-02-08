using Saga.Network.Packets;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// A wide movement used by ships and trains.
    /// </remarks>
    /// <id>
    /// 0319
    /// </id>
    internal class SMSG_WIDEMOVEMENTSTART : RelayPacket
    {
        public SMSG_WIDEMOVEMENTSTART()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0319;
            this.data = new byte[8];
            this.data[6] = 4;
        }

        public uint SourceActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public ushort Speed
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 2); }
        }

        public void AddWaypoint(Point x, Rotator yaw)
        {
            int offset = this.data.Length;
            Array.Resize<byte>(ref this.data, offset + 16);
            FloatToArray(x.x, this.data, offset);
            FloatToArray(x.y, this.data, offset + 4);
            FloatToArray(x.z, this.data, offset + 8);
            Array.Copy(BitConverter.GetBytes(yaw.rotation), 0, this.data, offset + 12, 2);
            Array.Copy(BitConverter.GetBytes(yaw.unknown), 0, this.data, offset + 14, 2);
            this.data[7]++;
        }
    }
}