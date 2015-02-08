using Saga.Enumarations;
using Saga.Map;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Quests;
using Saga.Shared.NetworkCore;
using Saga.Structures;
using Saga.Tasks;
using Saga.Templates;
using System.Collections.Generic;
using System.Diagnostics;

public static class CommonFunctions
{
    #region NPC: Dialog Scripts

    public static void AcknowledgeMenuPressed(Character target, DialogType button, byte menu)
    {
        SMSG_NPCMENU spkt = new SMSG_NPCMENU();
        spkt.ButtonID = (byte)button;
        spkt.MenuID = menu;
        spkt.SessionId = target.id;
        target.client.Send((byte[])spkt);
    }

    public static void ShowAuction(Character target, MapObject c)
    {
        SMSG_MARKETSTART spkt = new SMSG_MARKETSTART();
        spkt.Unknown = 1;
        spkt.Actor = c.id;
        spkt.SessionId = target.id;
        target.client.Send((byte[])spkt);
    }

    public static void ShowWarpOptions(Character target, MapObject source, IEnumerable<ushort> warplocations)
    {
        Saga.Factory.Warps.Info WarpInfo;
        SMSG_WARPDIALOG spkt = new SMSG_WARPDIALOG();
        spkt.ActorId = source.id;
        spkt.SessionId = target.id;

        foreach (ushort i in warplocations)
            if (Singleton.Warps.TryFind(i, out WarpInfo))
                spkt.AddItem(i, WarpInfo.price);

        target.client.Send((byte[])spkt);
    }

    #endregion NPC: Dialog Scripts

    #region Special Operations

    public static void UpdateCharacterInfo(Character target, byte update)
    {
        SMSG_CHARSTATUS spkt = new SMSG_CHARSTATUS();
        spkt.Job = target.job;
        spkt.Exp = target.Cexp;
        spkt.JobExp = target.Jexp;
        spkt.LC = target._status.CurrentOxygen;
        spkt.MaxLC = target._status.MaximumOxygen;
        spkt.HP = target.HP;
        spkt.MaxHP = target.HPMAX;
        spkt.SP = target.SP;
        spkt.MaxSP = target.SPMAX;
        spkt.LP = target._status.CurrentLp;
        spkt.MaxLP = 7;
        spkt.FieldOfSight = update;
        spkt.SessionId = target.id;
        target.client.Send((byte[])spkt);
    }

    public static void UpdateZeny(Character target)
    {
        SMSG_SENDZENY spkt = new SMSG_SENDZENY();
        spkt.Zeny = target.ZENY;
        spkt.SessionId = target.id;
        target.client.Send((byte[])spkt);
    }

    public static void UpdateShopZeny(Character target)
    {
        BaseNPC targetA = target.Target as BaseNPC;
        if (targetA != null)
        {
            SMSG_SHOPZENYUPDATE spkt = new SMSG_SHOPZENYUPDATE();
            spkt.Actor = target.Target.id;
            spkt.Zeny = targetA.Zeny;
            spkt.SessionId = target.id;
            target.client.Send((byte[])spkt);
        }
    }

    #endregion Special Operations

    #region SomeJunk

    private static IEnumerable<uint> CheckQuest(Character target, IEnumerable<uint> id)
    {
        foreach (uint c in id)
            if (target.QuestObjectives[c] == null) yield return c;
    }

    public static void SendRebuylist(Character target)
    {
        SMSG_REBUYLIST spkt = new SMSG_REBUYLIST();
        spkt.SessionId = target.id;

        byte i = 0;
        foreach (Rag2Item item in target.REBUY)
        {
            spkt.Count = ++i;
            spkt.Add(item.info.item, (byte)item.count);
        }
        target.client.Send((byte[])spkt);
    }

    #endregion SomeJunk

    #region More Junk

    public static void SendExchangeStatus(Character target, uint addition, uint time)
    {
        SMSG_ADDITIONBEGIN spkt = new SMSG_ADDITIONBEGIN();
        spkt.Duration = time;
        spkt.SourceActor = target.id;
        spkt.StatusID = addition;
        spkt.SessionId = target.id;
        target.client.Send((byte[])spkt);
    }

    public static void SendDeleteStatus(Character target, uint addition)
    {
        SMSG_ADDITIONEND spkt = new SMSG_ADDITIONEND();
        spkt.SourceActor = target.id;
        spkt.StatusID = addition;
        spkt.SessionId = target.id;
        target.client.Send((byte[])spkt);
    }

