using System;

namespace Saga.Enumarations
{
    [Flags()]
    public enum SearchFlags
    {
        Characters = 1,
        MapItems = 2,
        Npcs = 4,
        DynamicObjects = 7,
        StaticObjects = 8
    }
}