using Saga.Network.Packets;
using System;
using System.Collections.Generic;

namespace Saga.Packets
{
    internal class SMSG_SENDQUESTLIST : RelayPacket
    {
        public SMSG_SENDQUESTLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0702;
            this.data = new byte[5];
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public void SetQuests(uint[] ID)
        {
            byte[] tmp = new byte[this.data.Length + (ID.Length * 4)];
            this.data.CopyTo(tmp, 0);
            this.data = tmp;
            this.data[0] = (byte)ID.Length;

            for (int i = 0; i < ID.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(ID[i]), 0, this.data, 5 + (i * 4), 4);
            }
        }

        public void SetQuests(IEnumerable<uint> ID)
        {
            int i = 0;
            int a = 0;
            foreach (uint c in ID)
            {
                Array.Resize<byte>(ref this.data, this.data.Length + 4);
                Array.Copy(BitConverter.GetBytes(c), 0, this.data, 5 + i, 4);
                i += 4;
                a += 1;
            }
            this.data[0] = (byte)a;
        }
    }
}