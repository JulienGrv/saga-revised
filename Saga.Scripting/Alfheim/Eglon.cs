using Saga.Templates;
using System;

namespace Saga.Scripting.Alfheim
{
    internal class Eglon : BaseNPC
    {
        /// <summary>
        /// Overrides the onregiser to these npc's are not seen
        /// as unique npc's
        /// </summary>
        public override void OnRegister()
        {
            try
            {
                //this.currentzone.actormanger.Register(this);
                base.OnRegister();
            }
            catch (Exception e)
            {
                HostContext.Current.UnhandeldExceptionList.Add(e);
            }
        }

        /// <summary>
        /// Overrides the onregiser to these npc's are not seen
        /// as unique npc's
        /// </summary>
        public override void OnDeregister()
        {
            try
            {
                base.OnDeregister();
            }
            catch (Exception e)
            {
                HostContext.Current.UnhandeldExceptionList.Add(e);
            }
        }
    }
}