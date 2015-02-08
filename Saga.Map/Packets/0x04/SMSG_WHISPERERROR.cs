using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Thia packet is sent to player as a result of a personal message whisper.
    ///
    /// Results:
    /// 1 - Unable to find whisper target
    /// 2 - Target does not accept whispers
    /// 3 - Can't send a PM to somebody in your blacklist
    /// </remarks>
    /// <id>
    /// 0403
    /// </id>
    internal class SMSG_WHISPERERROR : RelayPacket
    {
        public SMSG_WHISPERERROR()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0403;
            this.data = new byte[1];
        }

        public byte Result
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
    }
}