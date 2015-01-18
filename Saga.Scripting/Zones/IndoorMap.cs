using System;
using System.Collections.Generic;
using System.Text;
using Saga.Map;

namespace Saga.Scripting.Zones
{
    class IndoorMap : Saga.PrimaryTypes.Zone, ICloneable
    {

        public override void OnChangeWeather(int Weather)
        {
            //Do not update the weather for indoor maps
            //So we don't call the base method
            //base.UpdateWeather(Weather);
        }

        #region ICloneable Members

        object ICloneable.Clone()
        {
            IndoorMap b = new IndoorMap();
            b.Map = this.Map;
            b.ProsmiseLocation = this.ProsmiseLocation;            
            b.CathelayaLocation = this.CathelayaLocation;
            b.RegionCode = this.RegionCode;
            b.Regiontree = new Regiontree();
            b.Type = this.Type;
            return b;
        }

        #endregion

    }
}
