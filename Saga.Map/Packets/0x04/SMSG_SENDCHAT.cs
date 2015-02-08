using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Thia packet is sent by the player to indicate he/she is changing
    /// her state. For example when the player switches from sitting to
    /// lying position.
    /// </remarks>
    /// <id>
    /// 0401
    /// </id>
    internal class SMSG_SENDCHAT : RelayPacket
    {
        public enum MESSAGE_TYPE { NORMAL, PARTY, YELL, SYSTEM_MESSAGE, CHANEL, SYSTEM_MESSAGE_RED };

        public SMSG_SENDCHAT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0401;
            this.data = new byte[36];
        }

        public MESSAGE_TYPE MessageType
        {
            get
            {
                return (MESSAGE_TYPE)this.data[0];
            }
            set
            {
                this.data[0] = (byte)value;
            }
        }

        public byte MessageType2
        {
            get
            {
                return this.data[0];
            }
            set
            {
                this.data[0] = (byte)value;
            }
        }

        public string Name
        {
            set
            {
                Encoding.Unicode.GetBytes(value, 0, Math.Min(16, value.Length), this.data, 1);
            }
        }

        public string Message
        {
            get
            {
                return Encoding.Unicode.GetString(this.data, 2, this.data[1]);
            }
            set
            {
                int length = Math.Min(127, value.Length);
                int byte_count = Encoding.Unicode.GetByteCount(value.ToCharArray(), 0, length);
                byte[] tmp = new byte[36 + byte_count];
                this.data[35] = (byte)byte_count;
                Array.Copy(this.data, 0, tmp, 0, Math.Min(tmp.Length, this.data.Length));
                Encoding.Unicode.GetBytes(value, 0, length, tmp, 36);
                this.data = tmp;
            }
        }
    }
}