using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Shared.PacketLib.Map;
using Saga.Structures;
using System;

namespace Saga.Map.Client
{
    partial class InternalClient
    {
        public void WORLD_NOTIFYCATION()
        {
            SMSG_WORLDINSTANCE spkt = new SMSG_WORLDINSTANCE();
            spkt.WorldId = (byte)Saga.Managers.NetworkService._worldid;
            spkt.MaximumPlayerCount = Singleton.NetworkService.PlayerLimit;
            spkt.Proof = Singleton.NetworkService.Proof;
            spkt.Port = Singleton.NetworkService.WorldPort;
            this.Send((byte[])spkt);
        }

        private void WORLD_RECONNECT()
        {
            SMSG_WORLDINSTANCE spkt = new SMSG_WORLDINSTANCE();
            spkt.WorldId = (byte)Saga.Managers.NetworkService._worldid;
            spkt.MaximumPlayerCount = Singleton.NetworkService.PlayerLimit;
            spkt.Proof = Singleton.NetworkService.Proof;
            spkt.IsReconnected = 1;
            spkt.Port = Singleton.NetworkService.WorldPort;
            this.Send((byte[])spkt);
        }

        #region Test

        public void MAINTENANCE_ENTER()
        {
            SMSG_MAINTENANCEENTER spkt = new SMSG_MAINTENANCEENTER();
            this.Send((byte[])spkt);
        }

        public void MAINTENANCE_LEAVE()
        {
            SMSG_MAINTENANCELEAVE spkt = new SMSG_MAINTENANCELEAVE();
            this.Send((byte[])spkt);
        }

        #endregion Test

        #region Useless

        private void CHARACTER_CREATE(CMSG_INTERNAL_CHARACTERCREATE cpkt)
        {
            //Argument with creation data given by the authentication server
            CharCreationArgument argument = new CharCreationArgument();
            argument.CharName = cpkt.Name;
            argument.WeaponName = cpkt.WeaponName;
            argument.WeaponAffix = cpkt.WeaponAffix;
            argument.FaceDetails = cpkt.FaceDetails;
            argument.UserId = cpkt.UserId;

            //Only normans are supported by this time
            Character character = null;
            Saga.Factory.CharacterConfiguration.IDefaultCharacterSettings settings =
                Singleton.CharacterConfiguration.Normans;

            try
            {
                //CHECK IF NAME ALREADY EXISTS
                if (Singleton.Database.VerifyNameExists(cpkt.Name.ToUpperInvariant()))
                {
                    SMSG_CHAR_CREATE spkt = new SMSG_CHAR_CREATE();
                    spkt.Result = 0xA;
                    spkt.SessionId = cpkt.SessionId;
                    this.Send((byte[])spkt);
                }
                //CREATE CHARACTER AND IF FAILED SENT ERROR MESSAGE
                else if (!settings.create(out character, argument))
                {
                    SMSG_CHAR_CREATE spkt = new SMSG_CHAR_CREATE();
                    spkt.Result = 0x4;
                    spkt.SessionId = cpkt.SessionId;
                    this.Send((byte[])spkt);
                }
                //SUCCESSFULL CREATION
                else
                {
                    SMSG_CHAR_CREATE spkt = new SMSG_CHAR_CREATE();
                    spkt.CharatcerId = character.ModelId;
                    spkt.SessionId = cpkt.SessionId;
                    this.Send((byte[])spkt);
                }
            }
            //DATABASE ERROR
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                SMSG_CHAR_CREATE spkt = new SMSG_CHAR_CREATE();
                spkt.Result = 0x6;
                spkt.SessionId = cpkt.SessionId;
                this.Send((byte[])spkt);
            }
        }

        private void DELETE_CHARACTER(CMSG_INTERNAL_CHARACTERDELETE cpkt)
        {
            SMSG_INTERNAL_CHARACTERDELETEREPLY spkt = new SMSG_INTERNAL_CHARACTERDELETEREPLY();
            spkt.SessionId = cpkt.SessionId;

            try
            {
                if (!Singleton.Database.DeleteCharacterById(cpkt.CharacterId))
                    spkt.Result = 0x5;
            }
            catch (Exception)
            {
                spkt.Result = 0x6;
            }
            finally
            {
                this.Send((byte[])spkt);
            }
        }

