using Saga.Enumarations;
using Saga.PrimaryTypes;
using Saga.Templates;
using System;

namespace Saga.Structures
{
    /// <summary>
    /// Base class for all npc functions.
    /// </summary>
    public class NpcFunction
    {
        #region Public Members

        /// <summary>
        /// Initializes a new NpcFunction of the type t defined by the generic.
        /// </summary>
        /// <typeparam name="T">Type to initiakize</typeparam>
        /// <param name="npc">Npc on who to register the script</param>
        /// <returns>Registered NpcFunction</returns>
        public static NpcFunction Create<T>(BaseNPC npc)
            where T : NpcFunction, new()
        {
            NpcFunction myfunc = new T();
            myfunc.OnRegister(npc);
            npc.state.RegisteredNpcFunctions.Add(myfunc);
            return myfunc;
        }

        #endregion Public Members

        #region Protected Internal Methods

        /// <summary>
        /// Event method called when registering the npcfunction on the npc
        /// </summary>
        /// <param name="npc">Npc on who to register the function</param>
        protected internal virtual void OnRegister(BaseNPC npc)
        {
        }

        /// <summary>
        /// Event method called when refreshing the npcfunction on the npc
        /// </summary>
        /// <param name="npc">Npc on who to refresh the function</param>
        protected internal virtual void OnRefresh(BaseNPC npc)
        {
        }

        /// <summary>
        /// Event method called when cashing dialog information
        /// </summary>
        /// <param name="npc">Npc on who to cache the function</param>
        /// <param name="name">Propertyname of the associated value</param>
        /// <param name="value">Value of the associated propertyname</param>
        protected internal virtual void OnCacheDialogInfo(BaseNPC npc, string name, uint value)
        {
        }

        /// <summary>
        /// Event method called when checking if a button should be visible
        /// </summary>
        /// <param name="npc">Npc on who to cache the function</param>
        /// <param name="dialog">DialogId to check (as in defined in DialogTypes)</param>
        /// <param name="target">Target who to check</param>
        /// <returns>True if the button should be visible</returns>
        protected internal virtual bool OnCheckDialogIsVisible(BaseNPC npc, DialogType dialog, Character target)
        {
            return true;
        }

        #endregion Protected Internal Methods

        #region Protected Internal Delegates

        /// <summary>
        /// Delegate to match methods for function callbacks.
        /// </summary>
        /// <param name="npc">Npc who calls the function</param>
        /// <param name="target">Target actor who invoked the npc</param>
        protected internal delegate void FunctionCallback(BaseNPC npc, Character target);

        #endregion Protected Internal Delegates

        #region Protected Methods

        /// <summary>
        /// Registers a new callback method for when pressing a button
        /// </summary>
        /// <param name="npc">Npc who owns the function</param>
        /// <param name="dialog">DialogType to register</param>
        /// <param name="callback">Associated callback method</param>
        protected void RegisterDialog(BaseNPC npc, DialogType dialog, FunctionCallback callback)
        {
            try
            {
                npc.state.Add(dialog, callback);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(
                    string.Format("Dialog with of the type {0} is already added on class {1}", dialog, npc),
                    e
                );
            }
        }

        /// <summary>
        /// Registers a new callback method for a submenu when selecting a button
        /// </summary>
        /// <param name="npc">Npc who owns the function</param>
        /// <param name="dialog">DialogType to register</param>
        /// <param name="menu">MenuId of the selected item</param>
        /// <param name="callback">Associated callback method</param>
        protected void RegisterDialog(BaseNPC npc, DialogType dialog, byte menu, FunctionCallback callback)
        {
            try
            {
                npc.state.Add(dialog, menu, callback);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(
                    string.Format("Sub dialog with of the type {0}-{1} is already added on class {2}", dialog, menu, npc),
                    e
                );
            }
        }

        #endregion Protected Methods
    }
}