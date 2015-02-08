using Saga.Configuration;
using Saga.Enumarations;
using Saga.Map;
using Saga.Map.Librairies;
using Saga.Packets;
using Saga.PrimaryTypes;
using Saga.Quests;
using Saga.Structures;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Saga.Managers
{
    public class Quests : ManagerBase2
    {
        #region Ctor/Dtor

        public Quests()
        {
        }

        #endregion Ctor/Dtor

        #region Internal Members

        //Settings
        private static string directory;

        private static string directory2;
        private static Type type = null;
        private static Type type2 = null;
        private static ConstructorInfo info;
        private static ConstructorInfo info2;

        #endregion Internal Members

        #region Protected Methods

        protected override void QuerySettings()
        {
            QuestSettings section = (QuestSettings)ConfigurationManager.GetSection("Saga.Manager.Quest");
            if (section != null)
            {
                directory = Saga.Structures.Server.SecurePath(section.Directory);
                directory2 = Saga.Structures.Server.SecurePath(section.SDirectory);
                object mytype;
                if (CoreService.TryFindType(section.Provider, out mytype))
                {
                    if (mytype is QuestBase)
                    {
                        type = mytype.GetType();
                        info = type.GetConstructor(new Type[] { });
                    }
                    else
                    {
                        WriteError("QuestManager", "General quest provider has a invalid base type");
                        return;
                    }
                }
                else
                {
                    WriteError("QuestManager", "General quest provider could not be loaded, or found");
                    return;
                }

                if (CoreService.TryFindType(section.ScenarioProvider, out mytype))
                {
                    if (mytype is QuestBase)
                    {
                        type2 = mytype.GetType();
                        info2 = type2.GetConstructor(new Type[] { });
                    }
                    else
                    {
                        WriteError("QuestManager", "Scenario quest provider has a invalid base type");
                        return;
                    }
                }
                else
                {
                    WriteError("QuestManager", "Scenario quest provider could not be loaded, or found");
                    return;
                }
            }
            else
            {
                WriteError("QuestManager", "Section Saga.Manager.Quest was not found, this section is required to continue loading.");
                return;
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public bool TryFindQuests(uint questId, out QuestBase quest)
        {
            try
            {
                object a = info.Invoke(new object[] { });
                quest = (QuestBase)a;
                quest.QuestId = questId;
                return quest.Initialize(questId, directory);
            }
            catch (FileNotFoundException)
            {
                Trace.TraceWarning("Quest could not be found: {0}", questId);
                quest = null;
                return false;
            }
        }

        public bool TryFindScenarioQuest(uint questId, out QuestBase quest)
        {
            try
            {
                object a = info2.Invoke(new object[] { });
                quest = (QuestBase)a;
                quest.QuestId = questId;
                return quest.Initialize(questId, directory2);
            }
            catch (FileNotFoundException)
            {
                Trace.TraceWarning("Scenario Quest could not be found: {0}", questId);
                quest = null;
                return false;
            }
        }

        public void OpenQuest(Character target, MapObject item)
        {
            target._target = item;
            CommonFunctions.SendQuestList
            (
                target, item,
                    FilterActiveQuests(target, Singleton.Database.GetAvailableQuestsByRegion(target, item.ModelId))
            );
        }

        private IEnumerable<uint> FilterActiveQuests(Character target, IEnumerable<uint> c)
        {
            foreach (uint d in c)
                if (target.QuestObjectives[d] == null) yield return d;
        }

        #endregion Public Methods

        #region Public Properties

        public string QuestDirectory
        {
            get
            {
                return directory;
            }
        }

        public string ScenarioDirectory
        {
            get
            {
                return directory2;
            }
        }

        #endregion Public Properties
    }
}

namespace Saga.Quests
{
    using ObjectiveList = Saga.Quests.Objectives.ObjectiveList;

    /// <summary>
    /// Abstract base interface to derive custom quests from.
    /// </summary>
    [Serializable()]
    public abstract class QuestBase : ISerializable
    {
        #region QuestBase - Internal Members

        protected internal byte questtype = 1;
        protected internal uint QuestId = 0;
        protected internal uint index = 0;
        protected internal bool isnew = false;
        protected internal bool IsWaypointsCleared = true;

        #endregion QuestBase - Internal Members

        #region QuestBase - Abstract

        public abstract bool Initialize(uint cid, string BaseDirectory);

        public abstract int OnCheckQuest(uint cid);

        public abstract int OnCancel(uint cid);

        public abstract int OnFinish(uint cid);

        public abstract int OnStart(uint cid);

        public abstract int OnVerify(uint cid);

        #endregion QuestBase - Abstract

        #region Public Methods

        public int CheckQuest(Character Target)
        {
            int count = OnCheckQuest(Target.id);
            if (IsWaypointsCleared == true)
            {
                int i = 0;
                SMSG_SENDNAVIGATIONPOINT spkt2 = new SMSG_SENDNAVIGATIONPOINT();
                spkt2.SessionId = Target.id;
                spkt2.QuestID = this.QuestId;
                foreach (ObjectiveList.Waypoint waypoint in QuestBase.UserGetWaypoints(Target, this.QuestId))
                {
                    Predicate<MapObject> IsNpc = delegate(MapObject match)
                    {
                        return match.ModelId == waypoint.NpcId;
                    };

                    MapObject myObject = Target.currentzone.Regiontree.SearchActor(IsNpc, SearchFlags.Npcs);
                    if (myObject != null)
                    {
                        i++;
                        spkt2.AddPosition(waypoint.NpcId, myObject.Position.x, myObject.Position.y, myObject.Position.z);
                    }
                }
                if (i > 0)
                {
                    Target.client.Send((byte[])spkt2);
                    IsWaypointsCleared = false;
                }
            }
            return count;
        }

        #endregion Public Methods

        #region Static Methods

        public static bool IsElimintationTarget(uint NpcId, Character Target)
        {
            Predicate<ObjectiveList.Elimination> FindElimintation
                = delegate(ObjectiveList.Elimination Objective)
            {
                return Objective.NpcId == NpcId;
            };

            return Target.QuestObjectives.Elimintations.Exists(FindElimintation);
        }

        public static bool IsDiscardAble(uint ItemId, Character Target)
        {
            Predicate<ObjectiveList.Loot2> FindElimintation =
                delegate(ObjectiveList.Loot2 Objective)
                {
                    return Objective.ItemId == ItemId;
                };

            return Target.QuestObjectives.NonDiscardableItems.Exists(FindElimintation);
        }

        public static bool IsActionObjectActivated(uint NpcId, Character Target)
        {
            Predicate<ObjectiveList.Activation> FindActionObjective =
                delegate(ObjectiveList.Activation Objective)
                {
                    return Objective.NpcId == NpcId;
                };

            return Target.QuestObjectives.ActivatedNpc.Exists(FindActionObjective);
        }

        public static bool IsTalkToObjective(uint NpcId, Character Target)
        {
            try
            {
                /*
                Predicate<ObjectiveList.Activation> FindActionObjective =
                    delegate(ObjectiveList.Activation Objective)
                {
                    return Objective.NpcId == NpcId;
                };

                */

                Predicate<ObjectiveList.Waypoint> FindWaypoints =
                    delegate(ObjectiveList.Waypoint objective)
                    {
                        //Check wether the current npc is the npc from the objective
                        bool isActivated = objective.NpcId == NpcId;

                        //Find associated substep
                        Predicate<ObjectiveList.SubStep> FindSubstep =
                            delegate(ObjectiveList.SubStep substep)
                            {
                                return substep.StepId == objective.StepId &&
                                        substep.SubStepId == objective.SubStepId &&
                                            substep.Quest == objective.Quest;
                            };

                        //Returns completed substep
                        return (isActivated && !Target.QuestObjectives.Substeps.Find(FindSubstep).Completed);
                    };

                //return Target.QuestObjectives.ActivatedNpc.Exists(FindActionObjective);
                return Target.QuestObjectives.GuidancePoints.Exists(FindWaypoints);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return false;
            }
        }

        public static bool HasGuidancePoint(uint NpcId, uint Qid, Character Target)
        {
            Predicate<ObjectiveList.Waypoint> FindActionObjective =
                delegate(ObjectiveList.Waypoint Objective)
                {
                    return Objective.NpcId == NpcId && Objective.Quest == Qid;
                };

            return Target.QuestObjectives.GuidancePoints.Exists(FindActionObjective);
        }

        public static List<byte> QueryTalkObjectivesButtons(uint NpcId, Character Target)
        {
            Predicate<ObjectiveList.Activation> FindActionObjective =
                delegate(ObjectiveList.Activation Objective)
                {
                    return Objective.NpcId == NpcId;
                };

            List<byte> QueriedItems = new List<byte>();
            List<ObjectiveList.Activation> ActivatedQuests =
                Target.QuestObjectives.ActivatedNpc.FindAll(FindActionObjective);
            for (int i = 0; i < 3; i++)
            {
                if (Target.QuestObjectives.Quests[i] == null) continue;

                Predicate<ObjectiveList.Activation> Exists =
                    delegate(ObjectiveList.Activation Objective)
                    {
                        return Objective.Quest == Target.QuestObjectives.Quests[i].QuestId;
                    };

                if (ActivatedQuests.Exists(Exists))
                {
                    QueriedItems.Add((byte)i);
                }
            }

            return QueriedItems;
        }

        public static bool IsQuestItemActive(uint ItemId, Character Target)
        {
            Predicate<ObjectiveList.Loot> FindLootObjective =
                delegate(ObjectiveList.Loot Objective)
                {
                    return Objective.ItemId == ItemId;
                };

            return Target.QuestObjectives.LootObjectives.Exists(FindLootObjective);
        }

        public static void UserEliminateTarget(uint NpcId, Character Target)
        {
            try
            {
                Predicate<ObjectiveList.Elimination> FindElimintation =
                    delegate(ObjectiveList.Elimination Objective)
                    {
                        return Objective.NpcId == NpcId;
                    };

                ObjectiveList.Elimination EliminationObjective =
                    Target.QuestObjectives.Elimintations.Find(FindElimintation);
                ObjectiveList.SubStep Substep = null;
                if (EliminationObjective != null)
                {
                    //Find Substep
                    Predicate<ObjectiveList.SubStep> SubStepInfo
                        = delegate(ObjectiveList.SubStep Objective)
                    {
                        return Objective.StepId == EliminationObjective.StepId
                            && Objective.SubStepId == EliminationObjective.SubStepId;
                    };

                    Substep = Target.QuestObjectives.Substeps.Find(SubStepInfo);
                    if (Substep != null && Substep.current < Substep.max)
                    {
                        //Send update
                        SMSG_QUESTSUBSTEPUPDATE spkt = new SMSG_QUESTSUBSTEPUPDATE();
                        spkt.Amount = (byte)(++Substep.current);
                        spkt.QuestID = EliminationObjective.Quest;
                        spkt.SessionId = Target.id;
                        spkt.StepID = Substep.StepId;
                        spkt.SubStep = (byte)Substep.SubStepId;
                        spkt.Unknown = 1;
                        Target.client.Send((byte[])spkt);

                        //Find Objective
                        if (Substep.current >= Substep.max)
                        {
                            Substep.Completed = true;
                            Target.QuestObjectives[EliminationObjective.Quest].CheckQuest(Target);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        public static void UserObtainedItem(uint ItemId, Character Target)
        {
            Predicate<ObjectiveList.Loot> FindLootObjective =
                delegate(ObjectiveList.Loot Objective)
                {
                    return Objective.ItemId == ItemId;
                };

            ObjectiveList.Loot LootObjective =
                Target.QuestObjectives.LootObjectives.Find(FindLootObjective);
            ObjectiveList.SubStep Substep =
                null;

            if (LootObjective != null)
            {
                //Find Substep
                Predicate<ObjectiveList.SubStep> SubStepInfo =
                    delegate(ObjectiveList.SubStep Objective)
                    {
                        return Objective.StepId == LootObjective.StepId
                            && Objective.SubStepId == LootObjective.SubStepId;
                    };

                Substep = Target.QuestObjectives.Substeps.Find(SubStepInfo);
                if (Substep != null && Substep.current < Substep.max)
                {
                    //Send update
                    SMSG_QUESTSUBSTEPUPDATE spkt = new SMSG_QUESTSUBSTEPUPDATE();
                    spkt.Amount = (byte)(++Substep.current);
                    spkt.QuestID = LootObjective.Quest;
                    spkt.SessionId = Target.id;
                    spkt.StepID = Substep.StepId;
                    spkt.SubStep = (byte)Substep.SubStepId;
                    spkt.Unknown = 1;
                    Target.client.Send((byte[])spkt);

                    //Find Objective
                    if (Substep.current >= Substep.max)
                    {
                        Substep.Completed = true;
                        Target.QuestObjectives[LootObjective.Quest].CheckQuest(Target);
                    }
                }
            }
        }

        public static bool UserTalktoTarget(uint NpcId, Character Target, QuestBase Quest)
        {
            try
            {
                Quest.CheckQuest(Target);
                if (true)
                {
                    /*
                    //Predicate to remove the activated npc
                    Predicate<ObjectiveList.Activation> FindActivatedNpc =
                        delegate(ObjectiveList.Activation objective)
                    {
                        return objective.NpcId == NpcId && objective.Quest == Quest.QuestId;
                    };
                    */

                    //Target.QuestObjectives.ActivatedNpc.RemoveAll(FindActivatedNpc);
                    /*
                    MapObject myObject = Target.currentzone.Regiontree.SearchActor(IsNpc, Target, SearchFlags.Npcs);
                    if (myObject != null)
                    {
                        BaseMob temp = myObject as BaseMob;
                    }*/

                    /**/

                    Regiontree tree = Target.currentzone.Regiontree;
                    foreach (MapObject a in tree.SearchActors(Target, SearchFlags.Npcs))
                    {
                        if (a.ModelId == NpcId)
                        {
                            BaseMob mob = a as BaseMob;
                            if (mob != null)
                                Common.Actions.UpdateIcon(Target, mob);
                        }
                    }

                    return true;
                }
            }
            catch (QuestFailedException)
            {
                //Do nothing here
                return false;
            }
        }

        public static void UserCheckPosition(Character Target)
        {
            List<uint> QuestsId = new List<uint>();
            Predicate<ObjectiveList.Position> FindLootObjective =
                delegate(ObjectiveList.Position Objective)
                {
                    if (Objective.mapid != Target.map) return false;
                    return (!QuestsId.Contains(Objective.Quest)
                        && Vector.GetDistance2D(Target.Position, Objective.point) < 2000);
                };

            //DEFAULT_MANAGER_QUEST.CheckQuest(Target, baseQuest);
            foreach (ObjectiveList.Position Objective in
                Target.QuestObjectives.Points.FindAll(FindLootObjective))
            {
                Predicate<ObjectiveList.SubStep> SubStepInfo =
                    delegate(ObjectiveList.SubStep Substep)
                    {
                        return Substep.Quest == Objective.Quest
                            && Substep.StepId == Objective.StepId
                            && Substep.SubStepId == Objective.SubStepId;
                    };

                if (Vector.GetDistance3D(Target.Position, Objective.point) < Objective.range)
                {
                    ObjectiveList.SubStep Substep = Target.QuestObjectives.Substeps.Find(SubStepInfo);
                    if (Substep != null && Substep.current < Substep.max)
                    {
                        //Send update
                        SMSG_QUESTSUBSTEPUPDATE spkt = new SMSG_QUESTSUBSTEPUPDATE();
                        spkt.Amount = (byte)(++Substep.current);
                        spkt.QuestID = Objective.Quest;
                        spkt.SessionId = Target.id;
                        spkt.StepID = Substep.StepId;
                        spkt.SubStep = (byte)Substep.SubStepId;
                        spkt.Unknown = 1;
                        Target.client.Send((byte[])spkt);

                        //Find Objective
                        if (Substep.current >= Substep.max)
                        {
                            Substep.Completed = true;
                            Target.QuestObjectives[Objective.Quest].CheckQuest(Target);
                        }
                    }
                }
            }

            foreach (ObjectiveList.Position Objective in
                Target.QuestObjectives.ScenarioPosition.FindAll(FindLootObjective))
            {
                Target.QuestObjectives.Quests[3].CheckQuest(Target);
            }
        }

        internal static IEnumerable<ObjectiveList.Waypoint> UserGetWaypoints(Character Target, uint QuestId)
        {
            for (int i = 0; i < Target.QuestObjectives.GuidancePoints.Count; i++)
            {
                ObjectiveList.Waypoint point = Target.QuestObjectives.GuidancePoints[i];
                if (point.Quest == QuestId)
                    yield return Target.QuestObjectives.GuidancePoints[i];
            }
        }

        public static IEnumerable<Rag2Item> UserQuestLoot(uint NpcId, Character Target)
        {
            Predicate<ObjectiveList.Loot> FindLootObjective =
                delegate(ObjectiveList.Loot Objective)
                {
                    return Objective.Npc == NpcId;
                };

            List<ObjectiveList.Loot> Loots =
                Target.QuestObjectives.LootObjectives.FindAll(FindLootObjective);
            if (Loots != null)
            {
                for (int i = 0; i < Loots.Count; i++)
                {
                    Rag2Item myItem;
                    ObjectiveList.Loot objective = Loots[i];

                    if (Singleton.Item.TryGetItem(objective.ItemId, out myItem))
                    {
                        if (Singleton.Itemdrops.CheckAgainstDroprate(myItem, objective.Ratio, Target._DropRate))
                        {
                            yield return myItem;
                        }
                    }
                }
            }
        }

        public static void InvalidateStep(uint Quest, uint Step, Character Target)
        {
            Predicate<ObjectiveList.Elimination> FindElimintation =
                delegate(ObjectiveList.Elimination Objective)
                {
                    return Objective.Quest == Quest &&
                            Objective.StepId == Step;
                };

            Predicate<ObjectiveList.Loot> FindLootObjective =
                delegate(ObjectiveList.Loot Objective)
                {
                    return Objective.Quest == Quest &&
                            Objective.StepId == Step;
                };

            Predicate<ObjectiveList.Position> FindPoint =
                delegate(ObjectiveList.Position Objective)
                {
                    return Objective.Quest == Quest &&
                            Objective.StepId == Step;
                };

            Predicate<ObjectiveList.Activation> ActivatedNpc =
                delegate(ObjectiveList.Activation Objective)
                {
                    return Objective.Quest == Quest &&
                            Objective.StepId == Step;
                };

            Predicate<ObjectiveList.SubStep> FindSubsteps =
                delegate(ObjectiveList.SubStep Objective)
                {
                    return Objective.Quest == Quest &&
                            Objective.StepId == Step;
                };

            Predicate<ObjectiveList.Waypoint> FindGuidancePoints =
                delegate(ObjectiveList.Waypoint Objective)
                {
                    return Objective.Quest == Quest &&
                            Objective.StepId == Step;
                };

            Target.QuestObjectives.Elimintations.RemoveAll(FindElimintation);
            Target.QuestObjectives.ActivatedNpc.RemoveAll(ActivatedNpc);
            Target.QuestObjectives.LootObjectives.RemoveAll(FindLootObjective);
            Target.QuestObjectives.Points.RemoveAll(FindPoint);
            Target.QuestObjectives.Substeps.RemoveAll(FindSubsteps);
            Target.QuestObjectives.GuidancePoints.RemoveAll(FindGuidancePoints);
        }

        public static void InvalidateQuest(QuestBase Quest, Character Target)
        {
            List<ObjectiveList.Waypoint> removedpoints = new List<ObjectiveList.Waypoint>();

            Predicate<ObjectiveList.Elimination> FindElimintation =
                delegate(ObjectiveList.Elimination Objective)
                {
                    return Objective.Quest == Quest.QuestId;
                };

            Predicate<ObjectiveList.Loot> FindLootObjective =
                delegate(ObjectiveList.Loot Objective)
                {
                    return Objective.Quest == Quest.QuestId;
                };

            Predicate<ObjectiveList.Position> FindPoint =
                delegate(ObjectiveList.Position Objective)
                {
                    return Objective.Quest == Quest.QuestId;
                };

            Predicate<ObjectiveList.Activation> ActivatedNpc =
                delegate(ObjectiveList.Activation Objective)
                {
                    return Objective.Quest == Quest.QuestId;
                };

            Predicate<ObjectiveList.SubStep> FindSubsteps =
                delegate(ObjectiveList.SubStep Objective)
                {
                    return Objective.Quest == Quest.QuestId;
                };

            Predicate<ObjectiveList.Waypoint> FindGuidancePoints =
                delegate(ObjectiveList.Waypoint Objective)
                {
                    bool isQuest = Objective.Quest == Quest.QuestId;
                    if (isQuest)
                        removedpoints.Add(Objective);
                    return isQuest;
                };

            Predicate<ObjectiveList.Loot2> FindNonDiscardableItems =
                delegate(ObjectiveList.Loot2 objective)
                {
                    return objective.Quest == Quest.QuestId;
                };

            Target.QuestObjectives.Elimintations.RemoveAll(FindElimintation);
            Target.QuestObjectives.ActivatedNpc.RemoveAll(ActivatedNpc);
            Target.QuestObjectives.LootObjectives.RemoveAll(FindLootObjective);
            Target.QuestObjectives.Points.RemoveAll(FindPoint);
            Target.QuestObjectives.Substeps.RemoveAll(FindSubsteps);
            Target.QuestObjectives.Steps.Remove(Quest.QuestId);
            Target.QuestObjectives.NonDiscardableItems.RemoveAll(FindNonDiscardableItems);
            Target.QuestObjectives.GuidancePoints.RemoveAll(FindGuidancePoints);

            foreach (ObjectiveList.Waypoint objective in removedpoints)
            {
                Regiontree tree = Target.currentzone.Regiontree;
                foreach (MapObject a in tree.SearchActors(Target, SearchFlags.Npcs))
                {
                    BaseMob mob = a as BaseMob;
                    if (a.ModelId == objective.NpcId && mob != null)
                        Common.Actions.UpdateIcon(Target, mob);
                }
            }
        }

        internal static List<ObjectiveList.StepInfo> GetSteps(Character Target, uint Quest)
        {
            List<ObjectiveList.StepInfo> info;
            if (Target.QuestObjectives.Steps.TryGetValue(Quest, out info))
            {
                return info;
            }
            else
            {
                info = new List<ObjectiveList.StepInfo>();
                Target.QuestObjectives.Steps[Quest] = info;
                return info;
            }
        }

        #endregion Static Methods

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("index", index);
            info.AddValue("isnew", isnew);
            info.AddValue("cleared", IsWaypointsCleared);
            info.AddValue("questid", QuestId);
            info.AddValue("questtype", questtype);
        }

        public QuestBase()
        {
        }

        protected QuestBase(SerializationInfo info, StreamingContext context)
        {
            index = info.GetUInt32("index");
            isnew = info.GetBoolean("isnew");
            IsWaypointsCleared = info.GetBoolean("cleared");
            QuestId = info.GetUInt32("questid");
            questtype = info.GetByte("questtype");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        #endregion ISerializable Members
    }

    internal class QuestFailedException : Exception { }

    public interface ISceneraioQuest { }

    public interface IQuest { }
}

namespace Saga.Quests.Objectives
{
    [Serializable()]
    public class ObjectiveList : IEnumerable<QuestBase>
    {
        #region Internal Members

        internal readonly QuestBase[] Quests = new QuestBase[4];
        internal readonly List<Elimination> Elimintations = new List<Elimination>();
        internal readonly List<Activation> ActivatedNpc = new List<Activation>();
        internal readonly List<Waypoint> GuidancePoints = new List<Waypoint>();
        internal readonly List<Position> Points = new List<Position>();
        internal readonly List<Loot> LootObjectives = new List<Loot>();
        internal readonly List<SubStep> Substeps = new List<SubStep>();
        internal readonly List<Loot2> NonDiscardableItems = new List<Loot2>();
        internal readonly Dictionary<uint, List<StepInfo>> Steps = new Dictionary<uint, List<StepInfo>>();
        internal readonly Dictionary<uint, List<StepInfo>> ScenarioSteps = new Dictionary<uint, List<StepInfo>>();
        internal readonly List<Position> ScenarioPosition = new List<Position>();

        internal bool WaypointsCleared = false;

        #endregion Internal Members

        #region Public Members

        public QuestBase ScenarioQuest
        {
            get
            {
                return Quests[3];
            }
        }

        public QuestBase PersonalQuest
        {
            get
            {
                return Quests[1];
            }
            set
            {
                Quests[1] = value;
            }
        }

        public QuestBase OfficialQuest
        {
            get
            {
                return Quests[0];
            }
            set
            {
                Quests[0] = value;
            }
        }

        public QuestBase this[uint QuestId]
        {
            get
            {
                for (int i = 0; i < 3; i++)
                {
                    if (Quests[i] == null) continue;
                    if (Quests[i].QuestId == QuestId) return Quests[i];
                }
                return null;
            }
            set
            {
                for (int i = 0; i < 3; i++)
                {
                    if (Quests[i] == null) continue;
                    if (Quests[i].QuestId == QuestId) Quests[i] = value;
                }
            }
        }

        public IEnumerator<QuestBase> GetEnumerator()
        {
            for (int i = 0; i < 3; i++)
            {
                if (Quests[i] != null)
                    yield return Quests[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int FindNextSubStep(uint Quest, uint StepId)
        {
            //Find the highest stepindex
            int stepindex = -1;
            for (int i = 0; i < Substeps.Count; i++)
            {
                SubStep step = Substeps[i];
                if (step.Quest == Quest && step.StepId == StepId)
                    stepindex = (int)step.SubStepId;
            }

            //Gets the next stepindex;
            return stepindex + 1;
        }

        public bool IsSubstepAdded(uint QuestId, uint StepId, uint Substep)
        {
            Predicate<SubStep> FindSubStep = delegate(SubStep substepinf)
            {
                return substepinf.Quest == QuestId
                    && substepinf.StepId == StepId
                    && substepinf.SubStepId == Substep;
            };

            return this.Substeps.Exists(FindSubStep);
        }

        #endregion Public Members

        #region Serialisation

        public static void Serialize(Stream stream, ObjectiveList list)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                if (list != null)
                    formatter.Serialize(stream, list);
            }
            catch (SerializationException e)
            {
                Trace.TraceError("Failed to serialize. Reason: " + e.Message);
                throw;
            }
        }

        public static void Deserialize(Stream stream, out ObjectiveList list)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                list = formatter.Deserialize(stream) as ObjectiveList;
            }
            catch (SerializationException e)
            {
                Trace.TraceError("Failed to deserialize. Reason: " + e.Message);
                list = null;
                throw;
            }
        }

        #endregion Serialisation

        #region Nested Classes

        [Serializable()]
        protected internal class QuestObjective
        {
            public uint Quest;
            public uint StepId;

            public QuestObjective(uint baseQuest, uint stepId)
            {
                Quest = baseQuest;
                StepId = stepId;
            }
        }

        [Serializable()]
        protected internal class SubStep : QuestObjective
        {
            public int SubStepId;
            public int current;
            public int max;
            public bool Completed;

            public SubStep(int SubStepId, bool Completed, int current, int max, uint quest, uint StepId)
                : base(quest, StepId)
            {
                this.SubStepId = SubStepId;
                this.Completed = Completed;
                this.current = current;
                this.max = max;
            }
        }

        [Serializable()]
        protected internal class StepInfo : QuestObjective
        {
            public byte State;

            public StepInfo(byte State, uint quest, uint StepId)
                : base(quest, StepId)
            {
                this.State = State;
            }
        }

        [Serializable()]
        protected internal class Elimination : QuestObjective
        {
            public uint NpcId;
            public int SubStepId;

            public Elimination(uint NpcId, uint quest, uint StepId, int SubStepId)
                : base(quest, StepId)
            {
                this.NpcId = NpcId;
                this.SubStepId = SubStepId;
            }
        }

        [Serializable()]
        protected internal class Loot : QuestObjective
        {
            public uint ItemId;
            public uint Ratio;
            public uint Npc;
            public int SubStepId;

            public Loot(uint ItemId, uint Ratio, uint Npc, uint quest, uint StepId, int SubStepId)
                : base(quest, StepId)
            {
                this.ItemId = ItemId;
                this.Ratio = Ratio;
                this.Npc = Npc;
                this.SubStepId = SubStepId;
            }
        }

        [Serializable()]
        protected internal class Loot2 : QuestObjective
        {
            public uint ItemId;

            public Loot2(uint ItemId, uint quest, uint StepId)
                : base(quest, StepId)
            {
                this.ItemId = ItemId;
            }
        }

        [Serializable()]
        protected internal class Activation : QuestObjective
        {
            public uint NpcId;

            public Activation(uint NpcId, uint quest, uint StepId)
                : base(quest, StepId)
            {
                this.NpcId = NpcId;
            }
        }

        [Serializable()]
        protected internal class Waypoint : QuestObjective
        {
            public uint NpcId;
            public int SubStepId;

            public Waypoint(uint NpcId, uint quest, uint StepId, int SubStepId)
                : base(quest, StepId)
            {
                this.NpcId = NpcId;
                this.SubStepId = SubStepId;
            }
        }

        [Serializable()]
        protected internal class Position : QuestObjective
        {
            public Point point;
            public byte mapid;
            public uint range;
            public int SubStepId;

            public Position(float x, float y, float z, byte mapid, uint range, uint quest, uint step, int SubStepId)
                : base(quest, step)
            {
                this.point = new Point(x, y, z);
                this.SubStepId = SubStepId;
                this.mapid = mapid;
                this.range = range;
            }
        }

        #endregion Nested Classes
    }
}