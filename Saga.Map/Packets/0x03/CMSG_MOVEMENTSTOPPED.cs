using Saga.Network.Packets;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent as a result of the given player has stopped moving.
    /// </remarks>
    /// <id>
    /// 0303
    /// </id>
    internal class CMSG_MOVEMENTSTOPPED : RelayPacket
    {
        public CMSG_MOVEMENTSTOPPED()
        {
            this.data = new byte[31];
        }

        public float X
        {
            get { return ArrayToFloat(this.data, 0); }
        }

        public float Y
        {
            get { return ArrayToFloat(this.data, 4); }
        }

        public float Z
        {
            get { return ArrayToFloat(this.data, 8); }
        }

        public Rotator Yaw
        {
            get
            {
                return new Rotator(
                    BitConverter.ToUInt16(this.data, 12),
                    BitConverter.ToUInt16(this.data, 14));
            }
        }

        public uint DelayTime
        {
            get { return BitConverter.ToUInt32(this.data, 16); }
        }

        public ushort Speed
        {
            get { return BitConverter.ToUInt16(this.data, 20); }
        }

        #region Conversions

        public static explicit operator CMSG_MOVEMENTSTOPPED(byte[] p)
        {
            /*
            // Creates a new byte with the length of data
            // plus 4. The first size bytes are used like
            // [PacketSize][PacketId][PacketBody]
            //
            // Where Packet Size equals the length of the
            // Packet body, Packet Identifier, Packet Size
            // Container.
            */

            CMSG_MOVEMENTSTOPPED pkt = new CMSG_MOVEMENTSTOPPED();
            pkt.data = new byte[p.Length - 14];
            pkt.session = BitConverter.ToUInt32(p, 2);
            Array.Copy(p, 6, pkt.cmd, 0, 2);
            Array.Copy(p, 12, pkt.id, 0, 2);
            Array.Copy(p, 14, pkt.data, 0, p.Length - 14);
            return pkt;
        }

        #endregion Conversions
    }
}