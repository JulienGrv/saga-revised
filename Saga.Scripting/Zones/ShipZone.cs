using Saga.Map;
using System;

namespace Saga.Scripting.Zones
{
    internal class Shipzone : Saga.PrimaryTypes.Zone, ICloneable
    {
        public override void OnLeave(Saga.PrimaryTypes.Character character)
        {
            //Removes the actor from the shipzone
            base.OnLeave(character);

            //Always force to leave from shipzone
            character.map = this.CathelayaLocation.map;
            character.Position = this.CathelayaLocation.coords;

            System.Console.WriteLine("{0} {1}", this.CathelayaLocation.map, this.CathelayaLocation.coords);
        }

        #region ICloneable Members

        object ICloneable.Clone()
        {
            Shipzone b = new Shipzone();
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