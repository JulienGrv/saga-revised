using Saga.Network.Packets;
using Saga.Structures;
using System;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is send by the player as result of the player is
    /// moving.
    /// </remarks>
    /// <id>
    /// 0302
    /// </id>
    internal class CMSG_MOVEMENTSTART : RelayPacket
    {
        /// <summary>
        /// Creates a new movement start
        /// </summary>
        public CMSG_MOVEMENTSTART()
        {
            this.data = new byte[31];
        }

        /// <summary>
        /// Get's the x-axis
        /// </summary>
        public float X
        {
            get { return ArrayToFloat(this.data, 0); }
        }

        /// <summary>
        /// Get's the y-axis
        /// </summary>
        public float Y
        {
            get { return ArrayToFloat(this.data, 4); }
        }

        /// <summary>
        /// Get's the z-axis
        /// </summary>
        public float Z
        {
            get { return ArrayToFloat(this.data, 8); }
        }

        /// <summary>
        /// Get's the acceleration of the x axis
        /// </summary>
        public float AccelerationX
        {
            get { return ArrayToFloat(this.data, 12); }
        }

        /// <summary>
        /// Get's the acceleration of the y axis
        /// </summary>
        public float AccelerationY
        {
            get { return ArrayToFloat(this.data, 16); }
        }

        /// <summary>
        /// Get's the acceleration of the z axis
        /// </summary>
        public float AccelerationZ
        {
            get { return ArrayToFloat(this.data, 20); }
        }

        /// <summary>
        /// Unknown
        /// </summary>
        public uint U1
        {
            get { return BitConverter.ToUInt32(this.data, 24); }
        }

        /// <summary>
        /// Unknown
        /// </summary>
        public ushort Temp01
        {
            get { return BitConverter.ToUInt16(this.data, 24); }
        }

        /// <summary>
        /// Unknown
        /// </summary>
        public ushort Temp02
        {
            get { return BitConverter.ToUInt16(this.data, 26); }
        }

        /// <summary>
        /// Get's the current yaw rotator
        /// </summary>
        public Rotator Yaw
        {
            get
            {
                return new Rotator(
                    BitConverter.ToUInt16(this.data, 28),
                    BitConverter.ToUInt16(this.data, 30)
                );
            }
        }

        /// <summary>
        /// Get's the delay time
        /// </summary>
        public uint DelayTime
        {
            get { return BitConverter.ToUInt32(this.data, 32); }
        }

        /// <summary>
        /// Get's the movement type
        /// </summary>
        public byte MovementType
        {
            get { return this.data[36]; }
        }

        /// <summary>
        /// Unknown
        /// </summary>
        public uint U2
        {
            get { return BitConverter.ToUInt32(this.data, 37); }
        }

        #region Conversions

        public static explicit operator CMSG_MOVEMENTSTART(byte[] p)
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

            CMSG_MOVEMENTSTART pkt = new CMSG_MOVEMENTSTART();
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