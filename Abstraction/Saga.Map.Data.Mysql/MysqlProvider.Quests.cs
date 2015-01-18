using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saga.Shared.Definitions;
using MySql.Data.MySqlClient;
using System.IO;
using System.IO.Compression;
using Saga.Quests.Objectives;
using System.Runtime.Serialization;
using System.Data;
using Saga.PrimaryTypes;
using System.Diagnostics;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {

        public bool IsQuestComplete(MySqlConnection connection, uint charId, uint QuestId)
        {
            MySqlCommand command = new MySqlCommand(_query_52, connection);
            command.Parameters.AddWithValue("QuestId", QuestId);
            command.Parameters.AddWithValue("CharId", charId);
            List<uint> quests = new List<uint>();
            MySqlDataReader reader = null;

            try
            {
                return Convert.ToUInt32(command.ExecuteScalar()) > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return true;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// Get all available quests per given region
        /// </summary>
        /// <param name="target"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public IEnumerable<uint> GetAvailableQuestsByRegion(MySqlConnection connection, Character target, uint modelid)
        {
            if (__canUseStoredProcedures == true)
            {
                MySqlCommand command = new MySqlCommand(_query_53, connection);
                command.Parameters.AddWithValue("Clvl", target.Clvl + 5);
                command.Parameters.AddWithValue("Jlvl", target.Jlvl + 5);
                command.Parameters.AddWithValue("Modelid", modelid);
                command.Parameters.AddWithValue("CharId", target.ModelId);
                List<uint> quests = new List<uint>();
                MySqlDataReader reader = null;

                try
                {

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        quests.Add(reader.GetUInt32(0));
                    }

                    return quests;
                }
                catch (Exception e)
                {
                    __dbtracelog.WriteError("Database", e.Message);
                    return quests;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
            else
            {
                MySqlCommand command = new MySqlCommand(_query_02, connection);
                command.Parameters.AddWithValue("Clvl", target.Clvl + 5);
                command.Parameters.AddWithValue("Jlvl", target.Jlvl + 5);
                command.Parameters.AddWithValue("Modelid", modelid);
                command.Parameters.AddWithValue("CharId", target.ModelId);
                List<uint> quests = new List<uint>();
                MySqlDataReader reader = null;

                try
                {

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        quests.Add(reader.GetUInt32(0));
                    }

                    return quests;
                }
                catch (Exception e)
                {
                    __dbtracelog.WriteError("Database", e.Message);
                    return quests;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }

        


        /// <summary>
        /// Gets a list of personal requests.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="region"></param>
        /// <param name="CurrentPersonalQuest"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<uint, uint>> GetPersonalAvailableQuestsByRegion(Character target, byte region, uint CurrentPersonalQuest)
        {
            if (__canUseStoredProcedures == true)
            {
                MySqlConnection connection = ConnectionPool.Request();
                MySqlCommand command = new MySqlCommand(_query_54, connection);
                command.Parameters.AddWithValue("CharId", target.ModelId);
                command.Parameters.AddWithValue("Region", region);
                command.Parameters.AddWithValue("Clvl", target.Clvl + 5);
                command.Parameters.AddWithValue("Jlvl", target.Jlvl + 5);
                command.Parameters.AddWithValue("Quest", CurrentPersonalQuest);
                MySqlDataReader reader = null;

                try
                {
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        yield return new KeyValuePair<uint, uint>(reader.GetUInt32(0), reader.GetUInt32(1));
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    ConnectionPool.Release(connection);
                }
            }
            else
            {

                MySqlConnection connection = ConnectionPool.Request();
                MySqlCommand command = new MySqlCommand(_query_01, connection);
                command.Parameters.AddWithValue("CharId", target.ModelId);
                command.Parameters.AddWithValue("Clvl", target.Clvl + 5);
                command.Parameters.AddWithValue("Jlvl", target.Jlvl + 5);
                command.Parameters.AddWithValue("Quest", CurrentPersonalQuest);
                MySqlDataReader reader = null;

                try
                {
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        yield return new KeyValuePair<uint, uint>(reader.GetUInt32(0), reader.GetUInt32(1));
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    ConnectionPool.Release(connection);
                }                
            }
        }

        

        /// <summary>
        /// Completes a quest directly into the database.
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="QuestId"></param>
        /// <returns></returns>
        public bool QuestComplete(uint charId, uint QuestId)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_55, connection);
            command.Parameters.AddWithValue("?CharId", charId);
            command.Parameters.AddWithValue("?QuestId", QuestId);
            bool result;

            try
            {
                result = command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }

            return result;
        }

       
    

       



    }
}
