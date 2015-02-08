using Saga.Enumarations;
using Saga.Map;
using Saga.Map.Utils.Structures;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Structures.Collections;
using Saga.Templates;
using System;

namespace Saga.Npc.Functions
{
    /// <summary>
    /// This is a skillmaster conversation. Include this in a npc to show a
    /// catheleya dialog and submenu.
    /// </summary>
    /// <remarks>
    /// This skillmasters bookstore has it's items read from
    /// shops/{npcmodelid}.xml.
    /// </remarks>
    public class SkillMasterConversation : NpcFunction
    {
        #region Private Members

        /// <summary>
        /// Contains a list of items used for buying/selling.
        /// </summary>
        private BaseShopCollection _shoplist;

        /// <summary>
        /// Dialog to say when showing the skillmaster menu
        /// </summary>
        protected uint _ShowSkillMasterMenu = 823;

        /// <summary>
        /// Dialog to say when changing jobs
        /// </summary>
        protected uint _ChangeJob = 823;

        /// <summary>
        /// Dialog to say when showing special a
        /// </summary>
        protected uint _OnSpecialAbillities = 823;

        protected uint _OnSkillbook = 823;

        #endregion Private Members

        #region Protected Methods

        /// <summary>
        /// Registers all the menu's and buttons on the npc.
        /// </summary>
        /// <param name="npc">Npc who to register with</param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.BookStore, new FunctionCallback(OnSkillmasterMenu));
            RegisterDialog(npc, DialogType.BookStore, 31, new FunctionCallback(OnChangeJob));
            RegisterDialog(npc, DialogType.BookStore, 32, new FunctionCallback(OnSpecialAbillities));
            RegisterDialog(npc, DialogType.BookStore, 33, new FunctionCallback(OnSkillbook));
            OnCacheShop(npc);
        }

        /// <summary>
        /// Refreshes the bookstore
        /// </summary>
        /// <param name="npc">Target npc</param>
        protected internal override void OnRefresh(BaseNPC npc)
        {
            OnCacheShop(npc);
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
                case "SHOWSKILLMASTERMENU": _ShowSkillMasterMenu = value; break;
                case "CHANGEJOB": _ChangeJob = value; break;
                case "SPECIALABILLITIES": _OnSpecialAbillities = value; break;
                case "SKILLBOOK": _OnSkillbook = value; break;
            }
        }

        /// masters submenu.
        /// </summary>
        /// <param name="npc">Npc who calls the event</param><summary>
        /// Shows the skill
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnSkillmasterMenu(BaseNPC npc, Character target)
        {
            Console.WriteLine("Open skill master menu");

            Common.Actions.OpenSubmenu(target, npc,
                _ShowSkillMasterMenu,   //Dialog script to show
                DialogType.BookStore,   //Button function
                31,                     //Change Job
                32,                     //Special skills
                33                      //Skill books
            );
        }

        /// <summary>
        /// Shows all available jobs
        /// </summary>
        /// <param name="npc">Npc who calls the event</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnChangeJob(BaseNPC npc, Character target)
        {
            //Create the collection and show it
            JobChangeCollection.Create(target).Show(target);

            //Show dialog
            Common.Actions.OpenMenu(
                target, npc,
                _ChangeJob,
                DialogType.BookStore,
                npc.GetDialogButtons(target)
            );
        }

        /// <summary>
        /// Shows all special abillities.
        /// </summary>
        /// <param name="npc">Npc who calls the event</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnSpecialAbillities(BaseNPC npc, Character target)
        {
            CommonDialogs.OpenSpecialSkillsDialog(target, Singleton.Database.GetAllLearnedSkills(target));
            //CommonFunctions.ShowDialog(target, npc, _OnSpecialAbillities, npc.GetDialogButtons(target));

            //Show dialog
            Common.Actions.OpenMenu(
                target, npc,
                _OnSpecialAbillities,
                DialogType.BookStore,
                npc.GetDialogButtons(target)
            );
        }

        /// <summary>
        /// Shows all a list of skillbooks.
        /// (uses shoppinglist object)
        /// </summary>
        /// <param name="npc">Npc who calls the event</param>
        /// <param name="target">Character who requires interaction</param>
        protected virtual void OnSkillbook(BaseNPC npc, Character target)
        {
            //Open bookstore
            _shoplist.Open(target, npc);

            //Show dialog
            Common.Actions.OpenMenu(
                target, npc,
                _OnSkillbook,
                DialogType.BookStore,
                npc.GetDialogButtons(target)
            );
        }

        /// <summary>
        /// Caches the shop
        /// </summary>
        /// <param name="npc">Npc who requires his shop to be cached</param>
        protected virtual void OnCacheShop(BaseNPC npc)
        {
            string filename = Server.SecurePath("~/bookstore/{0}.xml", npc.ModelId);
            _shoplist = BookstoreCollection.FromFile(filename);
        }

        #endregion Protected Methods
    }
}