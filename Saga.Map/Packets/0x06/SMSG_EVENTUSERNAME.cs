using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This shows a 'alert' window with either "player %s participated with the event" or "unable to find player."
    /// We believe this is for some event invitation purpose.
    /// </remarks>
    /// <id>
    /// 061C
    /// </id>
    internal class SMSG_EVENTUSERNAME : RelayPacket
    {
        public SMSG_EVENTUSERNAME()
        {
            this.Cmd = 0x0601;
            this.Id = 0x061C;
            this.data = new byte[35];
        }

        public byte Result
        {
            set
            {
                this.data[0] = value;
            }
        }

        public string Name
        {
            set
            {
                Encoding.Unicode.GetBytes(value, 0, Math.Min(value.Length, 16), this.data, 1);
            }
        }
    }
}