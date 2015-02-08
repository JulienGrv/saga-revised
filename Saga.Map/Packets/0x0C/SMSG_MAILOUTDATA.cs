using Saga.Network.Packets;
using Saga.PrimaryTypes;
using System;
using System.Globalization;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent containing the full data of a mailitem.
    /// </remarks>
    /// <id>
    /// 0C09
    /// </id>
    internal class SMSG_MAILOUTDATA : RelayPacket
    {
        public SMSG_MAILOUTDATA()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C09;
            this.data = new byte[594];
        }

        public byte Unknown
        {
            set { this.data[0] = value; }
        }

        public uint Zeny
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public string Sender
        {
            set { Encoding.Unicode.GetBytes(value, 0, Math.Min(value.Length, 16), this.data, 5); }
        }

        public DateTime Date
        {
            set
            {
                string timestamp = String.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2} {3}:{4}:{5}", value.Day, value.Month, value.Year, value.Hour, value.Minute, value.Second);
                Encoding.Unicode.GetBytes(timestamp, 0, Math.Min(timestamp.Length, 19), this.data, 39);
            }
        }

        public string Topic
        {
            set { Encoding.Unicode.GetBytes(value, 0, Math.Min(value.Length, 20), this.data, 79); }
        }

        public string Content
        {
            set { Encoding.Unicode.GetBytes(value, 0, Math.Min(value.Length, 200), this.data, 121); }
        }

        public Rag2Item item
        {
            set
            {
                if (value != null)
                    Rag2Item.Serialize(value, this.data, 523);
            }
        }
    }
}