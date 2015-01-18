using System;
using System.Collections.Generic;
using System.Text;
using Saga.Packets;
using Saga.Shared.Definitions;
using System.Collections;
using Saga.Map.Librairies;

namespace Saga.Map.Client
{
    partial class Client
    {

        #region For debugging purposes

        private char[] HEXDIGITS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        private byte[] HexStringToBytes(string value)
        {
            byte[] aBytes = new byte[value.Length / 2];
            int offset = 0;
            for (int i = 0; i < value.Length; i++)
            {
                string tmp = "";
                tmp += value[i];
                tmp += value[i + 1];
                aBytes[offset] = byte.Parse(tmp, System.Globalization.NumberStyles.HexNumber);
                offset++;
                i++;
            }
            return aBytes;
        }

        public void SendRaw(string hexbytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hexbytes.Length; i++)
                if (hexbytes[i] != ' ' && hexbytes[i] != '\n' && hexbytes[i] != '\t' && hexbytes[i] != '\r') sb.Append(hexbytes[i]);

            byte[] buffer = HexStringToBytes(sb.ToString());
            this.Send(buffer);
        }

        #endregion

        public void Error(uint i)
        {
            SMSG_ERROR spkt = new SMSG_ERROR();
            spkt.ErrorCode = i;
            spkt.SessionId = this.character.id;
            this.Send((byte[])spkt);
        }

        static uint lvl2 = 0;
        public void wup()
        {
            SMSG_WEAPONADJUST spkt = new SMSG_WEAPONADJUST();
            spkt.SessionId = this.session;
            spkt.Slot = 0;
            spkt.Function = 1;
            spkt.Value = ++lvl2;
            this.Send((byte[])spkt);
        }

        public void wlvl(uint lvl)
        {
            lvl2 = lvl;
            SMSG_WEAPONADJUST spkt = new SMSG_WEAPONADJUST();
            spkt.SessionId = this.session;
            spkt.Slot = 0;
            spkt.Function = 1;
            spkt.Value = lvl;
            this.Send((byte[])spkt);
        }

        public void wdura(uint dura)
        {
            SMSG_WEAPONADJUST spkt = new SMSG_WEAPONADJUST();
            spkt.SessionId = this.session;
            spkt.Slot = 0;
            spkt.Function = 3;
            spkt.Value = dura;
            this.Send((byte[])spkt);
        }

        public void clvl(byte lvl)
        {
            this.character.clvl = lvl;
            this.character.cexp = Singleton.experience.FindRequiredCEXP(lvl);
            this.character._HPMAX = Character.ComputerMaxHp(this.character);
            this.character._SPMAX = Character.ComputerMaxHp(this.character);
            CommonFunctions.UpdateCharacterInfo(this.character, 0);
        }

        uint index = 900;
        public void Create()
        {
            SMSG_CHARACTERINFO spkt = new SMSG_CHARACTERINFO();
            spkt.race = 0;
            spkt.Gender = 1;
            spkt.Name = this.character.name;
            spkt.X = this.character.Position.x;
            spkt.Y = this.character.Position.y;
            spkt.Z = this.character.Position.z;
            spkt.ActorID = index;
            spkt.face = this.character.FaceDetails;
            spkt.AugeSkillID = 1000;
            spkt.yaw = 0;
            spkt.Job = 5;

            spkt.SetHeadTop(3493, 0);       //ALIEN HORNS
            spkt.SetHeadMiddle(900024, 0);  //FAKE KISS
            spkt.SetHeadBottom(950018, 3);  //MASK OF ROBBER
            spkt.SetShield(200058, 0);      //FAIRLY BUCKET                
            spkt.SetBack(3410, 0);          //PORING BAG
            spkt.SetFeet(18775, 0);         //Beginner's Sandals
            spkt.SetGloves(300025, 0);      //PRACTIVE GLOVES
            spkt.SetPants(19173, 0);        //Adventurer's Breeches
            spkt.SetShirt(100167, 0);       //Beginners Effort Shirt               
            spkt.SetWeapon(101);            //SET WEAPON

            spkt.SessionId = this.character.id;
            this.Send((byte[])spkt);            
        }

        public void hp(ushort hp)
        {
            this.character._HP = hp;
            CommonFunctions.UpdateCharacterInfo(this.character, 0);
        }
    }
}