        private void SELECT_CHARACTER(CMSG_FINDCHARACTERDETAILS cpkt)
        {
            //HELPER VARIABLES
            Character newCharacter = new Character(cpkt.CharacterId);

            //SEND OVER ALL CHARACTER DETAILS
            SMSG_FINDCHARACTERDETAILS spkt = new SMSG_FINDCHARACTERDETAILS();
            spkt.CharacterId = cpkt.CharacterId;
            spkt.SessionId = cpkt.SessionId;

            //WeaponCollection weapon = new WeaponCollection();
            //Rag2Item[] Equipment = new Rag2Item[20];
            if (Singleton.Database.TransLoad(newCharacter) || (Singleton.Database.TransRepair(newCharacter) && Singleton.Database.TransLoad(newCharacter)))
            {
                //Singleton.Database.LoadEquipment(cpkt.CharacterId, Equipment);
                //Singleton.Database.LoadWeaponary(cpkt.CharacterId, weapon);
                int index = newCharacter.weapons.ActiveWeaponIndex == 1 ? newCharacter.weapons.SeconairyWeaponIndex : newCharacter.weapons.PrimaryWeaponIndex;
                spkt.FaceDetails = newCharacter.FaceDetails;

                if (newCharacter.Equipment[0] != null)
                    spkt.HeadTop = newCharacter.Equipment[0];
                if (newCharacter.Equipment[1] != null)
                    spkt.HeadMiddle = newCharacter.Equipment[1];
                if (newCharacter.Equipment[2] != null)
                    spkt.HeadBottom = newCharacter.Equipment[2];
                if (newCharacter.Equipment[3] != null)
                    spkt.Shirt = newCharacter.Equipment[3];
                if (newCharacter.Equipment[4] != null)
                    spkt.Legs = newCharacter.Equipment[4];
                if (newCharacter.Equipment[5] != null)
                    spkt.Gloves = newCharacter.Equipment[5];
                if (newCharacter.Equipment[6] != null)
                    spkt.Shoes = newCharacter.Equipment[6];
                if (newCharacter.Equipment[7] != null)
                    spkt.Belt = newCharacter.Equipment[7];
                if (newCharacter.Equipment[8] != null)
                    spkt.Back = newCharacter.Equipment[8];
                if (newCharacter.Equipment[9] != null)
                    spkt.LeftFinger = newCharacter.Equipment[9];
                if (newCharacter.Equipment[10] != null)
                    spkt.RightFinger = newCharacter.Equipment[10];
                if (newCharacter.Equipment[11] != null)
                    spkt.Necklace = newCharacter.Equipment[11];
                if (newCharacter.Equipment[12] != null)
                    spkt.Earring = newCharacter.Equipment[12];
                if (newCharacter.Equipment[13] != null)
                    spkt.Ammo = newCharacter.Equipment[13];
                if (newCharacter.Equipment[14] != null)
                    spkt.LeftShield = newCharacter.Equipment[14];
                if (newCharacter.Equipment[15] != null)
                    spkt.RightShield = newCharacter.Equipment[15];
                if (index < newCharacter.weapons.UnlockedWeaponSlots && newCharacter.weapons[index] != null && newCharacter.weapons[index]._active == 1)
                    spkt.AugeSkill = newCharacter.weapons[index]._augeskill;

                spkt.SaveMap = newCharacter.map;
                spkt.JobExperience = newCharacter.Jexp;
            }

            this.Send((byte[])spkt);
        }

        private void SELECT_CHARACTERS(CMSG_FINDCHARACTERS cpkt)
        {
            try
            {
                SMSG_FINDCHARACTERS spkt = new SMSG_FINDCHARACTERS();
                spkt.SessionId = cpkt.SessionId;
                foreach (CharInfo info in Singleton.Database.FindCharacters(cpkt.TargetActor))
                    spkt.AddChar(info.charId, info.name, 0, info.cexp, info.job, 1, info.map);
                this.Send((byte[])spkt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                byte[] buffer2 = new byte[] { 0x0B, 0x00, 0x74, 0x17, 0x91, 0x00, 0x02, 0x07, 0x00, 0x00, 0x01 };
                Array.Copy(BitConverter.GetBytes(cpkt.SessionId), 0, buffer2, 2, 4);
                this.Send(buffer2);
            }
        }

        #endregion Useless
    }
}