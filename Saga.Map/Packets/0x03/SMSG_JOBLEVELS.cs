using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This is a test
    ///
    /// Haha.
    /// </remarks>
    /// <id>
    /// 0321
    /// </id>
    internal class SMSG_JOBLEVELS : RelayPacket
    {
        public SMSG_JOBLEVELS()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0321;
            this.data = new byte[11];
        }

        public byte[] jobslevels
        {
            set
            {
                for (byte i = 0; i < 11; i++)
                    this.data[i] = value[i];
            }
        }
    }
}