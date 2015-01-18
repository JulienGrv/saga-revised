using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saga.Quests;
using LuaInterface;
using System.IO;
using System.Runtime.Serialization;

namespace Saga.Map.Data.LuaQuest
{

    [Serializable()]
    public class ScenarioLuaQuest : QuestBase, IDeserializationCallback, IDisposable, ISceneraioQuest
    {
        #region LuaQuest - Private Members

        private Lua myLua = new Lua();

        #endregion

        #region LuaQuest - Public Members

        /// <summary>
        /// Quest Start is handeld, 
        /// To start the quest e.d. add items to inventory, add new waypoints
        /// </summary>
        /// <returns></returns>
        public override int OnStart(uint cid)
        {
            try
            {
                string luachunk = String.Format(System.Globalization.CultureInfo.InvariantCulture, "return QUEST_START({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0), System.Globalization.CultureInfo.InvariantCulture);
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
                string luachunk = String.Format(System.Globalization.CultureInfo.InvariantCulture, "return QUEST_FINISH({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0), System.Globalization.CultureInfo.InvariantCulture);
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
                string luachunk = String.Format(System.Globalization.CultureInfo.InvariantCulture, "return QUEST_CANCEL({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0), System.Globalization.CultureInfo.InvariantCulture);
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
                string luachunk = String.Format(System.Globalization.CultureInfo.InvariantCulture, "return QUEST_VERIFY({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0), System.Globalization.CultureInfo.InvariantCulture);
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
                string luachunk = String.Format(System.Globalization.CultureInfo.InvariantCulture, "return QUEST_CHECK({0});", cid);
                return Convert.ToInt32(myLua.DoString(luachunk).GetValue(0), System.Globalization.CultureInfo.InvariantCulture);
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
        /// Binds all 
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public override bool Initialize(uint cid, string BaseDirectory)
        {
            try
            {
                string filename = Path.Combine(BaseDirectory, string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.lua", cid));
                myLua.NewTable("Saga");
                myLua.RegisterFunction("Saga.GetStepIndex", null, typeof(Saga.Quests.Scenario.QUEST_TABLE).GetMethod("GetCurrentStep2"));
                myLua.RegisterFunction("Saga.AddStep", null, typeof(Saga.Quests.Scenario.QUEST_TABLE).GetMethod("AddStep"));
                myLua.RegisterFunction("Saga.QuestStart", null, typeof(Saga.Quests.Scenario.QUEST_TABLE).GetMethod("StartQuest"));
                myLua.RegisterFunction("Saga.QuestComplete", null, typeof(Saga.Quests.Scenario.QUEST_TABLE).GetMethod("CompleteQuest"));
                myLua.RegisterFunction("Saga.StepComplete", null, typeof(Saga.Quests.Scenario.QUEST_TABLE).GetMethod("StepComplete2"));                
                myLua.RegisterFunction("Saga.StartEvent", null, typeof(Saga.Quests.Scenario.QUEST_TABLE).GetMethod("StartEvent"));
                myLua.RegisterFunction("Saga.FindPosition", null, typeof(Saga.Quests.Scenario.QUEST_TABLE).GetMethod("FindPosition"));                
                myLua.RegisterFunction("print", null, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
                myLua.DoFile(filename);
                return true;
            }
            catch (LuaException e)
            {
                __dbtracelog.WriteError("quest", "In quest {0} a error occured while loading: {1}", cid, e.Message);
                if (__dbtracelog.TraceVerbose)
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

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            myLua.Dispose();
        }

        #endregion

        #region Constructor

        public ScenarioLuaQuest() { }
        protected ScenarioLuaQuest(SerializationInfo info, StreamingContext context)
            : base (info, context){}


        ~ScenarioLuaQuest()
        {
            Dispose();
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            __dbtracelog.WriteInformation("quest", "Deserialize scenario quest: {0}", this.QuestId);
            Initialize(this.QuestId, Singleton.Quests.ScenarioDirectory);
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
