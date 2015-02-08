using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Thia packet is sent to player as a result of a personal message whisper.
    /// </remarks>
    /// <id>
    /// 0402
    /// </id>
    internal class SMSG_SENDWHISPER : RelayPacket
    {
        public SMSG_SENDWHISPER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0402;
            this.data = new byte[36];
        }

        public string Name
        {
            set
            {
                Encoding.Unicode.GetBytes(value, 0, Math.Min(16, value.Length), this.data, 0);
            }
            get
            {
                return Encoding.Unicode.GetString(this.data, 0, 34);
            }
        }

        public byte Result
        {
            get
            {
                return this.data[34];
            }
            set
            {
                this.data[34] = (byte)value;
            }
        }

        public string Message
        {
            get
            {
                return Encoding.Unicode.GetString(this.data, 36, this.data[35]);
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