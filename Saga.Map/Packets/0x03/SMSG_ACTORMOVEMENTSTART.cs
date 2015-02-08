using Saga.Network.Packets;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This is a test
    ///
    /// Haha.
    /// </remarks>
    /// <id>
    /// 0305
    /// </id>
    internal class SMSG_ACTORMOVEMENTSTART : RelayPacket
    {
        public SMSG_ACTORMOVEMENTSTART()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0305;
            this.data = new byte[32];
            this.data[6] = 0;
            this.data[7] = 2;
        }

        public uint SourceActorID
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public ushort Speed
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 4, 2); }
        }

        public byte Stance
        {
            set { this.data[7] = value; }
        }

        public void Source(Point x)
        {
            FloatToArray(x.x, this.data, 8);
            FloatToArray(x.y, this.data, 12);
            FloatToArray(x.z, this.data, 16);
        }

        public void Destination(Point z)
        {
            FloatToArray(z.x, this.data, 20);
            FloatToArray(z.y, this.data, 24);
            FloatToArray(z.z, this.data, 28);
        }
    }
}