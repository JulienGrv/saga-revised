using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Shared.PacketLib.Map
{
    internal class SMSG_FINDCHARACTERS : RelayPacket
    {
        public SMSG_FINDCHARACTERS()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0002;
            this.data = new byte[1];
        }

        public byte CountAllSurrentServer
        {
            set { this.data[0] = value; }
            get { return this.data[0]; }
        }

        public void AddChar(uint charID, string charName, byte charRace, uint cExp, byte job, byte pendingDeletion, byte map)
        {
            int offset = this.data.Length;
            Array.Resize<byte>(ref this.data, this.data.Length + 46);
            CountAllSurrentServer++;

            Array.Copy(BitConverter.GetBytes(charID), 0, this.data, offset, 4);
            UnicodeEncoding.Unicode.GetBytes(charName, 0, Math.Min(16, charName.Length), this.data, offset + 4);

            Array.Copy(BitConverter.GetBytes(cExp), 0, this.data, offset + 39, 4);  //CEXP
            this.data[offset + 43] = job; //JOB
            this.data[offset + 44] = pendingDeletion; //ACTIVE
            this.data[offset + 45] = map; //ZONE
        }
    }
}