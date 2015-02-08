using Saga.Enumarations;
using Saga.Map;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;
using System.Collections.Generic;

namespace Saga.Tasks
{
    public static class Shipservice
    {
        #region Private Members

        private static List<DepartureState> states = new List<DepartureState>();

        #endregion Private Members

        #region Public Members

        public static void Enqeuee(List<Character> characters, byte Map, Point A, byte Map2, int MinTime)
        {
            DepartureState Departure = new DepartureState(A, Map, Map2, MinTime);
            foreach (Character target in characters)
            {
                CommonFunctions.Warp(target, new Point(2468.964F, 446.603F, 611.224F), Departure.zone);
            }
            lock (states)
            {
                states.Add(Departure);
            }
        }

        public static void Deqeuee(Point a, byte MapId, byte departuremap)
        {
            lock (states)
            {
                for (int i = 0; i < states.Count; i++)
                {
                    DepartureState state = states[i];
                    if (state.detinationmap == MapId && state.zone.CathelayaLocation.map == departuremap && state.IsRound())
                    {
                        List<Character> characters = new List<Character>();
                        foreach (Character character in state.zone.Regiontree.SearchActors(SearchFlags.Characters))
                        {
                            characters.Add(character);
                        }

                        while (characters.Count > 0)
                        {
                            CommonFunctions.Warp(characters[0], state.detinationmap, a);
                            characters.RemoveAt(0);
                        }

                        states.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        #endregion Public Members

        #region Internal Members

        internal static void ForcePlayerFromShipZone(Character character)
        {
            lock (states)
            {
                for (int i = 0; i < states.Count; i++)
                {
                    DepartureState state = states[i];
                    if (state.zone == character.currentzone)
                    {
                        character.map = state.zone.CathelayaLocation.map;
                        character.Position = state.zone.CathelayaLocation.coords;
                        break;
                    }
                }
            }
        }

        #endregion Internal Members

        #region Nested Members

        private class DepartureState
        {
            public Zone zone;
            public byte detinationmap;
            public int DepartureTime;
            public int MinTime = 0;

            public DepartureState(Point departure, byte departuremap, byte detinationmap, int MinTime)
            {
                Singleton.Zones.TryFindClonedZone(10, out zone);
                zone.CathelayaLocation = new WorldCoordinate(departure, departuremap);
                this.detinationmap = detinationmap;
                this.DepartureTime = Environment.TickCount;
                this.MinTime = Environment.TickCount + MinTime;
            }

            public bool IsRound()
            {
                return this.MinTime - Environment.TickCount < 0;
            }
        }

        #endregion Nested Members
    }
}