using Saga.Map;
using System;

namespace Saga.Scripting.Zones
{
    internal class IndoorMap : Saga.PrimaryTypes.Zone, ICloneable
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

        #endregion ICloneable Members
    }
}