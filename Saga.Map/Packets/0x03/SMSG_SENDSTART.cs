using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is used to send to players to indicate they are transfering servers/maps.
    /// For the client it indicates to release old resources like item actors, waypoints.
    ///
    /// The server could probally make use for this tranfer the players data to another sharded
    /// zone server in the future.
    /// </remarks>
    /// <id>
    /// 0301
    /// </id>
    internal class SMSG_SENDSTART : RelayPacket
    {
        public SMSG_SENDSTART()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0301;
            this.data = new byte[18];
        }

        public byte Unknown
        {
            set { this.data[0] = value; }
        }

        public byte MapId
        {
            set { this.data[4] = value; }
        }

        public byte Channel
        {
            set { this.data[5] = value; }
        }

        public float X
        {
            set { FloatToArray(value, this.data, 6); }
        }

        public float Y
        {
            set { FloatToArray(value, this.data, 10); }
        }

        public float Z
        {
            set { FloatToArray(value, this.data, 14); }
        }
    }
}