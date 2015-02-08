using Saga.Core;
using Saga.PrimaryTypes;
using System;

namespace Saga.Scripting
{
    public static partial class Console
    {
        /**
         * <summary>Warps a specified player to a specified npc through the console</summary>
         * <remarks>
         * This function is an dumy example for console scripting.
         *
         * Using the console attribute to give extra information. This information will be used
         * when registering the command to the server. You cannot register duplicate names, therefor
         * you cannot override the default commands. We might make a extension for this in the nearby future.
         *
         * The Regiontree as used in this example is an tree of npc/monster/character objects. Grid over regionblock
         * where each region is 1000x1000 units. In this scenario we use it to search for an npc with the npcid of x
         * and we'll warp you to first found npc that matches the criteria.
         *
         * To enable the function please add: Namespace.Class.Function to the command section
         * under Saga.Manager.ConsoleSettings section.
         *
           <example>
           <![CDATA[
           <!-- Console settings -->
           <Saga.Manager.ConsoleSettings commandprefix="@" outputcommand="true" >
           <Commands>
              <add path="Saga.Scripting.Console.WarpToNpc" />
            </Commands>
            <GmCommands>
              <add path="Saga.Scripting.Console.Broadcast" />
            </GmCommands>
            ]]></example>
         *  </remarks>
         **/

        [ConsoleAttribute("Player.WarpToNpc", "Warps a player to unique npc registered.", "Player.WarpToNpc (Playername) (npcid)")]
        public static void WarpToNpc(string[] args)
        {
            MapObject b;
            uint NpcId = Convert.ToUInt32(args[2]);
            Character characterSource;

            Predicate<MapObject> IsNpc = delegate(MapObject match)
            {
                return match.ModelId == NpcId;
            };

            if (Tasks.LifeCycle.TryGetByName(args[1], out characterSource))
            {
                b = characterSource.currentzone.Regiontree.SearchActor(IsNpc, Saga.Enumarations.SearchFlags.Npcs);
                if (b != null)
                    CommonFunctions.Warp(characterSource, b.currentzone.Map, b.Position);
            }
        }
    }
}