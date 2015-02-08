using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This shows you a list of events to which you can participate.
    /// After this the packet 0x061E is sent (EVENTINFO2) in the event
    /// that is currently visible on kro2.
    /// </remarks>
    /// <id>
    /// 061B
    /// </id>
    internal class SMSG_EVENTINFO : RelayPacket
    {
        public SMSG_EVENTINFO()
        {
            this.Cmd = 0x0601;
            this.Id = 0x061B;
            this.data = new byte[21];
        }

        private int next = 1;

        public void Add(byte id, bool participated)
        {
            this.data[next] = id;
            this.data[next + 1] = (byte)(participated == true ? 1 : 0);
            this.data[0]++;
            next += 2;
        }
    }
}