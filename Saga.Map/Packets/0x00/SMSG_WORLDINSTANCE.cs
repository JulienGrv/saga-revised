using Saga.Network.Packets;
using System;
using System.Text;

namespace Saga.Shared.PacketLib.Map
{
    internal class SMSG_WORLDINSTANCE : RelayPacket
    {
        public SMSG_WORLDINSTANCE()
        {
            this.Cmd = 0x0701;
            this.Id = 0x0001;
            this.data = new byte[141];
        }

        public byte WorldId
        {
            set
            {
                this.data[0] = value;
            }
        }

        public byte RequiredAge
        {
            set
            {
                this.data[1] = value;
            }
        }

        public int MaximumPlayerCount
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 2, 4);
            }
        }

        public int Port
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 6, 4);
            }
        }

        public byte IsReconnected
        {
            set
            {
                this.data[10] = value;
            }
        }

        public string Proof
        {
            set
            {
                Encoding.Unicode.GetBytes(value, 0, value.Length, this.data, 11);
            }
        }
    }
}