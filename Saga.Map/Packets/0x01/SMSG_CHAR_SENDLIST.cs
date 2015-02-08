using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Shared.PacketLib.Map
{
    internal class SMSG_CHAR_SENDLIST : RelayPacket
    {
        public SMSG_CHAR_SENDLIST()
        {
            this.Cmd = 0x0401;
            this.Id = 0x0104;
            this.data = new byte[37];
        }

        public byte Result
        {
            set
            {
                this.data[0] = value;
            }
        }

        public string ServerName
        {
            set
            {
                Encoding.Unicode.GetBytes(value, 0, Math.Min(17, value.Length), this.data, 1);
            }
        }

        public byte CountAllServer
        {
            set { this.data[35] = value; }
        }

        public byte CountAllSurrentServer
        {
            set { this.data[36] = value; }
            get { return this.data[36]; }
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