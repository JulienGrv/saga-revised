using System;

namespace Saga.Enumarations
{
    /// <summary>
    /// Defines the zone type of the field.
    /// </summary>
    /// <remarks>
    /// Setting the zone type on zone instance will affect
    /// the way the promise stone will behave in saving it's point.
    /// </remarks>
    [Serializable()]
    public enum ZoneType : byte
    {
        /// <summary>
        /// The map is specifaid as dungeon.
        /// </summary>
        Dungeon,

        /// <summary>
        /// The map is specified as village
        /// </summary>
        Village,

        /// <summary>
        /// The map is specified as a field
        /// </summary>
        Field
    }
}