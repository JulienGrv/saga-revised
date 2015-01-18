using System;
using System.IO;
using LuaInterface;
using Saga.Quests;
using System.Reflection;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Saga.Map.Data.LuaQuest
{

    [Serializable()]
    public class LuaQuestProvider : QuestBase, IDeserializationCallback, IQuest
    {

        #region LuaQuestProvider - Private Members

        private Lua myLua = new Lua();

        #endregion

        #region LuaQuestProvider - Public Members

        /// <summary>
        /// Quest Start is handeld, 
        /// To start the quest e.d. add items to inventory, add new waypoints
        /// </summary>
        /// <returns></returns>
        public override int OnStart(uint cid)
        {
            try
            {
                GC.SuppressFinalize(myLua);
                string luachunk = String.Format("return QUEST_START({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0));
            }
            catch (LuaException e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error occured in 'start': {1}", this.QuestId, e.Message);
                if (__dbtracelog.TraceVerbose)
                Console.WriteLine("Error in quest {0}, see tracelog", this.QuestId);
                return -1;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error unhandeld error occured in 'start': {1}", cid, e.Message);
                if (__dbtracelog.TraceVerbose)
                    Console.WriteLine("Error in quest {0}, see tracelog", cid);
                return -1;
            }
        }

        /// <summary>
        /// Quest Finish is triggerd 
        /// When the client presses the Finish button that appears when
        /// you complete all steps.
        /// </summary>
        /// <remarks>
        /// This function should be used to give the appropiate awards
        /// </remarks>
        /// <returns></returns>
        public override int OnFinish(uint cid)
        {
            try
            {
                GC.SuppressFinalize(myLua);
                string luachunk = String.Format("return QUEST_FINISH({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0));
            }
            catch (LuaException e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error occured in 'finish': {1}", this.QuestId, e.Message);
                if (__dbtracelog.TraceVerbose)
                Console.WriteLine("Error in quest {0}, see tracelog", this.QuestId);
                return -1;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error unhandeld error occured in 'finish': {1}", cid, e.Message);
                if (__dbtracelog.TraceVerbose)
                    Console.WriteLine("Error in quest {0}, see tracelog", cid);
                return -1;
            }
        }


        /// <summary>
        /// Quest Cancel is triggerd:
        /// When the client abbons the quest when it has already started. This script
        /// should be used to clean up any inventory items that were given by the npc 
        /// which aren't rewards.
        /// </summary>
        /// <returns></returns>
        public override int OnCancel(uint cid)
        {
            try
            {
                GC.SuppressFinalize(myLua);
                string luachunk = String.Format("return QUEST_CANCEL({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0));
            }
            catch (LuaException e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error occured in 'cancel': {1}", this.QuestId, e.Message);
                if (__dbtracelog.TraceVerbose)
                Console.WriteLine("Error in quest {0}, see tracelog", this.QuestId);
                return -1;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error unhandeld error occured in 'cancel': {1}", cid, e.Message);
                if (__dbtracelog.TraceVerbose)
                    Console.WriteLine("Error in quest {0}, see tracelog", cid);
                return -1;
            }
        }

        /// <summary>
        /// Quest Cancel is triggerd:
        /// When the client abbons the quest when it has already started. This script
        /// should be used to clean up any inventory items that were given by the npc 
        /// which aren't rewards.
        /// </summary>
        /// <returns></returns>
        public override int OnVerify(uint cid)
        {
            try
            {
                GC.SuppressFinalize(myLua);
                string luachunk = String.Format("return QUEST_VERIFY({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0));
            }
            catch (LuaException e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error occured in 'verify': {1}", this.QuestId, e.Message);
                if (__dbtracelog.TraceVerbose)
                Console.WriteLine("Error in quest {0}, see tracelog", this.QuestId);
                return -1;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error unhandeld error occured in 'verify': {1}", cid, e.Message);
                if (__dbtracelog.TraceVerbose)
                    Console.WriteLine("Error in quest {0}, see tracelog", cid);
                return -1;
            }
        }


        /// <summary>
        /// Check Quest is triggered
        /// When a client talks to a npc, obtains a new items
        /// </summary>
        /// <returns></returns>
        public override int OnCheckQuest(uint cid)
        {
            try
            {
                GC.SuppressFinalize(myLua);
                string luachunk = String.Format("return QUEST_CHECK({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0));
            }
            catch (LuaException e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error occured in 'check': {1}", this.QuestId, e.Message);
                if (__dbtracelog.TraceVerbose)
                Console.WriteLine("Error in quest {0}, see tracelog", this.QuestId);
                return -1;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error unhandeld error occured in 'check': {1}", cid, e.Message);
                if (__dbtracelog.TraceVerbose)
                    Console.WriteLine("Error in quest {0}, see tracelog", cid);
                return -1;
            }
        }

        /// <summary>
        /// Binds all functions
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public override bool Initialize(uint cid, string BaseDirectory)
        {
            try
            {
                string filename = Path.Combine(BaseDirectory, string.Format("{0}.lua", cid));
                myLua.NewTable("Saga");

                BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
                myLua.RegisterFunction("Saga.FindQuestItem", null, typeof(QUEST_TABLE).GetMethod("FindQuestItem", flags));
                myLua.RegisterFunction("Saga.Eliminate", null, typeof(QUEST_TABLE).GetMethod("ObjectiveElemenation", flags));
                myLua.RegisterFunction("Saga.GetStepIndex", null, typeof(QUEST_TABLE).GetMethod("GetCurrentStep"));
                myLua.RegisterFunction("Saga.GetNPCIndex", null, typeof(QUEST_TABLE).GetMethod("GetNPCIndex"));
                myLua.RegisterFunction("Saga.GetActionObjectIndex", null, typeof(QUEST_TABLE).GetMethod("GetActionObjectIndex"));
                myLua.RegisterFunction("Saga.UserQuestFail", null, typeof(QUEST_TABLE).GetMethod("QuestFail"));
                myLua.RegisterFunction("Saga.AddStep", null, typeof(QUEST_TABLE).GetMethod("AddStep"));
                myLua.RegisterFunction("Saga.StepComplete", null, typeof(QUEST_TABLE).GetMethod("StepComplete"));
                myLua.RegisterFunction("Saga.QuestComplete", null, typeof(QUEST_TABLE).GetMethod("QuestComplete"));
                myLua.RegisterFunction("Saga.GiveItem", null, typeof(QUEST_TABLE).GetMethod("GiveItem"));
                myLua.RegisterFunction("Saga.GiveZeny", null, typeof(QUEST_TABLE).GetMethod("GiveZeny"));
                myLua.RegisterFunction("Saga.GiveExp", null, typeof(QUEST_TABLE).GetMethod("GiveExp"));
                myLua.RegisterFunction("Saga.GeneralDialog", null, typeof(QUEST_TABLE).GetMethod("GeneralDialog"));
                myLua.RegisterFunction("Saga.AddWaypoint", null, typeof(QUEST_TABLE).GetMethod("AddWayPoint"));
                myLua.RegisterFunction("Saga.ClearWaypoints", null, typeof(QUEST_TABLE).GetMethod("ClearWayPoints"));
                myLua.RegisterFunction("Saga.UserUpdateActionObjectType", null, typeof(QUEST_TABLE).GetMethod("UserUpdateActionObjectType"));
                myLua.RegisterFunction("Saga.CheckUserInventory", null, typeof(QUEST_TABLE).GetMethod("CheckUserInventory"));
                myLua.RegisterFunction("Saga.FreeInventoryCount", null, typeof(QUEST_TABLE).GetMethod("FreeInventoryCount"));
                myLua.RegisterFunction("Saga.NpcTakeItem", null, typeof(QUEST_TABLE).GetMethod("NpcTakeItem"));
                myLua.RegisterFunction("Saga.NpcGiveItem", null, typeof(QUEST_TABLE).GetMethod("NpcGiveItem"));
                myLua.RegisterFunction("Saga.InsertQuest", null, typeof(QUEST_TABLE).GetMethod("InsertQuest"));
                myLua.RegisterFunction("Saga.RestoreDiscardableItems", null, typeof(QUEST_TABLE).GetMethod("RestoreDiscardableItems"));
                myLua.RegisterFunction("Saga.FindPosition", null, typeof(QUEST_TABLE).GetMethod("FindPosition"));
                myLua.RegisterFunction("Saga.SubstepComplete", null, typeof(QUEST_TABLE).GetMethod("SubStepComplete"));
                myLua.RegisterFunction("Saga.IsSubStepCompleted", null, typeof(QUEST_TABLE).GetMethod("IsSubStepCompleted"));
                myLua.RegisterFunction("Saga.EmptyInventory", null, typeof(QUEST_TABLE).GetMethod("SendEmptyInventory"));
                myLua.RegisterFunction("Saga.ItemNotFound", null, typeof(QUEST_TABLE).GetMethod("SendInventoryNotFound"));
                myLua.RegisterFunction("Saga.SpawnNpc", null, typeof(QUEST_TABLE).GetMethod("Spawn"));

                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs, true))
                {
                    myLua.DoString(reader.ReadToEnd());
                }

                this.QuestId = cid;

                return true;
            }
            catch (LuaException e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error occured while loading: {1}", cid, e.Message);
                if( __dbtracelog.TraceVerbose )
                Console.WriteLine("Error in quest {0}, see tracelog", cid);
                return false;
            }
            catch (FileNotFoundException)
            {
                __dbtracelog.WriteWarning("quest", "Quest {0} was not found and could not be loaded", cid);
                if (__dbtracelog.TraceVerbose)
                Console.WriteLine("Error in quest {0}, see tracelog", cid);
                return false;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error unhandeld error occured while loading: {1}", cid, e.Message);
                if (__dbtracelog.TraceVerbose)
                Console.WriteLine("Error in quest {0}, see tracelog", cid);
                return false;
            }
        }

        #endregion

        #region IDeserializationCallback Members

        public LuaQuestProvider() { }

        protected LuaQuestProvider(SerializationInfo info, StreamingContext context)
            : base (info, context){}


        public void OnDeserialization(object sender)
        {
            __dbtracelog.WriteInformation("quest", "Deserialize personal/official quest: {0}", this.QuestId);
            Initialize(this.QuestId, Singleton.Quests.QuestDirectory);
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            OnDeserialization(sender);
        }

        #endregion

        #region ITrace Members

        static TraceLog __dbtracelog = new TraceLog("Lua", "Trace switch for the lua plugin", 4);

        #endregion

    }
}
