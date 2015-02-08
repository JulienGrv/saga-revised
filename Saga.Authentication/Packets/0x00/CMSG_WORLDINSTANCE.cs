using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Shared.PacketLib.Map
{
    public class CMSG_WORLDINSTANCE : RelayPacket
    {
        public CMSG_WORLDINSTANCE()
        {
            this.data = new byte[0];
        }

        public byte WorldId
        {
            get
            {
                return this.data[0];
            }
        }

        public byte RequiredAge
        {
            get
            {
                return this.data[1];
            }
        }

        public int MaximumPlayers
        {
            get
            {
                return BitConverter.ToInt32(this.data, 2);
            }
        }

        public int Port
        {
            get
            {
                return BitConverter.ToInt32(this.data, 6);
            }
        }

        public byte IsReconnected
        {
            get
            {
                return this.data[10];
            }
        }

        public string Proof
        {
            get
            {
                return Encoding.Unicode.GetString(this.data, 11, 128).TrimEnd((char)0);
            }
        }

        #region Conversions

        public static explicit operator CMSG_WORLDINSTANCE(byte[] p)
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

            CMSG_WORLDINSTANCE pkt = new CMSG_WORLDINSTANCE();
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