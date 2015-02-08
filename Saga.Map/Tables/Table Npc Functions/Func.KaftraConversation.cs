using Saga.Enumarations;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Templates;

namespace Saga.Npc.Functions
{
    /// <summary>
    /// This is a kaftra conversation. Include this in a npc to show a
    /// kaftra dialog and submenu.
    /// </summary>
    public class KaftraConversation : NpcFunction
    {
        #region Private Members

        /// <summary>
        /// Dialog to say after saved location
        /// </summary>
        public uint _SaveLocation = 823;

        /// <summary>
        /// Dialog to say after storage is opened
        /// </summary>
        public uint _Storage = 823;

        /// <summary>
        /// Dialog to say after Kaftra service is opened
        /// </summary>
        public uint _KaftraService = 823;

        #endregion Private Members

        #region Protected Methods

        /// <summary>
        /// Registers all the menu's and buttons on the npc.
        /// </summary>
        /// <param name="npc">Npc who to register with</param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.Kaftra, new FunctionCallback(OnKaftraService));
            RegisterDialog(npc, DialogType.Kaftra, 10, new FunctionCallback(OnSaveLocation));
            RegisterDialog(npc, DialogType.Kaftra, 11, new FunctionCallback(OnWarehouse));
            RegisterDialog(npc, DialogType.EventA, new FunctionCallback(OnEvents));
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
                case "SAVELOCATION": _SaveLocation = value; break;
                case "STORAGE": _Storage = value; break;
                case "KAFTRASERVICE": _KaftraService = value; break;
            }
        }

        /// <summary>
        /// Occurs after the Kaftra button is pressed.
        /// </summary>
        /// <param name="npc">Npc who called the event</param>
        /// <param name="target">Character who requirs interaction</param>
        protected virtual void OnKaftraService(BaseNPC npc, Character target)
        {
            Common.Actions.OpenSubmenu(
                target, npc,
                _KaftraService,
                DialogType.Kaftra,
                10,
                11
            );
        }

        /// <summary>
        /// Occurs after the Event button is pressed.
        /// </summary>
        /// <param name="npc">Npc who called the event</param>
        /// <param name="target">Character who requirs interaction</param>
        protected virtual void OnEvents(BaseNPC npc, Character target)
        {
            Common.Actions.ShowEvents(target);
        }

        /// <summary>
        /// Occurs after submenu 'Save Location' is pressed.
        /// </summary>
        /// <param name="npc">Npc who called the event</param>
        /// <param name="target">Character who requirs interaction</param>
        protected virtual void OnSaveLocation(BaseNPC npc, Character target)
        {
            Common.Actions.OpenMenu(
                target, npc,
                _SaveLocation,
                DialogType.Kaftra,
                new byte[] { }
            );

            target.currentzone.SaveLocation(target);

            //Common.Actions(target);
        }

        /// <summary>
        /// Occurs after submenu 'Warehouse' is pressed.
        /// </summary>
        /// <param name="npc">Npc who called the event</param>
        /// <param name="target">Character who requirs interaction</param>
        protected virtual void OnWarehouse(BaseNPC npc, Character target)
        {
            Common.Actions.OpenMenu(
                target, npc,
                _Storage,
                DialogType.Kaftra,
                new byte[] { }
            );

            //CommonFunctions.ShowDialog(target, _Storage);
            Common.Actions.ShowStorage(target);
        }

        #endregion Protected Methods
    }
}