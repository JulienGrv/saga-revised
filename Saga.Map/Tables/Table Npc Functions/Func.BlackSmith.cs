using Saga.Enumarations;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Templates;
using System.Collections.Generic;
using System.Diagnostics;

namespace Saga.Npc.Functions
{
    public sealed class FunctionState
    {
        #region Private Members

        /// <summary>
        /// Dictonairy to hold container functions
        /// </summary>
        private Dictionary<byte, ButtonCallback> __callbacks =
            new Dictionary<byte, ButtonCallback>();

        /// <summary>
        /// List of hosted npc functions
        /// </summary>
        internal List<NpcFunction> RegisteredNpcFunctions =
            new List<NpcFunction>();

        #endregion Private Members

        #region Private Methods

        private bool CheckDialog(DialogType dialog, BaseNPC npc, Character target)
        {
            bool result = true;
            for (int i = 0; i < RegisteredNpcFunctions.Count; i++)
            {
                result &= RegisteredNpcFunctions[i].OnCheckDialogIsVisible(npc, dialog, target);
            }
            return result;
        }

        #endregion Private Methods

        #region Public Methods

        public IEnumerable<NpcFunction> GetNpcFunctions()
        {
            for (int i = 0; i < RegisteredNpcFunctions.Count; i++)
            {
                yield return RegisteredNpcFunctions[i];
            }
        }

