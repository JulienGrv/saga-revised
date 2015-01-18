using System;
using System.Collections.Generic;
using System.Text;
using Saga.Shared.PacketLib;
using Saga.Map.Definitions.Misc;
using Saga.Shared.Definitions;
using Saga.Network.Packets;

namespace Saga.Packets
{


    [Obsolete("Error",true)]
    [CLSCompliant(false)]
    public class SMSG_PARTYMEMBERS: RelayPacket
    {

        public SMSG_PARTYMEMBERS(int items)
        {
            this.Cmd = 0x0601;
            this.Id = 0x0601;
            this.data = new byte[5 + (items * 67)];
            this.data[4] = (byte)items;
        }

        public byte LeaderIndex
        {
            set { this.data[0] = value; }
        }

        public uint LeaderId
        {
            set { Array.Copy(BitConverter.GetBytes(value),0, this.data,1,4); }
        }

        public byte Settings1
        {
            set { this.data[5] = value; }
        }

        public byte Settings2
        {
            set { this.data[6] = value; }
        }

        public byte Settings3
        {
            set { this.data[7] = value; }
        }

        public uint Settings4
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 8, 4); }
        }

        int offset = 12;
        public void AddItem(Character item)
        {


            /*
            this.data[offset] = index + 1;
            Array.Copy(BitConverter.GetBytes(item.id), 0, this.data, offset + 1, 4);
            Encoding.Unicode.GetBytes(item.name, 0, Math.Min(16, value.Length), this.data, offset + 17);



            this.PutByte((byte)(i + 1), (ushort)(16 + i * 54));//index
            this.PutUInt(client.Char.id, (ushort)(17 + i * 54));
            string name = client.Char.name;
            Global.SetStringLength(name, 16);
            this.PutString(name, (ushort)(21 + i * 54));
            this.PutUShort(0x66, (ushort)(55 + i * 54));//unknown
            this.PutByte(client.Char.mapID, (ushort)(57 + i * 54));//map
            this.PutUShort(client.Char.maxHP, (ushort)(58 + i * 54));
            this.PutUShort(client.Char.HP, (ushort)(60 + i * 54));
            this.PutUShort(client.Char.maxSP, (ushort)(62 + i * 54));
            this.PutUShort(client.Char.SP, (ushort)(64 + i * 54));
            this.PutByte(client.Char.LP, (ushort)(66 + i * 54));
            this.PutByte((byte)client.Char.cLevel, (ushort)(67 + i * 54));
            this.PutByte((byte)client.Char.job, (ushort)(68 + i * 54));
            this.PutByte((byte)client.Char.jLevel, (ushort)(69 + i * 54));
            i++;
            */







            offset += 54;
        }
    }
}

