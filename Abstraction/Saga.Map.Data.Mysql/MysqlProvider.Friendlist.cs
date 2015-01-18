using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Saga.Shared.Definitions;
using Saga.PrimaryTypes;
using System.Diagnostics;

namespace Saga.Map.Data.Mysql
{
    partial class MysqlProvider
    {

        /// <summary>
        /// Adds a certain character name on the friendlist.
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="friend"></param>
        /// <returns></returns>
        public bool InsertAsFriend(uint charId, string friend)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_25, connection);
            command.Parameters.AddWithValue("?CharId", charId);
            command.Parameters.AddWithValue("?FriendName", friend);
            bool result = false;

            try
            {
                result = command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
            }
            finally
            {
                ConnectionPool.Release(connection);
            }

            return result;
        }


        /// <summary>
        /// Deletes a certain character name from the friendlist
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="friend"></param>
        /// <returns></returns>
        public bool DeleteFriend(MySqlConnection connection, uint charId, string friend)
        {
            MySqlCommand command = new MySqlCommand(_query_26, connection);
            command.Parameters.AddWithValue("?CharId", charId);
            command.Parameters.AddWithValue("?FriendName", friend);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }


        /// <summary>
        /// Inserts a certain character on the blacklist
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="friend"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool InsertAsBlacklist(uint charId, string friend, byte reason)
        {
            MySqlConnection connection = ConnectionPool.Request();
            MySqlCommand command = new MySqlCommand(_query_27, connection);
            command.Parameters.AddWithValue("?CharId", charId);
            command.Parameters.AddWithValue("?FriendName", friend);
            command.Parameters.AddWithValue("?Reason", reason);
            bool result = false;

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

        /// <summary>
        /// Deletes a certain character from the blacklist
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="friend"></param>
        /// <returns></returns>
        public bool DeleteBlacklist(MySqlConnection connection, uint charId, string friend)
        {
            MySqlCommand command = new MySqlCommand(_query_28, connection);
            command.Parameters.AddWithValue("?CharId", charId);
            command.Parameters.AddWithValue("?FriendName", friend);

            try
            {
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                __dbtracelog.WriteError("Database", e.Message);
                return false;
            }
        }


    }
}
