using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Saga.Authentication.Utils.Definitions.Misc;
using System.Net;
using System.Diagnostics;

namespace Saga.Authentication.Data.Mysql
{
    partial class MysqlProvider : IDatabase
    {

        List<string> GetAllCharactersOnline()
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_28, connection);
            MySqlDataReader reader = null;
            List<string> mstring = new List<string>();

            try
            {

                reader = command.ExecuteReader();  // argument CommandBehavior.SingleRow removed (Darkin)
                while (reader.Read())
                {
                    mstring.Add( reader.GetString(0) );
                } 
               
                return mstring;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return mstring;
            }
            finally
            {
                if( reader != null ) reader.Close();
                ConnectionPool.Release(connection);
            }
        }



        /// <summary>
        /// Gets the user credentionals so we can verify in our code 
        /// if the user is valid for a login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool Login(string username, out LoginResult result)
        {

            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_01, connection);
            command.Parameters.AddWithValue("Username", username);
            MySqlDataReader reader = null;
            bool res = false;
            result = new LoginResult();

            try
            {
                reader = command.ExecuteReader(); // argument CommandBehavior.SingleRow removed (Darkin)
                while (reader.Read())
                {
                    //GET USERID
                    result.userid = reader.GetUInt32(0);

                    //GET USERNAME
                    result.lg_username = reader.GetString(1);

                    //GET PASSWORD
                    result.lg_password = reader.GetString(2);

                    //GET GENDER
                    result.lg_gender = reader.GetByte(3);

                    //GET ACTIVE SESSION
                    result.ative_session = reader.IsDBNull(6) ? 0 : reader.GetUInt32(6);

                    //GET LAST LOGIN DATE                   
                    result.lg_entry = reader.IsDBNull(7) ? DateTime.Now : reader.GetDateTime(7);

                    //GET ACTIVATED
                    result.is_activated = reader.GetBoolean(8);

                    //GET BANNED
                    result.is_banned = reader.GetBoolean(9);

                    //HAS AGREED
                    result.has_agreed = reader.GetBoolean(10);

                    //DATE OF BIRTH
                    result.DateOfBirth = reader.IsDBNull(11) ? DateTime.Now : reader.GetDateTime(11);

                    //TEST ACCOUNT
                    result.is_testaccount = reader.GetBoolean(13);

                    //LAST CONNECTED WORLD
                    result.last_server = reader.GetByte(14);

                    //GET GM LEVEL
                    result.gmlevel = reader.GetByte(15);


                    //OUTPUT TRUE
                    res = true;
                }

                reader.Close();
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }

            return res;
        }

        /// <summary>
        /// Updates the last login date
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        public bool UpdateLoginEntry(uint AccountId, IPAddress address)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_02, connection);
            command.Parameters.AddWithValue("IP", address);
            command.Parameters.AddWithValue("UserId", AccountId);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                throw e;
            }
            finally
            {
                ConnectionPool.Release(connection);
            }

        }

        /// <summary>
        /// Creates a new login entry
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public bool CreateLoginEntry(string username, string password, byte gender, int gmlevel)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_03, connection);


            command.Parameters.AddWithValue("Username", username);
            command.Parameters.AddWithValue("Password", password);
            command.Parameters.AddWithValue("Gender", gender);
            command.Parameters.AddWithValue("GmLevel", gmlevel);
            command.Parameters.AddWithValue("DateOfLastLogin", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("DateOfBirth", "1929-04-04");
            command.Parameters.AddWithValue("DateOfRegistration", DateTime.Now.ToString("yyyy-MM-dd"));

            try
            {
                return command.ExecuteNonQuery() > 0;
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
        }

        /// <summary>
        /// Bans a login entry
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool BanLoginEntry(string username)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_04, connection);
            command.Parameters.AddWithValue("Username", username);

            try
            {
                return command.ExecuteNonQuery() > 0;
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
        }

        /// <summary>
        /// Returns the numbers of characters found for a given world id
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="CharPerWorld"></param>
        /// <returns></returns>
        public bool GetCharacterWorldCount(uint userid, out Dictionary<byte, byte> CharPerWorld)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_05, connection);
            command.Parameters.AddWithValue("UserId", userid);
            MySqlDataReader reader = null;
            CharPerWorld = new Dictionary<byte, byte>();

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //OUTPUT TRUE
                    CharPerWorld.Add(reader.GetByte(1), reader.GetByte(2));
                }

                return true;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }
        }

        /// <summary>
        /// Updates the number of characters on World
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="worldid"></param>
        /// <returns></returns>
        public bool UpdateCharacterOnWorld(uint userid, uint worldid)
        {

            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_06, connection);
            command.Parameters.AddWithValue("UserId", userid);
            command.Parameters.AddWithValue("WorldId", worldid);

            try
            {
                if (command.ExecuteNonQuery() == 0)
                {
                    MySqlCommand command2 = new MySqlCommand(_query_07, connection);
                    command2.Parameters.AddWithValue("UserId", userid);
                    command2.Parameters.AddWithValue("WorldId", worldid);
                    command2.ExecuteNonQuery();
                }
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

            return true;
        }

        /// <summary>
        /// Updates the number of characters on World
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="worldid"></param>
        /// <returns></returns>
        public bool RemoveCharacterOnWorld(uint userid, uint worldid)
        {

            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_08, connection);
            command.Parameters.AddWithValue("UserId", userid);
            command.Parameters.AddWithValue("WorldId", worldid);
            MySqlDataReader reader = null;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
                ConnectionPool.Release(connection);
            }

            return true;
        }
    }
}
