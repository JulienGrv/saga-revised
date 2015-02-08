using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This shows a message in the system chat of "you've succesfully participated with the event".
    /// This packet is most likely to be used in conjunction with the SMSG_USERNAME.
    /// </remarks>
    /// <id>
    /// 061D
    /// </id>
    internal class SMSG_EVENTSUCCESS : RelayPacket
    {
        public SMSG_EVENTSUCCESS()
        {
            this.Cmd = 0x0601;
            this.Id = 0x061D;
            this.data = new byte[0];
        }
    }
}