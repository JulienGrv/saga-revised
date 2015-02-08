using Saga.Map.Definitions.Misc;
using Saga.Packets;
using Saga.PrimaryTypes;
using System;

namespace Common
{
    internal static class Internal
    {
        /// <summary>
        /// Used to generate a CharacterInformation packet.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="spkt"></param>
        [Obsolete("Error", true)]
        public static void GenerateCharacterInfo(Character character, out SMSG_CHARACTERINFO spkt)
        {
            //HELPER VARIABLES
            Rag2Item item;

            //STRUCTIRIZE GENERAL INFORMATION
            spkt = new SMSG_CHARACTERINFO();
            spkt.race = 0;
            spkt.Gender = character.gender;
            spkt.Name = character.Name;
            spkt.X = character.Position.x;
            spkt.Y = character.Position.y;
            spkt.Z = character.Position.z;
            spkt.ActorID = character.id;
            spkt.face = character.FaceDetails;
            spkt.AugeSkillID = character.ComputeAugeSkill();
            spkt.yaw = character.Yaw;
            spkt.Job = character.job;
            spkt.Stance = character.stance;

            //STRUCTURIZE EQUIPMENT INFORMATION
            item = character.Equipment[0];
            if (item != null && item.active > 0) spkt.SetHeadTop(item.info.item, item.dyecolor);

            item = character.Equipment[1];
            if (item != null && item.active > 0) spkt.SetHeadMiddle(item.info.item, item.dyecolor);

            item = character.Equipment[2];
            if (item != null && item.active > 0) spkt.SetHeadBottom(item.info.item, item.dyecolor);

            item = character.Equipment[14];
            if (item != null && item.active > 0) spkt.SetShield(item.info.item, item.dyecolor);

            item = character.Equipment[8];
            if (item != null && item.active > 0) spkt.SetBack(item.info.item, item.dyecolor);

            item = character.Equipment[3];
            if (item != null && item.active > 0) spkt.SetShirt(item.info.item, item.dyecolor);

            item = character.Equipment[4];
            if (item != null && item.active > 0) spkt.SetPants(item.info.item, item.dyecolor);

            item = character.Equipment[5];
            if (item != null && item.active > 0) spkt.SetGloves(item.info.item, item.dyecolor);

            item = character.Equipment[6];
            if (item != null && item.active > 0) spkt.SetFeet(item.info.item, item.dyecolor);

            foreach (AdditionState state in character._additions)
                spkt.SetWeapon(state.Addition, state.Lifetime);
        }

        [Obsolete("Error", true)]
        public static bool IsWeaponSlotActive(Character character, byte slot)
        {
            int ActiveWeaponIndex = (character.weapons.ActiveWeaponIndex == 1) ? character.weapons.SeconairyWeaponIndex : character.weapons.PrimaryWeaponIndex;
            return ActiveWeaponIndex == slot;
        }

        /// <summary>
        /// Used interally to see if the HP/SP doesn't overlap their max boundaries.
        /// </summary>
        /// <param name="character"></param>
        public static void CharacterCheckCapacities(Character character)
        {
            BattleStatus status = character._status;
            status.CurrentSp = (status.CurrentSp > (int)status.MaxSP) ? (ushort)status.MaxSP : status.CurrentSp;
            status.CurrentHp = (status.CurrentHp > (int)status.MaxHP) ? (ushort)status.MaxHP : status.CurrentHp;
        }

        public static void MailArrived(Character character, uint amount)
        {
            SMSG_MAILARRIVED spkt = new SMSG_MAILARRIVED();
            spkt.Amount = amount;
            spkt.SessionId = character.id;

            if (character.client.isloaded == true)
                character.client.Send((byte[])spkt);
        }

        public static void AckMenuPressed(Character character, byte button, byte menu)
        {
            SMSG_NPCMENU spkt = new SMSG_NPCMENU();
            spkt.ButtonID = button;
            spkt.MenuID = menu;
            spkt.SessionId = character.id;
            character.client.Send((byte[])spkt);
        }

        public static void CheckWeaponary(Character character)
        {
            //Disable weapon
            for (int i = 0; i < 5; i++)
            {
                //Verify if the weapon exists
                Weapon weapon = character.weapons[i];
                if (weapon == null) continue;

                bool Active = weapon._active == 1;
                bool NewActive = Common.Skills.HasRootSkillPresent(
                        character, weapon.Info.weapon_skill);
                if (Active == NewActive) continue;
                weapon._active = (byte)((NewActive == true) ? 1 : 0);

                SMSG_WEAPONADJUST spkt = new SMSG_WEAPONADJUST();
                spkt.Function = 6;
                spkt.Slot = (byte)i;
                spkt.SessionId = character.id;
                spkt.Value = weapon._active;
                character.client.Send((byte[])spkt);
            }
        }
    }
}