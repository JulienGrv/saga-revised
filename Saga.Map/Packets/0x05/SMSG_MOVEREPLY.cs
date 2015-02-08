using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent as a reply to indicate the movement is sucessfull
    /// or has some errors. The client after the request to move the item will
    /// block all other packets untill he/she has attained this packet.
    /// </remarks>
    /// <id>
    /// 0501
    /// </id>
    internal class SMSG_MOVEREPLY : RelayPacket
    {
        public SMSG_MOVEREPLY()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0501;
            this.data = new byte[3];
        }

        public byte Index
        {
            set { this.data[0] = value; }
        }

        public byte MovementType
        {
            set { this.data[1] = value; }
        }

        public byte Message
        {
            set { this.data[2] = value; }
        }
    }
}