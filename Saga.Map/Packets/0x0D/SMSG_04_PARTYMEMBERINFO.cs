using Saga.Network.Packets;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Packets
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// This packet is sent to a player to set a list of users
    /// that are in the party.
    ///
    /// At offset 39 of each member info. There is a field we'll be refering to
    /// zone state. In our experience it's a byte long. The value of this byte is
    /// depending on which map you're on.
    ///
    /// In SMSG_MAPSTART this same value is also saved. When the field in SMSG_MAPSTART
    /// and in the party related packets differ. The minimap won't display any additional
    /// members.
    ///
    /// </remarks>
    /// <id>
    /// 0D04
    /// </id>
    internal class SMSG_PARTYMEMBERINFO : RelayPacket
    {
        public SMSG_PARTYMEMBERINFO()
        {
            this.Cmd = 0x0601;
            this.Id = 0x0D04;
            this.data = new byte[283];
        }

        public byte LeaderIndex
        {
            set
            {
                this.data[0] = value;
            }
        }

        public uint Leader
        {
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.data, 1, 4);
            }
        }

        public byte Setting1
        {
            set
            {
                this.data[5] = value;
            }
        }

        public byte Setting2
        {
            set
            {
                this.data[6] = value;
            }
        }

        public byte Setting3
        {
            set
            {
                this.data[7] = value;
            }
        }

        public uint Setting4
        {
            set { Array.Copy(BitConverter.GetBytes(value), 0, this.data, 8, 4); }
        }

        public void AddMemberInfo(IEnumerable<Character> chars)
        {
            int i = 0;
            foreach (Character character in chars)
            {
                int offset = 12 + i * 54;
                this.data[offset] = 1;
                Array.Copy(BitConverter.GetBytes(character.id), 0, this.data, offset + 1, 4);
                Encoding.Unicode.GetBytes(character.Name, 0, Math.Min(character.Name.Length, 16), this.data, offset + 5);
                this.data[offset + 39] = (byte)(character.map + 0x65);
                this.data[offset + 41] = character.map;
                Array.Copy(BitConverter.GetBytes(character.HPMAX), 0, this.data, offset + 42, 2);
                Array.Copy(BitConverter.GetBytes(character.HP), 0, this.data, offset + 44, 2);
                Array.Copy(BitConverter.GetBytes(character.SPMAX), 0, this.data, offset + 46, 2);
                Array.Copy(BitConverter.GetBytes(character.SP), 0, this.data, offset + 48, 2);
                this.data[offset + 50] = character._status.CurrentLp;
                this.data[offset + 51] = character._level;
                this.data[offset + 52] = character.job;
                this.data[offset + 53] = character.jlvl;
                i++;
            }
        }

        public void AddCharacterInfo(params Character[] chars)
        {
            int i = 0;
            foreach (Character character in chars)
            {
                int offset = 12 + i * 54;
                this.data[offset] = 1;
                Array.Copy(BitConverter.GetBytes(character.id), 0, this.data, offset + 1, 4);
                Encoding.Unicode.GetBytes(character.Name, 0, Math.Min(character.Name.Length, 16), this.data, offset + 5);
                this.data[offset + 39] = (byte)(character.map + 0x65);
                this.data[offset + 41] = character.map;
                Array.Copy(BitConverter.GetBytes(character.HPMAX), 0, this.data, offset + 42, 2);
                Array.Copy(BitConverter.GetBytes(character.HP), 0, this.data, offset + 44, 2);
                Array.Copy(BitConverter.GetBytes(character.SPMAX), 0, this.data, offset + 46, 2);
                Array.Copy(BitConverter.GetBytes(character.SP), 0, this.data, offset + 48, 2);
                this.data[offset + 50] = character._status.CurrentLp;
                this.data[offset + 51] = character._level;
                this.data[offset + 52] = character.job;
                this.data[offset + 53] = character.jlvl;
                i++;
            }
        }

        public byte Result
        {
            set
            {
                this.data[282] = value;
            }
        }
    }
}