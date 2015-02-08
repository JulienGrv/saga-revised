using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Authentication.Packets
{
    public class SMSG_LOGINAWNSER : RelayPacket
    {
        public SMSG_LOGINAWNSER()
        {
            this.data = new byte[39];
            this.Cmd = 0x0401;
            this.Id = 0x0101;
        }

        public byte Gender
        {
            set
            {
                this.data[0] = value;
            }
        }

        public byte MaxChars
        {
            set
            {
                this.data[1] = value;
            }
        }

        public DateTime LastLogin
        {
            set
            {
                string s = value.ToShortDateString();
                Encoding.Unicode.GetBytes(s, 0, Math.Min(16, s.Length), this.data, 2);
            }
        }

        public LoginError LoginError
        {
            set
            {
                this.data[36] = (byte)value;
            }
        }

        public byte Unknown
        {
            set
            {
                this.data[37] = (byte)value;
            }
        }

        public byte Advertisment
        {
            set
            {
                this.data[38] = (byte)value;
            }
        }
    }
}