    public static void SendBattleStatus(Character character)
    {
        SMSG_BATTLESTATS spkt = new SMSG_BATTLESTATS();
        spkt.SpiritResistance = character._status.SpiritResistance;
        spkt.WindResistance = character._status.ThunderResistance;
        spkt.DarkResistance = character._status.DarkResistance;
        spkt.FireResistance = character._status.FireResistance;
        spkt.GhostResistance = character._status.GhostResitance;
        spkt.HolyResistance = character._status.HolyResistance;
        spkt.IceResistance = character._status.IceResistance;
        spkt.MagicalAttackMax = (ushort)character._status.BaseMinMAttack;
        spkt.MagicalAttackMin = (ushort)character._status.BaseMinMAttack;
        spkt.MagicalDefense = (ushort)character._status.DefenceMagical;
        spkt.MagicalEvasion = (ushort)character._status.BaseMEvasionrate;
        spkt.PhysicalAttackMax = (ushort)(character._status.BaseMaxPAttack + character._status.MaxWPAttack);
        spkt.PhysicalAttackMin = (ushort)(character._status.BaseMinPAttack);
        spkt.PhysicalDefense = (ushort)character._status.DefencePhysical;
        spkt.PhysicalEvasion = (ushort)character._status.BasePEvasionrate;
        spkt.PhysicalRangedAttackMax = (ushort)character._status.BaseMaxRAttack;
        spkt.PhysicalRangedAttackMin = (ushort)character._status.BaseMinRAttack;
        spkt.PhysicalRangedDefense = (ushort)character._status.DefenceRanged;
        spkt.PhysicalRangedEvasion = (ushort)character._status.BaseREvasionrate;
        spkt.SessionId = character.id;
        character.client.Send((byte[])spkt);
    }

    public static void SendExtStats(Character character)
    {
        SMSG_EXTSTATS spkt = new SMSG_EXTSTATS();
        spkt.base_stats_1 = character.stats.BASE;
        spkt.base_stats_2 = character.stats.CHARACTER;
        spkt.base_stats_jobs = character.stats.EQUIPMENT;
        spkt.base_stats_bonus = character.stats.ENCHANTMENT;
        spkt.statpoints = character.stats.REMAINING;
        spkt.SessionId = character.id;
        character.client.Send((byte[])spkt);
    }

    public static Client GetClient(Character target)
    {
        //Only temp...
        return target.client;
    }

    #endregion More Junk

    #region More Junk

    public static void UpdateState(Character target)
    {
        Regiontree tree = target.currentzone.Regiontree;
        foreach (Character regionObject in tree.SearchActors(target, SearchFlags.Characters))
            Common.Actions.UpdateStance(regionObject, target);
    }

    internal static void SendOxygenTakeDamage(Character c, uint p)
    {
        /*
         * This functions sends a oxygen damage packet
         *
         * Where the damage is already already decided. And take from
         * the character.
         */

        SMSG_TAKEDAMAGE spkt = new SMSG_TAKEDAMAGE();
        spkt.Damage = p;
        spkt.Reason = c.HP > 0 ? (byte)TakeDamageReason.Oxygen : (byte)TakeDamageReason.Suffocated;
        spkt.SessionId = c.id;
        c.client.Send((byte[])spkt);
    }

    public static void UpdateTimeWeather(Character character)
    {
        /*
         * This function updates the time and Weather for the
         * selected player.
         */

        SMSG_TIMEWEATHER spkt = new SMSG_TIMEWEATHER();
        spkt.weather = (byte)character.currentzone.Weather;
        spkt.day = Saga.Tasks.WorldTime.Time[0];
        spkt.hour = Saga.Tasks.WorldTime.Time[1];
        spkt.min = Saga.Tasks.WorldTime.Time[2];
        spkt.SessionId = character.id;
        character.client.Send((byte[])spkt);
    }

    #endregion More Junk

    public static void RefreshPersonalRequests(Character target)
    {
        if (target != null)
        {
            Dictionary<uint, uint> tmp = new Dictionary<uint, uint>();
            QuestBase PersonalQuest = target.QuestObjectives.PersonalQuest;
            uint PersonalQuestId = (PersonalQuest != null) ? PersonalQuest.QuestId : 0;
            foreach (KeyValuePair<uint, uint> pair in
                Singleton.Database.GetPersonalAvailableQuestsByRegion(target, (byte)target.currentzone.RegionCode, PersonalQuestId))
                tmp.Add(pair.Value, pair.Key);
            lock (target.client.AvailablePersonalRequests)
                target.client.AvailablePersonalRequests = tmp;
        }
    }

    public static void UpdateNpcIcons(Character target)
    {
        foreach (MapObject myObject in target.currentzone.GetObjectsInRegionalRange(target))
        {
            if (MapObject.IsNotMonster(myObject)
            && target.currentzone.IsInSightRangeBySquare(target.Position, myObject.Position))
            {
                BaseMob temp = myObject as BaseMob;
                if (temp != null)
                    Common.Actions.UpdateIcon(target, temp);
            }
        }
    }

