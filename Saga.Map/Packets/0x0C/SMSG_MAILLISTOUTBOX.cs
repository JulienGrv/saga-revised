using Saga.Network.Packets;
using System;
using System.Globalization;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent over all the items in the outbox tab.
    /// </remarks>
    /// <id>
    /// 0C07
    /// </id>
    internal class SMSG_MAILLISTOUTBOX : RelayPacket
    {
        public SMSG_MAILLISTOUTBOX()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C07;
            this.data = new byte[5];
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public void AddMail(uint ID, string Sender, string topic, byte valid, DateTime date, uint item, byte stack)
        {
            int _age = GetDaysBetweenDates(DateTime.Now, date);
            int pos = this.data.Length;
            string timestamp = String.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2} {3}:{4}:{5}", date.Day, date.Month, date.Year, date.Hour, date.Minute, date.Second);
            Array.Resize(ref this.data, this.data.Length + 130);

            Array.Copy(BitConverter.GetBytes(ID), 0, this.data, pos, 4);
            if (item > 0) this.data[pos + 4] = 2;
            Array.Copy(BitConverter.GetBytes(item), 0, this.data, pos + 5, 4);
            this.data[pos + 9] = 0;
            Encoding.Unicode.GetBytes(Sender, 0, Math.Min(Sender.Length, 16), this.data, pos + 9);
            Encoding.Unicode.GetBytes(timestamp, 0, Math.Min(timestamp.Length, 19), this.data, pos + 41);
            Encoding.Unicode.GetBytes(topic, 0, Math.Min(topic.Length, 19), this.data, pos + 84);
            this.data[83] = (byte)_age;
            this.data[0]++;
        }

        private int GetDaysBetweenDates(DateTime firstDate, DateTime secondDate)
        {
            return secondDate.Subtract(firstDate).Days;
        }
    }
}