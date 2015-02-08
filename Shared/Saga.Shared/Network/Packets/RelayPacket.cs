using System;
using System.Text;

namespace Saga.Network.Packets
{
    public class RelayPacket
    {
        protected byte[] data;
        protected byte[] cmd = new byte[2];
        protected byte[] id = new byte[2];
        protected uint session = 0;

        #region Init

        public RelayPacket()
        {
            /*
            // Creates a new packet with no data,
            // no identifier set.
            */

            this.data = new byte[0];
        }

        public RelayPacket(byte[] data)
        {
            /*
            // Parses raw-content to a packet-body
            // and we'll assume the id are set by
            // a inherited class or network manager.
            */
            this.data = data;
        }

        public RelayPacket(byte[] data, PacketContains flags)
        {
            /*
            // Parses raw-content to a packet-body
            // and we'll assume the id are set by
            // a inherited class or network manager.
            //
            // However this time the raw-data will have some
            // flags. That says the raw data contains an size
            // along with some other stuff
            */

            //if (flags == PacketContains.HasId)
            //{
            //    this.data = new byte[data.Length - 2];

            //    Array.Copy(data, 0, this.id, 0, 2);
            //    Array.Copy(data, 2, this.data, 0, data.Length - 2);
            //}
            //else
            //{
            //    this.data = new byte[data.Length - 4];
            //    Array.Copy(data, 2, this.id, 0, 2);
            //    Array.Copy(data, 4, this.data, 0, data.Length - 4);
            //}
        }

        #endregion Init

        #region Regulair Fields

        // TODO:
        // - Add missing stuff

        public ushort Size
        {
            get
            {
                return (ushort)(data.Length + 14);
            }
        }

        public uint Id
        {
            get
            {
                return (ushort)(id[1] + (id[0] << 8));
            }
            set
            {
                byte[] idbytes = BitConverter.GetBytes(value);
                id[0] = idbytes[1];
                id[1] = idbytes[0];
            }
        }

        public uint Cmd
        {
            get
            {
                return (ushort)(cmd[1] + (cmd[0] << 8));
            }
            set
            {
                byte[] idbytes = BitConverter.GetBytes(value);
                cmd[0] = idbytes[1];
                cmd[1] = idbytes[0];
            }
        }

        public uint SessionId
        {
            get
            {
                return this.session;
            }
            set
            {
                this.session = value;
            }
        }

        #endregion Regulair Fields

        #region Special Functions

        public float ArrayToFloat(byte[] array, int index)
        {
            return (float)((float)(BitConverter.ToInt32(array, index)) * 1.0e-3);
        }

        public void FloatToArray(float value, byte[] array, int index)
        {
            value *= 1.0e3f;
            BitConverter.GetBytes((uint)value).CopyTo(array, index);
        }

        #endregion Special Functions

        #region Conversions

        // TODO:
        // - Add a type casting for implecit: packet to byte[].
        // - Add a type casting for explicit: byte[] to packet.
        // - Add a type casting for implecit: byte[] to packet.

        public override string ToString()
        {
            /*
            // Processes all of the packets packet body
            // thus without any identifiers and size. And
            // convert it to hexidecimal format.
            //
            // Where each pair of two digits/characters format
            // an byte value e.d. 00 <-> 0 ~ FF <-> 255
            */

            StringBuilder a = new StringBuilder();
            foreach (byte b in data)
            {
                a.AppendFormat("{0:X2}", b);
            }
            return a.ToString();
        }

        public string ToHex()
        {
            StringBuilder a = new StringBuilder();
            foreach (byte b in data)
            {
                a.AppendFormat("{0:X2} ", b);
            }
            return a.ToString();
        }

        public static explicit operator byte[](RelayPacket p)
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

            byte[] tmp = new byte[p.Size];
            Array.Copy(BitConverter.GetBytes(p.Size), 0, tmp, 0, 2);
            Array.Copy(BitConverter.GetBytes(p.session), 0, tmp, 2, 4);
            Array.Copy(p.cmd, 0, tmp, 6, 2);
            Array.Copy(BitConverter.GetBytes(p.Size - 10), 0, tmp, 10, 2);
            Array.Copy(p.id, 0, tmp, 12, 2);
            Array.Copy(p.data, 0, tmp, 14, p.data.Length);
            return tmp;
        }

        public static explicit operator RelayPacket(byte[] p)
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

            RelayPacket pkt = new RelayPacket();
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