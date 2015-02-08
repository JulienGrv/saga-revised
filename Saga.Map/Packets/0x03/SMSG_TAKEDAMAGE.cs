using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This function inidactes you have taken damage. This can either be
    /// because you just felt. Or because you suffer from lack of ogygen.
    /// </remarks>
    /// <id>
    /// 031F
    /// </id>
    internal class SMSG_TAKEDAMAGE : RelayPacket
    {
        public SMSG_TAKEDAMAGE()
        {
            this.Cmd = 0x0601;
            this.Id = 0x031F;
            this.data = new byte[5];
        }

        public byte Reason
        {
            set { this.data[0] = value; }
        }

        public uint Damage
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }
    }
}