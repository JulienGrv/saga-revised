using Saga.Packets;
using Saga.PrimaryTypes;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Saga.Tasks
{
    /// <summary>
    /// Task that checks for scheduled maintenance appointments.
    /// </summary>
    public static class Maintenance
    {
        #region Public Members

        /// <summary>
        /// Boolean containing we should check for a scheduled maintenance
        /// </summary>
        public static bool IsScheduled;

        /// <summary>
        /// Last Time we've broadcasted the maintenance
        /// </summary>
        public static DateTime LastBroadCast;

        /// <summary>
        /// Next Scheduled maintenance appointment.
        /// </summary>
        public static DateTime NextSceduledMaintenance;

        #endregion Public Members

        #region Internal Members

        /// <summary>
        /// Process our maintenance schedule.
        /// </summary>
        internal static void Process()
        {
            try
            {
                if (IsScheduled == true)
                {
                    TimeSpan span = NextSceduledMaintenance - DateTime.Now;
                    if (span.TotalMinutes > 0 && span.TotalMinutes < 50)
                    {
                        TimeSpan span2 = DateTime.Now - LastBroadCast;
                        if (span2.TotalMinutes > 10)
                        {
                            LastBroadCast = DateTime.Now;

                            //GENERATE BROADCAST
                            SMSG_SENDCHAT spkt = new SMSG_SENDCHAT();
                            spkt.Name = "GM";
                            spkt.MessageType = SMSG_SENDCHAT.MESSAGE_TYPE.SYSTEM_MESSAGE_RED;
                            spkt.Message = String.Format(CultureInfo.InvariantCulture, "Maintenance starting at: {0}", NextSceduledMaintenance.ToShortTimeString());

                            //FORWARD THE BROADCAST
                            foreach (Character characterTarget in LifeCycle.Characters)
                            {
                                if (characterTarget.client.isloaded == false) continue;
                                spkt.SessionId = characterTarget.id;
                                characterTarget.client.Send((byte[])spkt);
                            }
                        }
                    }
                    else if (span.TotalMinutes < 0)
                    {
                        Saga.Managers.NetworkService.InterNetwork.MAINTENANCE_ENTER();
                        IsScheduled = false;

                        while (LifeCycle.Count > 0)
                        {
                            try
                            {
                                foreach (Character characterTarget in LifeCycle.Characters)
                                {
                                    characterTarget.client.Close();
                                    //LifeCycle.ListOfUsersByName.Remove(pair.Key);
                                    LifeCycle.Unsubscribe(characterTarget);
                                }
                            }
                            catch (Exception)
                            {
                                //Do nothing
                            }
                        }

                        //Wait half a minute
                        Thread.Sleep(30000);

                        //Exit's the process
                        Environment.Exit(2);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        #endregion Internal Members

        #region Constructor / Decontructor

        static Maintenance()
        {
            IsScheduled = false;
            LastBroadCast = DateTime.Now;
            NextSceduledMaintenance = DateTime.Now;
        }

        #endregion Constructor / Decontructor
    }
}