    #region Chat - Functions

    public static void Broadcast(Character sender, string message)
    {
        string nmessage = message != null ? message : string.Empty;
        foreach (Character target in LifeCycle.Characters)
        {
            SMSG_SENDCHAT spkt = new SMSG_SENDCHAT();
            spkt.Message = nmessage;
            spkt.Name = sender.Name;
            spkt.MessageType = SMSG_SENDCHAT.MESSAGE_TYPE.SYSTEM_MESSAGE;
            spkt.SessionId = target.id;
            target.client.Send((byte[])spkt);
        }
    }

    public static void Broadcast(Character sender, Character target, string message)
    {
        SMSG_SENDCHAT spkt = new SMSG_SENDCHAT();
        spkt.Message = message != null ? message : string.Empty;
        spkt.Name = sender.Name;
        spkt.MessageType = SMSG_SENDCHAT.MESSAGE_TYPE.SYSTEM_MESSAGE;
        spkt.SessionId = target.id;
        target.client.Send((byte[])spkt);
    }

    public static void SystemMessage(Character sender, string message)
    {
        string nmessage = message != null ? message : string.Empty;
        foreach (Character target in LifeCycle.Characters)
        {
            SMSG_SENDCHAT spkt = new SMSG_SENDCHAT();
            spkt.Message = nmessage;
            spkt.Name = sender.Name;
            spkt.MessageType = SMSG_SENDCHAT.MESSAGE_TYPE.SYSTEM_MESSAGE_RED;
            spkt.SessionId = target.id;
            target.client.Send((byte[])spkt);
        }
    }

    public static void SystemMessage(Character sender, Character target, string message)
    {
        SMSG_SENDCHAT spkt = new SMSG_SENDCHAT();
        spkt.Message = message != null ? message : string.Empty;
        spkt.Name = sender.Name;
        spkt.MessageType = SMSG_SENDCHAT.MESSAGE_TYPE.SYSTEM_MESSAGE_RED;
        spkt.SessionId = target.id;
        target.client.Send((byte[])spkt);
    }

    #endregion Chat - Functions

    #region Warp - Functions

    public static bool Warp(Character target)
    {
        return Warp(target, target.savelocation.map, target.savelocation.coords);
    }

    public static bool Warp(Character target, byte zone)
    {
        Zone NewZone;
        if (Singleton.Zones.TryGetZone(zone, out NewZone))
        {
            if (NewZone.CathelayaLocation.map == zone)
                return Warp(target, NewZone.CathelayaLocation.coords, NewZone);
            else
                return Warp(target, new Point(0, 0, 0), NewZone);
        }
        else
        {
            return false;
        }
    }

    public static bool Warp(Character target, byte zone, Point destination)
    {
        Zone NewZone;
        if (Singleton.Zones.TryGetZone(zone, out NewZone))
        {
            return Warp(target, destination, NewZone);
        }
        else
        {
            return false;
        }
    }

