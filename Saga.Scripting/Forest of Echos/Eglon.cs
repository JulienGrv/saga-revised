using System;
using System.Collections.Generic;
using System.Text;
using Saga.Templates;
using Saga.PrimaryTypes;

namespace Saga.Scripting.ForestOfEcho
{
    class Yordi : BaseNPC
    {

        /// <summary>
        /// Overrides the on OnGossip event so Yordi is not able 
        /// to interact with a character. Just a statue.
        /// </summary>
        public override void OnGossip(Character target)
        {
            //Do nothing
        }

    }


}