        public IEnumerable<DialogType> GetDialogButtons(BaseNPC npc, Character target)
        {
            foreach (KeyValuePair<byte, ButtonCallback> pair in __callbacks)
            {
                DialogType dialog = (DialogType)pair.Key;
                if (CheckDialog(dialog, npc, target))
                {
                    yield return dialog;
                }
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal bool Open(byte button, BaseNPC npc, Character target)
        {
            bool result = false;
            ButtonCallback callback;

            if (__callbacks.TryGetValue(button, out callback) && callback != null)
            {
                result = callback.callback != null;
                if (result == true)
                {
                    callback.callback.Invoke(npc, target);
                }
            }
            return result;
        }

        internal bool Open(byte button, byte menu, BaseNPC npc, Character target)
        {
            //HELPDER VARIABLES
            bool result = false;
            NpcFunction.FunctionCallback callback2;
            ButtonCallback callback;

            //START INVOKING THE COMMAND
            if (__callbacks.TryGetValue(button, out callback) && callback != null)
                if (callback.callbacks.TryGetValue(menu, out callback2))
                {
                    result = callback2 != null;
                    if (result == true)
                        callback2.Invoke(npc, target);
                }

            return result;
        }

        internal void Recache()
        {
        }

        /// <summary>
        /// Adds a menu callback
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        [DebuggerNonUserCode()]
        internal void Add(DialogType id, NpcFunction.FunctionCallback callback)
        {
            ButtonCallback mycallback = new ButtonCallback();
            mycallback.callback = callback;
            __callbacks.Add((byte)id, mycallback);
        }

        /// <summary>
        /// Adds a summenu callback
        /// </summary>
        /// <param name="id"></param>
        /// <param name="menu"></param>
        /// <param name="callback"></param>
        [DebuggerNonUserCode()]
        internal void Add(DialogType id, byte menu, NpcFunction.FunctionCallback callback)
        {
            ButtonCallback mycallback = __callbacks[(byte)id];
            mycallback.callbacks.Add(menu, callback);
        }

        #endregion Internal Methods

        #region Nested Classes/Structures

        /// <summary>
        /// Class container for holding button callnaks
        /// </summary>
        private class ButtonCallback
        {
            /// <summary>
            /// Default button callback
            /// </summary>
            internal NpcFunction.FunctionCallback callback;

            /// <summary>
            /// Submenu button callbacks
            /// </summary>
            internal Dictionary<byte, NpcFunction.FunctionCallback> callbacks
                = new Dictionary<byte, NpcFunction.FunctionCallback>();
        }

        #endregion Nested Classes/Structures
    }

    /// <summary>
    /// BlackSmith conversation is to be added to a npc to show the default
    /// blacksmith interactions.
    /// </summary>
    public class BlackSmith : NpcFunction
    {
        #region Private Members

        /// <summary>
        /// Dialog Id to show when opening the blacksmith menu
        /// </summary>
        protected uint _BlackSmithMenu = 1842;

        /// <summary>
        /// Dialog Id to show when opening the repair equipment menu
        /// </summary>
        protected uint _RepairEquipment = 1845;

        /// <summary>
        /// Dialog Id to show when opening the weapon change menu
        /// </summary>
        protected uint _WeaponChange = 1851;

        /// <summary>
        /// Dialog Id to show when opening the weapon upgrade menu
        /// </summary>
        protected uint _WeaponUpgrade = 1848;

        /// <summary>
        /// Dialog Id to show when opening the suffix change menu
        /// </summary>
        protected uint _WeaponSuffixChange = 823;

        #endregion Private Members

        #region Protected Methods

        /// <summary>
        /// Registers all menu's
        /// </summary>
        /// <param name="npc">Npc who calls for registration</param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.Smith, new FunctionCallback(OnBlackSmithMenu));
            RegisterDialog(npc, DialogType.Smith, 50, new FunctionCallback(OnRepairEquipment));
            RegisterDialog(npc, DialogType.Smith, 52, new FunctionCallback(OnChangeWeapon));
            RegisterDialog(npc, DialogType.Smith, 53, new FunctionCallback(OnUpgradeWeapon));
            RegisterDialog(npc, DialogType.Smith, 55, new FunctionCallback(OnChangeWeaponSuffix));
        }

        /// <summary>
        /// Caches all dialog information
        /// </summary>
        /// <param name="npc">Npc who calls for registration</param>
        protected internal override void OnCacheDialogInfo(BaseNPC npc, string name, uint value)
        {
            switch (name.ToUpperInvariant())
            {
                case "BLACKSMITHMENU": _BlackSmithMenu = value; break;
                case "WEAPONTYPECHANGE": _WeaponChange = value; break;
                case "SUFFIXCHANGE": _WeaponSuffixChange = value; break;
                case "WEAPONUPGRADE": _WeaponUpgrade = value; break;
                case "REPAIREQUIPMENT": _RepairEquipment = value; break;
            }
        }

        /// <summary>
        /// Occurs when opening the blacksmith menu
        /// </summary>
        /// <param name="npc">Npc who requires shows the menu</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnBlackSmithMenu(BaseNPC npc, Character target)
        {
            Common.Actions.OpenSubmenu(target, npc,
                _BlackSmithMenu,        //Dialog script to show
                DialogType.Smith,       //Button function
                50,                     //Repair
                52,                     //Change Type
                53,                     //Upgrade
                55                      //Change Suffix
            );
        }

        /// <summary>
        /// Occurs when opening the WeaponChange menu
        /// </summary>
        /// <param name="npc">Npc who requires shows the menu</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnChangeWeapon(BaseNPC npc, Character target)
        {
            Common.Actions.OpenMenu(target, npc,
                _WeaponChange,                  //Dialog script for weapon changing
                DialogType.Smith,               //Button function
                npc.GetDialogButtons(target)    //Dialog buttons
            );
        }

        /// <summary>
        /// Occurs when opening the WeaponChangeSuffix menu
        /// </summary>
        /// <param name="npc">Npc who requires shows the menu</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnChangeWeaponSuffix(BaseNPC npc, Character target)
        {
            Common.Actions.OpenMenu(target, npc,
                _WeaponSuffixChange,            //Dialog script for suffix changing
                DialogType.Smith,               //Button function
                npc.GetDialogButtons(target)    //Dialog buttons
            );
        }

        /// <summary>
        /// Occurs when opening the UpgradeWeapon menu
        /// </summary>
        /// <param name="npc">Npc who requires shows the menu</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnUpgradeWeapon(BaseNPC npc, Character target)
        {
            Common.Actions.OpenMenu(target, npc,
                _WeaponUpgrade,                 //Dialog script for weapon upgrade
                DialogType.Smith,               //Button function
                npc.GetDialogButtons(target)    //Dialog buttons
            );
        }

        /// <summary>
        /// Occurs when opening the RepairEquipment menu
        /// </summary>
        /// <param name="npc">Npc who requires shows the menu</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnRepairEquipment(BaseNPC npc, Character target)
        {
            Common.Actions.OpenMenu(target, npc,
               _RepairEquipment,               //Dialog sceipt for equipment reparing
               DialogType.Smith,               //Button function
               npc.GetDialogButtons(target)    //Dialog buttons
           );
        }

        #endregion Protected Methods
    }
}