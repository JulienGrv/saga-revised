using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet updates the current time/weather. This packet should be send for
    /// on maploading and when the weather changes.
    /// </remarks>
    /// <id>
    /// 0313
    /// </id>
    internal class SMSG_TIMEWEATHER : RelayPacket
    {
        public SMSG_TIMEWEATHER()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0313;
            this.data = new byte[4];
        }

        public byte day
        {
            set { this.data[0] = value; }
        }

        public byte hour
        {
            set { this.data[1] = value; }
        }

        public byte min
        {
            set { this.data[2] = value; }
        }

        public byte weather
        {
            set { this.data[3] = value; }
        }
    }
}