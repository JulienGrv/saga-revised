using Saga.Network.Packets;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is used to teleport a user to another set of
    /// x,y,z coords on the same map. This will result in the client
    /// NOT to release all resources but to reuse them.
    /// </remarks>
    /// <id>
    /// 0315
    /// </id>
    internal class SMSG_ACTORTELEPORT : RelayPacket
    {
        public SMSG_ACTORTELEPORT()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0315;
            this.data = new byte[14];
            this.data[0] = 0;
            this.data[1] = 1;
        }

        public float x
        {
            set { FloatToArray(value, this.data, 2); }
        }

        public float y
        {
            set { FloatToArray(value, this.data, 6); }
        }

        public float Z
        {
            set { FloatToArray(value, this.data, 10); }
        }
    }
}