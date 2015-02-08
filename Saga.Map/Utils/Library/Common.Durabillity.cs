using Saga.Packets;
using Saga.PrimaryTypes;

namespace Common
{
    public static class Durabillity
    {
        #region Public Methods

        /// <summary>
        /// Performs a loose of durabillity on the active weapon.
        /// </summary>
        /// <param name="target"></param>
        public static void DoWeapon(Character target)
        {
            if (target == null) return;
            byte index = target.weapons.ActiveWeaponIndex == 1 ? target.weapons.SeconairyWeaponIndex : target.weapons.PrimaryWeaponIndex;
            if (index < target.weapons.UnlockedWeaponSlots)
            {
                Weapon ActiveWeapon = target.weapons[index];

                //Do durabillity lost
                if (ActiveWeapon._durabillity > 1)
                {
                    //Lose durability
                    ActiveWeapon._durabillity -= 1;
                    SMSG_WEAPONADJUST spkt = new SMSG_WEAPONADJUST();
                    spkt.SessionId = target.id;
                    spkt.Slot = index;
                    spkt.Function = 3;
                    spkt.Value = ActiveWeapon._durabillity;
                    target.client.Send((byte[])spkt);

                    //Do deactivation
                    if (ActiveWeapon._durabillity == 0)
                    {
                        CommonFunctions.SendBattleStatus(target);
                    }
                }
            }
        }

        /// <summary>
        /// Looses durabillity loss on defensive equipment
        /// </summary>
        /// <remarks>
        /// Durabillity loss on kro2 is calculated with a ratio of 50 : 1.
        /// Shield, Shirt, Pants, Shoes each durabillity loss it will rotate
        /// over the next equipment set.
        /// </remarks>
        public static void DoEquipment(Character target, uint damage)
        {
            if (target == null) return;
            bool canupdate = false;
            int NewDurabillity = 0;
            byte Slot = 0;

            target.TakenDamage += damage;
            while (target.TakenDamage > 50)
            {
                target.TakenDamage -= 50;

                switch (target.LastEquipmentDuraLoss)
                {
                    case 0:    //Shield
                        Rag2Item equip = target.Equipment[15];
                        if (equip == null || equip.active == 1 || equip.durabillty == 0) goto case 1;
                        Slot = 15;
                        equip.durabillty -= 1;
                        NewDurabillity = equip.durabillty;
                        target.LastEquipmentDuraLoss = 1;
                        canupdate = true;
                        break;

                    case 1:     //Shirt
                        Rag2Item shirt = target.Equipment[3];
                        if (shirt == null || shirt.active == 1 || shirt.durabillty == 0) goto case 2;
                        Slot = 3;
                        shirt.durabillty -= 1;
                        NewDurabillity = shirt.durabillty;
                        target.LastEquipmentDuraLoss = 2;
                        canupdate = true;
                        break;

                    case 2:     //Pants
                        Rag2Item pants = target.Equipment[4];
                        if (pants == null || pants.active == 1 || pants.durabillty == 0) goto case 3;
                        Slot = 4;
                        pants.durabillty -= 1;
                        NewDurabillity = pants.durabillty;
                        target.LastEquipmentDuraLoss = 3;
                        canupdate = true;
                        break;

                    case 3:     //Shoes
                        Rag2Item shoes = target.Equipment[5];
                        if (shoes == null || shoes.active == 1 || shoes.durabillty == 0) return;
                        Slot = 5;
                        shoes.durabillty -= 1;
                        NewDurabillity = shoes.durabillty;
                        target.LastEquipmentDuraLoss = 0;
                        canupdate = true;
                        break;
                }

                if (canupdate == true)
                {
                    SMSG_ITEMADJUST spkt = new SMSG_ITEMADJUST();
                    spkt.Container = 1;
                    spkt.Function = 3;
                    spkt.Slot = Slot;
                    spkt.SessionId = target.id;
                    spkt.Value = (uint)NewDurabillity;
                    target.client.Send((byte[])spkt);
                }
            }
        }

        #endregion Public Methods
    }
}