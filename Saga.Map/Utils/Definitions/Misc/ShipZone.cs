using Saga.PrimaryTypes;
using Saga.Structures;
using System;
using System.Collections.Generic;

namespace Saga
{
    public static class Shipzone
    {
        #region Private Members

        /// <summary>
        /// A list of passengers that are waiting for a ship to arrive.
        /// </summary>
        private static readonly List<PendingPassengers> PendingPassengersList = new List<PendingPassengers>();

        #endregion Private Members

        #region Public Members

        /// <summary>
        /// Adds a list of characters as passengers.
        /// </summary>
        /// <param name="passengers">A Iteratation of passengers</param>
        /// <param name="destination">Destination coords</param>
        /// <param name="map">Destination MapId</param>
        /// <param name="travelingtime">Required traveltiem</param>
        public static void ShipSetSail(IEnumerable<Character> passengers, Point destination, byte map, int travelingtime)
        {
            int TimeStamp = Environment.TickCount;

            foreach (Character character in passengers)
            {
                PendingPassengers pending = new PendingPassengers();
                pending.Character = character;
                pending.DesintationCoords = destination;
                pending.DestinationMap = map;
                pending.TimeStamp = TimeStamp;
                pending.TravelTime = travelingtime;
            }
        }

        /// <summary>
        /// Removes a list of pending passengers
        /// </summary>
        /// <param name="map">CurrentMapId</param>
        public static void ShipArrive(byte map)
        {
            //HELPER VARIABLES
            int TimeStamp = Environment.TickCount;
            Predicate<PendingPassengers> callback = delegate(PendingPassengers pending)
            {
                return TimeStamp - pending.TimeStamp > pending.TravelTime;
            };

            //GET A LIST OF PENDING PASSENGERS
            List<PendingPassengers> pendingpassengers = PendingPassengersList.FindAll(callback);

            //REMOVE ALL PENDING PASSENGERS (WE HAVE A STRONG REFERENCED COPY AFTER ALL)
            PendingPassengersList.RemoveAll(callback);

            //PROCESS THE WARPING FOR EVERY PASSENGER
            foreach (PendingPassengers pending in pendingpassengers)
            {
                CommonFunctions.Warp(pending.Character, pending.DestinationMap, pending.DesintationCoords);
            }
        }

        #endregion Public Members

        #region Nested

        /// <summary>
        /// Internal structure to define a passenger
        /// </summary>
        private class PendingPassengers
        {
            public Character Character;
            public Point DesintationCoords;
            public byte DestinationMap;
            public int TimeStamp;
            public int TravelTime;
        }

        #endregion Nested
    }
}