    public static bool Warp(Character target, Point destination, Zone NewZoneInstance)
    {
        if (target.client.isloaded == true)
        {
            if (target.map > 0 && NewZoneInstance.Map > 0)
            {
                if (target.map != NewZoneInstance.Map)
                {
                    #region Not the same Zone

                    if (target.currentzone.Type != ZoneType.Dungeon)
                    {
                        WorldCoordinate worldcoord;
                        worldcoord.coords = target.Position;
                        worldcoord.map = target.currentzone.Map;
                        target.lastlocation = worldcoord;
                    }

                    //Start loading
                    target.client.isloaded = false;
                    Zone currentZone = target.currentzone;
                    target.currentzone.OnLeave(target);

                    lock (target)
                    {
                        //Unregister
                        currentZone.Regiontree.Unsubscribe(target);

                        //Set position to the new location
                        target.map = NewZoneInstance.Map;

                        //Set new position
                        target.Position = new Point(
                                destination.x,
                                destination.y,
                                destination.z
                        );
                    }

                    //Update party members
                    if (target.sessionParty != null)
                        foreach (Character partyTarget in target.sessionParty)
                        {
                            //switch the map
                            SMSG_PARTYMEMBERMAPCHANGE spkt4 = new SMSG_PARTYMEMBERMAPCHANGE();
                            spkt4.Index = 1;
                            spkt4.ActorId = target.id;
                            spkt4.Unknown = (byte)(target.map + 0x065);
                            spkt4.Zone = target.map;
                            spkt4.SessionId = partyTarget.id;
                            partyTarget.client.Send((byte[])spkt4);
                        }

                    //Set zone instance
                    target.currentzone = NewZoneInstance;

                    //Send over load packet
                    SMSG_SENDSTART spkt = new SMSG_SENDSTART();
                    spkt.SessionId = target.id;
                    spkt.X = destination.x;
                    spkt.Y = destination.y;
                    spkt.Z = destination.z;
                    spkt.MapId = NewZoneInstance.Map;
                    spkt.Unknown = (byte)(target.map + 0x65);
                    spkt.Channel = 1;
                    target.client.Send((byte[])spkt);
                    return true;

                    #endregion Not the same Zone
                }
                else
                {
                    #region Notify People That Actor Disapears

                    Point origin = target.Position;

                    Regiontree tree;
                    tree = target.currentzone.Regiontree;
                    foreach (MapObject c in tree.SearchActors(target, SearchFlags.DynamicObjects))
                    {
                        bool insightrangeold = Point.IsInSightRangeByRadius(origin, c.Position);
                        bool insightrangenew = Point.IsInSightRangeByRadius(destination, c.Position);
                        if (c.id == target.id) continue;

                        //If vible from old postion but not from new position hide
                        if (insightrangeold && !insightrangenew)
                        {
                            double distance = Point.GetDistance3D(destination, c.Position);
                            //CROSS NOTIFY PLAYERS ACTORS DISAPPPEARS
                            if (MapObject.IsPlayer(c))
                            {
                                Character targetChar = c as Character;
                                targetChar.HideObject(target);
                            }

                            c.HideObject(target);
                            c.Disappear(target);
                        }
                    }

                    #endregion Notify People That Actor Disapears

                    #region Update Region

                    target.Position = new Point(destination.x, destination.y, destination.z);
                    Regiontree.UpdateRegion(target);

                    #endregion Update Region

                    #region Teleport

                    SMSG_ACTORTELEPORT spkt = new SMSG_ACTORTELEPORT();
                    spkt.SessionId = target.id;
                    spkt.x = destination.x;
                    spkt.y = destination.y;
                    spkt.Z = destination.z;
                    target.client.Send((byte[])spkt);

                    #endregion Teleport

                    #region Reset State

                    target.ISONBATTLE = false;
                    target.stance = (byte)StancePosition.Reborn;
                    target._targetid = 0;

                    SMSG_ACTORCHANGESTATE spkt4 = new SMSG_ACTORCHANGESTATE();
                    spkt4.SessionId = target.id;
                    spkt4.Stance = target.stance;
                    spkt4.TargetActor = target._targetid;
                    spkt4.State = (target.ISONBATTLE == true) ? (byte)1 : (byte)0;
                    spkt4.ActorID = target.id;
                    target.client.Send((byte[])spkt4);

                    #endregion Reset State

                    #region Notify People That the Actor Appears

                    tree = target.currentzone.Regiontree;
                    foreach (MapObject myObject in tree.SearchActors(target, SearchFlags.DynamicObjects))
                    {
                        bool insightrangeold = Point.IsInSightRangeByRadius(origin, myObject.Position);
                        bool insightrangenew = Point.IsInSightRangeByRadius(destination, myObject.Position);

                        double distance = Point.GetDistance3D(destination, myObject.Position);

                        //If visible from new postion but not from old postion
                        if (insightrangenew && !insightrangeold)
                        {
                            myObject.ShowObject(target);
                            myObject.Appears(target);

                            if (MapObject.IsPlayer(myObject))
                            {
                                Character current = (Character)myObject;
                                if (current.client.isloaded == false) continue;
                                target.ShowObject(current);
                            }
                        }
                    }

                    #endregion Notify People That the Actor Appears

                    #region Notify Party Members

                    if (target.sessionParty != null)
                    {
                        foreach (Character partyTarget in target.sessionParty)
                        {
                            if (partyTarget.id == target.id) continue;
                            SMSG_PARTYMEMBERLOCATION spkt3 = new SMSG_PARTYMEMBERLOCATION();
                            spkt3.Index = 1;
                            spkt3.ActorId = target.id;
                            spkt3.SessionId = partyTarget.id;
                            spkt3.X = (destination.x / 1000);
                            spkt3.Y = (destination.y / 1000);
                            partyTarget.client.Send((byte[])spkt3);
                        }
                    }

                    #endregion Notify Party Members

                    return true;
                }
            }
            else
            {
                Trace.TraceError("Warping player {0} to unsupported map {1}", target.Name, 0);
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    #endregion Warp - Functions

    internal static void SendQuestList(Character target, MapObject source,
        IEnumerable<uint> iEnumerable)
    {
        SMSG_SENDQUESTLIST spkt = new SMSG_SENDQUESTLIST();
        spkt.SessionId = target.id;
        spkt.SetQuests(iEnumerable);
        spkt.SourceActor = source.id;
        target.client.Send((byte[])spkt);
    }
}