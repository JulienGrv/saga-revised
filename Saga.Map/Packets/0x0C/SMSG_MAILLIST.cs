using Saga.Map.Definitions.Misc;
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
    /// This packet is sent over all the items in the inbox tab.
    /// </remarks>
    /// <id>
    /// 0C07
    /// </id>
    internal class SMSG_MAILLIST : RelayPacket
    {
        public SMSG_MAILLIST()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0C01;
            this.data = new byte[5];
        }

        public byte Count
        {
            set { this.data[0] = value; }
            get { return this.data[0]; }
        }

        public uint SourceActor
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4); }
        }

        public void AddMail(Mail item)
        {
            //HELPERS
            int attachments = 0;
            int pos = this.data.Length;
            Array.Resize(ref this.data, this.data.Length + 131);

            string timestamp = String.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2} {3}:{4}:{5}", item.Time.Day, item.Time.Month, item.Time.Year, item.Time.Hour, item.Time.Minute, item.Time.Second);

            if (item.Zeny > 0)
            {
                attachments |= 1;
            }
            if (item.Attachment != null)
            {
                attachments |= 2;
                Array.Copy(BitConverter.GetBytes(item.Attachment.info.item), 0, this.data, pos + 5, 4);
                this.data[pos + 9] = (byte)item.Attachment.count;
            }

            Array.Copy(BitConverter.GetBytes(item.MailId), 0, this.data, pos, 4);
            this.data[pos + 4] = (byte)attachments;
            Encoding.Unicode.GetBytes(item.Sender, 0, Math.Min(item.Sender.Length, 16), this.data, pos + 13);
            Encoding.Unicode.GetBytes(timestamp, 0, Math.Min(timestamp.Length, 19), this.data, pos + 45);
            this.data[pos + 87] = item.Valid;
            Encoding.Unicode.GetBytes(item.Topic, 0, Math.Min(item.Topic.Length, 19), this.data, pos + 88);
            this.data[pos + 130] = item.IsRead;
            this.Count++;
        }
    }
}