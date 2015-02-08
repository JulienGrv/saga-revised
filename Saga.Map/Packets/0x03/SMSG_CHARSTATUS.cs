using Saga.Network.Packets;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send to the player to notify himself of his active changes.
    /// These changes can be JEXP, EXP, HP, SP,  the Flag "FieldOfSignt" indicates
    /// what to display.
    ///
    /// 1 = Joblvl up, 2 = Jexp, 16 = CLvl up, 32 = Cexp
    /// </remarks>
    /// <id>
    /// 0308
    /// </id>
    internal class SMSG_CHARSTATUS : RelayPacket
    {
        public SMSG_CHARSTATUS()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0308;
            this.data = new byte[27];
        }

        public uint Exp
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 0, 4); }
        }

        public byte Job
        {
            set { this.data[4] = value; }
            get { return this.data[4]; }
        }

        public uint JobExp
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 5, 4); }
        }

        public ushort HP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 9, 2); }
        }

        public ushort MaxHP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 11, 2); }
        }

        public ushort SP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 13, 2); }
        }

        public ushort MaxSP
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 15, 2); }
        }

        public byte LC
        {
            set { this.data[17] = value; }
            get { return this.data[17]; }
        }

        public byte MaxLC
        {
            set { this.data[18] = value; }
            get { return this.data[18]; }
        }

        public byte LP
        {
            set { this.data[19] = value; }
            get { return this.data[19]; }
        }

        public byte MaxLP
        {
            set { this.data[20] = value; }
            get { return this.data[20]; }
        }

        public ushort FieldOfSight
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 25, 2); }
        }
    }
}