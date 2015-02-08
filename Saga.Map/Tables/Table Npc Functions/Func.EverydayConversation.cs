using Saga.Enumarations;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Templates;
using System.Collections.Generic;

namespace Saga.Npc.Functions
{
    /// <summary>
    /// Everyday conversation is to be added to a npc to show the default
    /// everyday interactions.
    /// </summary>
    public class EverydayConversation : NpcFunction
    {
        #region Private Members

        /// <summary>
        /// List of dialog to show in sequentional order
        /// </summary>
        protected List<uint> _EverydayConversation = new List<uint>();

        #endregion Private Members

        #region Protected Methods

        /// <summary>
        /// Registers all the menu's and buttons on the npc.
        /// </summary>
        /// <param name="npc">Npc who to register with</param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.EverydayConversation, new FunctionCallback(OnConversation));
        }

        /// <summary>
        /// Loads all dialog dialog information.
        /// </summary>
        /// <remarks>
        /// The information is loaded from the data folder in dialogtemplates/{npcmodelid}.xml
        /// </remarks>
        /// <param name="npc">Npc who requires caching</param>
        /// <param name="name">Name of the found string</param>
        /// <param name="value">Value of the found string</param>
        protected internal override void OnCacheDialogInfo(BaseNPC npc, string name, uint value)
        {
            switch (name.ToUpperInvariant())
            {
                case "EVERYDAYCONVERSATION": _EverydayConversation.Add(value); break;
            }
        }

        /// <summary>
        /// Occurs after the EverydayConversation button is pressed
        /// by the client.
        /// </summary>
        /// <param name="npc">Npc who hold the button</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnConversation(BaseNPC npc, Character target)
        {
            EverydaySequence seq = target.Tag is EverydaySequence ? target.Tag as EverydaySequence : new EverydaySequence(this._EverydayConversation);
            target.Tag = seq;
            if (seq.HasInfo)
            {
                Common.Actions.OpenMenu(target, npc, seq.Current,
                    DialogType.EverydayConversation,
                    npc.GetDialogButtons(target)
                );

                seq.Increment();
            }
        }

        #endregion Protected Methods

        #region Nested Types

        protected class EverydaySequence
        {
            public EverydaySequence(List<uint> dialogs)
            {
                this.dialogs = dialogs;
            }

            public void Increment()
            {
                current = ++current < dialogs.Count ? (byte)current : (byte)0;
            }

            public bool HasInfo
            {
                get
                {
                    return current < dialogs.Count;
                }
            }

            public uint Current
            {
                get
                {
                    return this.dialogs[current];
                }
            }

            private List<uint> dialogs;
            private byte current;
        }

        #endregion Nested Types
    }
}