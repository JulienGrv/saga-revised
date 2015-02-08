using Saga.Enumarations;
using Saga.PrimaryTypes;
using Saga.Structures;
using Saga.Structures.Collections;
using Saga.Templates;
using System;

namespace Saga.Npc.Functions
{
    /// <summary>
    /// This is a cathelya conversation. Include this in a npc to show a
    /// catheleya dialog and submenu.
    /// </summary>
    /// <remarks>
    /// This cathelya shop has prefixed items these items are read from
    /// shops/cathelya.xml by default. The shoplist is a list of items
    /// which is set to the tag property of the character. Our core system
    /// will handle all error checking based upon that item.
    /// </remarks>
    public class CatheleyaConversation : NpcFunction
    {
        #region Private Members

        /// <summary>
        /// Contains a list of items used for buying/selling.
        /// </summary>
        private BaseShopCollection _shoplist;

        /// <summary>
        /// Dialog to say when opening bookstore.
        /// </summary>
        public uint _BookStore = 823;

        /// <summary>
        /// Dialog to say when opening the catheleya shop
        /// </summary>
        public uint _Shop = 823;

        /// <summary>
        /// Dialog to say when performing a heal.
        /// </summary>
        public uint _Heal = 823;

        #endregion Private Members

        #region Protected Methods

        /// <summary>
        /// Registers all submenu's and cathelya buttons.
        /// </summary>
        /// <param name="npc"></param>
        protected internal override void OnRegister(BaseNPC npc)
        {
            RegisterDialog(npc, DialogType.BookStore, new FunctionCallback(OnBookStore));
            RegisterDialog(npc, DialogType.BookStore, 30, new FunctionCallback(PerformHeal));
            RegisterDialog(npc, DialogType.BookStore, 34, new FunctionCallback(OnShop));
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
                case "BOOKSTORE": _BookStore = value; break;
                case "SHOP": _Shop = value; break;
                case "HEAL": _Heal = value; break;
            }
        }

        /// <summary>
        /// Refreshes the shop
        /// </summary>
        /// <param name="npc">Target npc</param>
        protected internal override void OnRefresh(BaseNPC npc)
        {
            OnCacheShop(npc);
        }

        /// <summary>
        /// Calculates the heal costs to heal the
        /// selected character.
        /// </summary>
        /// <param name="target">Character who to heal</param>
        /// <returns></returns>
        protected int ComputeHealCosts(Character target)
        {
            return 8 + (Math.Max(target._level - 5, 0) * 8);
        }

        /// <summary>
        /// Performs a heal on the selectected character.
        /// </summary>
        /// <param name="npc">Npc who performs the heal</param>
        /// <param name="target">Character who requires heal</param>
        protected void PerformHeal(BaseNPC npc, Character target)
        {
            //CALCULATE PRICE
            uint RequiredZeny = (uint)ComputeHealCosts(target);

            //NOT ENOUGH MONEY
            if (RequiredZeny > target.ZENY)
            {
                Common.Errors.CatheleyaHealError(target, 1);
            }
            //FULL HP/SP
            else if (target.SP == target.SPMAX && target.HP == target.HPMAX)
            {
                Common.Errors.CatheleyaHealError(target, 2);
            }
            //OKAY
            else
            {
                target.HP = target.HPMAX;
                target.SP = target.SPMAX;

                target.ZENY = target.ZENY - RequiredZeny;
                npc.Zeny += RequiredZeny;

                CommonFunctions.UpdateCharacterInfo(target, 0);
                CommonFunctions.UpdateZeny(target);
                CommonFunctions.UpdateShopZeny(target);

                OnHeal(npc, target);
            }
        }

        /// <summary>
        /// Occurs when the catheleya subemnu should be opened
        /// </summary>
        /// <param name="npc">Npc who opens the submenu</param>
        /// <param name="target">Character who recieves the submenu</param>
        protected virtual void OnBookStore(BaseNPC npc, Character target)
        {
            Common.Actions.OpenSubmenu(target, npc,
                _BookStore,             //Dialog script to show
                DialogType.BookStore,   //Button function
                30,                     //Repair
                34                      //Change Type
            );
        }

        /// <summary>
        /// Occurs after heal has been performed.
        /// </summary>
        /// <param name="npc">Npc who performed the heal</param>
        /// <param name="target">Character on who the heal has been performed</param>
        protected virtual void OnHeal(BaseNPC npc, Character target)
        {
            Common.Actions.OpenMenu(
                target, npc,
                _Heal,
                DialogType.EverydayConversation,
                new byte[] { }
            );
        }

        /// <summary>
        /// Occurs when the shop should be opend
        /// </summary>
        /// <param name="npc">Npc who opens the shop</param>
        /// <param name="target">Character who should see the shop</param>
        protected virtual void OnShop(BaseNPC npc, Character target)
        {
            //Show dialog
            Common.Actions.OpenMenu(target, npc,
                _Shop,
                DialogType.EverydayConversation,
                new byte[] { }
            );

            //Open shop list
            _shoplist.Open(target, npc);
        }

        /// <summary>
        /// Caches the shop items.
        /// </summary>
        /// <remarks>
        /// This will load all shop items from the data directory from read from
        /// file shops/cathelya.xml.
        /// </remarks>
        /// <param name="npc">Npc who requires caching</param>
        protected virtual void OnCacheShop(BaseNPC npc)
        {
            string filename = Server.SecurePath("~/shops/Catheleya.xml");
            _shoplist = ShopCollection.FromFile(filename);
        }

        #endregion Protected Methods
    }
}