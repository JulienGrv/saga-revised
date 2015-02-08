using Saga.Enumarations;
using Saga.IO;
using Saga.Map;
using Saga.Npc.Functions;
using Saga.PrimaryTypes;
using Saga.Quests;
using Saga.Structures;
using System;
using System.Collections.Generic;
using System.IO;

namespace Saga.Templates
{
    public abstract class BaseNPC : BaseMob
    {
        #region Base Members

        public uint Zeny = int.MaxValue;
        protected internal FunctionState state = new FunctionState();
        protected static TraceLog log = new TraceLog("Scripting", "Provides a scripting tracelog", 3);

        #endregion Base Members

        #region Public Members

        public DialogType[] GetDialogButtons(Character target)
        {
            List<DialogType> types = new List<DialogType>();
            types.AddRange(this.state.GetDialogButtons(this, target));

            DialogType[] typesa = types.ToArray();
            return typesa;
        }

        public override int ComputeIcon(Character target)
        {
            int dialog = 0;
            if (target.client.AvailablePersonalRequests.ContainsKey(this.ModelId))
                dialog |= 1;
            if (QuestBase.IsTalkToObjective(this.ModelId, target))
                dialog |= 2;
            return (int)(base.ComputeIcon(target) | dialog);
        }

        #endregion Public Members

        #region Virtual Members

        /// <summary>
        /// Spawns the npc.
        /// </summary>
        /// <remarks>
        /// This calles Template manger to fill the npc's position.
        /// </remarks>
        public override void OnSpawn()
        {
            Singleton.Templates.FillByTemplate(this.ModelId, this);
            base.OnSpawn();
        }

        protected void RefreshNpcFunctions()
        {
            foreach (NpcFunction f in this.state.GetNpcFunctions())
                f.OnRefresh(this);
        }

        protected uint _Gossip = 1810;

        public virtual void OnGossip(Character target)
        {
            Common.Actions.OpenMenu(
                target, this,
                _Gossip,
                DialogType.None,
                GetDialogButtons(target)
            );
        }

        public virtual void OnRefresh()
        {
            RefreshNpcFunctions();
        }

        public virtual void OnCacheConversations()
        {
            //HELPER VARABLES
            string file = Server.SecurePath("~/dialogtemplates/{0}.xml", this.ModelId);
            if (File.Exists(file))
                using (DialogReader reader = DialogReader.Open(file))
                {
                    while (reader.Read())
                    {
                        if (reader.HasInformation)
                        {
                            //Always uppercase (invariant for globalisation issues)
                            string name = reader.Name.ToUpperInvariant();
                            if (name == "GOSSIP")
                            {
                                //Gossip is a base function of ours
                                _Gossip = reader.Value;
                            }
                            else
                            {
                                //Parse to all child registered functions
                                for (int i = 0; i < state.RegisteredNpcFunctions.Count; i++)
                                {
                                    state.RegisteredNpcFunctions[i].OnCacheDialogInfo(this, reader.Name, reader.Value);
                                }
                            }
                        }
                    }
                }
        }

        public override void OnRegister()
        {
            base.OnRegister();
        }

        /// <summary>
        /// Default callback for recontruction the dialog messages.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        #endregion Virtual Members

        #region MapObject Members

        public override void OnLoad()
        {
            try
            {
                Initialize();
                OnCacheConversations();
            }
            catch (Exception)
            {
                log.WriteError("Npc", "Error loading file on caching conversations on npc: {0}", this.ModelId);
            }
        }

        public override void Disappear(Character character)
        {
            //DO NOTHING HERE
            if (character.Target == this)
                character._target = null;
        }

        public override void Appears(Character character)
        {
            //DO NOTHING HERE
        }

        #endregion MapObject Members
    }
}